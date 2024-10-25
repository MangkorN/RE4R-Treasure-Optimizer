using System;
using System.Collections.Generic;
using RE4R_Treasure_Optimizer.Constants;
using static RE4R_Treasure_Optimizer.Core;

namespace RE4R_Treasure_Optimizer
{
    public class Program
    {
        public const int PRESET_ID_MIN = 0;
        public const int PRESET_ID_MAX = 3;

        private static bool _presetIsActive = false;
        private static int _presetId = -1;
        private static string _presetName = "";

        private static void Main()
        {
            WriteLineInColor("Welcome to the RE4R Treasure Optimizer!", ConsoleColor.White);
            RunInteractiveMode();
        }

        static void RunInteractiveMode()
        {
            PlayerInventory playerInventory = new();

            while (true)
            {
                WriteLineCommand("Please enter a command:");
                string command = (Console.ReadLine() ?? "").ToLower();

                switch (command)
                {
                    // GENERAL
                    case "help":
                        DisplayHelp();
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    case "exit":
                        return;

                    // MAIN FEATURES
                    case "optimize":
                        Optimize(playerInventory);
                        PrintResult(playerInventory);
                        break;
                    case "view":
                        playerInventory.ViewAll();
                        break;
                    case "reset":
                        Reset(playerInventory);
                        break;

                    // ADD/REMOVE FROM INVENTORY
                    case "names":
                        DisplayGemAndTreasureNames();
                        break;

                    // EXTRAS
                    case "test":
                        TestAllPresets(playerInventory);
                        break;

                    default:

                        // VIEW INVENTORY AND TREASURE LISTS
                        if (ViewCommand(command, playerInventory))
                            break;

                        // LOAD PRESET
                        if (LoadPresetCommand(command, playerInventory))
                            break;

                        // INVENTORY
                        if (InventoryCommand(command, playerInventory))
                            break;

                        Console.WriteLine("Unknown command. Type 'help' for available commands.");
                        break;
                }

                Console.WriteLine();
            }
        }

        #region Multi Input Commands

        #region Utilities

        private enum ArgumentType { Command, Number, Text }

        private readonly struct ArgumentPattern
        {
            private readonly string[] _validArguments;
            private readonly ArgumentType _type;

            public ArgumentPattern(ArgumentType type, params string[] validArguments)
            {
                _type = type;
                _validArguments = validArguments ?? Array.Empty<string>();
            }

            public bool Matches(string argument)
            {
                return _type switch
                {
                    ArgumentType.Number => int.TryParse(argument, out _),
                    ArgumentType.Text => !string.IsNullOrWhiteSpace(argument),
                    ArgumentType.Command => _validArguments.Length > 0 && Array.Exists(_validArguments, arg => arg == argument),
                    _ => false
                };
            }
        }

        private static bool ValidateCommandFormat(string commandLine, ArgumentPattern[] patterns, out string[] args)
        {
            args = commandLine.Split(' ', patterns.Length);

            if (args.Length != patterns.Length)
                return false;

            for (int i = 0; i < patterns.Length; i++)
            {
                if (!patterns[i].Matches(args[i]))
                    return false;
            }
            return true;
        }

        #endregion

        /// <summary>
        /// Run a command to view inventory.
        /// </summary>
        /// <param name="command">The command to try execute.</param>
        /// <param name="playerInventory">The player inventory to execute the command on.</param>
        /// <returns>TRUE if the command was a successful. FALSE otherwise.</returns>
        private static bool ViewCommand(string command, PlayerInventory playerInventory)
        {
            ArgumentPattern[] patterns = {
                new(ArgumentType.Command, "view", "v"),
                new(ArgumentType.Command, "start", "s", "new", "n", "inventory", "i")
            };

            if (!ValidateCommandFormat(command, patterns, out var args))
                return false;

            switch (args[1])
            {
                case "start":
                case "s":
                    playerInventory.ViewStartingTreasureList();
                    return true;

                case "new":
                case "n":
                    playerInventory.ViewNewTreasureList();
                    return true;

                case "inventory":
                case "i":
                    playerInventory.ViewInventory();
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Run a command to load a preset.
        /// </summary>
        /// <param name="command">The command to try execute.</param>
        /// <param name="playerInventory">The player inventory to execute the command on.</param>
        /// <returns>TRUE if the command was a successful. FALSE otherwise.</returns>
        private static bool LoadPresetCommand(string command, PlayerInventory playerInventory)
        {
            ArgumentPattern[] patterns = {
                new(ArgumentType.Command, "load"),
                new(ArgumentType.Number)
            };

            if (!ValidateCommandFormat(command, patterns, out var args))
                return false;

            int id = int.Parse(args[1]);
            TryLoadPreset(playerInventory, id, true);

            return true;
        }

        /// <summary>
        /// Run a command to modify the inventory.
        /// </summary>
        /// <param name="command">The command to try execute.</param>
        /// <param name="playerInventory">The player inventory to execute the command on.</param>
        /// <returns>TRUE if the command was a successful. FALSE otherwise.</returns>
        private static bool InventoryCommand(string command, PlayerInventory playerInventory)
        {
            ArgumentPattern[] patterns = {
                new(ArgumentType.Command, "add", "rm"),
                new(ArgumentType.Number),
                new(ArgumentType.Text)
            };

            if (!ValidateCommandFormat(command, patterns, out var args))
                return false;

            string action = args[0];
            int quantity = int.Parse(args[1]);
            string name = args[2];

            if (action == "add")
            {
                // no need to reverse quantity
            }
            else if (action == "rm")
            {
                quantity = -quantity;
            }

            if (playerInventory.AddToInventory(name, quantity))
            {
                SetPresetLoadStatus(false); // Exisitng preset (if any) has been modified, so mark as "dirty".
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Display Help

        private static void DisplayHelp()
        {
            WriteLineTitle("AVAILABLE COMMANDS");

            //Console.WriteLine();
            WriteLineInColor("~ GENERAL ~", ConsoleColor.DarkGray);
            Console.WriteLine("help                     - Show available commands");
            Console.WriteLine("clear                    - Clears the screen");
            Console.WriteLine("exit                     - Exit the application");

            Console.WriteLine();
            WriteLineInColor("~ MAIN FEATURES ~", ConsoleColor.DarkGray);
            Console.WriteLine("load                     - Load preset to generate starting treasure list");
            Console.WriteLine("optimize                 - Generate new and optimized treasure list");
            Console.WriteLine("view                     - View treasure lists and inventory");
            Console.WriteLine("reset                    - Reset treasure lists and inventory");

            Console.WriteLine();
            WriteLineInColor("~ ADD/REMOVE FROM INVENTORY ~", ConsoleColor.DarkGray);
            Console.WriteLine("add [quantity] [name]   - Add [quantity] amount(s) of [name] to inventory.");
            Console.WriteLine("rm [quantity] [name]    - Remove [quantity] amount(s) of [name] from inventory.");
            Console.WriteLine("names                   - View all gem and treasure names.");

            Console.WriteLine();
            WriteLineInColor("~ EXTRAS ~", ConsoleColor.DarkGray);
            Console.WriteLine("view start, view s       - View starting treasure list");
            Console.WriteLine("view inventory, view i   - View current inventory");
            Console.WriteLine("view new, view n         - View new treasure list");
            Console.WriteLine("test                     - Test all presets");
            Console.WriteLine("reset                    - Reset treasure lists and inventory");

            Console.WriteLine();
            WriteLineInColor("~ PRESETS ~", ConsoleColor.DarkGray);
            for (int i = PRESET_ID_MIN; i < PRESET_ID_MAX + 1; i++)
            {
                Console.WriteLine($"{i}: {IdToPresetName(i)}");
            }
        }

        private static void DisplayGemAndTreasureNames()
        {
            Console.WriteLine();
            WriteLineInColor("GEM NAMES", ConsoleColor.DarkGray);
            Console.WriteLine($"- {GetNameAndAliases(GemNames.RUBY, GemNames.GemAliases)}");
            Console.WriteLine($"- {GetNameAndAliases(GemNames.SAPPHIRE, GemNames.GemAliases)}");
            Console.WriteLine($"- {GetNameAndAliases(GemNames.YELLOW_DIAMOND, GemNames.GemAliases)}");
            Console.WriteLine($"- {GetNameAndAliases(GemNames.EMERALD, GemNames.GemAliases)}");
            Console.WriteLine($"- {GetNameAndAliases(GemNames.ALEXANDRITE, GemNames.GemAliases)}");
            Console.WriteLine($"- {GetNameAndAliases(GemNames.RED_BERYL, GemNames.GemAliases)}");
            WriteLineInColor("TREASURE NAMES", ConsoleColor.DarkGray);
            Console.WriteLine($"- {GetNameAndAliases(TreasureNames.BUTTERFLY_LAMP, TreasureNames.TreasureAliases)}");
            Console.WriteLine($"- {GetNameAndAliases(TreasureNames.CHALICE_OF_ATONEMENT, TreasureNames.TreasureAliases)}");
            Console.WriteLine($"- {GetNameAndAliases(TreasureNames.ELEGANT_BANGLE, TreasureNames.TreasureAliases)}");
            Console.WriteLine($"- {GetNameAndAliases(TreasureNames.ELEGANT_CROWN, TreasureNames.TreasureAliases)}");
            Console.WriteLine($"- {GetNameAndAliases(TreasureNames.ELEGANT_MASK, TreasureNames.TreasureAliases)}");
            Console.WriteLine($"- {GetNameAndAliases(TreasureNames.EXTRAVAGANT_CLOCK, TreasureNames.TreasureAliases)}");
            Console.WriteLine($"- {GetNameAndAliases(TreasureNames.FLAGON, TreasureNames.TreasureAliases)}");
            Console.WriteLine($"- {GetNameAndAliases(TreasureNames.GOLDEN_LYNX, TreasureNames.TreasureAliases)}");
            Console.WriteLine($"- {GetNameAndAliases(TreasureNames.ORNATE_NECKLACE, TreasureNames.TreasureAliases)}");
            Console.WriteLine($"- {GetNameAndAliases(TreasureNames.SPLENDID_BANGLE, TreasureNames.TreasureAliases)}");
            Console.WriteLine();
        }

        /// <summary>
        /// Note: This function assumes that the first element of the aliases is the same as the canonical name.
        /// </summary>
        /// <param name="name">The canonical name. (e.g. Ruby)</param>
        /// <param name="aliases">An array of aliases. (e.g. Ruby, ruby, r)</param>
        /// <returns>A formatted string that describes a canonical name and its aliases.</returns>
        private static string GetNameAndAliases(string name, Dictionary<string, string[]> aliases)
        {
            return $"{name} ({string.Join(", ", aliases[name].Skip(1))})";
        }

        #endregion

        #region Loading Presets

        private static bool TryLoadPreset(PlayerInventory playerInventory, int presetId, bool verbose = true)
        {
            string presetName = IdToPresetName(presetId);

            bool success = playerInventory.SetStartingTreasureListAndInventoryFromPreset(presetName);

            if (success)
                SetPresetLoadStatus(true, presetId, presetName);

            if (verbose)
            {
                WriteLineInColor(success ? $"Preset '{presetName}' loaded. ({playerInventory.StartingSellPrice})"
                                         : "Failed to load preset", ConsoleColor.DarkCyan);
            }

            return success;
        }

        private static string IdToPresetName(int presetId)
        {
            if (presetId < PRESET_ID_MIN || presetId > PRESET_ID_MAX)
            {
                Console.Error.WriteLine($"Error: Preset ID '{presetId}' is out of the valid range ({PRESET_ID_MIN}-{PRESET_ID_MAX}).");
                return string.Empty;
            }

            switch (presetId)
            {
                case 0: return PresetNames.BASEGAME_MERCHANT_DLC;
                case 1: return PresetNames.BASEGAME_DLC;
                case 2: return PresetNames.BASEGAME_MERCHANT;
                case 3: return PresetNames.BASEGAME;
                default:
                    break;
            }

            Console.Error.WriteLine($"Error: Preset ID '{presetId}' is not recognized.");
            return string.Empty;
        }

        private static void SetPresetLoadStatus(bool isActive, int id = -1, string name = "")
        {
            _presetIsActive = isActive;
            _presetId = id;
            _presetName = name;
        }

        #endregion

        private static void Optimize(PlayerInventory playerInventory)
        {
            playerInventory.GenerateNewTreasureListWithORToolsRunner();
        }

        private static void Reset(PlayerInventory playerInventory)
        {
            playerInventory.Clear();
            SetPresetLoadStatus(false);
        }

        private static void TestAllPresets(PlayerInventory playerInventory)
        {
            WriteLineInColor("- TEST START -", ConsoleColor.DarkGray);

            for (int i = PRESET_ID_MIN; i < PRESET_ID_MAX + 1; i++)
            {
                Reset(playerInventory);
                TryLoadPreset(playerInventory, i, false);
                Optimize(playerInventory);
                PrintResult(playerInventory);
            }
            Reset(playerInventory);

            WriteLineInColor("- TEST END -", ConsoleColor.DarkGray);
        }

        private static void PrintResult(PlayerInventory playerInventory)
        {
            WriteLineInColor(GetResult(playerInventory), ConsoleColor.Yellow);
        }

        private static string GetResult(PlayerInventory playerInventory)
        {
            int goal = playerInventory.StartingSellPrice;
            int result = playerInventory.NewSellPrice;

            // When a preset is active, our "goal" is to reach or exceed the total price of the preset.
            // If we're not using a preset, then we don't have a goal, and we only care about the result.
            string presetInfo = _presetIsActive ? $" ({_presetId}) < {_presetName} >" : "";
            string goalInfo = _presetIsActive ? $"Goal: {goal}, " : "";
            
            return goalInfo + $"Result: {result}" + presetInfo;
        }
    }
}