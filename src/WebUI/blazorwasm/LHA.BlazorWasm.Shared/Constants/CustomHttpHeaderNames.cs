namespace LHA.BlazorWasm.Shared.Constants;

public static class CustomHttpHeaderNames
{
    // Enterprise / Custom Tracing Headers
    public const string CorrelationId = "X-Correlation-ID";
    public const string RequestId = "X-Request-ID";
    public const string TenantId = "X-Tenant-ID";
    public const string DeviceId = "X-Device-ID";
    public const string ClientVersion = "X-Client-Version";
    public const string ApiKey = "X-API-Key";

    // Standard Request Headers
    public const string Accept = "Accept";
    public const string AcceptCharset = "Accept-Charset";
    public const string AcceptEncoding = "Accept-Encoding";
    public const string AcceptLanguage = "Accept-Language";
    public const string Authorization = "Authorization";
    public const string CacheControl = "Cache-Control";
    public const string Connection = "Connection";
    public const string ContentLength = "Content-Length";
    public const string ContentType = "Content-Type";
    public const string Cookie = "Cookie";
    public const string Date = "Date";
    public const string Expect = "Expect";
    public const string Forwarded = "Forwarded";
    public const string From = "From";
    public const string Host = "Host";
    public const string IfMatch = "If-Match";
    public const string IfModifiedSince = "If-Modified-Since";
    public const string IfNoneMatch = "If-None-Match";
    public const string IfRange = "If-Range";
    public const string IfUnmodifiedSince = "If-Unmodified-Since";
    public const string MaxForwards = "Max-Forwards";
    public const string Origin = "Origin";
    public const string Pragma = "Pragma";
    public const string ProxyAuthorization = "Proxy-Authorization";
    public const string Range = "Range";
    public const string Referer = "Referer";
    public const string TE = "TE";
    public const string UserAgent = "User-Agent";
    public const string Upgrade = "Upgrade";
    public const string Via = "Via";
    public const string Warning = "Warning";

    // Standard Response Headers
    public const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
    public const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";
    public const string AccessControlExposeHeaders = "Access-Control-Expose-Headers";
    public const string AccessControlMaxAge = "Access-Control-Max-Age";
    public const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
    public const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
    public const string AcceptPatch = "Accept-Patch";
    public const string AcceptRanges = "Accept-Ranges";
    public const string Age = "Age";
    public const string Allow = "Allow";
    public const string AltSvc = "Alt-Svc";
    public const string ContentDisposition = "Content-Disposition";
    public const string ContentEncoding = "Content-Encoding";
    public const string ContentLanguage = "Content-Language";
    public const string ContentLocation = "Content-Location";
    public const string ContentRange = "Content-Range";
    public const string ETag = "ETag";
    public const string Expires = "Expires";
    public const string LastModified = "Last-Modified";
    public const string Link = "Link";
    public const string Location = "Location";
    public const string RetryAfter = "Retry-After";
    public const string Server = "Server";
    public const string SetCookie = "Set-Cookie";
    public const string StrictTransportSecurity = "Strict-Transport-Security";
    public const string TransferEncoding = "Transfer-Encoding";
    public const string UpgradeInsecureRequests = "Upgrade-Insecure-Requests";
    public const string Vary = "Vary";
    public const string WWWAuthenticate = "WWW-Authenticate";
    public const string XFrameOptions = "X-Frame-Options";

    // Secure Communication Headers
    public const string RequestNonce = "X-Request-Nonce";
    public const string RequestSignature = "X-Request-Signature";
    public const string RequestKey = "X-Request-Key";
    public const string RequestEncrypted = "X-Request-Encrypted";
}
