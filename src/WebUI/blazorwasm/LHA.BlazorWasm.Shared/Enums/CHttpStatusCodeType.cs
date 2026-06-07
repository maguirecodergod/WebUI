using LHA;

namespace LHA.BlazorWasm.Shared
{
    public enum CHttpStatusCodeType
    {
        /// <summary>
        /// 0 - None
        /// </summary>
        [StatusBadge(CBadgeSemantic.Unknown, Icon = "bi-question-circle", Tooltip = "HttpStatus.Unknown", Variant = CBadgeVariant.Outline)]
        None = 0,

        // =========================
        // 1xx Informational
        // =========================
        /// <summary>
        /// 100 - Continue
        /// </summary>
        [StatusBadge(CBadgeSemantic.Processing, Icon = "bi-hourglass", Tooltip = "HttpStatus.Continue", Variant = CBadgeVariant.Outline)]
        Continue = 100,

        /// <summary>
        /// 101 - SwitchingProtocols
        /// </summary>
        [StatusBadge(CBadgeSemantic.Processing, Icon = "bi-arrow-left-right", Tooltip = "HttpStatus.SwitchingProtocols", Variant = CBadgeVariant.Outline)]
        SwitchingProtocols = 101,

        /// <summary>
        /// 102 - Processing
        /// </summary>
        [StatusBadge(CBadgeSemantic.Processing, Icon = "bi-gear", Tooltip = "HttpStatus.Processing", Variant = CBadgeVariant.Outline)]
        Processing = 102,

        /// <summary>
        /// 103 - EarlyHints
        /// </summary>
        [StatusBadge(CBadgeSemantic.Processing, Icon = "bi-lightning", Tooltip = "HttpStatus.EarlyHints", Variant = CBadgeVariant.Outline)]
        EarlyHints = 103,

        // =========================
        // 2xx Success
        // =========================
        /// <summary>
        /// 200 - OK
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-check-circle", Tooltip = "HttpStatus.OK", Variant = CBadgeVariant.Outline)]
        OK = 200,

        /// <summary>
        /// 201 - Created
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-plus-circle", Tooltip = "HttpStatus.Created", Variant = CBadgeVariant.Outline)]
        Created = 201,

        /// <summary>
        /// 202 - Accepted
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-clock-history", Tooltip = "HttpStatus.Accepted", Variant = CBadgeVariant.Outline)]
        Accepted = 202,

        /// <summary>
        /// 203 - NonAuthoritativeInformation
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-info-circle", Tooltip = "HttpStatus.NonAuthoritativeInformation", Variant = CBadgeVariant.Outline)]
        NonAuthoritativeInformation = 203,

        /// <summary>
        /// 204 - NoContent
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-slash-circle", Tooltip = "HttpStatus.NoContent", Variant = CBadgeVariant.Outline)]
        NoContent = 204,

        /// <summary>
        /// 205 - ResetContent
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-arrow-counterclockwise", Tooltip = "HttpStatus.ResetContent", Variant = CBadgeVariant.Outline)]
        ResetContent = 205,

        /// <summary>
        /// 206 - PartialContent
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-pause-circle", Tooltip = "HttpStatus.PartialContent", Variant = CBadgeVariant.Outline)]
        PartialContent = 206,

        /// <summary>
        /// 207 - MultiStatus
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-layers", Tooltip = "HttpStatus.MultiStatus", Variant = CBadgeVariant.Outline)]
        MultiStatus = 207,

        /// <summary>
        /// 208 - AlreadyReported
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-check2-all", Tooltip = "HttpStatus.AlreadyReported", Variant = CBadgeVariant.Outline)]
        AlreadyReported = 208,

        /// <summary>
        /// 226 - IMUsed
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-lightning-charge", Tooltip = "HttpStatus.IMUsed", Variant = CBadgeVariant.Outline)]
        IMUsed = 226,

        // =========================
        // 3xx Redirection
        // =========================
        /// <summary>
        /// 300 - MultipleChoices
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-list", Tooltip = "HttpStatus.MultipleChoices", Variant = CBadgeVariant.Outline)]
        MultipleChoices = 300,

        /// <summary>
        /// 301 - MovedPermanently
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-arrow-right", Tooltip = "HttpStatus.MovedPermanently", Variant = CBadgeVariant.Outline)]
        MovedPermanently = 301,

        /// <summary>
        /// 302 - Found
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-arrow-right-circle", Tooltip = "HttpStatus.Found", Variant = CBadgeVariant.Outline)]
        Found = 302,

        /// <summary>
        /// 303 - SeeOther
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-arrow-return-right", Tooltip = "HttpStatus.SeeOther", Variant = CBadgeVariant.Outline)]
        SeeOther = 303,

        /// <summary>
        /// 304 - NotModified
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-dash-circle", Tooltip = "HttpStatus.NotModified", Variant = CBadgeVariant.Outline)]
        NotModified = 304,

        /// <summary>
        /// 305 - UseProxy
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-shield", Tooltip = "HttpStatus.UseProxy", Variant = CBadgeVariant.Outline)]
        UseProxy = 305,

        /// <summary>
        /// 306 - Unused
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-ban", Tooltip = "HttpStatus.Unused", Variant = CBadgeVariant.Outline)]
        Unused = 306,

        /// <summary>
        /// 307 - TemporaryRedirect
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-arrow-repeat", Tooltip = "HttpStatus.TemporaryRedirect", Variant = CBadgeVariant.Outline)]
        TemporaryRedirect = 307,

        /// <summary>
        /// 308 - PermanentRedirect
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-arrow-right-square", Tooltip = "HttpStatus.PermanentRedirect", Variant = CBadgeVariant.Outline)]
        PermanentRedirect = 308,

        // =========================
        // 4xx Client Errors
        // =========================
        /// <summary>
        /// 400 - BadRequest
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-exclamation-circle", Tooltip = "HttpStatus.BadRequest", Variant = CBadgeVariant.Outline)]
        BadRequest = 400,

        /// <summary>
        /// 401 - Unauthorized
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-shield-lock", Tooltip = "HttpStatus.Unauthorized", Variant = CBadgeVariant.Outline)]
        Unauthorized = 401,

        /// <summary>
        /// 402 - PaymentRequired
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-credit-card", Tooltip = "HttpStatus.PaymentRequired", Variant = CBadgeVariant.Outline)]
        PaymentRequired = 402,

        /// <summary>
        /// 403 - Forbidden
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-slash-octagon", Tooltip = "HttpStatus.Forbidden", Variant = CBadgeVariant.Outline)]
        Forbidden = 403,

        /// <summary>
        /// 404 - NotFound
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-search", Tooltip = "HttpStatus.NotFound", Variant = CBadgeVariant.Outline)]
        NotFound = 404,

        /// <summary>
        /// 405 - MethodNotAllowed
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-x-square", Tooltip = "HttpStatus.MethodNotAllowed", Variant = CBadgeVariant.Outline)]
        MethodNotAllowed = 405,

        /// <summary>
        /// 406 - NotAcceptable
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-filter-circle", Tooltip = "HttpStatus.NotAcceptable", Variant = CBadgeVariant.Outline)]
        NotAcceptable = 406,

        /// <summary>
        /// 407 - ProxyAuthenticationRequired
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-shield-exclamation", Tooltip = "HttpStatus.ProxyAuthenticationRequired", Variant = CBadgeVariant.Outline)]
        ProxyAuthenticationRequired = 407,

        /// <summary>
        /// 408 - RequestTimeout
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-clock", Tooltip = "HttpStatus.RequestTimeout", Variant = CBadgeVariant.Outline)]
        RequestTimeout = 408,

        /// <summary>
        /// 409 - Conflict
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-shuffle", Tooltip = "HttpStatus.Conflict", Variant = CBadgeVariant.Outline)]
        Conflict = 409,

        /// <summary>
        /// 410 - Gone
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-trash", Tooltip = "HttpStatus.Gone", Variant = CBadgeVariant.Outline)]
        Gone = 410,

        /// <summary>
        /// 411 - LengthRequired
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-text-paragraph", Tooltip = "HttpStatus.LengthRequired", Variant = CBadgeVariant.Outline)]
        LengthRequired = 411,

        /// <summary>
        /// 412 - PreconditionFailed
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-check2-square", Tooltip = "HttpStatus.PreconditionFailed", Variant = CBadgeVariant.Outline)]
        PreconditionFailed = 412,

        /// <summary>
        /// 413 - PayloadTooLarge
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-file-earmark-x", Tooltip = "HttpStatus.PayloadTooLarge", Variant = CBadgeVariant.Outline)]
        PayloadTooLarge = 413,

        /// <summary>
        /// 414 - URITooLong
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-link-45deg", Tooltip = "HttpStatus.URITooLong", Variant = CBadgeVariant.Outline)]
        URITooLong = 414,

        /// <summary>
        /// 415 - UnsupportedMediaType
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-file-earmark-break", Tooltip = "HttpStatus.UnsupportedMediaType", Variant = CBadgeVariant.Outline)]
        UnsupportedMediaType = 415,

        /// <summary>
        /// 416 - RangeNotSatisfiable
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-arrows-collapse", Tooltip = "HttpStatus.RangeNotSatisfiable", Variant = CBadgeVariant.Outline)]
        RangeNotSatisfiable = 416,

        /// <summary>
        /// 417 - ExpectationFailed
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-lightbulb-off", Tooltip = "HttpStatus.ExpectationFailed", Variant = CBadgeVariant.Outline)]
        ExpectationFailed = 417,

        /// <summary>
        /// 418 - ImATeapot
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-cup-hot", Tooltip = "HttpStatus.ImATeapot", Variant = CBadgeVariant.Outline)]
        ImATeapot = 418,

        /// <summary>
        /// 421 - MisdirectedRequest
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-diagram-3", Tooltip = "HttpStatus.MisdirectedRequest", Variant = CBadgeVariant.Outline)]
        MisdirectedRequest = 421,

        /// <summary>
        /// 422 - UnprocessableEntity
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-pencil-square", Tooltip = "HttpStatus.UnprocessableEntity", Variant = CBadgeVariant.Outline)]
        UnprocessableEntity = 422,

        /// <summary>
        /// 423 - Locked
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-lock", Tooltip = "HttpStatus.Locked", Variant = CBadgeVariant.Outline)]
        Locked = 423,

        /// <summary>
        /// 424 - FailedDependency
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-x-octagon", Tooltip = "HttpStatus.FailedDependency", Variant = CBadgeVariant.Outline)]
        FailedDependency = 424,

        /// <summary>
        /// 425 - TooEarly
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-clock-history", Tooltip = "HttpStatus.TooEarly", Variant = CBadgeVariant.Outline)]
        TooEarly = 425,

        /// <summary>
        /// 426 - UpgradeRequired
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-arrow-up-circle", Tooltip = "HttpStatus.UpgradeRequired", Variant = CBadgeVariant.Outline)]
        UpgradeRequired = 426,

        /// <summary>
        /// 428 - PreconditionRequired
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-check-circle", Tooltip = "HttpStatus.PreconditionRequired", Variant = CBadgeVariant.Outline)]
        PreconditionRequired = 428,

        /// <summary>
        /// 429 - TooManyRequests
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-speedometer2", Tooltip = "HttpStatus.TooManyRequests", Variant = CBadgeVariant.Outline)]
        TooManyRequests = 429,

        /// <summary>
        /// 431 - RequestHeaderFieldsTooLarge
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-layout-text-window", Tooltip = "HttpStatus.RequestHeaderFieldsTooLarge", Variant = CBadgeVariant.Outline)]
        RequestHeaderFieldsTooLarge = 431,

        /// <summary>
        /// 451 - UnavailableForLegalReasons
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-scale", Tooltip = "HttpStatus.UnavailableForLegalReasons", Variant = CBadgeVariant.Outline)]
        UnavailableForLegalReasons = 451,

        // =========================
        // 5xx Server Errors
        // =========================
        /// <summary>
        /// 500 - InternalServerError
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-x-circle", Tooltip = "HttpStatus.InternalServerError", Variant = CBadgeVariant.Outline)]
        InternalServerError = 500,

        /// <summary>
        /// 501 - NotImplemented
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-tools", Tooltip = "HttpStatus.NotImplemented", Variant = CBadgeVariant.Outline)]
        NotImplemented = 501,

        /// <summary>
        /// 502 - BadGateway
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-hdd-network", Tooltip = "HttpStatus.BadGateway", Variant = CBadgeVariant.Outline)]
        BadGateway = 502,

        /// <summary>
        /// 503 - ServiceUnavailable
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-server", Tooltip = "HttpStatus.ServiceUnavailable", Variant = CBadgeVariant.Outline)]
        ServiceUnavailable = 503,

        /// <summary>
        /// 504 - GatewayTimeout
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-clock", Tooltip = "HttpStatus.GatewayTimeout", Variant = CBadgeVariant.Outline)]
        GatewayTimeout = 504,

        /// <summary>
        /// 505 - HTTPVersionNotSupported
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-code-slash", Tooltip = "HttpStatus.HTTPVersionNotSupported", Variant = CBadgeVariant.Outline)]
        HTTPVersionNotSupported = 505,

        /// <summary>
        /// 506 - VariantAlsoNegotiates
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-diagram-2", Tooltip = "HttpStatus.VariantAlsoNegotiates", Variant = CBadgeVariant.Outline)]
        VariantAlsoNegotiates = 506,

        /// <summary>
        /// 507 - InsufficientStorage
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-database-exclamation", Tooltip = "HttpStatus.InsufficientStorage", Variant = CBadgeVariant.Outline)]
        InsufficientStorage = 507,

        /// <summary>
        /// 508 - LoopDetected
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-arrow-repeat", Tooltip = "HttpStatus.LoopDetected", Variant = CBadgeVariant.Outline)]
        LoopDetected = 508,

        /// <summary>
        /// 510 - NotExtended
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-puzzle", Tooltip = "HttpStatus.NotExtended", Variant = CBadgeVariant.Outline)]
        NotExtended = 510,

        /// <summary>
        /// 511 - NetworkAuthenticationRequired
        /// </summary>
        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-shield-lock", Tooltip = "HttpStatus.NetworkAuthenticationRequired", Variant = CBadgeVariant.Outline)]
        NetworkAuthenticationRequired = 511
    }
}