using UnityEngine;

namespace MilasQuest.GameData
{
    public class BaseData : ScriptableObject
    {
        [HideInInspector]
        public int id = -1;
        [HideInInspector]
        public string definedName;
    }
}

