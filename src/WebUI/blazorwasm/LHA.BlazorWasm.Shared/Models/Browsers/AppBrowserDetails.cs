namespace LHA.BlazorWasm.Shared.Models
{
    public class AppBrowserDetails
    {
        public CBrowserType Browser { get; set; } = CBrowserType.Unknown;
        public COperatingSystem OS { get; set; } = COperatingSystem.Unknown;

        public bool IsSvgIcon => Browser is CBrowserType.Postman
            or CBrowserType.Bruno
            or CBrowserType.Curl
            or CBrowserType.Grpc
            or CBrowserType.MessageQueue
            or CBrowserType.BackgroundJob;
    }
}