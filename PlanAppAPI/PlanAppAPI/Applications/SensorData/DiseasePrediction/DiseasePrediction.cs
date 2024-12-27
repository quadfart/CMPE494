using System.Drawing; // Add reference to System.Drawing.Common
using System.Text;
using MediatR;
using Newtonsoft.Json;
using PlanAppAPI.Core;
using System.Net.Http;
using System.IO;
using Domain.Tables;
using Microsoft.EntityFrameworkCore;
using Persistence;
using PlanAppAPI.Applications.SensorData.DiseasePrediction.Model;

namespace PlanAppAPI.Applications.SensorData.DiseasePrediction;

public class DiseasePrediction
{
    public class Command : IRequest<Result<List<DiseasePredictionViewModel>>>
    {
        public required IFormFile File { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<List<DiseasePredictionViewModel>>>
    {
        private readonly ILogger<DiseasePrediction> _logger;
        private readonly DataContext _context;

        public Handler(ILogger<DiseasePrediction> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<Result<List<DiseasePredictionViewModel>>> Handle(Command request, CancellationToken cancellationToken)
        {
            try
            {
                // API endpoint
                var apiUrl = "http://cmpe494-flask-api-1:5000/predictDisease";

                using var httpClient = new HttpClient();
                using var form = new MultipartFormDataContent();

                // Resize the image to 224x224
                var resizedImage = ResizeImage(request.File, 224, 224);

                // Convert the resized image to a stream
                var resizedStream = new MemoryStream(resizedImage);
                var fileContent = new StreamContent(resizedStream);

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
                    var predictionResponseModel = JsonConvert.DeserializeObject<DiseasePredictionResponseModel>(content);

                    var predictionResponse = predictionResponseModel.DiseasePredictionResponse
                        .ToDictionary(x => x.Key, x => float.Parse(x.Value));

                    var orderedPredictionResponse = predictionResponse
                        .OrderByDescending(x => x.Value)
                        .Take(3)
                        .ToDictionary(x => x.Key, x => x.Value);

                    var matchingDiseases = await _context.Diseases
                        .Where(disease => orderedPredictionResponse.Keys.Contains(disease.Name))
                        .ToListAsync(cancellationToken);

                    var responseModel = matchingDiseases.Select(x => new DiseasePredictionViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Symptoms = x.Symptoms,
                        Treatments = x.Treatments,
                        DiseasePredictionConfidence = predictionResponse.GetValueOrDefault(x.Name)
                    }).OrderByDescending(x => x.DiseasePredictionConfidence).ToList();

                    return Result<List<DiseasePredictionViewModel>>.Success(responseModel);
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
                _logger.LogError("Error in Disease Prediction: {Message}", ex.Message);
                throw new Exception("Error in Disease Prediction: " + ex.Message, ex);
            }
        }

        private byte[] ResizeImage(IFormFile file, int width, int height)
        {
            using var inputStream = file.OpenReadStream();
            using var image = Image.FromStream(inputStream);
            using var resized = new Bitmap(image, new Size(width, height));
            using var outputStream = new MemoryStream();
            resized.Save(outputStream, image.RawFormat);
            return outputStream.ToArray();
        }
    }
}
