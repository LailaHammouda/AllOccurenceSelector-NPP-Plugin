# AllOccurenceSelector-NPP-Plugin
A plugin for notepad++ which performs the functionality of selecting all occurrences of the currently highlighted string and multi-cursoring starting at the position of these strings. It offers two modes: the first one selects all whole word occurences identical to the highlighted string, and the second one selects all identical characters whether as a whole word or as substrings of another string.

## Key Features
- Selecting all occurrences of highlighted string whether identical whole words or identical characters (whether whole words or substings).
- Replacing all the selected occurrences highlighted above at once.
- Using multiple cursors at the positions of the strings highlighted above and moving theses cursors freely using arrow keys from the keyboard

## Demo
![Demo_GIF](https://github.com/LailaHammouda/AllOccurenceSelector-NPP-Plugin/assets/54313648/70112f37-10e2-4eb5-b3c5-f93f3b8e383f)

## Installation Steps
1. Download the **"AllOccurenceSelector.dll"** file from this repo.
2. Open the plugins folder of Notepad++ on your laptop which is usually in this path *"C:\Program Files\Notepad++\plugins"*.
3. Create a new folder, name it **"AllOccurenceSelector"**.
4. Move the downloaded .dll file into the "AllOccurenceSelector" folder created above.
5. Restart Notepad++.
6. Open *"Settings"*, then *"Preferences"*, then *"Editing"*. Make sure the *"Enable Multi-Editing (Ctrl+Mouse click/selection)"* option is enabled.

## How To Use
After completeing the installation process above, you can now easily use the plugin by either clicking on it from the *"Plugins"* menu or by using the below shortcuts.
- For selecting all identical **whole word** occurrences: Highlight the string needed and press on **Shift** + **3** keys on the keyboard.
- For selecting all identical **characters** occurrences: Highlight the string needed and press on **Shift** + **4** keys on the keyboard.
After doing one of the above, you will now be able to replace all these strings concurrently and navigate throgh the docment with the multi-cursors located at all the occurrences using the arrow keys on the keyboard.

## License
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
