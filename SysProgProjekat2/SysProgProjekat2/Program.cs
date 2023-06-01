using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        using (var listener = new HttpListener())
        {
            listener.Prefixes.Add("http://localhost:5050/");
            listener.Start();

            Console.WriteLine("Server je startovan, ceka zahteve. . .");

            while (true)
            {
                var context = await listener.GetContextAsync();
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var stopwatch = new Stopwatch();
                        stopwatch.Restart();
                        var request = context.Request;
                        var filePath = request.QueryString["file"];
                        var keyword = request.QueryString["keyword"];
                        if (filePath != null && File.Exists(filePath))
                        {
                            stopwatch.Restart();
                            await OdgovorFajlAsync(context, filePath);
                            TimeSpan elapsed = stopwatch.Elapsed;
                            Console.WriteLine($"Fajl skinut za {elapsed}\n");
                        }
                        else if (keyword != null && !keyword.Equals(String.Empty))
                        {
                            await OdgovorPretrageAsync(context, keyword);
                            TimeSpan elapsed = stopwatch.Elapsed;
                            Console.WriteLine($"Zahtev obradjen za {elapsed}\n");
                        }
                        else
                        {
                            Greska(context, HttpStatusCode.BadRequest, "Zahtev nije validan");
                        }
                        stopwatch.Stop();
                    }
                    catch (Exception ex)
                    {
                        Greska(context, HttpStatusCode.InternalServerError, ex.Message);
                    }
                    finally
                    {
                        context.Response.Close();
                    }
                });
            }
        }
    }
    static async Task OdgovorFajlAsync(HttpListenerContext context, string filePath)
    {
        context.Response.ContentType = "application/octet-stream";
        context.Response.AddHeader("Content-Disposition", $"attachment; filename=\"{Path.GetFileName(filePath)}\"");
        context.Response.ContentLength64 = new FileInfo(filePath).Length;
        using (var fileStream = File.OpenRead(filePath))
        {
            await fileStream.CopyToAsync(context.Response.OutputStream);
        }
    }
    static async Task OdgovorPretrageAsync(HttpListenerContext context, string keyword)
    {
        var cache = MemoryCache.Default;
        var cacheKey = $"search_{keyword}";

        if (cache.Contains(cacheKey))
        {
            var cachedHtml = (string)cache.Get(cacheKey);
            var buffer = Encoding.UTF8.GetBytes(cachedHtml);
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
        else
        {
            var files = Directory.GetFiles(".", $"*{keyword}*");
            var html = new StringBuilder("<html><body>");
            if (files.Length > 0)
            {
                html.Append("<ul>");
                html.Append("<h1>Pronadjeni fajlovi:</h1>");
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    var fileUrl = $"http://localhost:5050/?file={file}";
                    html.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", fileUrl, fileName);
                }
                html.Append("</ul>");
            }
            else
            {
                html.Append("<p>Nema fajlova koji ispunjavaju uslov.</p>");
            }
            html.Append("</body></html>");

            var buffer = Encoding.UTF8.GetBytes(html.ToString());
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);

            var policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5) };
            cache.Set(cacheKey, html.ToString(), policy);
        }
    }
    static void Greska(HttpListenerContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "text/plain";
        context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(message);
        context.Response.OutputStream.Write(Encoding.UTF8.GetBytes(message), 0, message.Length);
    }
}