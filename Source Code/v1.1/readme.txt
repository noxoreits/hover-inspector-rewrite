# Mod Details + Safety Verification
Steam Mod : https://steamcommunity.com/sharedfiles/filedetails/?id=3681175833

If you want to verify that this is the same file as in the workshop version, then subscribe to the mod and check: 
C:\Program Files (x86)\Steam\steamapps\workshop\content\1118200\3681175833

In there is the files of the actual installed mod, or just install it from this repo directly if you are familiar with PPG modding.

# Versions
- **Version 0.1 | 8/03/26 :** First implementation, extremely basic. Main goal was to remove redundant checks and simplify the original mod via a complete rewrite. Similar frameworks due to their simplicities, but ended up being 3x faster.
- **Version 1.0 | 9/03/26 :** Replaced most of 0.1's code and implemented additional safeguards to ensure robustness & stability, while adding small QoL, such as limb deletion not closing the inspect menu. Maintains even better performance than 0.1 despite the additions.
- **Version 1.1 | 10/03/26 :** Thumbnail updated to reduce confusion with the word "optimisation". Now referred to as "lightweight" as a better descriptor.

## My Goal
This mod's goal is simple:
- Show the inspect menu of a person whenever you hover your mouse
- Maximise performance over unnecessary bloat
- Be compatible with any and every mod you have or ever *will* have
That's it. No bloat like other versions of this mod, no lazy coding with the thought that "it's just a few frames". It's designed with YOU in mind, whether you have a 5090 or are stuck on integrated graphics.
It does what the name implies. Nothing more, nothing less.

## Performance
**Specs used to test: Ryzen 5 3400G, GTX 1650 Super, 16GB DDR4 3200MHZ**
**Tested with no other mods & 10 humans on the 'Substructure' map**

Consistently pulling over 130 fps with 10 humans, all severely injured and bleeding, whereas the other person's version would plummet as low as 30fps with extreme instability.

Many people have tested this and you can see in the comments of the workshop mod (as of 9/03/2026) that many people report good performance on this mod.

If you are afraid of another "FPS++" situation, the source code is in the **mod.cs** file> You can verify it yourself or ask someone who knows coding to check it for you.
The source code is exactly what is in the workshop mod, and is updated alongside it.
This GitHub also includes the file directory for where the mod from the Workshop is installed at the top of this readme so you can verify that it is the same.

# Permissions:
This repository and everything within it is protected by the MIT License. 
## **For more details, see "LICENSE" file**
tldr : You can use my code, but you must credit *clearly* anywhere that it is used, even if modified.
