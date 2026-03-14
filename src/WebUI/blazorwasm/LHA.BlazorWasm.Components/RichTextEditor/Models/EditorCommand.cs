namespace LHA.BlazorWasm.Components.RichTextEditor.Models;

/// <summary>
/// All supported rich text editor commands mapped to document.execCommand names.
/// </summary>
public enum EditorCommand
{
    Bold,
    Italic,
    Underline,
    StrikeThrough,
    Subscript,
    Superscript,
    JustifyLeft,
    JustifyCenter,
    JustifyRight,
    JustifyFull,
    InsertOrderedList,
    InsertUnorderedList,
    Indent,
    Outdent,
    FormatBlock,
    CreateLink,
    Unlink,
    InsertImage,
    InsertTable,
    InsertHorizontalRule,
    InsertHtml,
    RemoveFormat,
    Undo,
    Redo,
    Copy,
    Cut,
    Paste,
    SelectAll,
    ForeColor,
    BackColor,
    FontName,
    FontSize
}
