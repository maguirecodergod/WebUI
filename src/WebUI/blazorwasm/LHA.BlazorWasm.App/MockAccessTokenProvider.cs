using LHA.BlazorWasm.HttpApi.Client.Abstractions;

namespace LHA.BlazorWasm.App.Services;

public class MockAccessTokenProvider : IAccessTokenProvider
{
    public ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        // Return a dummy token for testing purposes
        return new ValueTask<string?>("mock-test-token");
    }
}
