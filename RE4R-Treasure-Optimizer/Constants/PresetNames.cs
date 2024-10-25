namespace RE4R_Treasure_Optimizer.Constants
{
    public static class PresetNames
    {
        public const string BASEGAME_MERCHANT_DLC = "Base Game w/Merchant + DLC";
        public const string BASEGAME_DLC = "Base Game + DLC";
        public const string BASEGAME_MERCHANT = "Base Game w/Merchant";
        public const string BASEGAME = "Base Game";

        public static readonly Dictionary<string, string[]> PresetAliases = new()
        {
            { BASEGAME_MERCHANT_DLC, new[] { BASEGAME_MERCHANT_DLC, BASEGAME_MERCHANT_DLC.ToLower(), "bmd" } },
            { BASEGAME_DLC, new[] { BASEGAME_DLC, BASEGAME_DLC.ToLower(), "bd" } },
            { BASEGAME_MERCHANT, new[] { BASEGAME_MERCHANT, BASEGAME_MERCHANT.ToLower(), "bm" } },
            { BASEGAME, new[] { BASEGAME, BASEGAME.ToLower(), "b" } }
        };

        public static string? GetPresetName(string input)
        {
            input = input.ToLower().Trim();

            foreach (var entry in PresetAliases)
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