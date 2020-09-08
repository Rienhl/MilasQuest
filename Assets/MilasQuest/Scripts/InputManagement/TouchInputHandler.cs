using UnityEngine;

namespace MilasQuest.InputManagement
{
    public class TouchInputHandler : InputHandler
    {
        private Touch _touch;

        private void Update()
        {
            if (Input.touchCount == 0)
                return;

            _touch = Input.GetTouch(0);

            switch (_touch.phase)
            {
                case TouchPhase.Began:
                    InputStarted(_touch.position);
                    break;
                case TouchPhase.Moved:
                    InputUpdated(_touch.position);
                    break;
                case TouchPhase.Stationary:
                    break;
                case TouchPhase.Ended:
                    InputEnded(_touch.position);
                    break;
                case TouchPhase.Canceled:
                    InputCancelled(_touch.position);
                    break;
                default:
                    break;
            }
        }
    }
}