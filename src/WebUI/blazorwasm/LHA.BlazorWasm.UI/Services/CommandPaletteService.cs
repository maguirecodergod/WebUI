namespace LHA.BlazorWasm.UI.Services;

/// <summary>
/// Service for the command palette (Ctrl+K).
/// Allows navigation, commands, and search from a unified interface.
/// </summary>
public interface ICommandPaletteService
{
    /// <summary>
    /// Register a command.
    /// </summary>
    void RegisterCommand(CommandDefinition command);

    /// <summary>
    /// Register multiple commands.
    /// </summary>
    void RegisterCommands(IEnumerable<CommandDefinition> commands);

    /// <summary>
    /// Get all registered commands, optionally filtered by search query.
    /// </summary>
    IReadOnlyList<CommandDefinition> GetCommands(string? searchQuery = null);

    /// <summary>
    /// Execute a command by ID.
    /// </summary>
    Task ExecuteAsync(string commandId);

    /// <summary>
    /// Event raised when commands change.
    /// </summary>
    event Action? OnCommandsChanged;
}

/// <summary>
/// Defines a command for the command palette.
/// </summary>
public sealed class CommandDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Shortcut { get; set; }
    public string Category { get; set; } = "General";
    public string? Permission { get; set; }
    public int Order { get; set; }

    /// <summary>
    /// Handler executed when the command is invoked.
    /// </summary>
    public Func<Task>? Handler { get; set; }

    /// <summary>
    /// Route to navigate to (alternative to Handler).
    /// </summary>
    public string? NavigateTo { get; set; }
}

/// <summary>
/// Default implementation of the command palette service.
/// </summary>
public sealed class CommandPaletteService : ICommandPaletteService
{
    private readonly List<CommandDefinition> _commands = [];

    public event Action? OnCommandsChanged;

    public void RegisterCommand(CommandDefinition command)
    {
        _commands.Add(command);
        OnCommandsChanged?.Invoke();
    }

    public void RegisterCommands(IEnumerable<CommandDefinition> commands)
    {
        _commands.AddRange(commands);
        OnCommandsChanged?.Invoke();
    }

    public IReadOnlyList<CommandDefinition> GetCommands(string? searchQuery = null)
    {
        var query = _commands.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var search = searchQuery.Trim();
            query = query.Where(c =>
                c.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (c.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                c.Category.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        return query.OrderBy(c => c.Category).ThenBy(c => c.Order).ThenBy(c => c.Title).ToList().AsReadOnly();
    }

    public async Task ExecuteAsync(string commandId)
    {
        var command = _commands.FirstOrDefault(c =>
            string.Equals(c.Id, commandId, StringComparison.OrdinalIgnoreCase));

        if (command?.Handler is not null)
        {
            await command.Handler();
        }
    }
}
