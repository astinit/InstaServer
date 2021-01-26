using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace InstaServer.BLL.Helpers
{
    public static class FileLoader
    {
        public static async Task<FileLoadResponse> GetFileLoadResponse(IEnumerable<string> links)
        {
            var response = new FileLoadResponse();
            try
            {
                var encodedFiles = await GetEncodedFilesAsync(links);
                response.Files.AddRange(encodedFiles);
            }
            catch(Exception)
            {
                response.Error = Error.InternalServerException;
            }
            return response;
        }
        public static async Task<string> GetEncodedFileAsync(string link)
        {
            string encodedFile = string.Empty;
            if (!string.IsNullOrEmpty(link) || !string.IsNullOrWhiteSpace(link))
            {
                try
                {
                    var uri = new Uri(link);
                    if (uri.Scheme != "blob")
                    {
                        using var httpClient = new HttpClient();
                        var file = await httpClient.GetByteArrayAsync(link);
                        encodedFile = Convert.ToBase64String(file);
                    }
                }
                catch(Exception ex)
                {
                    var loggerFactory = LoggerFactory.Create(builder => builder.AddEventLog());
                    var logger = loggerFactory.CreateLogger(typeof(FileLoader));
                    logger.LogError(JsonConvert.SerializeObject(ex));
                    logger.LogInformation(link);
                    throw;
                }
            }
            return encodedFile;
        }

        private static async Task<IEnumerable<string>> GetEncodedFilesAsync(IEnumerable<string> links)
        {
            var linksCount = links.Count();
            var files = new string[linksCount];
            int i = 0;
            foreach(var link in links)
            {
                files[i] = await GetEncodedFileAsync(link);
                i++;
            }
            return files;
        }
    }
}
