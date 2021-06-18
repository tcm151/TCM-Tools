using UnityEngine;


namespace TCM.Interfaces
{
    public class Interactor : MonoBehaviour
    {
        //- LOCAL VARIABLES
        private readonly Camera view = Camera.current;
        public float interactionDistance = 2.5f;

        //> CHECK FOR INTERACTIONS
        private void Update()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;

            Ray inputRay = view.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(inputRay, out RaycastHit hit, interactionDistance))
            {
                var interactable = hit.collider.GetComponentInParent<IInteractable>();
                if (interactable == null) return;

                // EventManager.Active.ShowPopup(true, interactable.textPrompt);
                if (Input.GetKeyDown(KeyCode.E)) interactable.Interact();
            }
            // else EventManager.Active.ShowPopup(false, "you shouldn't be seeing this");
        }
    }
}