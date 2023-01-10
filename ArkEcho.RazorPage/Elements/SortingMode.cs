namespace ArkEcho.RazorPage.Elements
{
    public class SortingMode
    {
        public enum SortModes
        {
            NameAscending,
            NameDescending,
            InterpretAscending,
            InterpretDescending
        }

        public static readonly List<SortingMode> sortModes = new List<SortingMode>()
        {
            new SortingMode() { Mode = SortingMode.SortModes.NameAscending, DisplayName = "Name Aufsteigend" },
            new SortingMode() { Mode = SortingMode.SortModes.NameDescending, DisplayName = "Name Absteigend" },
        };

        public SortModes Mode { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        // Note: this is important so the MudSelect can compare pizzas
        public override bool Equals(object o)
        {
            var other = o as SortingMode;
            if (other == null) return false;
            return other.Mode == Mode && other.DisplayName.Equals(DisplayName, StringComparison.OrdinalIgnoreCase);
        }

        // Note: this is important too!
        public override int GetHashCode() => DisplayName?.GetHashCode() ?? 0;

        // Implement this for the Pizza to display correctly in MudSelect
        public override string ToString() => DisplayName;
    }
}
