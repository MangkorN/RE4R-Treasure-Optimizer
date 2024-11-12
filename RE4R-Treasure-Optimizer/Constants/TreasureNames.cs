namespace RE4R_Treasure_Optimizer.Constants
{
    public static class TreasureNames
    {
        public const string BUTTERFLY_LAMP = "Butterfly Lamp";
        public const string CHALICE_OF_ATONEMENT = "Chalice of Atonement";
        public const string ELEGANT_BANGLE = "Elegant Bangle";
        public const string ELEGANT_CROWN = "Elegant Crown";
        public const string ELEGANT_MASK = "Elegant Mask";
        public const string EXTRAVAGANT_CLOCK = "Extravagant Clock";
        public const string FLAGON = "Flagon";
        public const string GOLDEN_LYNX = "Golden Lynx";
        public const string ORNATE_NECKLACE = "Ornate Necklace";
        public const string SPLENDID_BANGLE = "Splendid Bangle";

        public static readonly Dictionary<string, string[]> TreasureAliases = new()
        {
            { BUTTERFLY_LAMP, new[] { BUTTERFLY_LAMP, BUTTERFLY_LAMP.ToLower(), "bl", "butterflylamp" } },
            { CHALICE_OF_ATONEMENT, new[] { CHALICE_OF_ATONEMENT, CHALICE_OF_ATONEMENT.ToLower(), "coa", "chaliceofatonement" } },
            { ELEGANT_BANGLE, new[] { ELEGANT_BANGLE, ELEGANT_BANGLE.ToLower(), "eb", "elegantbangle" } },
            { ELEGANT_CROWN, new[] { ELEGANT_CROWN, ELEGANT_CROWN.ToLower(), "ec", "elegantcrown" } },
            { ELEGANT_MASK, new[] { ELEGANT_MASK, ELEGANT_MASK.ToLower(), "em", "elegantmask" } },
            { EXTRAVAGANT_CLOCK, new[] { EXTRAVAGANT_CLOCK, EXTRAVAGANT_CLOCK.ToLower(), "exc", "extravagantclock" } },
            { FLAGON, new[] { FLAGON, FLAGON.ToLower(), "f" } },
            { GOLDEN_LYNX, new[] { GOLDEN_LYNX, GOLDEN_LYNX.ToLower(), "gl", "goldenlynx" } },
            { ORNATE_NECKLACE, new[] { ORNATE_NECKLACE, ORNATE_NECKLACE.ToLower(), "on", "ornatenecklace" } },
            { SPLENDID_BANGLE, new[] { SPLENDID_BANGLE, SPLENDID_BANGLE.ToLower(), "sb", "splendidbangle" } }
        };

        public static string? GetTreasureName(string input)
        {
            input = input.ToLower().Trim();

            foreach (var entry in TreasureAliases)
            {
                if (entry.Value.Contains(input))
                {
                    return entry.Key; // Return the canonical name
                }
            }

            return null; // Return null if no match found
        }
    }
}