using System.Collections.Generic;
using UnityEngine;

// All code written by me (noxoreits)
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
        private PersonBehaviour lastPerson;
        LimbStatusViewBehaviour inspector = Object.FindObjectOfType<LimbStatusViewBehaviour>(true);

        private void Update()
        {
            Vector2 mouse = Global.main.MousePosition;
            Collider2D target = Physics2D.OverlapPoint(mouse);

            // Less checks being taken per frame unnecessarily than original
            if (target != null && lastPerson != target.GetComponentInParent<LimbBehaviour>()){
                LimbBehaviour limb = target.GetComponentInParent<LimbBehaviour>();
                PersonBehaviour person = limb.Person;
                
                // If person or inspector is missing, cancel before causing errors
                if (inspector == null || person == null) return;

                // Gets each limb and appends to a list
                List<LimbBehaviour> selectedLimbs = new List<LimbBehaviour>();
                foreach (var pLimbs in person.Limbs)
                {
                    if (pLimbs != null) selectedLimbs.Add(pLimbs); // Gets existing limbs
                }

                // Update inspector once per hover on a person
                inspector.gameObject.SetActive(true);
                inspector.Limbs = selectedLimbs;
                lastPerson = person;
            }

        }
    }
}