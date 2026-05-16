using System.Text.Json;

namespace LHA.Notification.Infrastructure.Channels.Push.Fcm;

public sealed class FcmResponseMapper
{
    public FcmErrorResult MapError(string responseBody)
    {
        try
        {
            using JsonDocument doc = JsonDocument.Parse(responseBody);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("error", out JsonElement errorElement))
            {
                string status = errorElement.TryGetProperty("status", out JsonElement statusEl) ? statusEl.GetString() ?? "" : "";
                string message = errorElement.TryGetProperty("message", out JsonElement msgEl) ? msgEl.GetString() ?? "" : "";

                bool isTokenExpired = message.Contains("registration-token-not-registered", StringComparison.OrdinalIgnoreCase)
                    || status.Equals("NOT_FOUND", StringComparison.OrdinalIgnoreCase);

                return new FcmErrorResult(status, message, isTokenExpired);
            }
        }
        catch (JsonException)
        {
        }

        return new FcmErrorResult("UNKNOWN", responseBody, false);
    }
}

public sealed record FcmErrorResult(string Status, string ErrorMessage, bool IsTokenExpired);
