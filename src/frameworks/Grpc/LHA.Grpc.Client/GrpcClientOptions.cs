namespace LHA.Grpc.Client;

public sealed class GrpcClientOptions
{
    /// <summary>
    /// Default deadline applied to calls that don't specify one.
    /// Set to <c>null</c> to disable automatic deadline enforcement.
    /// </summary>
    public TimeSpan? DefaultDeadline { get; set; } = TimeSpan.FromSeconds(30);
}
