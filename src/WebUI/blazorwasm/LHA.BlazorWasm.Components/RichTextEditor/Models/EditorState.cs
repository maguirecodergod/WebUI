namespace LHA.BlazorWasm.Components.RichTextEditor.Models;

/// <summary>
/// Represents the current formatting state of the editor selection.
/// Used to sync toolbar button active states.
/// </summary>
public class EditorState
{
    public bool IsBold { get; set; }
    public bool IsItalic { get; set; }
    public bool IsUnderline { get; set; }
    public bool IsStrikeThrough { get; set; }
    public bool IsSubscript { get; set; }
    public bool IsSuperscript { get; set; }
    public bool IsOrderedList { get; set; }
    public bool IsUnorderedList { get; set; }

    public string Alignment { get; set; } = "left";
    public string FontColor { get; set; } = "#000000";
    public string BackColor { get; set; } = "transparent";
    public string FontName { get; set; } = "";
    public string FontSize { get; set; } = "";
    public string CurrentBlock { get; set; } = "p";

    public int WordCount { get; set; }
    public int CharCount { get; set; }

    /// <summary>
    /// HTML tag path breadcrumb, e.g., "body > div > p > strong"
    /// </summary>
    public string ElementPath { get; set; } = "";

    /// <summary>
    /// Whether the editor is currently in source code view mode.
    /// </summary>
    public bool IsSourceView { get; set; }

    /// <summary>
    /// Whether the editor is in fullscreen mode.
    /// </summary>
    public bool IsFullscreen { get; set; }
}
