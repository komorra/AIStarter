using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AIStarter.Core
{
    internal static class Inference
    {
        public static Action<string> Log { get; set; } = Console.WriteLine;

        public static async Task<string> Run(string dockerRunCommand, string inputJson, string predictionUrl)
        {
            var fullDockerCommand = $"docker run {dockerRunCommand}";
            var processStartInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", "/c " + fullDockerCommand)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new System.Diagnostics.Process())
            {
                process.StartInfo = processStartInfo;
                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                await process.WaitForExitAsync();
                if (process.ExitCode != 0)
                {
                    Log($"Error running Docker command: {error}");
                }                
            }

            var client = new HttpClient();
            var endpoint = predictionUrl;

            // JSON payload as a string
            var json = inputJson;

            // Prepare the content
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send the POST request
            var response = await client.PostAsync(endpoint, content);

            // Read the response
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);

            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseBody);
            var outputData = jsonResponse.output.ToString();

            return outputData;
        }

        public static string OutputDataToTempFile(string outputData)
        {
            var outputDirectory = "Output";
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
            var dataType = outputData.Split(';')[0].Split(':')[1].Trim();
            var data = Convert.FromBase64String(outputData.Split(';')[1].Substring(7));

            var combinedPath = Path.Combine(outputDirectory, $"{DateTime.UtcNow.Ticks}{GetExtensionByDataType(dataType)}");
            File.WriteAllBytes(combinedPath, data);

            return combinedPath;
        }

        private static string GetExtensionByDataType(string dataType)
        {
            var extension = dataType switch
            {
                "image/png" => ".png",
                "image/jpeg" => ".jpg",
                "image/gif" => ".gif",
                "text/plain" => ".txt",
                "audio/wav" => ".wav",  
                "audio/mpeg" => ".mp3",               
                "video/mp4" => ".mp4",  
                "application/pdf" => ".pdf",
                _ => throw new NotSupportedException($"Unsupported data type: {dataType}")
            };

            return extension;
        }
    }
}
