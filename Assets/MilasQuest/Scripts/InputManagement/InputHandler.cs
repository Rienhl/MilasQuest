using System;
using UnityEngine;

namespace MilasQuest.InputManagement
{
    public abstract class InputHandler : MonoBehaviour
    {
        public Action<Vector2> OnInputStarted;
        public Action<Vector2> OnInputUpdated;
        public Action<Vector2> OnInputEnded;
        public Action<Vector2> OnInputCanceled;

        public void InputStarted(Vector2 input)
        {
            OnInputStarted?.Invoke(input);
        }

        public void InputUpdated(Vector2 input)
        {
            OnInputUpdated?.Invoke(input);
        }

        public void InputEnded(Vector2 input)
        {
            OnInputEnded?.Invoke(input);
        }

        public void InputCancelled(Vector2 input)
        {
            OnInputCanceled?.Invoke(input);
        }
    }
}