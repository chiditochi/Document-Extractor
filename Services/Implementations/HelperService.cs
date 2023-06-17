

using Document_Extractor.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Document_Extractor.Services.Implementation;

public class HelperService : IHelperService
{
    private readonly ILogger<HelperService> _logger;

    public HelperService(
        ILogger<HelperService> logger
    )
    {
        _logger = logger;
    }
    public Task CustomLogError(Exception e, string action)
    {
        string message = e.Message;
        if (e.InnerException != null && e.InnerException.Message != null)
        {
            message = e.InnerException.Message;
        }
        if (!string.IsNullOrEmpty(e.StackTrace))
        {
            message += $"\n {e.StackTrace}";
        }
        _logger.LogError($"{action}:: {message}");
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteFile(string filePath)
    {
        bool result;
        try
        {
            var pathAndFileName = filePath;
            if (File.Exists(pathAndFileName))
            {
                File.Delete(pathAndFileName);
            }
            result = true;
        }
        catch (Exception ex)
        {
            await CustomLogError(ex, $"DeleteFile");
            result = false;
        }
        return result;
    }
    public async Task<bool> DeleteFiles(string[] filePaths)
    {
        bool result;
        try
        {
            foreach (var item in filePaths)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    await DeleteFile(item);
                }
            }

            result = true;
        }
        catch (Exception ex)
        {
            await CustomLogError(ex, $"DeleteFiles");
            result = false;
        }
        return result;
    }

    private static string GetSqlString<T>(List<T> items)
    {
        var result = string.Empty;
        var list = new List<object>();
        if (typeof(T) == typeof(string))
        {
            foreach (var item in items)
            {
                list.Add($"'{item}'");
            }
        }
        else
        {
            foreach (var item in items)
            {
                list.Add($"{item}");
            }
        }

        return $"({string.Join(", ", list)})";
    }


        public List<string> GetModelStateErrors(ModelStateDictionary modelState)
        {
            return modelState.Values.SelectMany(v => v.Errors)
                                        .Select(v => v.ErrorMessage + " " + v.Exception)
                                        .ToList();
        }














}