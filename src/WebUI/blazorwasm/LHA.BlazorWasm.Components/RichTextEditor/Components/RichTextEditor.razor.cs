using LHA.BlazorWasm.Components.RichTextEditor.Interop;
using LHA.BlazorWasm.Components.RichTextEditor.Models;
using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.RichTextEditor.Components;

/// <summary>
/// A full-featured WYSIWYG Rich Text Editor component for Blazor WASM.
/// Provides two-way binding for HTML content, configurable toolbar, and status bar.
/// 
/// Usage:
/// <code>
/// &lt;RichTextEditor @bind-Value="htmlContent" /&gt;
/// </code>
/// </summary>
public partial class RichTextEditor : LHAComponentBase, IAsyncDisposable
{
    /// <summary>
    /// HTML content of the editor (two-way bindable).
    /// </summary>
    [Parameter] public string? Value { get; set; }

    /// <summary>
    /// Callback when Value changes.
    /// </summary>
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Editor configuration options.
    /// </summary>
    [Parameter] public EditorOptions Options { get; set; } = new();

    /// <summary>
    /// Fires when the editor content changes.
    /// </summary>
    [Parameter] public EventCallback<string> OnContentChanged { get; set; }

    /// <summary>
    /// Fires when the fullscreen state changes.
    /// </summary>
    [Parameter] public EventCallback<bool> OnFullscreenChanged { get; set; }

    /// <summary>
    /// Unique editor instance identifier.
    /// </summary>
    private string EditorId { get; } = $"rte-{Guid.NewGuid():N}";

    /// <summary>
    /// Current editor state (formatting, word count, etc).
    /// </summary>
    private EditorState State { get; set; } = new();

    private RichTextEditorInterop? _interop;
    private bool _initialized;
    private string? _lastValue;

    protected override async Task OnParametersSetAsync()
    {
        if (_initialized && Value != _lastValue)
        {
            _lastValue = Value;
            if (_interop != null)
            {
                await _interop.SetHtmlAsync(EditorId, Value ?? "");
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _interop = new RichTextEditorInterop(JS);

            // Subscribe to JS callbacks
            _interop.OnContentChanged += OnContentChangedFromJs;
            _interop.OnSelectionChanged += OnSelectionChangedFromJs;

            // Initialize the editor in JS
            await _interop.InitEditorAsync(EditorId, Options);

            // Set initial content if provided
            if (!string.IsNullOrEmpty(Value))
            {
                _lastValue = Value;
                await _interop.SetHtmlAsync(EditorId, Value);
            }

            _initialized = true;
        }
    }

    private async Task OnContentChangedFromJs(string html)
    {
        _lastValue = html;
        Value = html;


        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(html);
        }

        if (OnContentChanged.HasDelegate)
        {
            await OnContentChanged.InvokeAsync(html);
        }

        await InvokeAsync(StateHasChanged);
    }

    private async Task OnSelectionChangedFromJs(EditorState state)
    {
        State = state;
        await InvokeAsync(StateHasChanged);
    }

    private async Task HandleCommand((string command, string? value) args)
    {
        if (_interop == null || !_initialized) return;
        await _interop.ExecCommandAsync(EditorId, args.command, args.value);
    }

    private async Task HandleToggleSourceView()
    {
        if (_interop == null || !_initialized) return;
        var isSourceView = await _interop.ToggleSourceViewAsync(EditorId);
        State.IsSourceView = isSourceView;
        StateHasChanged();
    }

    private async Task HandleToggleFullscreen()
    {
        if (_interop == null || !_initialized) return;
        var isFullscreen = await _interop.ToggleFullscreenAsync(EditorId);
        State.IsFullscreen = isFullscreen;

        if (OnFullscreenChanged.HasDelegate)
        {
            await OnFullscreenChanged.InvokeAsync(isFullscreen);
        }

        StateHasChanged();
    }

    private async Task HandleInsertLink(LinkDialogResult result)
    {
        if (_interop == null || !_initialized) return;

        var target = result.OpenInNewTab ? " target=\"_blank\"" : "";
        var title = !string.IsNullOrEmpty(result.Title) ? $" title=\"{result.Title}\"" : "";
        var html = $"<a href=\"{result.Url}\"{target}{title}>{result.DisplayText}</a>";

        await _interop.InsertHtmlAsync(EditorId, html);
    }

    private async Task HandleInsertImage(ImageDialogResult result)
    {
        if (_interop == null || !_initialized) return;

        var style = "";
        if (!string.IsNullOrEmpty(result.Width))
            style += $"width:{result.Width};";
        if (!string.IsNullOrEmpty(result.Height))
            style += $"height:{result.Height};";

        var styleAttr = !string.IsNullOrEmpty(style) ? $" style=\"{style}\"" : "";
        var alt = !string.IsNullOrEmpty(result.AltText) ? result.AltText : "";
        var html = $"<img src=\"{result.Url}\" alt=\"{alt}\"{styleAttr}/>";

        await _interop.InsertHtmlAsync(EditorId, html);
    }

    private async Task HandleInsertTable((int rows, int cols) size)
    {
        if (_interop == null || !_initialized) return;
        await _interop.InsertTableAsync(EditorId, size.rows, size.cols);
    }

    private async Task HandleInsertCodeBlock(CodeBlockResult result)
    {
        if (_interop == null || !_initialized) return;
        await _interop.InsertCodeBlockAsync(EditorId, result.Code, result.Language);
    }

    private async Task HandleClearContentAsync()
    {
        await SetHtmlAsync("");
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync("");
        }
        if (OnContentChanged.HasDelegate)
        {
            await OnContentChanged.InvokeAsync("");
        }
    }

    /// <summary>
    /// Programmatically get the current HTML content.
    /// </summary>
    public async Task<string> GetHtmlAsync()
    {
        if (_interop == null) return Value ?? "";
        return await _interop.GetHtmlAsync(EditorId);
    }

    /// <summary>
    /// Programmatically set HTML content.
    /// </summary>
    public async Task SetHtmlAsync(string html)
    {
        if (_interop == null) return;
        await _interop.SetHtmlAsync(EditorId, html);
        Value = html;
    }

    /// <summary>
    /// Focus the editor.
    /// </summary>
    public async Task FocusAsync()
    {
        if (_interop == null) return;
        await _interop.FocusAsync(EditorId);
    }

    public async ValueTask DisposeAsync()
    {
        if (_interop != null)
        {
            _interop.OnContentChanged -= OnContentChangedFromJs;
            _interop.OnSelectionChanged -= OnSelectionChangedFromJs;
            await _interop.DisposeAsync();
        }
    }
}
