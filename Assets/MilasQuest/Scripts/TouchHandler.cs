using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;


namespace MilasQuest.Input
{
    public class TouchHandler : MonoBehaviour
    {
        private Camera _cam;
        private void Awake()
        {
            EnhancedTouchSupport.Enable();
            TouchSimulation.Enable();
            _cam = Camera.main;
        }

        private void Update()
        {
            if (Touch.activeFingers.Count == 1)
            {
                Touch activeTouch = Touch.activeFingers[0].currentTouch;
                if (activeTouch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                    Debug.Log("Began");

                if (activeTouch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
                    Debug.Log("Ended");
                //Debug.Log($"Phase: { activeTouch.phase} | Position: {_cam.ScreenToWorldPoint(activeTouch.screenPosition}");
            }
        }
    }
}