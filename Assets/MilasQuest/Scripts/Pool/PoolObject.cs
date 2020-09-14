using UnityEngine;

namespace MilasQuest.Pools
{
    public class PoolObject : MonoBehaviour
    {
        public Pool Pool { get; private set; }

        public void SetPool(Pool pool)
        {
            Pool = pool;
        }

        public void Despawn()
        {
            Pool.Despawn(this);
        }
    }
}

