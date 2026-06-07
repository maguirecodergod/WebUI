namespace LHA.BlazorWasm.Components.RichTextEditor.Models;

/// <summary>
/// All supported rich text editor commands mapped to document.execCommand names.
/// </summary>
public enum CEditorCommand
{
    /// <summary>
    /// 0 - Bold
    /// </summary>
    Bold,
    /// <summary>
    /// 1 - Italic
    /// </summary>
    Italic,
    /// <summary>
    /// 2 - Underline
    /// </summary>
    Underline,
    /// <summary>
    /// 3 - StrikeThrough
    /// </summary>
    StrikeThrough,
    /// <summary>
    /// 4 - Subscript
    /// </summary>
    Subscript,
    /// <summary>
    /// 5 - Superscript
    /// </summary>
    Superscript,
    /// <summary>
    /// 6 - JustifyLeft
    /// </summary>
    JustifyLeft,
    /// <summary>
    /// 7 - JustifyCenter
    /// </summary>
    JustifyCenter,
    /// <summary>
    /// 8 - JustifyRight
    /// </summary>
    JustifyRight,
    /// <summary>
    /// 9 - JustifyFull
    /// </summary>
    JustifyFull,
    /// <summary>
    /// 10 - InsertOrderedList
    /// </summary>
    InsertOrderedList,
    /// <summary>
    /// 11 - InsertUnorderedList
    /// </summary>
    InsertUnorderedList,
    /// <summary>
    /// 12 - Indent
    /// </summary>
    Indent,
    /// <summary>
    /// 13 - Outdent
    /// </summary>
    Outdent,
    /// <summary>
    /// 14 - FormatBlock
    /// </summary>
    FormatBlock,
    /// <summary>
    /// 15 - CreateLink
    /// </summary>
    CreateLink,
    /// <summary>
    /// 16 - Unlink
    /// </summary>
    Unlink,
    /// <summary>
    /// 17 - InsertImage
    /// </summary>
    InsertImage,
    /// <summary>
    /// 18 - InsertTable
    /// </summary>
    InsertTable,
    /// <summary>
    /// 19 - InsertHorizontalRule
    /// </summary>
    InsertHorizontalRule,
    /// <summary>
    /// 20 - InsertHtml
    /// </summary>
    InsertHtml,
    /// <summary>
    /// 21 - RemoveFormat
    /// </summary>
    RemoveFormat,
    /// <summary>
    /// 22 - Undo
    /// </summary>
    Undo,
    /// <summary>
    /// 23 - Redo
    /// </summary>
    Redo,
    /// <summary>
    /// 24 - Copy
    /// </summary>
    Copy,
    /// <summary>
    /// 25 - Cut
    /// </summary>
    Cut,
    /// <summary>
    /// 26 - Paste
    /// </summary>
    Paste,
    /// <summary>
    /// 27 - SelectAll
    /// </summary>
    SelectAll,
    /// <summary>
    /// 28 - ForeColor
    /// </summary>
    ForeColor,
    /// <summary>
    /// 29 - BackColor
    /// </summary>
    BackColor,
    /// <summary>
    /// 30 - FontName
    /// </summary>
    FontName,
    /// <summary>
    /// 31 - FontSize
    /// </summary>
    FontSize
}
