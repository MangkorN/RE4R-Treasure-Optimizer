# RE4R Treasure Optimizer

## Overview

**RE4R Treasure Optimizer** is a console application designed to optimize treasure and gem configurations for maximum sell value in **Resident Evil 4 Remake (RE4R)**.

The application enables users to:
- Load preset treasure and gem configurations.
- Manage an inventory by adding or removing gems and treasures.
- Optimize gem placements in treasures to maximize total sell price.
- View treasure lists, inventory, and optimization results.
- Run tests on all presets to compare optimization outcomes.

## Features

- **Preset Loading**: Load predefined treasure and gem combinations based on available presets.
- **Inventory Management**: Add or remove gems and treasures to customize your inventory.
- **Optimization Algorithm**: Automatically optimize gem placements in treasures to maximize sell price.
- **Sell Price Calculation**: Calculate and compare the total sell price before and after optimization.
- **Command Help**: Built-in help command to display all available commands and their usage.

## Getting Started

### Prerequisites

- **.NET 6.0 SDK** or later.

### How to Run

You can build the application as a self-contained executable for different platforms. This allows the application to run on machines without the .NET runtime installed.

1. **Clone the repository**:

    ```bash
    git clone https://github.com/MangkorN/RE4R-Treasure-Optimizer.git
    cd RE4R-Treasure-Optimizer
    ```

2. **Publish the application as a self-contained executable**:

    Replace `{RID}` with the Runtime Identifier (RID) for your target platform. Here are some common RIDs:

    - `win-x64` for Windows 64-bit
    - `linux-x64` for Linux 64-bit
    - `osx-x64` for macOS Intel 64-bit
    - `osx-arm64` for macOS Apple Silicon (M1/M2)

    ```bash
    dotnet publish -c Release -r {RID} --self-contained true -p:PublishSingleFile=true
    ```

    **Examples**:

    - **Windows 64-bit**:

      ```bash
      dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
      ```

    - **Linux 64-bit**:

      ```bash
      dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
      ```

    - **macOS Intel**:

      ```bash
      dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true
      ```

    - **macOS Apple Silicon**:

      ```bash
      dotnet publish -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true
      ```

    This command will create a self-contained, single-file executable in the `bin/Release/net6.0/{RID}/publish/` directory.

3. **Run the application**:

    Navigate to the publish directory:

    ```bash
    cd bin/Release/net6.0/{RID}/publish/
    ```

    - **On Windows**:

      ```bash
      RE4R-Treasure-Optimizer.exe
      ```

    - **On Linux/macOS**:

      ```bash
      ./RE4R-Treasure-Optimizer
      ```

    The application will start in **Interactive Mode**, allowing you to input various commands.

## Interactive Commands

### General

- `help` - Show available commands.
- `clear` - Clears the screen.
- `exit` - Exit the application.

### Main Features

- `load [preset_id]` - Load a preset to generate the starting treasure list.
- `optimize` - Generate a new and optimized treasure list.
- `view` - View treasure lists and inventory.
- `reset` - Reset treasure lists and inventory

### Inventory Management

- `add [quantity] [name]` - Add a specified quantity of a gem or treasure to your inventory.
- `rm [quantity] [name]` - Remove a specified quantity of a gem or treasure from your inventory.
- `names` - View all gem and treasure names and their aliases.

### Extras

- `view start` or `view s` - View the starting treasure list.
- `view inventory` or `view i` - View the current inventory.
- `view new` or `view n` - View the new treasure list after optimization.
- `test` - Test all presets.

## Presets

Below are the preset IDs available in the application:

- **0**: Base Game w/Merchant + DLC
- **1**: Base Game + DLC
- **2**: Base Game w/Merchant
- **3**: Base Game

## Gems and Treasures

Use the `names` command to see all available gem and treasure names and their aliases.

### Gem Names

- **Ruby** (`ruby`, `r`)
- **Sapphire** (`sapphire`, `s`)
- **Yellow Diamond** (`yellowdiamond`, `yellow`, `yd`)
- **Emerald** (`emerald`, `e`)
- **Alexandrite** (`alexandrite`, `a`)
- **Red Beryl** (`redberyl`, `red`, `rb`)

### Treasure Names

- **Butterfly Lamp** (`butterflylamp`, `bl`)
- **Chalice of Atonement** (`chaliceofatonement`, `coa`)
- **Elegant Bangle** (`elegantbangle`, `eb`)
- **Elegant Crown** (`elegantcrown`, `ec`)
- **Elegant Mask** (`elegantmask`, `em`)
- **Extravagant Clock** (`extravagantclock`, `exc`)
- **Flagon** (`flagon`, `f`)
- **Golden Lynx** (`goldenlynx`, `gl`)
- **Ornate Necklace** (`ornatenecklace`, `on`)
- **Splendid Bangle** (`splendidbangle`, `sb`)

## Example Usage

1. Load a preset (e.g., preset 0):

    ```plaintext
    load 0
    ```

2. View the starting treasure list:

    ```plaintext
    view start
    ```

3. Optimize the loaded preset:

    ```plaintext
    optimize
    ```

4. View the optimization result:

    ```plaintext
    view new
    ```

5. Add gems to your inventory:

    ```plaintext
    add 2 ruby
    ```

6. Remove gems from your inventory:

    ```plaintext
    rm 1 sapphire
    ```

7. View your inventory:

    ```plaintext
    view inventory
    ```

8. Reset the application:

    ```plaintext
    reset
    ```

## Contribution

Contributions are welcome! Feel free to open an issue or submit a pull request to improve the code, add new presets, or enhance the optimization algorithm.

## License

This project is licensed under the [MIT License](./LICENSE) for the original code. 

It also uses third-party components (such as Google OR-Tools), which are licensed under the [Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0). 

See the [LICENSE](./LICENSE) file in the repository root for more details.