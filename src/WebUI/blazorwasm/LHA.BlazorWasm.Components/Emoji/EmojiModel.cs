namespace LHA.BlazorWasm.Components.Emoji;

/// <summary>
/// Represents a single emoji item.
/// </summary>
public record EmojiModel(string Unicode, string Name, EmojiCategory Category, string[] Keywords);
