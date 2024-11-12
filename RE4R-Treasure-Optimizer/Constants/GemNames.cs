namespace RE4R_Treasure_Optimizer.Constants
{
    public static class GemNames
    {
        public const string RUBY = "Ruby";
        public const string SAPPHIRE = "Sapphire";
        public const string YELLOW_DIAMOND = "Yellow Diamond";
        public const string EMERALD = "Emerald";
        public const string ALEXANDRITE = "Alexandrite";
        public const string RED_BERYL = "Red Beryl";

        public static readonly Dictionary<string, string[]> GemAliases = new()
        {
            { RUBY, new[] { RUBY, RUBY.ToLower(), "r" } },
            { SAPPHIRE, new[] { SAPPHIRE, SAPPHIRE.ToLower(), "s" } },
            { YELLOW_DIAMOND, new[] { YELLOW_DIAMOND, YELLOW_DIAMOND.ToLower(), "y", "yd", "yellowdiamond", "yellowDiamond" } },
            { EMERALD, new[] { EMERALD, EMERALD.ToLower(), "e" } },
            { ALEXANDRITE, new[] { ALEXANDRITE, ALEXANDRITE.ToLower(), "a" } },
            { RED_BERYL, new[] { RED_BERYL, RED_BERYL.ToLower(), "rb", "redberyl", "redBeryl" } }
        };

        public static string? GetGemName(string input)
        {
            input = input.ToLower().Trim();

            foreach (var entry in GemAliases)
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