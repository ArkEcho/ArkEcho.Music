namespace ArkEcho.RazorPage.Elements
{
    public class DisplayMode
    {
        public enum DisplayModes
        {
            Titel,
            Album,
            Interpret,
        }

        public static readonly List<DisplayMode> displayModes = new List<DisplayMode>()
        {
            new DisplayMode(){Mode = DisplayMode.DisplayModes.Titel, DisplayName ="Titel" },
            new DisplayMode(){Mode = DisplayMode.DisplayModes.Album, DisplayName ="Album" },
            new DisplayMode(){Mode = DisplayMode.DisplayModes.Interpret, DisplayName ="Interpret" }
        };

        public DisplayModes Mode { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        // Note: this is important so the MudSelect can compare pizzas
        public override bool Equals(object o)
        {
            var other = o as DisplayMode;
            if (other == null) return false;
            return other.Mode == Mode && other.DisplayName.Equals(DisplayName, StringComparison.OrdinalIgnoreCase);
        }

        // Note: this is important too!
        public override int GetHashCode() => DisplayName?.GetHashCode() ?? 0;

        // Implement this for the Pizza to display correctly in MudSelect
        public override string ToString() => DisplayName;
    }
}
