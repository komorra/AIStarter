using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace AIStarter.Core
{
    public class SimpleHttpFileServer
    {
        private static SimpleHttpFileServer instance;

        public static SimpleHttpFileServer Instance => instance ??= new SimpleHttpFileServer("WebServer", 8080);
        public string BaseDirectory => baseDirectory;

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

            listener.Prefixes.Add($"http://localhost:{port}/");
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
    }

}
