using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Components.Select;

namespace LHA.BlazorWasm.Components.RichTextEditor.Components;

public partial class CodeBlockDialog : ComponentBase
{
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<CodeBlockResult> OnSubmit { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    private string Language { get; set; } = "javascript";
    private string Code { get; set; } = string.Empty;

    private List<SelectOption<string>> LanguageOptions => _languages.Select(l => new SelectOption<string> { Value = l.Key, Label = l.Value }).ToList();

    private readonly Dictionary<string, string> _languages = new()
    {
        { "javascript", "JavaScript" },
        { "typescript", "TypeScript" },
        { "csharp", "C#" },
        { "html", "HTML" },
        { "css", "CSS" },
        { "python", "Python" },
        { "java", "Java" },
        { "sql", "SQL" },
        { "json", "JSON" },
        { "xml", "XML" },
        { "bash", "Bash" },
        { "yaml", "YAML" },
        { "markdown", "Markdown" }
    };

    protected override void OnParametersSet()
    {
        if (IsVisible)
        {
            // Reset fields when dialog opens
            // Note: If you want to keep state between opens, remove this
            // But usually for a new insert, it should be empty
        }
    }

    private async Task Submit()
    {
        if (OnSubmit.HasDelegate && !string.IsNullOrWhiteSpace(Code))
        {
            await OnSubmit.InvokeAsync(new CodeBlockResult { Language = Language, Code = Code });
            Code = string.Empty; // Clear for next use
        }
    }

    private async Task Close()
    {
        if (OnClose.HasDelegate)
        {
            await OnClose.InvokeAsync();
        }
    }
}

public class CodeBlockResult
{
    public string Language { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
