using System.Collections.Generic;
using UnityEngine;

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
        private readonly List<LimbBehaviour> selectedLimbs = new List<LimbBehaviour>(16); 
        private int limbCount = 0;
        private Vector3 lastMousePos;

        private LimbBehaviour limb;
        private PersonBehaviour lastPerson;
        private LimbStatusViewBehaviour inspector;

        // ------------------------------------------- BUILT-IN FUNCTIONS ------------------------------------------- //

        private void Start() {
            GetInspector();
            lastMousePos = Input.mousePosition;
        }

        private void Update()
        {   
            // Skips if mouse hasnt moved, ONLY limitation is if something flies into the mouse it wont detect, but thats whatever, right?
            if (Input.mousePosition == lastMousePos) return;
            lastMousePos = Input.mousePosition;
            
            // Find what mouse is hovering over. This version increases null checks while maintaining performance
            Vector2 mouse = Global.main.MousePosition;
            Collider2D target = Physics2D.OverlapPoint(mouse);
            if (target == null) return;

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

        // ------------------------------------------- MAIN FUNCTIONS ------------------------------------------- //

        private void GetLimbs(PersonBehaviour person) {
            selectedLimbs.Clear();
            if (person.Limbs.Length > 0) {
                foreach (var pLimbs in person.Limbs) { // Using a for loop would be faster but by a margin so small it wouldn't even be a frame
                    if (pLimbs && pLimbs.gameObject.activeInHierarchy) selectedLimbs.Add(pLimbs); // Gets existing limbs
                }
            }
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
                Debug.Log(e);
                this.enabled = false; // Disables the script as something is wrong
            }
        }

        private void RefreshInspector() {
            if (inspector == null) return;

            GetLimbs(lastPerson);
            inspector.Limbs = selectedLimbs;
            limbCount = lastPerson.Limbs.Length;
            inspector.gameObject.SetActive(true);

            //inspector.SendMessage("OnEnable", SendMessageOptions.DontRequireReceiver);
        }

    }
}
// Originally uploaded by 'Noxoreits'. Do not reupload without their explicit permission.
