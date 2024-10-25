using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using RE4R_Treasure_Optimizer.Constants;

namespace RE4R_Treasure_Optimizer
{
    public static class Core
    {
        #region Core Types

        public class GemCombination
        {
            public string TreasureName { get; }
            public readonly Gems Gems;
            public readonly ColorStatus ColorStatus;
            public readonly int TreasurePrice;
            public readonly int GemPrice;
            public readonly int FinalPrice;
            public readonly int Profit;

            /// <summary>
            /// The position relative to other combinations for the treasure.<br></br>
            /// 'Index' is sorted by 'Final Price', e.g. Index=0 implies that this combination has the highest final price for the treasure.<br></br>
            /// 'Rank' is sorted by 'Profit', e.g. Rank=0 implies that this combination has the highest profit for the treasure.<br></br>
            /// </summary>
            public (int Index, int Rank) Position = (-1, -1);

            public GemCombination(string treasureName, int ruby = 0, int sapphire = 0, int yellowDiamond = 0, int emerald = 0, int alexandrite = 0, int redBeryl = 0)
            {
                TreasureName = treasureName;
                Gems = new(ruby, sapphire, yellowDiamond, emerald, alexandrite, redBeryl);
                ColorStatus = new(ruby, sapphire, yellowDiamond, emerald, alexandrite, redBeryl);

                var prices = GetTreasureGemCombinationPrices(treasureName, ruby, sapphire, yellowDiamond, emerald, alexandrite, redBeryl);
                if (prices != null)
                {
                    TreasurePrice = prices.Value.treasurePrice;
                    GemPrice = prices.Value.gemPrice;
                    FinalPrice = prices.Value.finalPrice;
                    Profit = prices.Value.profit;
                }             
            }

            public string ToStringNoPadding()
            {
                return $"{TreasureName}: [{Position.Index},{Position.Rank}]={FinalPrice} (P={Profit}, G={GemPrice}): {ColorStatus} ({Gems})";
            }

            public override string ToString()
            {
                return $"{LPad(TreasureName, 20)}: [{LPad(Position.Index, 2)},{LPad(Position.Rank, 2)}]={LPad(FinalPrice, 6)} (P={LPad(Profit, 5)}, G={LPad(GemPrice, 5)}): {ColorStatus} ({Gems})";
            }
        }

        public struct Gems
        {
            public int Ruby;
            public int Sapphire;
            public int YellowDiamond;
            public int Emerald;
            public int Alexandrite;
            public int RedBeryl;

            public Gems(int ruby = 0, int sapphire = 0, int yellowDiamond = 0, int emerald = 0, int alexandrite = 0, int redBeryl = 0)
            {
                Ruby = ruby;
                Sapphire = sapphire;
                YellowDiamond = yellowDiamond;
                Emerald = emerald;
                Alexandrite = alexandrite;
                RedBeryl = redBeryl;
            }

            public readonly (int ruby, int sapphire, int yellowDiamond, int emerald, int alexandrite, int redBeryl) GemAmounts()
            {
                return (Ruby, Sapphire, YellowDiamond, Emerald, Alexandrite, RedBeryl);
            }

            public readonly int GetGemCountByName(string name)
            {
                switch (name)
                {
                    case GemNames.RUBY: return Ruby;
                    case GemNames.SAPPHIRE: return Sapphire;
                    case GemNames.YELLOW_DIAMOND: return YellowDiamond;
                    case GemNames.EMERALD: return Emerald;
                    case GemNames.ALEXANDRITE: return Alexandrite;
                    case GemNames.RED_BERYL: return RedBeryl;
                    default: Console.Error.WriteLine($"Invalid gem name '{name}'!"); return 0;
                }

            }

            public override readonly string ToString()
            {
                if (Ruby + Sapphire + YellowDiamond + Emerald + Alexandrite + RedBeryl == 0)
                {
                    return "";
                }

                string gemNames = "";
                gemNames += RepeatGemName(GemNames.RUBY, Ruby);
                gemNames += RepeatGemName(GemNames.SAPPHIRE, Sapphire);
                gemNames += RepeatGemName(GemNames.YELLOW_DIAMOND, YellowDiamond);
                gemNames += RepeatGemName(GemNames.EMERALD, Emerald);
                gemNames += RepeatGemName(GemNames.ALEXANDRITE, Alexandrite);
                gemNames += RepeatGemName(GemNames.RED_BERYL, RedBeryl);
                return gemNames.Remove(gemNames.Length - 2);
            }

            private static string RepeatGemName(string gemName, int numRepeat)
            {
                string str = "";
                for (int i = 0; i < numRepeat; i++)
                {
                    str += $"{gemName}, ";
                }
                return str;
            }
        }

        public struct ColorStatus
        {
            public int Red;
            public int Blue;
            public int Yellow;
            public int Green;
            public int Purple;

            public ColorStatus(int ruby = 0, int sapphire = 0, int yellowDiamond = 0, int emerald = 0, int alexandrite = 0, int redBeryl = 0)
            {
                Red = ruby + redBeryl;
                Blue = sapphire;
                Yellow = yellowDiamond;
                Green = emerald;
                Purple = alexandrite;
            }

            public override readonly string ToString()
            {
                return
                  $"R{Red} " +
                  $"B{Blue} " +
                  $"Y{Yellow} " +
                  $"G{Green} " +
                  $"P{Purple}";
            }
        }

        public enum Color
        {
            Red, Blue, Yellow, Green, Purple
        }

        public enum Shape
        {
            Circle, Square
        }

        #endregion

        #region Generate Gem Combinations

        public static Dictionary<string, List<GemCombination>> GenerateTreasureToGemCombinations()
        {
            var treasureToGemCombinations = new Dictionary<string, List<GemCombination>>
            {
                { TreasureNames.BUTTERFLY_LAMP, BuildGemCombinationsFromResource(TreasureNames.BUTTERFLY_LAMP) },
                { TreasureNames.CHALICE_OF_ATONEMENT, BuildGemCombinationsFromResource(TreasureNames.CHALICE_OF_ATONEMENT) },
                { TreasureNames.ELEGANT_BANGLE, BuildGemCombinationsFromResource(TreasureNames.ELEGANT_BANGLE) },
                { TreasureNames.ELEGANT_CROWN, BuildGemCombinationsFromResource(TreasureNames.ELEGANT_CROWN) },
                { TreasureNames.ELEGANT_MASK, BuildGemCombinationsFromResource(TreasureNames.ELEGANT_MASK) },
                { TreasureNames.EXTRAVAGANT_CLOCK, BuildGemCombinationsFromResource(TreasureNames.EXTRAVAGANT_CLOCK) },
                { TreasureNames.FLAGON, BuildGemCombinationsFromResource(TreasureNames.FLAGON) },
                { TreasureNames.GOLDEN_LYNX, BuildGemCombinationsFromResource(TreasureNames.GOLDEN_LYNX) },
                { TreasureNames.ORNATE_NECKLACE, BuildGemCombinationsFromResource(TreasureNames.ORNATE_NECKLACE) },
                { TreasureNames.SPLENDID_BANGLE, BuildGemCombinationsFromResource(TreasureNames.SPLENDID_BANGLE) }
            };

            return treasureToGemCombinations;
        }

        /// <summary>
        /// Builds a list of <see cref="GemCombination"/> objects from the gem combination data read from a resource.
        /// </summary>
        /// <param name="resourceName">The name of the resource containing gem combination data.</param>
        /// <returns>
        /// A list of <see cref="GemCombination"/> objects constructed from the data in the resource.
        /// The list is sorted by profit and final price, with position indices assigned accordingly.
        /// </returns>
        /// <remarks>
        /// This method reads raw gem combination data from the specified resource,
        /// creates <see cref="GemCombination"/> instances, sorts them based on profit and final price,
        /// and assigns rank and index positions to each combination.
        /// </remarks>
        public static List<GemCombination> BuildGemCombinationsFromResource(string resourceName)
        {
            var gemCombinations = new List<GemCombination>();
            var rawCombinationData = ParseGemCombinationDataFromResource(resourceName);

            foreach (var data in rawCombinationData)
            {
                GemCombination c = new(
                    data.treasureName,
                    data.ruby,
                    data.sapphire,
                    data.yellowDiamond,
                    data.emerald,
                    data.alexandrite,
                    data.redBeryl);
                gemCombinations.Add(c);
            }

            SortGemCombinations(gemCombinations, true);
            for (int i = 0; i < gemCombinations.Count; i++)
                gemCombinations[i].Position.Rank = i;

            SortGemCombinations(gemCombinations, false);
            for (int i = 0; i < gemCombinations.Count; i++)
                gemCombinations[i].Position.Index = i;

            return gemCombinations;
        }

        /// <summary>
        /// Reads raw gem combination data from a resource and parses it into a list of tuples containing the data fields.
        /// </summary>
        /// <param name="resourceName">The name of the resource containing gem combination data.</param>
        /// <returns>
        /// A list of tuples, each containing the treasure name, final price, and counts of each gem type:
        /// (string treasureName, int finalPrice, int ruby, int sapphire, int yellowDiamond, int emerald, int alexandrite, int redBeryl).
        /// </returns>
        /// <remarks>
        /// Each line in the resource should be in the format:
        /// <c>TreasureName/FinalPrice/Ruby/Sapphire/YellowDiamond/Emerald/Alexandrite/RedBeryl</c>.
        /// The method parses each line, validates the data, and collects it into a list for further processing.
        /// </remarks>
        public static List<(string treasureName, int finalPrice, int ruby, int sapphire, int yellowDiamond, int emerald, int alexandrite, int redBeryl)>
            ParseGemCombinationDataFromResource(string resourceName)
        {
            string resourceContent = GetResource(resourceName);

            if (string.IsNullOrEmpty(resourceContent))
            {
                Console.Error.WriteLine("Invalid resource or resource not found!");
                return new();
            }

            var comboList = new List<(string treasureName, int finalPrice, int ruby, int sapphire, int yellowDiamond, int emerald, int alexandrite, int redBeryl)>();

            string[] lines = resourceContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] values = line.Split('/');

                if (values.Length != 8)
                {
                    Console.Error.WriteLine($"Unexpected format in line: {line}");
                    continue;
                }

                string treasureName = values[0].Trim();
                if (int.TryParse(values[1], out int finalPrice) &&
                    int.TryParse(values[2], out int ruby) &&
                    int.TryParse(values[3], out int sapphire) &&
                    int.TryParse(values[4], out int yellowDiamond) &&
                    int.TryParse(values[5], out int emerald) &&
                    int.TryParse(values[6], out int alexandrite) &&
                    int.TryParse(values[7], out int redBeryl))
                {
                    comboList.Add((treasureName, finalPrice, ruby, sapphire, yellowDiamond, emerald, alexandrite, redBeryl));
                }
                else
                {
                    Console.Error.WriteLine($"Invalid number format in line: {line}");
                }
            }

            return comboList;
        }

        private static string GetResource(string resourceName)
        {
            string resourcePath = "RE4R_Treasure_Optimizer.";

            switch (resourceName)
            {
                // Treasure Combinations
                case TreasureNames.BUTTERFLY_LAMP: resourcePath += TreasureCombinationPaths.BUTTERFLY_LAMP; break;
                case TreasureNames.CHALICE_OF_ATONEMENT: resourcePath += TreasureCombinationPaths.CHALICE_OF_ATONEMENT; break;
                case TreasureNames.ELEGANT_BANGLE: resourcePath += TreasureCombinationPaths.ELEGANT_BANGLE; break;
                case TreasureNames.ELEGANT_CROWN: resourcePath += TreasureCombinationPaths.ELEGANT_CROWN; break;
                case TreasureNames.ELEGANT_MASK: resourcePath += TreasureCombinationPaths.ELEGANT_MASK; break;
                case TreasureNames.EXTRAVAGANT_CLOCK: resourcePath += TreasureCombinationPaths.EXTRAVAGANT_CLOCK; break;
                case TreasureNames.FLAGON: resourcePath += TreasureCombinationPaths.FLAGON; break;
                case TreasureNames.GOLDEN_LYNX: resourcePath += TreasureCombinationPaths.GOLDEN_LYNX; break;
                case TreasureNames.ORNATE_NECKLACE: resourcePath += TreasureCombinationPaths.ORNATE_NECKLACE; break;
                case TreasureNames.SPLENDID_BANGLE: resourcePath += TreasureCombinationPaths.SPLENDID_BANGLE; break;

                // Sell Configuration Presets
                case PresetNames.BASEGAME_MERCHANT_DLC: resourcePath += PresetPaths.BASEGAME_MERCHANT_DLC; break;
                case PresetNames.BASEGAME_DLC: resourcePath += PresetPaths.BASEGAME_DLC; break;
                case PresetNames.BASEGAME_MERCHANT: resourcePath += PresetPaths.BASEGAME_MERCHANT; break;
                case PresetNames.BASEGAME: resourcePath += PresetPaths.BASEGAME; break;

                default:
                    Console.Error.WriteLine($"Resource '{resourceName}' undefined!");
                    return string.Empty;
            }

            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
            if (stream == null)
            {
                Console.Error.WriteLine($"Resource stream for '{resourceName}' not found!");
                return string.Empty;
            }

            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }

        #endregion

        #region Utilities

        public static bool GetTreasureInfo(string name, out (int price, int circles, int squares)? info)
        {
            info = null;

            switch (name)
            {
                case TreasureNames.BUTTERFLY_LAMP: info = (TreasurePrices.BUTTERFLY_LAMP, 3, 0); break;
                case TreasureNames.CHALICE_OF_ATONEMENT: info = (TreasurePrices.CHALICE_OF_ATONEMENT, 0, 3); break;
                case TreasureNames.ELEGANT_BANGLE: info = (TreasurePrices.ELEGANT_BANGLE, 2, 0); break;
                case TreasureNames.ELEGANT_CROWN: info = (TreasurePrices.ELEGANT_CROWN, 2, 3); break;
                case TreasureNames.ELEGANT_MASK: info = (TreasurePrices.ELEGANT_MASK, 3, 0); break;
                case TreasureNames.EXTRAVAGANT_CLOCK: info = (TreasurePrices.EXTRAVAGANT_CLOCK, 1, 1); break;
                case TreasureNames.FLAGON: info = (TreasurePrices.FLAGON, 2, 0); break;
                case TreasureNames.GOLDEN_LYNX: info = (TreasurePrices.GOLDEN_LYNX, 2, 1); break;
                case TreasureNames.ORNATE_NECKLACE: info = (TreasurePrices.ORNATE_NECKLACE, 2, 2); break;
                case TreasureNames.SPLENDID_BANGLE: info = (TreasurePrices.SPLENDID_BANGLE, 0, 2); break;
            }

            return info != null;
        }

        public static bool GetGemInfo(string name, out (int price, Color color, Shape shape)? info)
        {
            info = null;

            switch (name)
            {
                case GemNames.RUBY: info = (GemPrices.RUBY, Color.Red, Shape.Circle); break;
                case GemNames.SAPPHIRE: info = (GemPrices.SAPPHIRE, Color.Blue, Shape.Circle); break;
                case GemNames.YELLOW_DIAMOND: info = (GemPrices.YELLOW_DIAMOND, Color.Yellow, Shape.Circle); break;
                case GemNames.EMERALD: info = (GemPrices.EMERALD, Color.Green, Shape.Square); break;
                case GemNames.ALEXANDRITE: info = (GemPrices.ALEXANDRITE, Color.Purple, Shape.Square); break;
                case GemNames.RED_BERYL: info = (GemPrices.RED_BERYL, Color.Red, Shape.Square); break;
            }

            return info != null;
        }

        /// <summary>
        /// treasurePrice = base price of the treasure <br></br>
        /// gemPrice = sum of the base price of the gems <br></br>
        /// finalPrice = (treasurePrice + gemPrice) * multiplier <br></br>
        /// profit = finalPrice - (treasurePrice + gemPrice)
        /// </summary>
        public static (int treasurePrice, int gemPrice, int finalPrice, int profit)? GetTreasureGemCombinationPrices(string treasureName, int ruby = 0, int sapphire = 0, int yellowDiamond = 0, int emerald = 0, int alexandrite = 0, int redBeryl = 0)
        {
            if (!GetTreasureInfo(treasureName, out var info) || info == null)
            {
                Console.Error.WriteLine($"Treasure '{treasureName}' not found!");
                return null;
            }

            // Warns if the amount of circle and/or square gems exceed the treasure's max capacity.
            // Always check with GetTreasureInfo() before trying to slot gems into a treasure.
            if (info.Value.circles < ruby + sapphire + yellowDiamond ||
                info.Value.squares < emerald + alexandrite + redBeryl)
            {
                Console.Error.WriteLine($"WARNING: Gem combination for '{treasureName}' is illegal!");
            }

            int tPrice = info.Value.price;
            int gPrice = (ruby * GemPrices.RUBY)
                    + (sapphire * GemPrices.SAPPHIRE)
                    + (yellowDiamond * GemPrices.YELLOW_DIAMOND)
                    + (emerald * GemPrices.EMERALD)
                    + (alexandrite * GemPrices.ALEXANDRITE)
                    + (redBeryl * GemPrices.RED_BERYL);
            float mult = GetMultiplier(new(ruby, sapphire, yellowDiamond, emerald, alexandrite, redBeryl));
            int fPrice = (int)Math.Round((tPrice + gPrice) * mult);
            int profit = fPrice - (tPrice + gPrice);

            return (tPrice, gPrice, fPrice, profit);
        }

        /// <summary>
        /// Determines the multiplier based on the gem colors in the combination.
        /// </summary>
        /// <param name="colorStatus">An object representing the counts of each gem color.</param>
        /// <returns>
        /// A <see cref="float"/> representing the multiplier to be applied based on the gem color combination.
        /// </returns>
        /// <remarks>
        /// The multiplier is calculated according to the following rules:
        /// <list type="bullet">
        /// <item>
        /// <description><b>Five Colors (2.0):</b> Five gemstones of differing colors.</description>
        /// </item>
        /// <item>
        /// <description><b>Quintet (1.9):</b> Five gemstones of the same color.</description>
        /// </item>
        /// <item>
        /// <description><b>Duo &amp; Trio (1.8):</b> One duo and one trio.</description>
        /// </item>
        /// <item>
        /// <description><b>Quartet (1.7):</b> Four gemstones of the same color.</description>
        /// </item>
        /// <item>
        /// <description><b>Four Colors (1.6):</b> Four gemstones of differing colors.</description>
        /// </item>
        /// <item>
        /// <description><b>Two Duos (1.5):</b> Two duos (two pairs of gemstones of the same color).</description>
        /// </item>
        /// <item>
        /// <description><b>Trio (1.4):</b> Three gemstones of the same color.</description>
        /// </item>
        /// <item>
        /// <description><b>Three Colors (1.3):</b> Three gemstones of differing colors.</description>
        /// </item>
        /// <item>
        /// <description><b>Duo (1.2):</b> Two gemstones of the same color.</description>
        /// </item>
        /// <item>
        /// <description><b>Two Colors (1.1):</b> Two gemstones of differing colors.</description>
        /// </item>
        /// <item>
        /// <description><b>No Bonus (1.0):</b> Does not meet any special combination criteria.</description>
        /// </item>
        /// </list>
        /// </remarks>
        public static float GetMultiplier(ColorStatus colorStatus)
        {
            int uniqueColorCount = 0;
            if (colorStatus.Red > 0) uniqueColorCount++;
            if (colorStatus.Blue > 0) uniqueColorCount++;
            if (colorStatus.Yellow > 0) uniqueColorCount++;
            if (colorStatus.Green > 0) uniqueColorCount++;
            if (colorStatus.Purple > 0) uniqueColorCount++;

            (int duoGemsCount, int trioGemsCount, int quartetCount, int quintetCount) = CalculateSameColorCounts(colorStatus);

            if (uniqueColorCount == 5) return 2.0f; // Five Colors

            if (quintetCount == 1) return 1.9f; // Quintet

            if (duoGemsCount == 1 && trioGemsCount == 1) return 1.8f; // Duo & Trio

            if (quartetCount == 1) return 1.7f; // Quartet

            if (uniqueColorCount == 4) return 1.6f; // Four Colors

            if (duoGemsCount == 2) return 1.5f; // Two Duos

            if (trioGemsCount == 1) return 1.4f; // Trio

            if (uniqueColorCount == 3) return 1.3f; // Three Colors

            if (duoGemsCount == 1) return 1.2f; // Duo

            if (uniqueColorCount == 2) return 1.1f; // Two Colors

            return 1f; // Default multiplier
        }

        /// <summary>
        /// Calculate the number of duos and trios in the ColorStatus.
        /// </summary>
        public static (int duoGemsCount, int trioGemsCount, int quartetCount, int quintetCount) CalculateSameColorCounts(ColorStatus colorStatus)
        {
            int duoGemsCount = 0;
            int trioGemsCount = 0;
            int quartetCount = 0;
            int quintetCount = 0;

            if (colorStatus.Red == 2) duoGemsCount++;
            if (colorStatus.Red == 3) trioGemsCount++;
            if (colorStatus.Red == 4) quartetCount++;
            if (colorStatus.Red == 5) quintetCount++;

            if (colorStatus.Blue == 2) duoGemsCount++;
            if (colorStatus.Blue == 3) trioGemsCount++;
            if (colorStatus.Blue == 4) quartetCount++;
            if (colorStatus.Blue == 5) quintetCount++;

            if (colorStatus.Yellow == 2) duoGemsCount++;
            if (colorStatus.Yellow == 3) trioGemsCount++;
            if (colorStatus.Yellow == 4) quartetCount++;
            if (colorStatus.Yellow == 5) quintetCount++;

            if (colorStatus.Green == 2) duoGemsCount++;
            if (colorStatus.Green == 3) trioGemsCount++;
            if (colorStatus.Green == 4) quartetCount++;
            if (colorStatus.Green == 5) quintetCount++;

            if (colorStatus.Purple == 2) duoGemsCount++;
            if (colorStatus.Purple == 3) trioGemsCount++;
            if (colorStatus.Purple == 4) quartetCount++;
            if (colorStatus.Purple == 5) quintetCount++;

            return (duoGemsCount, trioGemsCount, quartetCount, quintetCount);
        }

        public static void PrintGemCombinations(List<GemCombination> gemCombinations, bool sortByProfit, bool batchLines)
        {
            StringBuilder sb = new();
            var combos = new List<GemCombination>(gemCombinations);

            SortGemCombinations(combos, sortByProfit);

            if (batchLines)
            {
                foreach (var c in combos)
                {
                    sb.AppendLine(c.ToString());
                }
                Console.WriteLine(sb.ToString());
            }
            else
            {
                foreach (var c in combos)
                {
                    Console.WriteLine(c.ToString());
                }
            }
        }

        public static void SortGemCombinations(List<GemCombination> gemCombinations, bool sortByProfit)
        {
            gemCombinations.Sort(sortByProfit
                ? (x, y) => y.Profit.CompareTo(x.Profit)
                : (x, y) => y.FinalPrice.CompareTo(x.FinalPrice));
        }

        public static string LPad(object input, int totalWidth)
        {
            return (input?.ToString() ?? string.Empty).PadLeft(totalWidth, ' ');
        }

        public static void WriteLineInColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void WriteLineTitle(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[{text}]");
            Console.ResetColor();
        }

        public static void WriteLineCommand(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"> {text}");
            Console.ResetColor();
            Console.Write("$ ");
        }

        #endregion
    }
}