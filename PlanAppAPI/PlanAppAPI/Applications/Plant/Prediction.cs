using System.Text;
using MediatR;
using Newtonsoft.Json;
using PlanAppAPI.Core;
using System.Net.Http;
using System.IO;
using PlanAppAPI.Applications.Plant.Model;

namespace PlanAppAPI.Applications.Plant;

public class Prediction
{
    public class Command : IRequest<Result<Domain.Tables.Plant>>
{
    public required IFormFile File { get; set; }
}

public class Handler : IRequestHandler<Command, Result<Domain.Tables.Plant>>
{
    private readonly ILogger<Prediction> _logger;

    public Handler(ILogger<Prediction> logger)
    {
        _logger = logger;
    }

    public async Task<Result<Domain.Tables.Plant>> Handle(Command request, CancellationToken cancellationToken)
    {
        try
        {
            // API endpoint
            var apiUrl = "http://localhost:5000/predict";

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
                var predictionResponse = JsonConvert.DeserializeObject<PredictionResponseModel>(content);

                return Result<Domain.Tables.Plant>.Success(new Domain.Tables.Plant());
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