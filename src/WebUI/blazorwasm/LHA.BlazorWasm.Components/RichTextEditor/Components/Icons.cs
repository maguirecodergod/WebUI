namespace LHA.BlazorWasm.Components.RichTextEditor.Components;

/// <summary>
/// SVG icon constants for the RichTextEditor toolbar buttons.
/// All icons are 20x20 viewBox, monochrome with currentColor.
/// </summary>
public static class Icons
{
    private const string V = "viewBox=\"0 0 24 24\" width=\"18\" height=\"18\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"";

    public const string Bold = $"<svg {V}><path d=\"M6 4h8a4 4 0 0 1 4 4 4 4 0 0 1-4 4H6z\"/><path d=\"M6 12h9a4 4 0 0 1 4 4 4 4 0 0 1-4 4H6z\"/></svg>";
    public const string Italic = $"<svg {V}><line x1=\"19\" y1=\"4\" x2=\"10\" y2=\"4\"/><line x1=\"14\" y1=\"20\" x2=\"5\" y2=\"20\"/><line x1=\"15\" y1=\"4\" x2=\"9\" y2=\"20\"/></svg>";
    public const string Underline = $"<svg {V}><path d=\"M6 4v6a6 6 0 0 0 12 0V4\"/><line x1=\"4\" y1=\"20\" x2=\"20\" y2=\"20\"/></svg>";
    public const string StrikeThrough = $"<svg {V}><line x1=\"4\" y1=\"12\" x2=\"20\" y2=\"12\"/><path d=\"M16 4H9a3 3 0 0 0 0 6h6a3 3 0 0 1 0 6H8\"/></svg>";

    public const string FontColor = "<svg viewBox=\"0 0 24 24\" width=\"18\" height=\"18\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\"><path d=\"M5 18h14\" stroke=\"#e53e3e\" stroke-width=\"3\"/><path d=\"M9 4l-4 12h2.5l1-3h7l1 3H19L15 4H9zm0 7l2.5-5.5L14 11H9z\" fill=\"currentColor\" stroke=\"none\"/></svg>";
    public const string BackColor = "<svg viewBox=\"0 0 24 24\" width=\"18\" height=\"18\" fill=\"none\"><rect x=\"3\" y=\"15\" width=\"18\" height=\"4\" rx=\"1\" fill=\"#ffd93d\"/><path d=\"M11 2L5 12h12L11 2z\" stroke=\"currentColor\" stroke-width=\"2\" fill=\"none\"/><path d=\"M18 10l2.5 4c.5 1-.2 2-1.5 2s-2-1-1.5-2L20 10\" stroke=\"currentColor\" stroke-width=\"1.5\"/></svg>";

    public const string AlignLeft = $"<svg {V}><line x1=\"3\" y1=\"6\" x2=\"21\" y2=\"6\"/><line x1=\"3\" y1=\"12\" x2=\"15\" y2=\"12\"/><line x1=\"3\" y1=\"18\" x2=\"21\" y2=\"18\"/></svg>";
    public const string AlignCenter = $"<svg {V}><line x1=\"3\" y1=\"6\" x2=\"21\" y2=\"6\"/><line x1=\"6\" y1=\"12\" x2=\"18\" y2=\"12\"/><line x1=\"3\" y1=\"18\" x2=\"21\" y2=\"18\"/></svg>";
    public const string AlignRight = $"<svg {V}><line x1=\"3\" y1=\"6\" x2=\"21\" y2=\"6\"/><line x1=\"9\" y1=\"12\" x2=\"21\" y2=\"12\"/><line x1=\"3\" y1=\"18\" x2=\"21\" y2=\"18\"/></svg>";
    public const string AlignJustify = $"<svg {V}><line x1=\"3\" y1=\"6\" x2=\"21\" y2=\"6\"/><line x1=\"3\" y1=\"12\" x2=\"21\" y2=\"12\"/><line x1=\"3\" y1=\"18\" x2=\"21\" y2=\"18\"/></svg>";

    public const string OrderedList = $"<svg {V}><line x1=\"10\" y1=\"6\" x2=\"21\" y2=\"6\"/><line x1=\"10\" y1=\"12\" x2=\"21\" y2=\"12\"/><line x1=\"10\" y1=\"18\" x2=\"21\" y2=\"18\"/><text x=\"4\" y=\"7\" font-size=\"6\" fill=\"currentColor\" stroke=\"none\" font-family=\"sans-serif\">1</text><text x=\"4\" y=\"13\" font-size=\"6\" fill=\"currentColor\" stroke=\"none\" font-family=\"sans-serif\">2</text><text x=\"4\" y=\"19\" font-size=\"6\" fill=\"currentColor\" stroke=\"none\" font-family=\"sans-serif\">3</text></svg>";
    public const string UnorderedList = $"<svg {V}><line x1=\"10\" y1=\"6\" x2=\"21\" y2=\"6\"/><line x1=\"10\" y1=\"12\" x2=\"21\" y2=\"12\"/><line x1=\"10\" y1=\"18\" x2=\"21\" y2=\"18\"/><circle cx=\"5\" cy=\"6\" r=\"1.5\" fill=\"currentColor\" stroke=\"none\"/><circle cx=\"5\" cy=\"12\" r=\"1.5\" fill=\"currentColor\" stroke=\"none\"/><circle cx=\"5\" cy=\"18\" r=\"1.5\" fill=\"currentColor\" stroke=\"none\"/></svg>";

    public const string Indent = $"<svg {V}><line x1=\"12\" y1=\"6\" x2=\"21\" y2=\"6\"/><line x1=\"12\" y1=\"12\" x2=\"21\" y2=\"12\"/><line x1=\"12\" y1=\"18\" x2=\"21\" y2=\"18\"/><polyline points=\"3 8 7 12 3 16\"/></svg>";
    public const string Outdent = $"<svg {V}><line x1=\"12\" y1=\"6\" x2=\"21\" y2=\"6\"/><line x1=\"12\" y1=\"12\" x2=\"21\" y2=\"12\"/><line x1=\"12\" y1=\"18\" x2=\"21\" y2=\"18\"/><polyline points=\"7 8 3 12 7 16\"/></svg>";

    public const string Blockquote = "<svg viewBox=\"0 0 24 24\" width=\"18\" height=\"18\" fill=\"currentColor\" stroke=\"none\"><path d=\"M10 8c-1.1 0-2 .9-2 2v2c0 1.1.9 2 2 2h2l-2 4h2.5l2-4v-4c0-1.1-.9-2-2-2h-2.5zM4 8c-1.1 0-2 .9-2 2v2c0 1.1.9 2 2 2h2l-2 4h2.5l2-4v-4c0-1.1-.9-2-2-2H4z\"/></svg>";

    public const string ClearFormat = "<svg viewBox=\"0 0 24 24\" width=\"18\" height=\"18\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\"><path d=\"M8 4h9l-4 16\"/><line x1=\"3\" y1=\"20\" x2=\"13\" y2=\"20\"/><line x1=\"16\" y1=\"4\" x2=\"21\" y2=\"9\" stroke=\"#e53e3e\"/><line x1=\"21\" y1=\"4\" x2=\"16\" y2=\"9\" stroke=\"#e53e3e\"/></svg>";
    public const string Cut = $"<svg {V}><circle cx=\"6\" cy=\"18\" r=\"3\"/><circle cx=\"18\" cy=\"18\" r=\"3\"/><line x1=\"8.6\" y1=\"15.4\" x2=\"18\" y2=\"4\"/><line x1=\"15.4\" y1=\"15.4\" x2=\"6\" y2=\"4\"/></svg>";
    public const string Copy = $"<svg {V}><rect x=\"9\" y=\"9\" width=\"13\" height=\"13\" rx=\"2\" ry=\"2\"/><path d=\"M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1\"/></svg>";
    public const string Paste = $"<svg {V}><path d=\"M16 4h2a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h2\"/><rect x=\"8\" y=\"2\" width=\"8\" height=\"4\" rx=\"1\" ry=\"1\"/></svg>";

    public const string Undo = $"<svg {V}><polyline points=\"1 4 1 10 7 10\"/><path d=\"M3.51 15a9 9 0 1 0 2.13-9.36L1 10\"/></svg>";
    public const string Redo = $"<svg {V}><polyline points=\"23 4 23 10 17 10\"/><path d=\"M20.49 15a9 9 0 1 1-2.13-9.36L23 10\"/></svg>";

    public const string Link = $"<svg {V}><path d=\"M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71\"/><path d=\"M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71\"/></svg>";
    public const string Unlink = "<svg viewBox=\"0 0 24 24\" width=\"18\" height=\"18\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\"><path d=\"M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71\"/><path d=\"M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71\"/><line x1=\"4\" y1=\"20\" x2=\"20\" y2=\"4\" stroke=\"#e53e3e\" stroke-width=\"2\"/></svg>";

    public const string SpecialChars = "<svg viewBox=\"0 0 24 24\" width=\"18\" height=\"18\" fill=\"currentColor\" stroke=\"none\"><text x=\"4\" y=\"18\" font-size=\"16\" font-family=\"serif\">Ω</text></svg>";
    public const string Emoji = $"<svg {V}><circle cx=\"12\" cy=\"12\" r=\"10\"/><path d=\"M8 14s1.5 2 4 2 4-2 4-2\"/><line x1=\"9\" y1=\"9\" x2=\"9.01\" y2=\"9\"/><line x1=\"15\" y1=\"9\" x2=\"15.01\" y2=\"9\"/></svg>";
    public const string HorizontalRule = "<svg viewBox=\"0 0 24 24\" width=\"18\" height=\"18\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\"><line x1=\"5\" y1=\"12\" x2=\"19\" y2=\"12\"/><line x1=\"2\" y1=\"12\" x2=\"3\" y2=\"12\"/><line x1=\"21\" y1=\"12\" x2=\"22\" y2=\"12\"/></svg>";

    public const string Table = "<svg viewBox=\"0 0 24 24\" width=\"18\" height=\"18\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\"><rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\"/><line x1=\"3\" y1=\"9\" x2=\"21\" y2=\"9\"/><line x1=\"3\" y1=\"15\" x2=\"21\" y2=\"15\"/><line x1=\"9\" y1=\"3\" x2=\"9\" y2=\"21\"/><line x1=\"15\" y1=\"3\" x2=\"15\" y2=\"21\"/></svg>";
    public const string Image = $"<svg {V}><rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\" ry=\"2\"/><circle cx=\"8.5\" cy=\"8.5\" r=\"1.5\"/><polyline points=\"21 15 16 10 5 21\"/></svg>";
    public const string Video = $"<svg {V}><path d=\"M23 7l-7 5 7 5V7z\"/><rect x=\"1\" y=\"5\" width=\"15\" height=\"14\" rx=\"2\" ry=\"2\"/></svg>";
    public const string Upload = $"<svg {V}><path d=\"M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4\"/><polyline points=\"17 8 12 3 7 8\"/><line x1=\"12\" y1=\"3\" x2=\"12\" y2=\"15\"/></svg>";
    public const string Trash = $"<svg {V}><polyline points=\"3 6 5 6 21 6\"/><path d=\"M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2\"/><line x1=\"10\" y1=\"11\" x2=\"10\" y2=\"17\"/><line x1=\"14\" y1=\"11\" x2=\"14\" y2=\"17\"/></svg>";
    public const string Camera = $"<svg {V}><path d=\"M23 19a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h4l2-3h6l2 3h4a2 2 0 0 1 2 2z\"/><circle cx=\"12\" cy=\"13\" r=\"4\"/></svg>";
    public const string DragDrop = $"<svg {V}><rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\" ry=\"2\" stroke-dasharray=\"4\"/><path d=\"M13 13l6 6\"/><path d=\"M13 13l3.5-9.5-2.5 10 9 2.5-10-3z\" fill=\"currentColor\"/></svg>";


    public const string Subscript = "<svg viewBox=\"0 0 24 24\" width=\"18\" height=\"18\" fill=\"currentColor\" stroke=\"none\"><path d=\"M3 4l6 8-6 8h3l4.5-6 4.5 6h3l-6-8 6-8h-3L10.5 10 6 4H3z\" font-size=\"10\"/><text x=\"17\" y=\"22\" font-size=\"8\" font-family=\"sans-serif\">2</text></svg>";
    public const string Superscript = "<svg viewBox=\"0 0 24 24\" width=\"18\" height=\"18\" fill=\"currentColor\" stroke=\"none\"><path d=\"M3 6l6 8-6 8h3l4.5-6 4.5 6h3l-6-8 6-8h-3L10.5 12 6 6H3z\" font-size=\"10\"/><text x=\"17\" y=\"10\" font-size=\"8\" font-family=\"sans-serif\">2</text></svg>";

    public const string SourceCode = "<svg viewBox=\"0 0 24 24\" width=\"18\" height=\"18\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><polyline points=\"16 18 22 12 16 6\"/><polyline points=\"8 6 2 12 8 18\"/></svg>";
    public const string Fullscreen = $"<svg {V}><polyline points=\"15 3 21 3 21 9\"/><polyline points=\"9 21 3 21 3 15\"/><line x1=\"21\" y1=\"3\" x2=\"14\" y2=\"10\"/><line x1=\"3\" y1=\"21\" x2=\"10\" y2=\"14\"/></svg>";
}
