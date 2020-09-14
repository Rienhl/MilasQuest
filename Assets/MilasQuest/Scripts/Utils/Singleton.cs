using UnityEngine;

namespace MilasQuest
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static bool applicationIsQuitting = false;

        private static T instance;
        public static T Instance
        {
            get
            {
                return instance;
            }
        }

        private void Awake()
        {
            DoAwake();
        }


        protected virtual void DoAwake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else
            {
                Debug.LogError("Singleton is duplicated! Will destroy this instance.");
                Destroy(gameObject);
                return;
            }
        }

        protected virtual void OnWillDestroySingletonInstance()
        {
        }

        protected virtual void OnDestroy()
        {
            applicationIsQuitting = true;
        }
    }
}