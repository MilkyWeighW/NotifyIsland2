using System.IO;
using System.Net;
using System.Text.Json;

namespace cn.lixiaotuan.notifyisland2.Services.NotificationAPIServer;

public class Notification
{
    public string title { get; set; } = "";
    public int title_duration { get; set; }
    public string content { get; set; } = "";
    public int content_duration { get; set; }
    public string title_voice { get; set; } = "";
    public string content_voice { get; set; } = "";
    public bool sound_enabled { get; set; }
    public bool effect_enabled { get; set; }
}

public class NotificationReceivedEventArgs : EventArgs
{
    public required Notification Notification { get; set; }
}

public class NotificationAPIServer : IDisposable
{
    public static NotificationAPIServer? Current { get; set; }
    public event EventHandler<NotificationReceivedEventArgs>? NotificationReceived;

    private readonly HttpListener _httpListener;
    private readonly string _prefixUrl;
    private readonly string _token;

    public NotificationAPIServer(string url = "http://localhost:1379/", string token = "")
    {
        _prefixUrl = url.Replace("0.0.0.0", "*").Replace("[::]", "*");
        _token = token;
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add(_prefixUrl);
        Start();
    }

    private void Start()
    {
        try { _httpListener.Start(); ListenAsync(); }
        catch (HttpListenerException) { }
    }

    private void Stop()
    {
        try { _httpListener.Stop(); } catch { }
    }

    private async void ListenAsync()
    {
        var enc = System.Text.Encoding.UTF8;
        while (_httpListener.IsListening)
        {
            HttpListenerContext ctx;
            try { ctx = await _httpListener.GetContextAsync(); }
            catch (HttpListenerException) { break; }
            catch (ObjectDisposedException) { break; }
            catch { continue; }

            var req = ctx.Request;
            var res = ctx.Response;
            res.ContentType = "application/json";
            res.ContentEncoding = enc;

            if (req.Url?.LocalPath != "/api/notify")
            {
                res.StatusCode = 404;
                await res.OutputStream.WriteAsync(enc.GetBytes("{\"success\":false,\"status\":-404}"));
                res.Close(); continue;
            }

            if (req.HttpMethod != "POST")
            {
                res.StatusCode = 405;
                await res.OutputStream.WriteAsync(enc.GetBytes("{\"success\":false,\"status\":-405}"));
                res.Close(); continue;
            }

            if (!string.IsNullOrEmpty(_token))
            {
                var ah = req.Headers.Get("Authorization");
                if (string.IsNullOrEmpty(ah))
                {
                    res.StatusCode = 401;
                    await res.OutputStream.WriteAsync(enc.GetBytes("{\"success\":false,\"status\":-401}"));
                    res.Close(); continue;
                }
                if (!ah.StartsWith("Bearer ") || ah[7..] != _token)
                {
                    res.StatusCode = 403;
                    await res.OutputStream.WriteAsync(enc.GetBytes("{\"success\":false,\"status\":-403}"));
                    res.Close(); continue;
                }
            }

            using var reader = new StreamReader(req.InputStream, req.ContentEncoding);
            try
            {
                var parsed = JsonSerializer.Deserialize<Notification>(await reader.ReadToEndAsync());
                NotificationReceived?.Invoke(this, new NotificationReceivedEventArgs { Notification = parsed! });
                res.StatusCode = 200;
                await res.OutputStream.WriteAsync(enc.GetBytes("{\"success\":true,\"status\":200}"));
            }
            catch
            {
                res.StatusCode = 400;
                await res.OutputStream.WriteAsync(enc.GetBytes("{\"success\":false,\"status\":-400}"));
            }
            res.Close();
        }
    }

    public void Dispose()
    {
        Stop();
        try { _httpListener.Close(); } catch { }
        Current = null;
    }
}
