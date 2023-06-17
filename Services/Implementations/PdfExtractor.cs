using System.Text;
using Document_Extractor.Models.Shared;
using Document_Extractor.Services.Interfaces;
using Spire.Pdf;
using Spire.Pdf.Exporting.Text;

namespace Document_Extractor.Services.Implementations;

public class PdfExtractor : IExtractor
{
    public async Task<AppResult<string>> ReadFile(string filePath, string targetFilePath)
    {
        var result = new AppResult<string>();

        try
        {
            PdfDocument doc = new PdfDocument();

            var f = filePath.Split(Path.DirectorySeparatorChar).Last();
            var fileName = f.Split(".").First();
            var storageFile = Path.Combine(targetFilePath, fileName + ".txt");
            //StorageName = storageFile;

            doc.LoadFromFile(filePath);
            StringBuilder buffer = new StringBuilder();

            SimpleTextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
            foreach (PdfPageBase page in doc.Pages)
            {
                buffer.Append(page.ExtractText(strategy));
            }
            doc.Close();
            //save text
            //await Extractor.RemoveFile(storageFile);
            //DataBuffer = buffer;
            File.WriteAllText(storageFile, buffer.ToString());
            //Launching the Text file.
            //System.Diagnostics.Process.Start(fileName);

            result.Data.Add(storageFile);
            result.Status = true;

        }
        catch (Exception ex)
        {
            result.Status = false;
            result.Message = ex.Message;
        }
        return result;

    }
}