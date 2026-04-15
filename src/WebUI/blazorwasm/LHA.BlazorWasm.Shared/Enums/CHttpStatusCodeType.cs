using LHA.BlazorWasm.Shared.Models.StatusBadge;

namespace LHA.BlazorWasm.Shared
{
    public enum CHttpStatusCodeType
    {
        [StatusBadge(CBadgeSemantic.Unknown, Icon = "bi-question-circle", Tooltip = "HttpStatus.Unknown", Variant = CBadgeVariant.Outline)]
        None = 0,

        // =========================
        // 1xx Informational
        // =========================
        [StatusBadge(CBadgeSemantic.Processing, Icon = "bi-hourglass", Tooltip = "HttpStatus.Continue", Variant = CBadgeVariant.Outline)]
        Continue = 100,

        [StatusBadge(CBadgeSemantic.Processing, Icon = "bi-arrow-left-right", Tooltip = "HttpStatus.SwitchingProtocols", Variant = CBadgeVariant.Outline)]
        SwitchingProtocols = 101,

        [StatusBadge(CBadgeSemantic.Processing, Icon = "bi-gear", Tooltip = "HttpStatus.Processing", Variant = CBadgeVariant.Outline)]
        Processing = 102,

        [StatusBadge(CBadgeSemantic.Processing, Icon = "bi-lightning", Tooltip = "HttpStatus.EarlyHints", Variant = CBadgeVariant.Outline)]
        EarlyHints = 103,

        // =========================
        // 2xx Success
        // =========================
        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-check-circle", Tooltip = "HttpStatus.OK", Variant = CBadgeVariant.Outline)]
        OK = 200,

        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-plus-circle", Tooltip = "HttpStatus.Created", Variant = CBadgeVariant.Outline)]
        Created = 201,

        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-clock-history", Tooltip = "HttpStatus.Accepted", Variant = CBadgeVariant.Outline)]
        Accepted = 202,

        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-info-circle", Tooltip = "HttpStatus.NonAuthoritativeInformation", Variant = CBadgeVariant.Outline)]
        NonAuthoritativeInformation = 203,

        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-slash-circle", Tooltip = "HttpStatus.NoContent", Variant = CBadgeVariant.Outline)]
        NoContent = 204,

        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-arrow-counterclockwise", Tooltip = "HttpStatus.ResetContent", Variant = CBadgeVariant.Outline)]
        ResetContent = 205,

        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-pause-circle", Tooltip = "HttpStatus.PartialContent", Variant = CBadgeVariant.Outline)]
        PartialContent = 206,

        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-layers", Tooltip = "HttpStatus.MultiStatus", Variant = CBadgeVariant.Outline)]
        MultiStatus = 207,

        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-check2-all", Tooltip = "HttpStatus.AlreadyReported", Variant = CBadgeVariant.Outline)]
        AlreadyReported = 208,

        [StatusBadge(CBadgeSemantic.Http2xx, Icon = "bi-lightning-charge", Tooltip = "HttpStatus.IMUsed", Variant = CBadgeVariant.Outline)]
        IMUsed = 226,

        // =========================
        // 3xx Redirection
        // =========================
        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-list", Tooltip = "HttpStatus.MultipleChoices", Variant = CBadgeVariant.Outline)]
        MultipleChoices = 300,

        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-arrow-right", Tooltip = "HttpStatus.MovedPermanently", Variant = CBadgeVariant.Outline)]
        MovedPermanently = 301,

        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-arrow-right-circle", Tooltip = "HttpStatus.Found", Variant = CBadgeVariant.Outline)]
        Found = 302,

        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-arrow-return-right", Tooltip = "HttpStatus.SeeOther", Variant = CBadgeVariant.Outline)]
        SeeOther = 303,

        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-dash-circle", Tooltip = "HttpStatus.NotModified", Variant = CBadgeVariant.Outline)]
        NotModified = 304,

        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-shield", Tooltip = "HttpStatus.UseProxy", Variant = CBadgeVariant.Outline)]
        UseProxy = 305,

        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-ban", Tooltip = "HttpStatus.Unused", Variant = CBadgeVariant.Outline)]
        Unused = 306,

        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-arrow-repeat", Tooltip = "HttpStatus.TemporaryRedirect", Variant = CBadgeVariant.Outline)]
        TemporaryRedirect = 307,

        [StatusBadge(CBadgeSemantic.Http3xx, Icon = "bi-arrow-right-square", Tooltip = "HttpStatus.PermanentRedirect", Variant = CBadgeVariant.Outline)]
        PermanentRedirect = 308,

        // =========================
        // 4xx Client Errors
        // =========================
        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-exclamation-circle", Tooltip = "HttpStatus.BadRequest", Variant = CBadgeVariant.Outline)]
        BadRequest = 400,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-shield-lock", Tooltip = "HttpStatus.Unauthorized", Variant = CBadgeVariant.Outline)]
        Unauthorized = 401,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-credit-card", Tooltip = "HttpStatus.PaymentRequired", Variant = CBadgeVariant.Outline)]
        PaymentRequired = 402,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-slash-octagon", Tooltip = "HttpStatus.Forbidden", Variant = CBadgeVariant.Outline)]
        Forbidden = 403,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-search", Tooltip = "HttpStatus.NotFound", Variant = CBadgeVariant.Outline)]
        NotFound = 404,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-x-square", Tooltip = "HttpStatus.MethodNotAllowed", Variant = CBadgeVariant.Outline)]
        MethodNotAllowed = 405,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-filter-circle", Tooltip = "HttpStatus.NotAcceptable", Variant = CBadgeVariant.Outline)]
        NotAcceptable = 406,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-shield-exclamation", Tooltip = "HttpStatus.ProxyAuthenticationRequired", Variant = CBadgeVariant.Outline)]
        ProxyAuthenticationRequired = 407,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-clock", Tooltip = "HttpStatus.RequestTimeout", Variant = CBadgeVariant.Outline)]
        RequestTimeout = 408,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-shuffle", Tooltip = "HttpStatus.Conflict", Variant = CBadgeVariant.Outline)]
        Conflict = 409,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-trash", Tooltip = "HttpStatus.Gone", Variant = CBadgeVariant.Outline)]
        Gone = 410,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-text-paragraph", Tooltip = "HttpStatus.LengthRequired", Variant = CBadgeVariant.Outline)]
        LengthRequired = 411,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-check2-square", Tooltip = "HttpStatus.PreconditionFailed", Variant = CBadgeVariant.Outline)]
        PreconditionFailed = 412,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-file-earmark-x", Tooltip = "HttpStatus.PayloadTooLarge", Variant = CBadgeVariant.Outline)]
        PayloadTooLarge = 413,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-link-45deg", Tooltip = "HttpStatus.URITooLong", Variant = CBadgeVariant.Outline)]
        URITooLong = 414,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-file-earmark-break", Tooltip = "HttpStatus.UnsupportedMediaType", Variant = CBadgeVariant.Outline)]
        UnsupportedMediaType = 415,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-arrows-collapse", Tooltip = "HttpStatus.RangeNotSatisfiable", Variant = CBadgeVariant.Outline)]
        RangeNotSatisfiable = 416,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-lightbulb-off", Tooltip = "HttpStatus.ExpectationFailed", Variant = CBadgeVariant.Outline)]
        ExpectationFailed = 417,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-cup-hot", Tooltip = "HttpStatus.ImATeapot", Variant = CBadgeVariant.Outline)]
        ImATeapot = 418,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-diagram-3", Tooltip = "HttpStatus.MisdirectedRequest", Variant = CBadgeVariant.Outline)]
        MisdirectedRequest = 421,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-pencil-square", Tooltip = "HttpStatus.UnprocessableEntity", Variant = CBadgeVariant.Outline)]
        UnprocessableEntity = 422,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-lock", Tooltip = "HttpStatus.Locked", Variant = CBadgeVariant.Outline)]
        Locked = 423,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-x-octagon", Tooltip = "HttpStatus.FailedDependency", Variant = CBadgeVariant.Outline)]
        FailedDependency = 424,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-clock-history", Tooltip = "HttpStatus.TooEarly", Variant = CBadgeVariant.Outline)]
        TooEarly = 425,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-arrow-up-circle", Tooltip = "HttpStatus.UpgradeRequired", Variant = CBadgeVariant.Outline)]
        UpgradeRequired = 426,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-check-circle", Tooltip = "HttpStatus.PreconditionRequired", Variant = CBadgeVariant.Outline)]
        PreconditionRequired = 428,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-speedometer2", Tooltip = "HttpStatus.TooManyRequests", Variant = CBadgeVariant.Outline)]
        TooManyRequests = 429,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-layout-text-window", Tooltip = "HttpStatus.RequestHeaderFieldsTooLarge", Variant = CBadgeVariant.Outline)]
        RequestHeaderFieldsTooLarge = 431,

        [StatusBadge(CBadgeSemantic.Http4xx, Icon = "bi-scale", Tooltip = "HttpStatus.UnavailableForLegalReasons", Variant = CBadgeVariant.Outline)]
        UnavailableForLegalReasons = 451,

        // =========================
        // 5xx Server Errors
        // =========================
        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-x-circle", Tooltip = "HttpStatus.InternalServerError", Variant = CBadgeVariant.Outline)]
        InternalServerError = 500,

        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-tools", Tooltip = "HttpStatus.NotImplemented", Variant = CBadgeVariant.Outline)]
        NotImplemented = 501,

        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-hdd-network", Tooltip = "HttpStatus.BadGateway", Variant = CBadgeVariant.Outline)]
        BadGateway = 502,

        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-server", Tooltip = "HttpStatus.ServiceUnavailable", Variant = CBadgeVariant.Outline)]
        ServiceUnavailable = 503,

        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-clock", Tooltip = "HttpStatus.GatewayTimeout", Variant = CBadgeVariant.Outline)]
        GatewayTimeout = 504,

        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-code-slash", Tooltip = "HttpStatus.HTTPVersionNotSupported", Variant = CBadgeVariant.Outline)]
        HTTPVersionNotSupported = 505,

        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-diagram-2", Tooltip = "HttpStatus.VariantAlsoNegotiates", Variant = CBadgeVariant.Outline)]
        VariantAlsoNegotiates = 506,

        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-database-exclamation", Tooltip = "HttpStatus.InsufficientStorage", Variant = CBadgeVariant.Outline)]
        InsufficientStorage = 507,

        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-arrow-repeat", Tooltip = "HttpStatus.LoopDetected", Variant = CBadgeVariant.Outline)]
        LoopDetected = 508,

        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-puzzle", Tooltip = "HttpStatus.NotExtended", Variant = CBadgeVariant.Outline)]
        NotExtended = 510,

        [StatusBadge(CBadgeSemantic.Http5xx, Icon = "bi-shield-lock", Tooltip = "HttpStatus.NetworkAuthenticationRequired", Variant = CBadgeVariant.Outline)]
        NetworkAuthenticationRequired = 511
    }
}