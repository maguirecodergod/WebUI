using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Emoji;

public partial class EmojiPicker : LhaComponentBase
{
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }
    [Parameter] public EventCallback<EmojiModel> OnEmojiSelected { get; set; }
    [Parameter] public string Placement { get; set; } = "bottom-start";
    [Parameter] public string MaxHeight { get; set; } = "400px";

    private string _searchQuery = string.Empty;
    private EmojiCategory _activeCategory = EmojiCategory.Smileys;

    private void HandleCategorySelected(EmojiCategory category)
    {
        _activeCategory = category;
        _searchQuery = string.Empty; // Clear search when switching categories
    }

    private void HandleEmojiSelected(EmojiModel emoji)
    {
        OnEmojiSelected.InvokeAsync(emoji);
        Close();
    }

    private void Close()
    {
        IsOpen = false;
        IsOpenChanged.InvokeAsync(false);
    }

    private void Toggle()
    {
        IsOpen = !IsOpen;
        IsOpenChanged.InvokeAsync(IsOpen);
    }

    private static readonly List<EmojiModel> AllEmojis = new()
    {
        // Smileys
        new EmojiModel("😀", "grinning face", EmojiCategory.Smileys, new[] { "happy", "smile", "face" }),
        new EmojiModel("😃", "grinning face with big eyes", EmojiCategory.Smileys, new[] { "happy", "smile", "face" }),
        new EmojiModel("😄", "grinning face with smiling eyes", EmojiCategory.Smileys, new[] { "happy", "smile", "face" }),
        new EmojiModel("😁", "beaming face with smiling eyes", EmojiCategory.Smileys, new[] { "happy", "smile", "face" }),
        new EmojiModel("😆", "grinning squinting face", EmojiCategory.Smileys, new[] { "happy", "smile", "face", "laugh" }),
        new EmojiModel("😅", "grinning face with sweat", EmojiCategory.Smileys, new[] { "happy", "smile", "face", "laugh", "sweat" }),
        new EmojiModel("🤣", "rolling on the floor laughing", EmojiCategory.Smileys, new[] { "laugh", "rofl", "rotfl" }),
        new EmojiModel("😂", "face with tears of joy", EmojiCategory.Smileys, new[] { "laugh", "tears", "joy" }),
        new EmojiModel("🙂", "slightly smiling face", EmojiCategory.Smileys, new[] { "smile", "face" }),
        new EmojiModel("🙃", "upside-down face", EmojiCategory.Smileys, new[] { "smile", "face", "upside" }),
        new EmojiModel("😉", "winking face", EmojiCategory.Smileys, new[] { "smile", "face", "wink" }),
        new EmojiModel("😊", "smiling face with smiling eyes", EmojiCategory.Smileys, new[] { "smile", "face", "happy" }),
        new EmojiModel("😇", "smiling face with halo", EmojiCategory.Smileys, new[] { "smile", "face", "angel", "halo" }),
        new EmojiModel("🥰", "smiling face with hearts", EmojiCategory.Smileys, new[] { "love", "heart", "face" }),
        new EmojiModel("😍", "smiling face with heart-eyes", EmojiCategory.Smileys, new[] { "love", "heart", "face" }),
        new EmojiModel("🤩", "star-struck", EmojiCategory.Smileys, new[] { "star", "face", "wow" }),
        new EmojiModel("😘", "face blowing a kiss", EmojiCategory.Smileys, new[] { "love", "kiss", "face" }),
        new EmojiModel("😗", "kissing face", EmojiCategory.Smileys, new[] { "kiss", "face" }),
        new EmojiModel("☺️", "smiling face", EmojiCategory.Smileys, new[] { "smile", "face" }),
        new EmojiModel("😚", "kissing face with closed eyes", EmojiCategory.Smileys, new[] { "kiss", "face" }),
        new EmojiModel("😙", "kissing face with smiling eyes", EmojiCategory.Smileys, new[] { "kiss", "face" }),
        new EmojiModel("😋", "face savoring food", EmojiCategory.Smileys, new[] { "food", "yummy", "face" }),
        new EmojiModel("😛", "face with tongue", EmojiCategory.Smileys, new[] { "tongue", "face" }),
        new EmojiModel("😜", "winking face with tongue", EmojiCategory.Smileys, new[] { "tongue", "face", "wink" }),
        new EmojiModel("🤪", "zany face", EmojiCategory.Smileys, new[] { "crazy", "face" }),
        new EmojiModel("😝", "squinting face with tongue", EmojiCategory.Smileys, new[] { "tongue", "face" }),
        new EmojiModel("🤑", "money-mouth face", EmojiCategory.Smileys, new[] { "money", "rich", "face" }),
        new EmojiModel("🤗", "hugging face", EmojiCategory.Smileys, new[] { "hug", "face" }),
        new EmojiModel("🤭", "face with hand over mouth", EmojiCategory.Smileys, new[] { "shh", "quiet", "face" }),
        new EmojiModel("🤫", "shushing face", EmojiCategory.Smileys, new[] { "shh", "quiet", "face" }),
        new EmojiModel("🤔", "thinking face", EmojiCategory.Smileys, new[] { "think", "face" }),

        // People
        new EmojiModel("👋", "waving hand", EmojiCategory.People, new[] { "hello", "hi", "bye", "wave" }),
        new EmojiModel("🤚", "raised back of hand", EmojiCategory.People, new[] { "hand", "high five" }),
        new EmojiModel("🖐️", "hand with fingers splayed", EmojiCategory.People, new[] { "hand" }),
        new EmojiModel("✋", "raised hand", EmojiCategory.People, new[] { "hand", "stop" }),
        new EmojiModel("🖖", "vulcan salute", EmojiCategory.People, new[] { "spock", "star trek" }),
        new EmojiModel("👌", "OK hand", EmojiCategory.People, new[] { "ok", "hand" }),
        new EmojiModel("🤌", "pinched fingers", EmojiCategory.People, new[] { "italian", "hand" }),
        new EmojiModel("🤏", "pinching hand", EmojiCategory.People, new[] { "small", "hand" }),
        new EmojiModel("✌️", "victory hand", EmojiCategory.People, new[] { "peace", "hand" }),
        new EmojiModel("🤞", "crossed fingers", EmojiCategory.People, new[] { "luck", "hand" }),
        new EmojiModel("🤟", "love-you gesture", EmojiCategory.People, new[] { "love", "hand" }),
        new EmojiModel("🤘", "sign of the horns", EmojiCategory.People, new[] { "rock", "hand" }),
        new EmojiModel("🤙", "call me hand", EmojiCategory.People, new[] { "call", "hand" }),
        new EmojiModel("👈", "backhand index pointing left", EmojiCategory.People, new[] { "point", "left" }),
        new EmojiModel("👉", "backhand index pointing right", EmojiCategory.People, new[] { "point", "right" }),
        new EmojiModel("👆", "backhand index pointing up", EmojiCategory.People, new[] { "point", "up" }),
        new EmojiModel("👇", "backhand index pointing down", EmojiCategory.People, new[] { "point", "down" }),
        new EmojiModel("☝️", "index pointing up", EmojiCategory.People, new[] { "point", "up" }),
        new EmojiModel("👍", "thumbs up", EmojiCategory.People, new[] { "good", "yes", "hand" }),
        new EmojiModel("👎", "thumbs down", EmojiCategory.People, new[] { "bad", "no", "hand" }),
        new EmojiModel("✊", "raised fist", EmojiCategory.People, new[] { "fist", "hand" }),
        new EmojiModel("👊", "oncoming fist", EmojiCategory.People, new[] { "fist", "punch" }),
        new EmojiModel("🤛", "left-facing fist", EmojiCategory.People, new[] { "fist" }),
        new EmojiModel("🤜", "right-facing fist", EmojiCategory.People, new[] { "fist" }),
        new EmojiModel("👏", "clapping hands", EmojiCategory.People, new[] { "clap", "good" }),
        new EmojiModel("🙌", "raising hands", EmojiCategory.People, new[] { "celebrate" }),
        new EmojiModel("👐", "open hands", EmojiCategory.People, new[] { "open" }),
        new EmojiModel("🤲", "palms up together", EmojiCategory.People, new[] { "prayer" }),
        new EmojiModel("🤝", "handshake", EmojiCategory.People, new[] { "deal", "hello" }),
        new EmojiModel("🙏", "folded hands", EmojiCategory.People, new[] { "please", "thanks", "prayer" }),

        // Animals
        new EmojiModel("🐶", "dog face", EmojiCategory.Animals, new[] { "dog", "puppy", "animal" }),
        new EmojiModel("🐱", "cat face", EmojiCategory.Animals, new[] { "cat", "kitty", "animal" }),
        new EmojiModel("🐭", "mouse face", EmojiCategory.Animals, new[] { "mouse", "animal" }),
        new EmojiModel("🐹", "hamster face", EmojiCategory.Animals, new[] { "hamster", "animal" }),
        new EmojiModel("🐰", "rabbit face", EmojiCategory.Animals, new[] { "rabbit", "bunny", "animal" }),
        new EmojiModel("🦊", "fox face", EmojiCategory.Animals, new[] { "fox", "animal" }),
        new EmojiModel("🐻", "bear face", EmojiCategory.Animals, new[] { "bear", "animal" }),
        new EmojiModel("🐼", "panda face", EmojiCategory.Animals, new[] { "panda", "animal" }),
        new EmojiModel("🐨", "koala", EmojiCategory.Animals, new[] { "koala", "animal" }),
        new EmojiModel("🐯", "tiger face", EmojiCategory.Animals, new[] { "tiger", "animal" }),
        new EmojiModel("🦁", "lion face", EmojiCategory.Animals, new[] { "lion", "animal" }),
        new EmojiModel("🐮", "cow face", EmojiCategory.Animals, new[] { "cow", "animal" }),
        new EmojiModel("🐷", "pig face", EmojiCategory.Animals, new[] { "pig", "animal" }),
        new EmojiModel("🐽", "pig nose", EmojiCategory.Animals, new[] { "pig", "nose" }),
        new EmojiModel("🐸", "frog face", EmojiCategory.Animals, new[] { "frog", "animal" }),
        new EmojiModel("🐵", "monkey face", EmojiCategory.Animals, new[] { "monkey", "animal" }),
        new EmojiModel("🙈", "see-no-evil monkey", EmojiCategory.Animals, new[] { "monkey", "blind" }),
        new EmojiModel("🙉", "hear-no-evil monkey", EmojiCategory.Animals, new[] { "monkey", "deaf" }),
        new EmojiModel("🙊", "speak-no-evil monkey", EmojiCategory.Animals, new[] { "monkey", "quiet" }),
        new EmojiModel("🐒", "monkey", EmojiCategory.Animals, new[] { "monkey", "animal" }),
        new EmojiModel("🦍", "gorilla", EmojiCategory.Animals, new[] { "gorilla", "animal" }),
        new EmojiModel("🦧", "orangutan", EmojiCategory.Animals, new[] { "monkey", "animal" }),
        new EmojiModel("🐶", "dog", EmojiCategory.Animals, new[] { "dog", "animal" }),
        new EmojiModel("🐕", "dog", EmojiCategory.Animals, new[] { "dog", "animal" }),
        new EmojiModel("🐩", "poodle", EmojiCategory.Animals, new[] { "dog", "animal" }),
        new EmojiModel("🐺", "wolf", EmojiCategory.Animals, new[] { "wolf", "animal" }),

        // Food
        new EmojiModel("🍏", "green apple", EmojiCategory.Food, new[] { "apple", "fruit", "food" }),
        new EmojiModel("🍎", "red apple", EmojiCategory.Food, new[] { "apple", "fruit", "food" }),
        new EmojiModel("🍐", "pear", EmojiCategory.Food, new[] { "pear", "fruit", "food" }),
        new EmojiModel("🍊", "tangerine", EmojiCategory.Food, new[] { "orange", "fruit", "food" }),
        new EmojiModel("🍋", "lemon", EmojiCategory.Food, new[] { "lemon", "fruit", "food" }),
        new EmojiModel("🍌", "banana", EmojiCategory.Food, new[] { "banana", "fruit", "food" }),
        new EmojiModel("🍉", "watermelon", EmojiCategory.Food, new[] { "watermelon", "fruit", "food" }),
        new EmojiModel("🍇", "grapes", EmojiCategory.Food, new[] { "grapes", "fruit", "food" }),
        new EmojiModel("🍓", "strawberry", EmojiCategory.Food, new[] { "strawberry", "fruit", "food" }),
        new EmojiModel("🫐", "blueberries", EmojiCategory.Food, new[] { "blueberries", "fruit", "food" }),
        new EmojiModel("🍈", "melon", EmojiCategory.Food, new[] { "melon", "fruit", "food" }),
        new EmojiModel("🍒", "cherries", EmojiCategory.Food, new[] { "cherries", "fruit", "food" }),
        new EmojiModel("🍑", "peach", EmojiCategory.Food, new[] { "peach", "fruit", "food" }),
        new EmojiModel("🥭", "mango", EmojiCategory.Food, new[] { "mango", "fruit", "food" }),
        new EmojiModel("🍍", "pineapple", EmojiCategory.Food, new[] { "pineapple", "fruit", "food" }),

        // Travel
        new EmojiModel("🚗", "automobile", EmojiCategory.Travel, new[] { "car", "drive", "travel" }),
        new EmojiModel("🚕", "taxi", EmojiCategory.Travel, new[] { "taxi", "travel" }),
        new EmojiModel("🚙", "sport utility vehicle", EmojiCategory.Travel, new[] { "car", "travel" }),
        new EmojiModel("🚌", "bus", EmojiCategory.Travel, new[] { "bus", "travel" }),
        new EmojiModel("🚎", "trolleybus", EmojiCategory.Travel, new[] { "bus", "travel" }),
        new EmojiModel("🏎️", "racing car", EmojiCategory.Travel, new[] { "car", "race" }),
        new EmojiModel("🚓", "police car", EmojiCategory.Travel, new[] { "car", "police" }),
        new EmojiModel("🚑", "ambulance", EmojiCategory.Travel, new[] { "hospital", "emergency" }),
        new EmojiModel("🚒", "fire engine", EmojiCategory.Travel, new[] { "fire", "emergency" }),
        new EmojiModel("🚐", "minibus", EmojiCategory.Travel, new[] { "van", "travel" }),

        // Objects
        new EmojiModel("⌚", "watch", EmojiCategory.Objects, new[] { "watch", "time" }),
        new EmojiModel("📱", "mobile phone", EmojiCategory.Objects, new[] { "phone", "mobile" }),
        new EmojiModel("📲", "mobile phone with arrow", EmojiCategory.Objects, new[] { "phone", "mobile" }),
        new EmojiModel("💻", "laptop", EmojiCategory.Objects, new[] { "computer", "laptop" }),
        new EmojiModel("⌨️", "keyboard", EmojiCategory.Objects, new[] { "computer", "keyboard" }),
        new EmojiModel("🖱️", "computer mouse", EmojiCategory.Objects, new[] { "computer", "mouse" }),
        new EmojiModel("🖲️", "trackball", EmojiCategory.Objects, new[] { "computer", "mouse" }),
        new EmojiModel("🕹️", "joystick", EmojiCategory.Objects, new[] { "game", "control" }),
        new EmojiModel("🗜️", "clamp", EmojiCategory.Objects, new[] { "tool" }),
        new EmojiModel("💽", "computer disk", EmojiCategory.Objects, new[] { "disk" }),
    };
}
