using LHA;

namespace LHA.BlazorWasm.Shared
{
    public enum CHttpMethodType
    {
        /// <summary>
        /// 0 - None
        /// </summary>
        [StatusBadge(CBadgeSemantic.Unknown)]
        None = 0,

        /// <summary>
        /// 1 - GET
        /// </summary>
        [StatusBadge(CBadgeSemantic.HttpGet, Icon = "bi-arrow-down-circle")]
        GET = 1,

        /// <summary>
        /// 2 - POST
        /// </summary>
        [StatusBadge(CBadgeSemantic.HttpPost, Icon = "bi-plus-circle")]
        POST = 2,

        /// <summary>
        /// 3 - PUT
        /// </summary>
        [StatusBadge(CBadgeSemantic.HttpPut, Icon = "bi-pencil-square")]
        PUT = 3,

        /// <summary>
        /// 4 - PATCH
        /// </summary>
        [StatusBadge(CBadgeSemantic.HttpPatch, Icon = "bi-wrench-adjustable")]
        PATCH = 4,

        /// <summary>
        /// 5 - DELETE
        /// </summary>
        [StatusBadge(CBadgeSemantic.HttpDelete, Icon = "bi-trash")]
        DELETE = 5,

        /// <summary>
        /// 6 - HEAD
        /// </summary>
        [StatusBadge(CBadgeSemantic.HttpHead, Icon = "bi-dash-circle")]
        HEAD = 6,

        /// <summary>
        /// 7 - OPTIONS
        /// </summary>
        [StatusBadge(CBadgeSemantic.HttpOptions, Icon = "bi-sliders")]
        OPTIONS = 7,

        /// <summary>
        /// 8 - TRACE
        /// </summary>
        [StatusBadge(CBadgeSemantic.HttpTrace, Icon = "bi-activity")]
        TRACE = 8,

        /// <summary>
        /// 9 - CONNECT
        /// </summary>
        [StatusBadge(CBadgeSemantic.HttpConnect, Icon = "bi-diagram-3")]
        CONNECT = 9,

        /// <summary>
        /// 10 - Other
        /// </summary>
        [StatusBadge(CBadgeSemantic.Unknown, Icon = "bi-question-circle")]
        Other = 10
    }
}