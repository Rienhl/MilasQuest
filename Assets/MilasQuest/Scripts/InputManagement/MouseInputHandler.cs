    using UnityEngine;

namespace MilasQuest.InputManagement
{
    public class MouseInputHandler : InputHandler
    {
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                InputStarted(Input.mousePosition);

            if (Input.GetMouseButton(0))
                InputUpdated(Input.mousePosition);

            if (Input.GetMouseButtonUp(0))
                InputEnded(Input.mousePosition);
        }
    }
}