using System.Globalization;
using System.Reflection;
using System.Text;
using Document_Extractor.Models.Shared;
using Document_Extractor.Services.Interfaces;
using Spire.Doc;

namespace Document_Extractor.Services.Implementation;
public class WordExtractor : IExtractor
{
    public async Task<AppResult<string>> ReadFile(string filePath)
    {
        var result = new AppResult<string>();

        try
        {
            //Create a Document object
            Document doc = new Document();
            //Load a Word file
            var f = filePath.Split(Path.DirectorySeparatorChar).Last();
            var fileName = f.Split(".").First();
            var storageFile = Path.Combine(Environment.CurrentDirectory, "Result", fileName + ".txt");
            //StorageName = storageFile;

            //IFormFile file = (IFormFile)File.OpenRead(storageFile);

            doc.LoadFromFile(filePath);

            //Convert the text in Word line by line into a txt file                      
            //await Extractor.RemoveFile(storageFile);
            //DataBuffer = new StringBuilder();

            doc.SaveToTxt(storageFile, Encoding.UTF8);
            //Read all lines of txt file
            //string[] lines = File.ReadAllLines(storageFile, System.Text.Encoding.Default);

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
