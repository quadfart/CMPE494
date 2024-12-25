using System.Text;
using MediatR;
using Newtonsoft.Json;
using PlanAppAPI.Core;
using System.Net.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Persistence;
using PlanAppAPI.Applications.Plant.Model;

namespace PlanAppAPI.Applications.Plant;

public class Prediction
{
    public class Command : IRequest<Result<List<PlantPredictionViewModel>>>
{
    public required IFormFile File { get; set; }
}

public class Handler : IRequestHandler<Command, Result<List<PlantPredictionViewModel>>>
{
    private readonly ILogger<Prediction> _logger;
    private readonly DataContext _context;

    public Handler(ILogger<Prediction> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<List<PlantPredictionViewModel>>> Handle(Command request, CancellationToken cancellationToken)
    {
        try
        {
            // API endpoint
            var apiUrl = "http://flask-api:5000/predict";

            using var httpClient = new HttpClient();
            using var form = new MultipartFormDataContent();

            // Read the file content from the request
            using var fileStream = request.File.OpenReadStream();
            var fileContent = new StreamContent(fileStream);

            // Set content headers
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(request.File.ContentType);
            form.Add(fileContent, "file", request.File.FileName);

            // Send the request to the API
            var response = await httpClient.PostAsync(apiUrl, form);

            if (response.IsSuccessStatusCode)
            {
                // Parse the API response
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Prediction Response: {Content}", content);
                var predictionResponseModel = JsonConvert.DeserializeObject<PredictionResponseModel>(content);

                var predictionResponse = predictionResponseModel.PredictionResponse
                    .ToDictionary(x => x.Key, x => float.Parse(x.Value));
                
                var orderedPredictionResponse = predictionResponse
                    .OrderByDescending(x => x.Value)
                    .Take(5)
                    .ToDictionary(x => x.Key, x => x.Value);

                var matchingPlants = await _context.Plants
                    .Where(plant => orderedPredictionResponse.Keys.Contains(plant.ScientificName))
                    .ToListAsync(cancellationToken);
                var responseModel = matchingPlants.Select(x => new PlantPredictionViewModel
                {
                    Id = x.Id,
                    ModTemp = x.ModTemp,
                    SoilType = x.SoilType,
                    LightNeed = x.LightNeed,
                    HumidityLevel = x.HumidityLevel,
                    WateringFrequency = x.WateringFrequency,
                    IrrigationAmount = x.IrrigationAmount,
                    ScientificName = x.ScientificName,
                    PredictionConfidence = predictionResponse.GetValueOrDefault(x.ScientificName)
                }).OrderByDescending(x=>x.PredictionConfidence).ToList();

                return Result<List<PlantPredictionViewModel>>.Success(responseModel);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Prediction API Error: {Error}", errorContent);
                throw new Exception($"API Error: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in Plant Prediction: {Message}", ex.Message);
            throw new Exception("Error in Plant Prediction: " + ex.Message, ex);
        }
    }
}

}