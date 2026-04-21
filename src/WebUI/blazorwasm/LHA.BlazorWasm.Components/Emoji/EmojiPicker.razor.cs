using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Emoji;

public partial class EmojiPicker : LHAComponentBase
{
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }
    [Parameter] public EventCallback<EmojiModel> OnEmojiSelected { get; set; }
    [Parameter] public string Placement { get; set; } = "bottom-start";
    [Parameter] public string MaxHeight { get; set; } = "400px";

    private string _searchQuery = string.Empty;
    private CEmojiCategory _activeCategory = CEmojiCategory.Smileys;

    private void HandleCategorySelected(CEmojiCategory category)
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
        new EmojiModel("😀", "grinning face", CEmojiCategory.Smileys, new[] { "happy", "smile", "face" }),
        new EmojiModel("😃", "grinning face with big eyes", CEmojiCategory.Smileys, new[] { "happy", "smile", "face" }),
        new EmojiModel("😄", "grinning face with smiling eyes", CEmojiCategory.Smileys, new[] { "happy", "smile", "face" }),
        new EmojiModel("😁", "beaming face with smiling eyes", CEmojiCategory.Smileys, new[] { "happy", "smile", "face" }),
        new EmojiModel("😆", "grinning squinting face", CEmojiCategory.Smileys, new[] { "happy", "smile", "face", "laugh" }),
        new EmojiModel("😅", "grinning face with sweat", CEmojiCategory.Smileys, new[] { "happy", "smile", "face", "laugh", "sweat" }),
        new EmojiModel("🤣", "rolling on the floor laughing", CEmojiCategory.Smileys, new[] { "laugh", "rofl", "rotfl" }),
        new EmojiModel("😂", "face with tears of joy", CEmojiCategory.Smileys, new[] { "laugh", "tears", "joy" }),
        new EmojiModel("🙂", "slightly smiling face", CEmojiCategory.Smileys, new[] { "smile", "face" }),
        new EmojiModel("🙃", "upside-down face", CEmojiCategory.Smileys, new[] { "smile", "face", "upside" }),
        new EmojiModel("😉", "winking face", CEmojiCategory.Smileys, new[] { "smile", "face", "wink" }),
        new EmojiModel("😊", "smiling face with smiling eyes", CEmojiCategory.Smileys, new[] { "smile", "face", "happy" }),
        new EmojiModel("😇", "smiling face with halo", CEmojiCategory.Smileys, new[] { "smile", "face", "angel", "halo" }),
        new EmojiModel("🥰", "smiling face with hearts", CEmojiCategory.Smileys, new[] { "love", "heart", "face" }),
        new EmojiModel("😍", "smiling face with heart-eyes", CEmojiCategory.Smileys, new[] { "love", "heart", "face" }),
        new EmojiModel("🤩", "star-struck", CEmojiCategory.Smileys, new[] { "star", "face", "wow" }),
        new EmojiModel("😘", "face blowing a kiss", CEmojiCategory.Smileys, new[] { "love", "kiss", "face" }),
        new EmojiModel("😗", "kissing face", CEmojiCategory.Smileys, new[] { "kiss", "face" }),
        new EmojiModel("☺️", "smiling face", CEmojiCategory.Smileys, new[] { "smile", "face" }),
        new EmojiModel("😚", "kissing face with closed eyes", CEmojiCategory.Smileys, new[] { "kiss", "face" }),
        new EmojiModel("😙", "kissing face with smiling eyes", CEmojiCategory.Smileys, new[] { "kiss", "face" }),
        new EmojiModel("😋", "face savoring food", CEmojiCategory.Smileys, new[] { "food", "yummy", "face" }),
        new EmojiModel("😛", "face with tongue", CEmojiCategory.Smileys, new[] { "tongue", "face" }),
        new EmojiModel("😜", "winking face with tongue", CEmojiCategory.Smileys, new[] { "tongue", "face", "wink" }),
        new EmojiModel("🤪", "zany face", CEmojiCategory.Smileys, new[] { "crazy", "face" }),
        new EmojiModel("😝", "squinting face with tongue", CEmojiCategory.Smileys, new[] { "tongue", "face" }),
        new EmojiModel("🤑", "money-mouth face", CEmojiCategory.Smileys, new[] { "money", "rich", "face" }),
        new EmojiModel("🤗", "hugging face", CEmojiCategory.Smileys, new[] { "hug", "face" }),
        new EmojiModel("🤭", "face with hand over mouth", CEmojiCategory.Smileys, new[] { "shh", "quiet", "face" }),
        new EmojiModel("🤫", "shushing face", CEmojiCategory.Smileys, new[] { "shh", "quiet", "face" }),
        new EmojiModel("🤔", "thinking face", CEmojiCategory.Smileys, new[] { "think", "face" }),

        // People
        new EmojiModel("👋", "waving hand", CEmojiCategory.People, new[] { "hello", "hi", "bye", "wave" }),
        new EmojiModel("🤚", "raised back of hand", CEmojiCategory.People, new[] { "hand", "high five" }),
        new EmojiModel("🖐️", "hand with fingers splayed", CEmojiCategory.People, new[] { "hand" }),
        new EmojiModel("✋", "raised hand", CEmojiCategory.People, new[] { "hand", "stop" }),
        new EmojiModel("🖖", "vulcan salute", CEmojiCategory.People, new[] { "spock", "star trek" }),
        new EmojiModel("👌", "OK hand", CEmojiCategory.People, new[] { "ok", "hand" }),
        new EmojiModel("🤌", "pinched fingers", CEmojiCategory.People, new[] { "italian", "hand" }),
        new EmojiModel("🤏", "pinching hand", CEmojiCategory.People, new[] { "small", "hand" }),
        new EmojiModel("✌️", "victory hand", CEmojiCategory.People, new[] { "peace", "hand" }),
        new EmojiModel("🤞", "crossed fingers", CEmojiCategory.People, new[] { "luck", "hand" }),
        new EmojiModel("🤟", "love-you gesture", CEmojiCategory.People, new[] { "love", "hand" }),
        new EmojiModel("🤘", "sign of the horns", CEmojiCategory.People, new[] { "rock", "hand" }),
        new EmojiModel("🤙", "call me hand", CEmojiCategory.People, new[] { "call", "hand" }),
        new EmojiModel("👈", "backhand index pointing left", CEmojiCategory.People, new[] { "point", "left" }),
        new EmojiModel("👉", "backhand index pointing right", CEmojiCategory.People, new[] { "point", "right" }),
        new EmojiModel("👆", "backhand index pointing up", CEmojiCategory.People, new[] { "point", "up" }),
        new EmojiModel("👇", "backhand index pointing down", CEmojiCategory.People, new[] { "point", "down" }),
        new EmojiModel("☝️", "index pointing up", CEmojiCategory.People, new[] { "point", "up" }),
        new EmojiModel("👍", "thumbs up", CEmojiCategory.People, new[] { "good", "yes", "hand" }),
        new EmojiModel("👎", "thumbs down", CEmojiCategory.People, new[] { "bad", "no", "hand" }),
        new EmojiModel("✊", "raised fist", CEmojiCategory.People, new[] { "fist", "hand" }),
        new EmojiModel("👊", "oncoming fist", CEmojiCategory.People, new[] { "fist", "punch" }),
        new EmojiModel("🤛", "left-facing fist", CEmojiCategory.People, new[] { "fist" }),
        new EmojiModel("🤜", "right-facing fist", CEmojiCategory.People, new[] { "fist" }),
        new EmojiModel("👏", "clapping hands", CEmojiCategory.People, new[] { "clap", "good" }),
        new EmojiModel("🙌", "raising hands", CEmojiCategory.People, new[] { "celebrate" }),
        new EmojiModel("👐", "open hands", CEmojiCategory.People, new[] { "open" }),
        new EmojiModel("🤲", "palms up together", CEmojiCategory.People, new[] { "prayer" }),
        new EmojiModel("🤝", "handshake", CEmojiCategory.People, new[] { "deal", "hello" }),
        new EmojiModel("🙏", "folded hands", CEmojiCategory.People, new[] { "please", "thanks", "prayer" }),

        // Animals
        new EmojiModel("🐶", "dog face", CEmojiCategory.Animals, new[] { "dog", "puppy", "animal" }),
        new EmojiModel("🐱", "cat face", CEmojiCategory.Animals, new[] { "cat", "kitty", "animal" }),
        new EmojiModel("🐭", "mouse face", CEmojiCategory.Animals, new[] { "mouse", "animal" }),
        new EmojiModel("🐹", "hamster face", CEmojiCategory.Animals, new[] { "hamster", "animal" }),
        new EmojiModel("🐰", "rabbit face", CEmojiCategory.Animals, new[] { "rabbit", "bunny", "animal" }),
        new EmojiModel("🦊", "fox face", CEmojiCategory.Animals, new[] { "fox", "animal" }),
        new EmojiModel("🐻", "bear face", CEmojiCategory.Animals, new[] { "bear", "animal" }),
        new EmojiModel("🐼", "panda face", CEmojiCategory.Animals, new[] { "panda", "animal" }),
        new EmojiModel("🐨", "koala", CEmojiCategory.Animals, new[] { "koala", "animal" }),
        new EmojiModel("🐯", "tiger face", CEmojiCategory.Animals, new[] { "tiger", "animal" }),
        new EmojiModel("🦁", "lion face", CEmojiCategory.Animals, new[] { "lion", "animal" }),
        new EmojiModel("🐮", "cow face", CEmojiCategory.Animals, new[] { "cow", "animal" }),
        new EmojiModel("🐷", "pig face", CEmojiCategory.Animals, new[] { "pig", "animal" }),
        new EmojiModel("🐽", "pig nose", CEmojiCategory.Animals, new[] { "pig", "nose" }),
        new EmojiModel("🐸", "frog face", CEmojiCategory.Animals, new[] { "frog", "animal" }),
        new EmojiModel("🐵", "monkey face", CEmojiCategory.Animals, new[] { "monkey", "animal" }),
        new EmojiModel("🙈", "see-no-evil monkey", CEmojiCategory.Animals, new[] { "monkey", "blind" }),
        new EmojiModel("🙉", "hear-no-evil monkey", CEmojiCategory.Animals, new[] { "monkey", "deaf" }),
        new EmojiModel("🙊", "speak-no-evil monkey", CEmojiCategory.Animals, new[] { "monkey", "quiet" }),
        new EmojiModel("🐒", "monkey", CEmojiCategory.Animals, new[] { "monkey", "animal" }),
        new EmojiModel("🦍", "gorilla", CEmojiCategory.Animals, new[] { "gorilla", "animal" }),
        new EmojiModel("🦧", "orangutan", CEmojiCategory.Animals, new[] { "monkey", "animal" }),
        new EmojiModel("🐶", "dog", CEmojiCategory.Animals, new[] { "dog", "animal" }),
        new EmojiModel("🐕", "dog", CEmojiCategory.Animals, new[] { "dog", "animal" }),
        new EmojiModel("🐩", "poodle", CEmojiCategory.Animals, new[] { "dog", "animal" }),
        new EmojiModel("🐺", "wolf", CEmojiCategory.Animals, new[] { "wolf", "animal" }),

        // Food
        new EmojiModel("🍏", "green apple", CEmojiCategory.Food, new[] { "apple", "fruit", "food" }),
        new EmojiModel("🍎", "red apple", CEmojiCategory.Food, new[] { "apple", "fruit", "food" }),
        new EmojiModel("🍐", "pear", CEmojiCategory.Food, new[] { "pear", "fruit", "food" }),
        new EmojiModel("🍊", "tangerine", CEmojiCategory.Food, new[] { "orange", "fruit", "food" }),
        new EmojiModel("🍋", "lemon", CEmojiCategory.Food, new[] { "lemon", "fruit", "food" }),
        new EmojiModel("🍌", "banana", CEmojiCategory.Food, new[] { "banana", "fruit", "food" }),
        new EmojiModel("🍉", "watermelon", CEmojiCategory.Food, new[] { "watermelon", "fruit", "food" }),
        new EmojiModel("🍇", "grapes", CEmojiCategory.Food, new[] { "grapes", "fruit", "food" }),
        new EmojiModel("🍓", "strawberry", CEmojiCategory.Food, new[] { "strawberry", "fruit", "food" }),
        new EmojiModel("🫐", "blueberries", CEmojiCategory.Food, new[] { "blueberries", "fruit", "food" }),
        new EmojiModel("🍈", "melon", CEmojiCategory.Food, new[] { "melon", "fruit", "food" }),
        new EmojiModel("🍒", "cherries", CEmojiCategory.Food, new[] { "cherries", "fruit", "food" }),
        new EmojiModel("🍑", "peach", CEmojiCategory.Food, new[] { "peach", "fruit", "food" }),
        new EmojiModel("🥭", "mango", CEmojiCategory.Food, new[] { "mango", "fruit", "food" }),
        new EmojiModel("🍍", "pineapple", CEmojiCategory.Food, new[] { "pineapple", "fruit", "food" }),

        // Travel
        new EmojiModel("🚗", "automobile", CEmojiCategory.Travel, new[] { "car", "drive", "travel" }),
        new EmojiModel("🚕", "taxi", CEmojiCategory.Travel, new[] { "taxi", "travel" }),
        new EmojiModel("🚙", "sport utility vehicle", CEmojiCategory.Travel, new[] { "car", "travel" }),
        new EmojiModel("🚌", "bus", CEmojiCategory.Travel, new[] { "bus", "travel" }),
        new EmojiModel("🚎", "trolleybus", CEmojiCategory.Travel, new[] { "bus", "travel" }),
        new EmojiModel("🏎️", "racing car", CEmojiCategory.Travel, new[] { "car", "race" }),
        new EmojiModel("🚓", "police car", CEmojiCategory.Travel, new[] { "car", "police" }),
        new EmojiModel("🚑", "ambulance", CEmojiCategory.Travel, new[] { "hospital", "emergency" }),
        new EmojiModel("🚒", "fire engine", CEmojiCategory.Travel, new[] { "fire", "emergency" }),
        new EmojiModel("🚐", "minibus", CEmojiCategory.Travel, new[] { "van", "travel" }),

        // Objects
        new EmojiModel("⌚", "watch", CEmojiCategory.Objects, new[] { "watch", "time" }),
        new EmojiModel("📱", "mobile phone", CEmojiCategory.Objects, new[] { "phone", "mobile" }),
        new EmojiModel("📲", "mobile phone with arrow", CEmojiCategory.Objects, new[] { "phone", "mobile" }),
        new EmojiModel("💻", "laptop", CEmojiCategory.Objects, new[] { "computer", "laptop" }),
        new EmojiModel("⌨️", "keyboard", CEmojiCategory.Objects, new[] { "computer", "keyboard" }),
        new EmojiModel("🖱️", "computer mouse", CEmojiCategory.Objects, new[] { "computer", "mouse" }),
        new EmojiModel("🖲️", "trackball", CEmojiCategory.Objects, new[] { "computer", "mouse" }),
        new EmojiModel("🕹️", "joystick", CEmojiCategory.Objects, new[] { "game", "control" }),
        new EmojiModel("🗜️", "clamp", CEmojiCategory.Objects, new[] { "tool" }),
        new EmojiModel("💽", "computer disk", CEmojiCategory.Objects, new[] { "disk" }),
    };
}
