public class BrowserInfoMiddleware
{
    private readonly RequestDelegate _next;

    public BrowserInfoMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Lấy thông tin từ header của request
        string userAgent = context.Request.Headers["User-Agent"].ToString();
        string acceptLanguage = context.Request.Headers["Accept-Language"].ToString();
        string scheme = context.Request.Scheme;

        // Lưu vào HttpContext để sử dụng sau này
        context.Items["UserAgent"] = userAgent;
        context.Items["AcceptLanguage"] = acceptLanguage;
        context.Items["Scheme"] = scheme;

        // Chuyển request tiếp tục đến pipeline tiếp theo
        await _next(context);
    }
}
