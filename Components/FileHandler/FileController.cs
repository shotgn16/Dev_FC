using ForestChurches.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace ForestChurches.Components.FileManager
{
    public class FileController : Controller, FileInterface
    {
        private IActionResult actionResult;
        private readonly ILogger<FileController> _logger;
        public FileController(ILogger<FileController> logger)
        {
            _logger = logger;
        }
        public async Task<IActionResult> CreateFile(Models.ChurchInformation content, string contentType, string fileName = "")
        {
            if (content == null || string.IsNullOrWhiteSpace(fileName)) 
            {
                throw new ArgumentException("Content or filename cannot be null or empty.");
            }

            string fileContent = await GenerateFileContent(content, fileName);
            fileName = EnsureValidFilename(fileName) + ".txt";

            try
            {
                await System.IO.File.WriteAllTextAsync(fileName, fileContent);

                actionResult = new FileStreamResult(new FileStream(fileName, FileMode.Open, FileAccess.Read), contentType)
                {
                    FileDownloadName = fileName
                };
            }

            catch (Exception ex)
            {
                throw new InvalidOperationException($"Falid to create file '{fileName}'.", ex);
            }

            return actionResult;
        }

        private async Task<string> GenerateFileContent(ChurchInformation content, string filename)
        {
            StringBuilder fileContent = new StringBuilder();
            string openingHours = await returnOpeningHours(content);

            try
            {
                if (content != null && filename == "Church Information")
                {
                    fileContent
                        .AppendLine("=========================")
                        .AppendLine($"Status: {content.Status}")
                        .AppendLine("=========================")
                        .AppendLine($"Church Name: {content.Name ?? "Unavailable"}")
                        .AppendLine($"Description: {content.Description ?? "Unavailable"}")
                        .AppendLine($"Denomination: {content.Denominaion ?? "Unavailable"}")
                        .AppendLine($"Congregation: {content.Congregation ?? "Unavailable"}")
                        .AppendLine("\nContact Information")
                        .AppendLine($"Website: {content?.Website ?? "Unavailable"}")
                        .AppendLine($"Address: {content.Address ?? "Unavailable"}")
                        .AppendLine($"Phone: {content.Phone ?? "Unavailable"}")
                        .AppendLine("\nFacilities")
                        .AppendLine($"Wheelchair Church access: {(content.WheelchairAccess == true ? "Available" : "Unavailable")}")
                        .AppendLine($"Refreshments: {(content.Refreshments == true ? "Available" : "Unavailable")}")
                        .AppendLine($"Restrooms: {(content.Restrooms == true ? "Available" : "Unavailable")}")
                        .AppendLine($"WiFi: {(content.Wifi == true ? "Available" : "Unavailable")}")
                        .AppendLine($"Parking: {(content.Parking == true ? "Available" : "Unavailable")}")
                        .AppendLine("\n\n" + openingHours);
                }

                else if (content != null && filename == "Event Information")
                {

                }

                else if (content == null)
                {
                    fileContent.AppendLine("An error occurred while processing your request...\nPlease try again later.\n\nIf this issue persists, please contact our support team: support@forestchurches.co.uk");
                }
            }

            catch (Exception ex)
            {
               _logger.LogError($"Error occured while generating file content: {ex.Message}");
                throw ex;
            }

            return fileContent.ToString();
        }

        private string EnsureValidFilename(string filename)
        {
            try
            {
                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                {
                    filename = filename.Replace(c, '_');
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error occured while ensuring valid filename: {ex.Message}");
            }

            return filename;
        }

        private async Task<string> returnOpeningHours(ChurchInformation church)
        {
            StringBuilder output = new StringBuilder();

            try
            {
                if (church.GetType() != null && church.ServiceTimes != null)
                {
                    output.AppendLine("Service Times:\n");

                    foreach (var item in church.ServiceTimes)
                    {
                        output.AppendLine($"{item.Note}: {item.Time}");
                    }
                }

                // BUG : Object reference not set to an instance of an object
                else if (church.ServiceTimes == null)
                {
                    output.Append("No Opening hours available");
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error occured while returning opening hours: {ex.Message}");
            }

            return output.ToString();
        }
    }
}
