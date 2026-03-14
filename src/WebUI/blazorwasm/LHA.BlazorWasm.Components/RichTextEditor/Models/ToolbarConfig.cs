namespace LHA.BlazorWasm.Components.RichTextEditor.Models;

/// <summary>
/// Defines which toolbar groups/buttons are shown and their layout.
/// </summary>
public class ToolbarConfig
{
    /// <summary>
    /// Toolbar rows configuration. Each row is a list of toolbar groups.
    /// Each group is a list of toolbar item identifiers.
    /// </summary>
    public List<List<List<string>>> Rows { get; set; } = [];

    /// <summary>
    /// Default toolbar matching richtexteditor.com demo layout.
    /// Row 1: Text formatting. Row 2: Utilities & Insert.
    /// </summary>
    public static ToolbarConfig Default => new()
    {
        Rows =
        [
            // Row 1: Text formatting & alignment
            [
                ["bold", "italic", "underline"],
                ["foreColor", "backColor"],
                ["justifyLeft", "justifyCenter", "justifyRight", "justifyFull"],
                ["insertOrderedList", "insertUnorderedList"],
                ["outdent", "indent"],
                ["blockquote", "emoji"],
                ["paragraphMarks"]
            ],
            // Row 2: Editing, Insert, View
            [
                ["removeFormat", "cut", "copy", "paste"],
                ["cleanCode", "findReplace"],
                ["createLink", "specialChars"],
                ["insertTable", "insertImage", "insertMedia", "insertFile", "insertTemplate"],
                ["preview", "sourceCode", "fullscreen"]
            ]
        ]
    };

    /// <summary>
    /// Minimal toolbar with just basic formatting.
    /// </summary>
    public static ToolbarConfig Minimal => new()
    {
        Rows =
        [
            [
                ["bold", "italic", "underline"],
                ["justifyLeft", "justifyCenter", "justifyRight"],
                ["insertOrderedList", "insertUnorderedList"],
                ["createLink", "insertImage"],
                ["sourceCode"]
            ]
        ]
    };
}
