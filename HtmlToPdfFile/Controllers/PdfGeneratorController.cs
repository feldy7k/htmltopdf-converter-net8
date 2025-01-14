using DinkToPdf;
using DinkToPdf.Contracts;
using HtmlToPdfFile.Request;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text.Json;

// author: feldy judah k
// .NET 8

namespace HtmlToPdfFile.Controllers
{
    [Route("pdf_generator")]
    [ApiController]
    public class PdfGeneratorController : ControllerBase
    {
        private readonly ILogger<PdfGeneratorController> _logger;

        // make singleton instance for SynchronizedConverter
        private static readonly IConverter _converter = new SynchronizedConverter(new PdfTools());

        public PdfGeneratorController(ILogger<PdfGeneratorController> logger)
        {
            _logger = logger;
        }

        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpPost("generate_pdf")]
        public async Task<IActionResult> GeneratePdf(PdfParameter request)
        {
            string jsonRequest = JsonSerializer.Serialize(request);
            try
            {
                string inputPdfPath = AppContext.BaseDirectory.Replace("\\bin\\Debug\\net8.0", "") + "InputFile\\";
                string outputPdfPath = AppContext.BaseDirectory.Replace("\\bin\\Debug\\net8.0", "") + "OutputFile\\";

                //// read HTML from file path
                string htmlContent = "";
                using (FileStream fileStream = new FileStream(Path.Combine(inputPdfPath, "form.html"), FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        // Read all text from the stream
                        htmlContent = reader.ReadToEnd();
                    }
                }
                htmlContent = htmlContent.Replace("=[NAME]", request.Name);
                htmlContent = htmlContent.Replace("=[EMAIL]", request.Email);
                htmlContent = htmlContent.Replace("=[MOBILEPHONE]", request.MobilePhone);
                htmlContent = htmlContent.Replace("=[DATEOFBIRTH]", request.DateOfBirth);
                htmlContent = htmlContent.Replace("=[OCCUPATION]", request.Occupation);
                if (request.Gender.ToLower() == "male")
                {
                    htmlContent = htmlContent.Replace("=[CHECKEDMALE]", "checked");
                    htmlContent = htmlContent.Replace("=[CHECKEDFEMALE]", "");
                }
                if (request.Gender.ToLower() == "female")
                {
                    htmlContent = htmlContent.Replace("=[CHECKEDMALE]", "");
                    htmlContent = htmlContent.Replace("=[CHECKEDFEMALE]", "checked");
                }
                htmlContent = htmlContent.Replace("=[ADDRESS]", request.Address);

                // file name by datetime now
                string datename = DateTime.Now.ToString("yyyyMMddHHmmss");

                // Set conversion options
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = new GlobalSettings
                    {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                        Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 },
                        Out = $"{outputPdfPath}form{datename}.pdf" // Output path
                    },
                    Objects = { new ObjectSettings { HtmlContent = htmlContent } }
                };

                // Convert the HTML to PDF
                byte[] pdfBytes = _converter.Convert(doc);

                // log the result
                _logger.LogInformation($"PDF saved to {$"{outputPdfPath}form{datename}.pdf"}");
                _logger.LogInformation($"GeneratePdf: success generate pdf file. Parameters: {jsonRequest}");

                return Ok(new { Status = "Ok", Message = $"GeneratePdf: success generate pdf file", Parameters = $"{jsonRequest}" });
            }
            catch (Exception ex)
            {
                // log the result
                _logger.LogError($"GeneratePdf: error generate pdf file. Parameters: {jsonRequest}");

                return BadRequest(new { Status = "BadRequest", Message = $"GeneratePdf: error generate pdf file", Parameters = $"{jsonRequest}" });
            }
        }

    }
}