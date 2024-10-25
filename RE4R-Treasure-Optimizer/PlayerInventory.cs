using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Google.OrTools.LinearSolver;
using RE4R_Treasure_Optimizer.Constants;
using static RE4R_Treasure_Optimizer.Core;

namespace RE4R_Treasure_Optimizer
{
    /// <summary>
    /// Represents the player inventory at any given time.
    /// </summary>
    public class PlayerInventory
    {
        #region Helper Types

        public class Treasure : IEquatable<Treasure>
        {
            public string TreasureName;
            public int Ruby;
            public int Sapphire;
            public int YellowDiamond;
            public int Emerald;
            public int Alexandrite;
            public int RedBeryl;

            public Treasure(string treasureName, GemCombination gemCombination)
                : this(treasureName,
                      gemCombination.Gems.Ruby,
                      gemCombination.Gems.Sapphire,
                      gemCombination.Gems.YellowDiamond,
                      gemCombination.Gems.Emerald,
                      gemCombination.Gems.Alexandrite,
                      gemCombination.Gems.RedBeryl)
            {
                // No need for additional initialization
            }

            public Treasure(string treasureName = "", int ruby = 0, int sapphire = 0, int yellowDiamond = 0, int emerald = 0, int alexandrite = 0, int redBeryl = 0)
            {
                TreasureName = treasureName;
                Ruby = ruby;
                Sapphire = sapphire;
                YellowDiamond = yellowDiamond;
                Emerald = emerald;
                Alexandrite = alexandrite;
                RedBeryl = redBeryl;
            }

            public int GetPrice()
            {
                var prices = GetTreasureGemCombinationPrices(TreasureName, Ruby, Sapphire, YellowDiamond, Emerald, Alexandrite, RedBeryl);
                return prices != null ? prices.Value.finalPrice : 0;
            }

            public bool Equals(Treasure? other)
            {
                if (other == null)
                    return false;

                return TreasureName == other.TreasureName &&
                       Ruby == other.Ruby &&
                       Sapphire == other.Sapphire &&
                       YellowDiamond == other.YellowDiamond &&
                       Emerald == other.Emerald &&
                       Alexandrite == other.Alexandrite &&
                       RedBeryl == other.RedBeryl;
            }

            public override bool Equals(object? obj)
            {
                return Equals(obj as Treasure);
            }

            public override int GetHashCode()
            {
                int hash = 17;
                hash = hash * 23 + (TreasureName?.GetHashCode() ?? 0);
                hash = hash * 23 + Ruby.GetHashCode();
                hash = hash * 23 + Sapphire.GetHashCode();
                hash = hash * 23 + YellowDiamond.GetHashCode();
                hash = hash * 23 + Emerald.GetHashCode();
                hash = hash * 23 + Alexandrite.GetHashCode();
                hash = hash * 23 + RedBeryl.GetHashCode();
                return hash;
            }

            public override string ToString()
            {
                Gems g = new(Ruby, Sapphire, YellowDiamond, Emerald, Alexandrite, RedBeryl);
                ColorStatus c = new(Ruby, Sapphire, YellowDiamond, Emerald, Alexandrite, RedBeryl);

                var prices = GetTreasureGemCombinationPrices(TreasureName, Ruby, Sapphire, YellowDiamond, Emerald, Alexandrite, RedBeryl);
                return prices != null 
                    ? $"{TreasureName}: {prices.Value.finalPrice} (P={prices.Value.profit}, G={prices.Value.gemPrice}): {c} ({g})"
                    : $"{TreasureName}: n/a (P=n/a, G=n/a): {c} ({g})";
            }
        }

        public class Inventory
        {
            public int Ruby;
            public int Sapphire;
            public int YellowDiamond;
            public int Emerald;
            public int Alexandrite;
            public int RedBeryl;
            public List<string> Treasures = new();

            public int GemCount => Ruby + Sapphire + YellowDiamond + Emerald + Alexandrite + RedBeryl;
            public int TreasureCount => Treasures.Count;

            public void Setup(List<Treasure> treasureList)
            {
                Clear();
                foreach (var t in treasureList)
                {
                    AddTreasureAndGems(t.TreasureName, t.Ruby, t.Sapphire, t.YellowDiamond, t.Emerald, t.Alexandrite, t.RedBeryl);
                }
            }

            public void Clear()
            {
                Ruby = 0;
                Sapphire = 0;
                YellowDiamond = 0;
                Emerald = 0;
                Alexandrite = 0;
                RedBeryl = 0;
                Treasures.Clear();
            }

            public bool AddToInventory(string name, int quantity, out string? canonName)
            {
                canonName = null;

                if (quantity == 0)
                    return false;

                canonName = GemNames.GetGemName(name) ?? TreasureNames.GetTreasureName(name);
                
                if (canonName == null)
                    return false;

                switch (canonName)
                {
                    case GemNames.RUBY: return AdjustGemCount(ref Ruby, quantity);
                    case GemNames.SAPPHIRE: return AdjustGemCount(ref Sapphire, quantity);
                    case GemNames.YELLOW_DIAMOND: return AdjustGemCount(ref YellowDiamond, quantity);
                    case GemNames.EMERALD: return AdjustGemCount(ref Emerald, quantity);
                    case GemNames.ALEXANDRITE: return AdjustGemCount(ref Alexandrite, quantity);
                    case GemNames.RED_BERYL: return AdjustGemCount(ref RedBeryl, quantity);
                }

                // Structure is kept this way (instead of using a HashSet, etc.), in case of new features in the future.
                switch (canonName)
                {
                    case TreasureNames.BUTTERFLY_LAMP:
                    case TreasureNames.CHALICE_OF_ATONEMENT:
                    case TreasureNames.ELEGANT_BANGLE:
                    case TreasureNames.ELEGANT_CROWN:
                    case TreasureNames.ELEGANT_MASK:
                    case TreasureNames.EXTRAVAGANT_CLOCK:
                    case TreasureNames.FLAGON:
                    case TreasureNames.GOLDEN_LYNX:
                    case TreasureNames.ORNATE_NECKLACE:
                    case TreasureNames.SPLENDID_BANGLE:

                        bool add = quantity > 0;
                        quantity = Math.Abs(quantity);
                        for (int i = 0; i < quantity; i++)
                        {
                            if (add)
                                Treasures.Add(canonName);
                            else
                                Treasures.Remove(canonName);
                        }

                        return true;
                    
                    default:
                        return false;
                }
            }

            private static bool AdjustGemCount(ref int gemCount, int quantity)
            {
                if (gemCount + quantity >= 0)
                {
                    gemCount += quantity;
                    return true;
                }
                return false;
            }

            private void AddTreasureAndGems(string treasureName, int ruby = 0, int sapphire = 0, int yellowDiamond = 0, int emerald = 0, int alexandrite = 0, int redBeryl = 0)
            {
                Treasures.Add(treasureName);
                Ruby += ruby;
                Sapphire += sapphire;
                YellowDiamond += yellowDiamond;
                Emerald += emerald;
                Alexandrite += alexandrite;
                RedBeryl += redBeryl;
            }

            /// <summary>
            /// Attempts to slot gems (from the inventory) into a treasure (also from the inventory) based on the given combination and then outputs said treasure from the inventory.
            /// </summary>
            /// <param name="combination">The gem combination to use.</param>
            /// <param name="treasure">If all requirements are met and the combination is used successfully, outputs a treasure with gems inside of it as
            /// described in the combination, otherwise outputs a NULL value.</param>
            /// <returns>If the inventory has enough gems and treasure to satisfy the attempt,
            /// deduct the same amount of gems and treasure from the inventory and return TRUE, otherwise nothing happens and returns FALSE.</returns>
            public bool TryUseCombination(GemCombination combination, out Treasure treasure)
            {
                (int ruby, int sapphire, int yellowDiamond, int emerald, int alexandrite, int redBeryl) = combination.Gems.GemAmounts();

                if (Ruby < ruby ||
                    Sapphire < sapphire ||
                    YellowDiamond < yellowDiamond ||
                    Emerald < emerald ||
                    Alexandrite < alexandrite ||
                    RedBeryl < redBeryl ||
                    !Treasures.Contains(combination.TreasureName))
                {
                    treasure = new();
                    return false;
                }

                Ruby -= ruby;
                Sapphire -= sapphire;
                YellowDiamond -= yellowDiamond;
                Emerald -= emerald;
                Alexandrite -= alexandrite;
                RedBeryl -= redBeryl;
                Treasures.Remove(combination.TreasureName);

                treasure = new(combination.TreasureName, combination);
                return true;
            }

            public override string ToString()
            {
                StringBuilder sb = new();

                sb.AppendLine($"Gem Count: {GemCount}");
                sb.AppendLine($"    {GemNames.RUBY}: {Ruby}");
                sb.AppendLine($"    {GemNames.SAPPHIRE}: {Sapphire}");
                sb.AppendLine($"    {GemNames.YELLOW_DIAMOND}: {YellowDiamond}");
                sb.AppendLine($"    {GemNames.EMERALD}: {Emerald}");
                sb.AppendLine($"    {GemNames.ALEXANDRITE}: {Alexandrite}");
                sb.AppendLine($"    {GemNames.RED_BERYL}: {RedBeryl}");

                sb.AppendLine($"Treasure Count: {TreasureCount}");
                var treasures = GetCounts(Treasures);
                foreach (var (treasure, count) in treasures)
                {
                    sb.AppendLine($"    {treasure} x{count}");
                }

                // Convert to string and remove the trailing newline if it exists
                string result = sb.ToString();
                return result.EndsWith(Environment.NewLine)
                    ? result.TrimEnd(Environment.NewLine.ToCharArray())
                    : result;
            }

            public SimpleInventory ToSimpleInventory()
            {
                SimpleInventory si = new();

                si.Ruby = Ruby;
                si.Sapphire = Sapphire;
                si.YellowDiamond = YellowDiamond;
                si.Emerald = Emerald;
                si.Alexandrite = Alexandrite;
                si.RedBeryl = RedBeryl;

                foreach (var treasureName in Treasures)
                {
                    si.AddTreasure(treasureName);
                }

                return si;
            }
        }

        public struct SimpleInventory
        {
            // Gems
            public int Ruby;
            public int Sapphire;
            public int YellowDiamond;
            public int Emerald;
            public int Alexandrite;
            public int RedBeryl;

            // Treasures
            public int ButterflyLamp;
            public int ChaliceofAtonement;
            public int ElegantBangle;
            public int ElegantCrown;
            public int ElegantMask;
            public int ExtravagantClock;
            public int Flagon;
            public int GoldenLynx;
            public int OrnateNecklace;
            public int SplendidBangle;

            public void AddTreasure(string treasureName) => IncrementTreasure(treasureName, true);

            public void RemoveTreasure(string treasureName) => IncrementTreasure(treasureName, false);

            private void IncrementTreasure(string treasureName, bool increment)
            {
                int value = increment ? 1 : -1;

                switch (treasureName)
                {
                    case TreasureNames.BUTTERFLY_LAMP: ButterflyLamp += value; break;
                    case TreasureNames.CHALICE_OF_ATONEMENT: ChaliceofAtonement += value; break;
                    case TreasureNames.ELEGANT_BANGLE: ElegantBangle += value; break;
                    case TreasureNames.ELEGANT_CROWN: ElegantCrown += value; break;
                    case TreasureNames.ELEGANT_MASK: ElegantMask += value; break;
                    case TreasureNames.EXTRAVAGANT_CLOCK: ExtravagantClock += value; break;
                    case TreasureNames.FLAGON: Flagon += value; break;
                    case TreasureNames.GOLDEN_LYNX: GoldenLynx += value; break;
                    case TreasureNames.ORNATE_NECKLACE: OrnateNecklace += value; break;
                    case TreasureNames.SPLENDID_BANGLE: SplendidBangle += value; break;
                }
            }

            private int GetTreasureCountByName(string treasureName)
            {
                switch (treasureName)
                {
                    case TreasureNames.BUTTERFLY_LAMP: return ButterflyLamp;
                    case TreasureNames.CHALICE_OF_ATONEMENT: return ChaliceofAtonement;
                    case TreasureNames.ELEGANT_BANGLE: return ElegantBangle;
                    case TreasureNames.ELEGANT_CROWN: return ElegantCrown;
                    case TreasureNames.ELEGANT_MASK: return ElegantMask;
                    case TreasureNames.EXTRAVAGANT_CLOCK: return ExtravagantClock;
                    case TreasureNames.FLAGON: return Flagon;
                    case TreasureNames.GOLDEN_LYNX: return GoldenLynx;
                    case TreasureNames.ORNATE_NECKLACE: return OrnateNecklace;
                    case TreasureNames.SPLENDID_BANGLE: return SplendidBangle;
                    default:
                        Console.Error.WriteLine("Invalid treasure name!"); return 0;
                }
            }

            public int GetTotalTreasureCount()
            {
                return ButterflyLamp
                    + ChaliceofAtonement
                    + ElegantBangle
                    + ElegantCrown
                    + ElegantMask
                    + ExtravagantClock
                    + Flagon
                    + GoldenLynx
                    + OrnateNecklace
                    + SplendidBangle;
            }

            public IEnumerable<string> GetTreasureNames()
            {
                for (int i = 0; i < ButterflyLamp; i++) yield return TreasureNames.BUTTERFLY_LAMP;
                for (int i = 0; i < ChaliceofAtonement; i++) yield return TreasureNames.CHALICE_OF_ATONEMENT;
                for (int i = 0; i < ElegantBangle; i++) yield return TreasureNames.ELEGANT_BANGLE;
                for (int i = 0; i < ElegantCrown; i++) yield return TreasureNames.ELEGANT_CROWN;
                for (int i = 0; i < ElegantMask; i++) yield return TreasureNames.ELEGANT_MASK;
                for (int i = 0; i < ExtravagantClock; i++) yield return TreasureNames.EXTRAVAGANT_CLOCK;
                for (int i = 0; i < Flagon; i++) yield return TreasureNames.FLAGON;
                for (int i = 0; i < GoldenLynx; i++) yield return TreasureNames.GOLDEN_LYNX;
                for (int i = 0; i < OrnateNecklace; i++) yield return TreasureNames.ORNATE_NECKLACE;
                for (int i = 0; i < SplendidBangle; i++) yield return TreasureNames.SPLENDID_BANGLE;
            }
        }

        #endregion

        #region Fields

        /*
            1. Stores a list of treasures initially slotted with gems. A preset can be used to generate this list.
            2. Represents the total sell price for the initial list of treasures.
            3. An inventory of all available gems and treasures, created by extracting gems from the treasures and adding them all into the inventory.
            4. A list of treasures with gems that have been re-slotted (either manually or via an algorithm) from the inventory.
                Ideally, the inventory should be empty by the end, and the treasures should be optimally slotted with gems.
            5. Represents the total sell price for the newly re-slotted treasures. If this matches or exceeds the expected sell price
                from step 2, then the configuration is either equal or optimized compared to the initial preset.
        */

        private List<Treasure> _startingTreasureList = new(); // #1
        private int _startingSellPrice; // #2
        private Inventory _inventory = new(); // #3
        private List<Treasure> _newTreasureList = new(); // 4
        private int _newSellPrice; // #5

        private Config _config = new();

        public int StartingSellPrice => _startingSellPrice;
        public int NewSellPrice => _newSellPrice;

        #endregion

        #region Initialization and Deinitialization

        public bool SetStartingTreasureListAndInventoryFromPreset(string presetName, bool clearCurrentSellConfiguration = true)
        {
            if (string.IsNullOrEmpty(presetName))
            {
                Console.Error.WriteLine("Preset name was undefined, cannot load preset.");
                return false;
            }

            var treasuresAndGems = ParseGemCombinationDataFromResource(presetName);

            _startingTreasureList.Clear();

            foreach (var tg in treasuresAndGems)
            {
                Treasure treasure = new(tg.treasureName, tg.ruby, tg.sapphire, tg.yellowDiamond, tg.emerald, tg.alexandrite, tg.redBeryl);
                _startingTreasureList.Add(treasure);
            }

            _inventory.Setup(_startingTreasureList);
            _startingSellPrice = GetTotalSellPriceFromTreasureList(_startingTreasureList);

            if (clearCurrentSellConfiguration)
            {
                _newTreasureList.Clear();
                _newSellPrice = 0;
            }

            return true;
        }

        public bool AddToInventory(string targetName, int quantity)
        {
            if (quantity == 0)
            {
                WriteLineInColor($"No changes made.", ConsoleColor.DarkMagenta);
                return true;
            }

            if (_inventory.AddToInventory(targetName, quantity, out string? canonName))
            {
                WriteLineInColor($"{(quantity > 0 ? "Added" : "Removed")} {quantity} {canonName}", ConsoleColor.DarkMagenta);
                return true;
            }
            else
            {
                WriteLineInColor($"FAILED to {(quantity > 0 ? "add" : "remove")} {quantity} {canonName}", ConsoleColor.Red);
                return false;
            }
        }

        public void Clear()
        {
            _startingTreasureList.Clear();
            _startingSellPrice = 0;
            _inventory.Clear();
            _newTreasureList.Clear();
            _newSellPrice = 0;
        }

        #endregion

        #region Algorithms

        /// <summary>
        /// Legacy algorithm, uses greedy approach.
        /// </summary>
        public void GenerateNewTreasureListByGreedyProfit()
        {
            _newTreasureList.Clear();
            var combosRankedByProfit = _config.GetAllGemCombinations(true);

            foreach (var combo in combosRankedByProfit)
            {
                int treasureCount = _inventory.Treasures.Count;
                for (int i = 0; i < treasureCount; i++)
                {
                    if (_inventory.TryUseCombination(combo, out var treasure))
                        _newTreasureList.Add(treasure);
                }

                if (_inventory.Treasures.Count == 0)
                    break;
            }

            _newSellPrice = GetTotalSellPriceFromTreasureList(_newTreasureList);
        }
        
        public void GenerateNewTreasureListWithORToolsRunner()
        {
            _newTreasureList.Clear();
            _newTreasureList = GenerateNewTreasureListWithORTools() ?? new();
            _newSellPrice = GetTotalSellPriceFromTreasureList(_newTreasureList);
            CompareTreasureLists();
        }
        
        /// <summary>
        /// Main optimization algorithm.
        /// </summary>
        public List<Treasure>? GenerateNewTreasureListWithORTools()
        {
            // Create simple inventory
            SimpleInventory si = _inventory.ToSimpleInventory(); // _inventory is preconfigured at SetStartingTreasureListAndInventoryFromPreset()

            // Initialize the solver
            Solver solver = Solver.CreateSolver("SCIP");
            if (solver is null)
            {
                Console.Error.WriteLine("Solver could not be created.");
                return null;
            }

            // Get treasure counts
            var treasureCounts = new Dictionary<string, int>
            {
                { TreasureNames.BUTTERFLY_LAMP, si.ButterflyLamp },
                { TreasureNames.CHALICE_OF_ATONEMENT, si.ChaliceofAtonement },
                { TreasureNames.ELEGANT_BANGLE, si.ElegantBangle },
                { TreasureNames.ELEGANT_CROWN, si.ElegantCrown },
                { TreasureNames.ELEGANT_MASK, si.ElegantMask },
                { TreasureNames.EXTRAVAGANT_CLOCK, si.ExtravagantClock },
                { TreasureNames.FLAGON, si.Flagon },
                { TreasureNames.GOLDEN_LYNX, si.GoldenLynx },
                { TreasureNames.ORNATE_NECKLACE, si.OrnateNecklace },
                { TreasureNames.SPLENDID_BANGLE, si.SplendidBangle }
            };

            // Get gem counts
            var gemCounts = new Dictionary<string, int>
            {
                { GemNames.RUBY, si.Ruby },
                { GemNames.SAPPHIRE, si.Sapphire },
                { GemNames.YELLOW_DIAMOND, si.YellowDiamond },
                { GemNames.EMERALD, si.Emerald },
                { GemNames.ALEXANDRITE, si.Alexandrite },
                { GemNames.RED_BERYL, si.RedBeryl }
            };

            // List to store variables and their associated gem combinations
            List<(GemCombination GemCombination, Variable Variable)> combinationVars = new();

            // Create variables for each gem combination
            foreach (var treasureName in treasureCounts.Keys)
            {
                int treasureCount = treasureCounts[treasureName];

                var gemCombinations = _config.GetGemCombinations(treasureName);

                foreach (var gemCombination in gemCombinations)
                {
                    // Variable name for debugging purposes
                    string varName = $"x_{gemCombination.TreasureName}_{gemCombination.Gems}";
                    Variable x_c = solver.MakeIntVar(0, treasureCount, varName);
                    combinationVars.Add((gemCombination, x_c));
                }
            }

            // Constraints for treasures
            foreach (var treasureName in treasureCounts.Keys)
            {
                int treasureCount = treasureCounts[treasureName];

                var varsForTreasure = combinationVars
                    .Where(cv => cv.GemCombination.TreasureName == treasureName)
                    .Select(cv => cv.Variable)
                    .ToArray();

                // Add constraint: sum of variables for this treasure <= available treasures
                solver.Add(varsForTreasure.Sum() <= treasureCount);
            }

            // Constraints for gems
            string[] gemNames = new string[]
            {
                GemNames.RUBY, GemNames.SAPPHIRE, GemNames.YELLOW_DIAMOND, GemNames.EMERALD, GemNames.ALEXANDRITE, GemNames.RED_BERYL
            };

            foreach (var gemName in gemNames)
            {
                int gemCount = gemCounts[gemName];

                // Create a new constraint for the gem usage
                Constraint gemConstraint = solver.MakeConstraint(0, gemCount, $"GemConstraint_{gemName}");

                foreach (var (gemCombination, x_c) in combinationVars)
                {
                    int gemUsage = gemCombination.Gems.GetGemCountByName(gemName);
                    if (gemUsage > 0)
                    {
                        gemConstraint.SetCoefficient(x_c, gemUsage);
                    }
                }
            }

            // Objective: Maximize total sell price
            Objective objective = solver.Objective();

            foreach (var (gemCombination, x_c) in combinationVars)
            {
                int finalPrice = gemCombination.FinalPrice;
                objective.SetCoefficient(x_c, finalPrice);
            }

            objective.SetMaximization();

            // Solve the problem
            Solver.ResultStatus resultStatus = solver.Solve();

            if (resultStatus == Solver.ResultStatus.OPTIMAL || resultStatus == Solver.ResultStatus.FEASIBLE)
            {
                // Retrieve the solution
                List<Treasure> result = new List<Treasure>();

                foreach (var (gemCombination, x_c) in combinationVars)
                {
                    int count = (int)x_c.SolutionValue();

                    for (int i = 0; i < count; i++)
                    {
                        // Create a Treasure object with the gem combination
                        Treasure treasure = new(gemCombination.TreasureName, gemCombination);
                        result.Add(treasure);
                    }
                }

                WriteLineInColor("Optimal solution found!", ConsoleColor.Magenta);
                return result;
            }
            else
            {
                Console.Error.WriteLine("No optimal solution found.");
                return null;
            }
        }

        #endregion

        #region Check Status

        public void ViewAll()
        {
            ViewStartingTreasureList();
            Console.WriteLine();

            ViewInventory();
            Console.WriteLine();

            ViewNewTreasureList();
        }

        public void ViewStartingTreasureList()
        {
            PrintTreasureList("STARTING TREASURE LIST", _startingTreasureList);
        }

        public void ViewNewTreasureList()
        {
            PrintTreasureList("NEW TREASURE LIST", _newTreasureList);
        }

        public void ViewInventory()
        {
            WriteLineTitle("CURRENT INVENTORY");
            Console.WriteLine(_inventory.ToString());
        }

        public void CompareTreasureLists()
        {
            if (_startingTreasureList.Count == 0)
            {
                WriteLineInColor("LIST DIFF: Nothing to compare.", ConsoleColor.DarkGray);
                return;
            }

            List<Treasure> startingTreasureListLeftovers = new(_startingTreasureList);
            List<Treasure> newTreasureListLeftovers = new(_newTreasureList);

            HashSet<int> startingTreasureListIndicesToRemove = new();
            HashSet<int> newTreasureListIndicesToRemove = new();

            for (int i = 0; i < startingTreasureListLeftovers.Count; i++)
            {
                for (int j = 0; j < newTreasureListLeftovers.Count; j++)
                {
                    if (newTreasureListIndicesToRemove.Contains(j))
                        continue;

                    if (startingTreasureListLeftovers[i].Equals(newTreasureListLeftovers[j]))
                    {
                        startingTreasureListIndicesToRemove.Add(i);
                        newTreasureListIndicesToRemove.Add(j);
                        break;
                    }
                }
            }

            for (int i = startingTreasureListLeftovers.Count - 1; i >= 0; i--)
            {
                if (startingTreasureListIndicesToRemove.Contains(i))
                    startingTreasureListLeftovers.RemoveAt(i);
            }

            for (int i = newTreasureListLeftovers.Count - 1; i >= 0; i--)
            {
                if (newTreasureListIndicesToRemove.Contains(i))
                    newTreasureListLeftovers.RemoveAt(i);
            }

            if (startingTreasureListLeftovers.Count == 0 && newTreasureListLeftovers.Count == 0)
            {
                WriteLineInColor("LIST DIFF: No difference.", ConsoleColor.DarkGray);
                return;
            }

            WriteLineInColor("LIST DIFF: Is different.", ConsoleColor.DarkGray);
            if (startingTreasureListLeftovers.Count > 0)
            {
                WriteLineInColor($"    [STARTING TREASURE LIST] : {GetTotalSellPriceFromTreasureList(startingTreasureListLeftovers)}"
                    , ConsoleColor.DarkGray);
                for (int i = 0; i < startingTreasureListLeftovers.Count; i++)
                {
                    Console.WriteLine($"        {i}. {startingTreasureListLeftovers[i]}");
                }
            }


            if (newTreasureListLeftovers.Count > 0)
            {
                WriteLineInColor($"    [NEW TREASURE LIST] : {GetTotalSellPriceFromTreasureList(newTreasureListLeftovers)}"
                    , ConsoleColor.DarkGray);
                for (int i = 0; i < newTreasureListLeftovers.Count; i++)
                {
                    Console.WriteLine($"        {i}. {newTreasureListLeftovers[i]}");
                }
            }
        }

        #endregion

        #region Utilities

        public static int GetTotalSellPriceFromTreasureList(List<Treasure> treasureList)
        {
            int totalSellPrice = 0;

            foreach (var t in treasureList)
            {
                totalSellPrice += t.GetPrice();
            }

            return totalSellPrice;
        }

        public static void PrintTreasureList(string listName, List<Treasure> treasureList)
        {
            WriteLineTitle(listName);
            Console.WriteLine($"Count: {treasureList.Count}");

            if (treasureList.Count > 0)
            {
                Console.WriteLine("Content: ");
                for (int i = 0; i < treasureList.Count; i++)
                {
                    Console.WriteLine($"    {LPad(i + 1, 2)}. {treasureList[i]}");
                }
            }

            Console.WriteLine($"Total Price: {GetTotalSellPriceFromTreasureList(treasureList)}");
        }

        private static Dictionary<T, int> GetCounts<T>(IEnumerable<T> items) where T : notnull
        {
            var counts = new Dictionary<T, int>();

            foreach (var item in items)
            {
                if (counts.ContainsKey(item))
                    counts[item]++;
                else
                    counts[item] = 1;
            }

            return counts;
        }

        #endregion
    }
}