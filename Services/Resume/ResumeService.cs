using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using SkillBridge.Services.AI;
using SkillBridge.DTOs.Resume;

namespace SkillBridge.Services.Resume
{
    public class ResumeService : IResumeService
    {
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
        private const string PdfContentType = "application/pdf";
        private readonly IGroqService _groqService;

        public ResumeService(IGroqService groqService)
        {
            _groqService = groqService;
        }

        public async Task<string> ExtractTextFromPdfAsync(IFormFile file)
        {
            // Validate file
            ValidateFile(file);

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    return await ExtractTextAsync(stream);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to extract text from PDF: {ex.Message}", ex);
            }
        }

        public async Task<ResumeExtractionResultDto> ExtractAndParseResumeAsync(IFormFile file)
        {
            // Extract text from PDF
            var extractedText = await ExtractTextFromPdfAsync(file);

            // Parse structured data using Groq
            var structuredData = await _groqService.ExtractResumeDataAsync(extractedText);

            return new ResumeExtractionResultDto
            {
                ExtractedText = extractedText,
                StructuredData = structuredData
            };
        }

        public async Task<object> ProcessResumeAsync(IFormFile file)
        {
            // Extract text from PDF
            var extractedText = await ExtractTextFromPdfAsync(file);

            // Parse structured data using Groq
            var structuredData = await _groqService.ExtractResumeDataAsync(extractedText);

            return new
            {
                message = "Resume processed successfully",
                fileName = file.FileName,
                textLength = extractedText?.Length ?? 0,
                extractedText = extractedText,
                structuredData = structuredData
            };
        }

        private void ValidateFile(IFormFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file), "File is required");
            }

            if (file.Length == 0)
            {
                throw new InvalidOperationException("File is empty");
            }

            if (file.Length > MaxFileSizeBytes)
            {
                throw new InvalidOperationException($"File size exceeds maximum allowed size of {MaxFileSizeBytes / (1024 * 1024)} MB");
            }

            if (!file.ContentType.Equals(PdfContentType, StringComparison.OrdinalIgnoreCase) &&
                !file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("File must be a PDF document");
            }
        }

        private async Task<string> ExtractTextAsync(Stream stream)
        {
            var textBuilder = new StringBuilder();

            using (var document = PdfDocument.Open(stream))
            {
                foreach (var page in document.GetPages())
                {
                    var pageText = page.Text;
                    if (!string.IsNullOrEmpty(pageText))
                    {
                        textBuilder.AppendLine(pageText);
                    }
                }
            }

            var extractedText = textBuilder.ToString();
            var cleanedText = CleanText(extractedText);

            return await Task.FromResult(cleanedText);
        }

        private string CleanText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            // Remove multiple consecutive newlines
            text = Regex.Replace(text, @"\n\s*\n", "\n");

            // Remove multiple consecutive spaces
            text = Regex.Replace(text, @" +", " ");

            // Remove leading/trailing whitespace from each line
            var lines = text.Split('\n');
            var cleanedLines = lines.Select(line => line.Trim()).Where(line => !string.IsNullOrEmpty(line));

            return string.Join("\n", cleanedLines);
        }
    }
}
