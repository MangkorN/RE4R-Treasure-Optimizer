using System.Collections.Generic;
using System.Diagnostics;
using static RE4R_Treasure_Optimizer.Core;

namespace RE4R_Treasure_Optimizer
{
    /// <summary>
    /// Contains all the information regarding gem combination for treasures.
    /// </summary>
    public class Config
    {
        private Dictionary<string, List<GemCombination>> _treasureToGemCombinations = new();

        /// <summary>
        /// Gets the dictionary mapping treasure names to their respective lists of <see cref="GemCombination"/> objects.
        /// If the dictionary is uninitialized or empty, it generates the treasure-gem combinations before returning.
        /// </summary>
        /// <returns>
        /// A dictionary where each key is a treasure name and the value is a list of corresponding gem combinations.
        /// </returns>
        public Dictionary<string, List<GemCombination>> TreasureToGemCombinations
        {
            get
            {
                if (_treasureToGemCombinations == null || _treasureToGemCombinations.Count == 0)
                {
                    _treasureToGemCombinations = GenerateTreasureToGemCombinations();
                }
                return _treasureToGemCombinations;
            }
        }

        /// <summary>
        /// Returns a list of all <see cref="GemCombination"/> objects from all treasures combined.
        /// The list is optionally sorted by profit if <paramref name="sortByProfit"/> is set to true.
        /// </summary>
        /// <param name="sortByProfit">Specifies whether to sort the list by profit. Default is false.</param>
        /// <returns>A list of all gem combinations from all treasures.</returns>
        public List<GemCombination> GetAllGemCombinations(bool sortByProfit = false)
        {
            var allCombos = new List<GemCombination>();
            foreach (var (_, gemCombinations) in TreasureToGemCombinations)
            {
                allCombos.AddRange(gemCombinations);
            }
            SortGemCombinations(allCombos, sortByProfit);
            return allCombos;
        }

        /// <summary>
        /// Returns a list of <see cref="GemCombination"/> objects for a specific treasure based on the provided <paramref name="treasureName"/>.
        /// The list is optionally sorted by profit if <paramref name="sortByProfit"/> is set to true.
        /// </summary>
        /// <param name="treasureName">The name of the treasure to retrieve gem combinations for.</param>
        /// <param name="sortByProfit">Specifies whether to sort the list by profit. Default is false.</param>
        /// <returns>A list of gem combinations for the specified treasure.</returns>
        public List<GemCombination> GetGemCombinations(string treasureName, bool sortByProfit = false)
        {
            var combos = new List<GemCombination>(TreasureToGemCombinations[treasureName]);
            SortGemCombinations(combos, sortByProfit);
            return combos;
        }
    }
}