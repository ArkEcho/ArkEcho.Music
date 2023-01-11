namespace ArkEcho.RazorPage.Elements
{
    public class DisplayMode
    {
        public enum DisplayModes
        {
            Album,
            Titel,
            AlbumInterpret,
        }

        public static readonly List<DisplayMode> displayModes = new List<DisplayMode>()
        {
            new DisplayMode(){Mode = DisplayMode.DisplayModes.Album, DisplayName ="Album" },
            new DisplayMode(){Mode = DisplayMode.DisplayModes.AlbumInterpret, DisplayName ="AlbumInterpret" },
            new DisplayMode(){Mode = DisplayMode.DisplayModes.Titel, DisplayName ="Titel" },
        };

        public DisplayModes Mode { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        public override bool Equals(object o)
        {
            var other = o as DisplayMode;
            if (other == null) return false;
            return other.Mode == Mode && other.DisplayName.Equals(DisplayName, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() => DisplayName?.GetHashCode() ?? 0;

        public override string ToString() => DisplayName;
    }
}
