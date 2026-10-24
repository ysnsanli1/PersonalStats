# Personal Stats — a Vintage Story mod

A personal stats panel for Vintage Story. See how far you have walked, how many blocks you have mined, how many times you have died and how many days you have survived.

> Client-side only. No need to install it on the server, and it works on any server you join.

## Features

- **Distance walked**
- **Blocks mined**
- **Death count**
- **Days survived**
- Stats are saved to disk per savegame, so they persist after closing the game.

## Installation

1. Download the latest `.zip` from the [Releases](../../releases) page (or from the ModDB page).
2. Put the zip file, without extracting it, into your Vintage Story `Mods` folder.
   - You can open it from the in-game Mods menu with **Open Mod Folder**.
   - It is usually located at `%appdata%\VintagestoryData\Mods` on Windows.
3. Start the game.

## Usage

**[TODO: describe how to open the stats panel, e.g. hotkey or menu entry]**

## Compatibility

- Vintage Story 1.22 or newer (built against .NET 10)
- Client-side only

Vintage Story updates can occasionally rename internal classes or methods. If the mod stops working after a game update, please open an issue.

## Building from source

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
2. Create an environment variable named `VINTAGE_STORY` that points to your Vintage Story **installation** folder (the folder that contains `VintagestoryAPI.dll`).
3. In the project folder, run:
   ```
   dotnet build -c Release
   ```
4. The output is in `bin/Release/net10.0/`. The files you need are the built `.dll` and `modinfo.json`. Zip them together to install the mod as described above.

## Reporting problems

Please [open an issue](../../issues) and include:

- Your Vintage Story version and the mod version
- What you did and what you expected to happen
- The relevant lines from `%appdata%\VintagestoryData\Logs\client-main.txt`

## License

Released under the [MIT License](LICENSE).
