using LHA.BlazorWasm.Shared.Models.StatusBadge;

namespace LHA.BlazorWasm.Shared
{
    public enum CHttpMethodType
    {
        [StatusBadge(CBadgeSemantic.Unknown)]
        None = 0,

        [StatusBadge(CBadgeSemantic.HttpGet, Icon = "bi-arrow-down-circle")]
        GET = 1,

        [StatusBadge(CBadgeSemantic.HttpPost, Icon = "bi-plus-circle")]
        POST = 2,

        [StatusBadge(CBadgeSemantic.HttpPut, Icon = "bi-pencil-square")]
        PUT = 3,

        [StatusBadge(CBadgeSemantic.HttpPatch, Icon = "bi-wrench-adjustable")]
        PATCH = 4,

        [StatusBadge(CBadgeSemantic.HttpDelete, Icon = "bi-trash")]
        DELETE = 5,

        [StatusBadge(CBadgeSemantic.HttpHead, Icon = "bi-dash-circle")]
        HEAD = 6,

        [StatusBadge(CBadgeSemantic.HttpOptions, Icon = "bi-sliders")]
        OPTIONS = 7,

        [StatusBadge(CBadgeSemantic.HttpTrace, Icon = "bi-activity")]
        TRACE = 8,

        [StatusBadge(CBadgeSemantic.HttpConnect, Icon = "bi-diagram-3")]
        CONNECT = 9,

        [StatusBadge(CBadgeSemantic.Unknown, Icon = "bi-question-circle")]
        Other = 10
    }
}