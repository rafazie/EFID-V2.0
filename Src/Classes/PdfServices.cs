using Rotativa;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SimplifikasiFID.Classes
{
    public class PdfServices : AsPdfResultBase
    {
        private readonly string _htmlContent;
        private readonly string _wkhtmltopdfPath;

        public PdfServices(string htmlContent, string wkhtmltopdfPath)
        {
            _htmlContent = htmlContent;
            _wkhtmltopdfPath = wkhtmltopdfPath;
        }

        protected override string GetUrl(ControllerContext context)
        {
            throw new NotImplementedException("GetUrl should not be called for HtmlAsPdf");
        }

        public byte[] GeneratePdf()
        {
            return ConvertHtmlToPdf(_htmlContent, GetConvertOptions(), _wkhtmltopdfPath);
        }

        protected override byte[] CallTheDriver(ControllerContext context)
        {
            return GeneratePdf();
        }

        public static byte[] ConvertHtmlToPdf(string htmlContent, string convertOptions, string wkhtmltopdfPath)
        {
            string tempHtmlFile = Path.GetTempFileName();
            File.WriteAllText(tempHtmlFile, htmlContent);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = wkhtmltopdfPath,
                Arguments = $"{convertOptions} \"{tempHtmlFile}\" -",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processStartInfo))
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Capture standard error output
                    string errorOutput = process.StandardError.ReadToEnd();
                    Debug.WriteLine("wkhtmltopdf Standard Error: " + errorOutput);

                    process.StandardOutput.BaseStream.CopyTo(memoryStream);
                    process.WaitForExit();

                    File.Delete(tempHtmlFile);

                    if (memoryStream.Length == 0)
                    {
                        Debug.WriteLine("wkhtmltopdf output is empty.");
                    }

                    return memoryStream.ToArray();
                }
            }
        }

        private string GetConvertOptions()
        {
            // Define any necessary options for wkhtmltopdf here
            return "--quiet --page-size A4 --orientation Portrait --margin-top 10mm --margin-right 10mm --margin-bottom 10mm --margin-left 10mm";
        }

        public async Task<byte[]> GeneratePdfAsync<TModel>(string viewName, TModel model, ControllerContext context)
        {
            // Log the view name and model details
            System.Diagnostics.Debug.WriteLine($"Generating PDF for view: {viewName}");
            System.Diagnostics.Debug.WriteLine($"Model: {model?.ToString() ?? "null"}");

            try
            {
                var viewResult = new ViewAsPdf(viewName, model)
                {
                    FileName = "File pdf PS",
                    PageSize = Rotativa.Options.Size.A4,
                    PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10),
                    CustomSwitches = "--footer-center \"[page]\" --footer-font-size \"10\" --footer-spacing \"5\""
                };

                var byteArray = await Task.Run(() => viewResult.BuildPdf(context));
                if (byteArray == null)
                {
                    throw new Exception("Failed to generate PDF. Byte array is null.");
                }

                return byteArray;
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error generating PDF: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}