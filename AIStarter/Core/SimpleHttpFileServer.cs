using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AIStarter.Core
{
    public class SimpleHttpFileServer
    {
        private static SimpleHttpFileServer instance;

        public static SimpleHttpFileServer Instance => instance ??= new SimpleHttpFileServer("Localhost", 8080);
        public string BaseDirectory => baseDirectory;

        public bool Running => listener?.IsListening ?? false;

        private readonly HttpListener listener = new HttpListener();
        private readonly string baseDirectory;
        private readonly int port;

        public SimpleHttpFileServer(string directory, int port = 8080)
        {
            baseDirectory = Path.GetFullPath(directory);
            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }
            this.port = port;

            var urlPrefix = $"http://*:{port}/";

            if (!IsUrlAclConfigured(urlPrefix))
            {
                var user = Environment.UserName;

                Console.WriteLine("UrlACL is missing. Trying to add it with admin privileges...");
                AddUrlAcl(urlPrefix, user);
            }
            else
            {
                Console.WriteLine("UrlACL is already configured.");
            }

            listener.Prefixes.Add(urlPrefix);

            _ = StartAsync();
        }

        static bool IsUrlAclConfigured(string prefix)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = "http show urlacl",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null) return false;

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output.Contains(prefix, StringComparison.OrdinalIgnoreCase);
        }

        static void AddUrlAcl(string prefix, string user)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = $"http add urlacl url={prefix} user={user}",
                Verb = "runas", // ← UAC prompt!
                UseShellExecute = true // ← Musi być true do UAC
            };

            try
            {
                Process.Start(psi)?.WaitForExit();
                Console.WriteLine("UrlACL added successfully.");
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Console.WriteLine("Operation cancelled or failed: " + ex.Message);
            }
        }

        public async Task StartAsync()
        {
            listener.Start();
            Console.WriteLine($"Server running at http://localhost:{port}/ and serving files from: {baseDirectory}");

            while (listener.IsListening)
            {
                try
                {
                    var context = await listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequestAsync(context));
                }
                catch (HttpListenerException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private async Task HandleRequestAsync(HttpListenerContext context)
        {
            string urlPath = context.Request.Url.LocalPath.TrimStart('/');
            string filePath = Path.Combine(baseDirectory, urlPath);

            if (Directory.Exists(filePath))
                filePath = Path.Combine(filePath, "index.html");

            if (File.Exists(filePath))
            {
                try
                {
                    byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
                    context.Response.ContentType = GetContentType(filePath);
                    context.Response.ContentLength64 = fileBytes.Length;
                    await context.Response.OutputStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500;
                    byte[] errorBytes = System.Text.Encoding.UTF8.GetBytes($"Server error: {ex.Message}");
                    await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                }
            }
            else
            {
                context.Response.StatusCode = 404;
                byte[] message = System.Text.Encoding.UTF8.GetBytes("File not found");
                await context.Response.OutputStream.WriteAsync(message, 0, message.Length);
            }

            context.Response.OutputStream.Close();
        }

        private string GetContentType(string path)
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();
            return extension switch
            {
                ".html" => "text/html",
                ".htm" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                ".json" => "application/json",
                ".txt" => "text/plain",

                // Audio MIME types
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".ogg" => "audio/ogg",
                ".flac" => "audio/flac",
                ".aac" => "audio/aac",
                ".m4a" => "audio/mp4",

                _ => "application/octet-stream"
            };
        }

        internal static string ConvertToLocalhostIfNeeded(string url)
        {
            if (url.StartsWith("http://"))
            {
                return url;
            }
            else if (url.StartsWith("https://"))
            {
                return url;
            }
            else
            {
                var target = Path.Combine(Instance.BaseDirectory, $"{DateTime.UtcNow.Ticks}{Path.GetExtension(url)}");
                File.Copy(url, target, true);

                var result = $"http://host.docker.internal:{Instance.port}/{Path.GetFileName(target)}";

                return result;
            }
        }
    }

}
