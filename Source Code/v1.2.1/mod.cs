using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using Object = UnityEngine.Object; // Fix ambiguous reference between system and unity

using Newtonsoft.Json; // For config file

namespace Mod
{
    public class Mod : MonoBehaviour
    {
        public static void Main()
        {
            ModAPI.Register<InspectOnHover>();
        }
    }

    public class InspectOnHover : MonoBehaviour
    {
        // Permanence Toggle
        public static KeyCode togglePermanence = KeyCode.LeftAlt; // LeftAlt default if config is not found
        private static bool keepUIOpen = true;
        private string configName = "config.json"; // Name whatever you want JUST KEEP IT .json

        // Essential Variables
        private readonly List<LimbBehaviour> selectedLimbs = new List<LimbBehaviour>(16); 
        private int limbCount = 0;
        private Vector3 lastMousePos;

        private LimbBehaviour limb;
        private PersonBehaviour lastPerson;
        private LimbStatusViewBehaviour inspector;

        // ------------------------------------------- BUILT-IN FUNCTIONS ------------------------------------------- //

        private void Start() {
            Application.wantsToQuit += WantsToQuit; // Can save config before game closes
            LoadConfig();
            GetInspector();
            lastMousePos = Input.mousePosition;
        }

        private void Update()
        {   
            // Self explanatory I hope, toggles the Inspector staying open after you stop hovering
            if (Input.GetKeyDown(togglePermanence))
            {
                keepUIOpen = !keepUIOpen;
                ModAPI.Notify(("Inspect on Hover Permanence: "+keepUIOpen).ToString());
            }

            // Skips if mouse hasnt moved, ONLY limitation is if something flies into the mouse it wont detect, but thats whatever, right?
            if (Input.mousePosition == lastMousePos) return;
            lastMousePos = Input.mousePosition;
            
            // Find what mouse is hovering over. This version increases null checks while maintaining performance
            Vector2 mouse = Global.main.MousePosition;
            Collider2D target = Physics2D.OverlapPoint(mouse);
            
            // Functionality for the permanence keybind incorporated into the null check
            if (target == null) {
                if (keepUIOpen){
                    return;
                }
                else{
                    lastPerson = null;
                    RefreshInspector();
                }
            }

            limb = target.GetComponentInParent<LimbBehaviour>();
            if (limb == null || limb.Person == null) return;

            PersonBehaviour person = limb.Person;

            // Less checks being taken per frame unnecessarily than original
            if (lastPerson != limb.Person || !inspector.gameObject.activeInHierarchy) {
                if (person == null) return;
                lastPerson = person;
                RefreshInspector();
            }
        }

        private void LateUpdate() {
            if (lastPerson == null) return;

            var limbs = lastPerson.Limbs;
            for (int c = 0; c < limbs.Length; c++) { // no way c++ like the language
                if (!limbs[c]) {
                    GetLimbs(lastPerson);
                    inspector.Limbs = selectedLimbs;
                    return;
                }
            }

        }

        // Saving config for if mod object is destroyed OR game closes via alt+f4/task manager
        private void OnDestroy(){
            SaveConfig();
        }
        
        bool WantsToQuit(){
            SaveConfig();
            return true; // let the game close
        }

        // ------------------------------------------- MAIN FUNCTIONS ------------------------------------------- //

        private void GetLimbs(PersonBehaviour person) {
            selectedLimbs.Clear();
            if (person.Limbs.Length > 0) {
                foreach (var pLimbs in person.Limbs) { // Using a for loop would be faster but by a margin so small it wouldn't even be a frame
                    if (pLimbs && pLimbs.gameObject.activeInHierarchy) selectedLimbs.Add(pLimbs); // Gets existing limbs
                }
            }
        }

        // this function is super ugly, you have been warned...............................
        private void LoadConfig() {
            // Get or create folder for the mod's assets (in this case just the config)
            string assetFolder = @"Mods\\Noxoreits\\HoverInspector\\";
            string configLocation = assetFolder + configName;
            if(!Directory.Exists(assetFolder)){
                Directory.CreateDirectory(assetFolder);
            }

            try{
                if (File.Exists(configLocation)){
                    string json = File.ReadAllText(configLocation); // Raw json information
                    ConfigData config = JsonConvert.DeserializeObject<ConfigData>(json); // Custom class for the data, good for expanding later

                    if (config == null || string.IsNullOrEmpty(config.TogglePermanenceKey))
                    {
                        togglePermanence = KeyCode.LeftAlt; // Fallback for if the json is wrong
                        ModAPI.Notify("Hover Inspector - Config invalid, falling back to defaults. Custom settings will need to be reapplied");
                        ModAPI.Notify("If it is your first time using this mod, you can ignore this.");
                    }
                    else
                    {
                        togglePermanence = (KeyCode)Enum.Parse(typeof(KeyCode), config.TogglePermanenceKey, true);
                        keepUIOpen = config.IsPermanenceEnabled;
                    }
                }
                else{ // In case config is deleted
                    ConfigData defaultConfig = new ConfigData { TogglePermanenceKey = "LeftAlt", IsPermanenceEnabled = true }; // Set key to LeftAlt automatically as default and the permanence default too
                    string json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented); // Reserialise cause unity shit i guess im bad at json
                    File.WriteAllText(configLocation, json); // Save new config.json yaaaaaaayyyyyyyy
                }
            }
            catch (Exception e){
                ModAPI.Notify("Hover Inspector - "+e.Message);
                ModAPI.Notify("Ensure your config.json is valid, or delete the file if unsure so it can regenerate at Mods/Noxoreits/HoverInspector");
                togglePermanence = KeyCode.LeftAlt;
            }

            ModAPI.Notify("Hover Inspector key: " + togglePermanence);
        }

        private void SaveConfig(){
            // Reuses a lot of code from LoadConfig im not repeating the comments for those
            string assetFolder = @"Mods\\Noxoreits\\HoverInspector\\";
            string configLocation = assetFolder + configName;
            if(!Directory.Exists(assetFolder)){
                Directory.CreateDirectory(assetFolder);
            }
            ConfigData defaultConfig = new ConfigData { TogglePermanenceKey = togglePermanence.ToString(), IsPermanenceEnabled = keepUIOpen }; // Keybind stays the same, save state of keepUIOpen for future
            string json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented); // Reserialise cause unity shit i guess im bad at json
            File.WriteAllText(configLocation, json); // Save new config.json yaaaaaaayyyyyyyy
        }

        // ------------------------------------------- INSPECTOR STUFF ------------------------------------------- //

        private void GetInspector() {
            try {
                int tries = 3; // Tries 3 times to instantiate
                while (tries > 0 && inspector == null) {
                    inspector = Object.FindObjectOfType<LimbStatusViewBehaviour>(true);
                    tries--;
                }
                // If inspector cannot be found, the script will be disabled until the next reload
                if (inspector == null) throw new System.Exception("Inspector failed to be instantiated - Inspect Hover Mod");;
            }
            catch (System.Exception e) {
                ModAPI.Notify(e);
                this.enabled = false; // Disables the script as something is wrong
            }
        }

        private void RefreshInspector() {
            if (inspector == null) return;

            GetLimbs(lastPerson);
            inspector.Limbs = selectedLimbs;
            limbCount = lastPerson.Limbs.Length;
            inspector.gameObject.SetActive(true);
        }

    }

    // ccccccclasses im tired this is how the json will format shit for config
    [Serializable]
    public class ConfigData
    {
        public string TogglePermanenceKey;
        public bool IsPermanenceEnabled;
    }
}
// Originally uploaded by 'Noxoreits'. Do not reupload without their explicit permission.
