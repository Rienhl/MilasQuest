using MilasQuest.Grids;
using MilasQuest.InputManagement;
using UnityEngine;

namespace MilasQuest
{
    public class Game : MonoBehaviour
    {
        public GridView gridView;

        private GridInputSampler gridInputSampler;
        private InputHandler _inputHandler;


        private void Start()
        {
            gridView.Init(new GridState(new PointInt2D() { X = 10, Y = 15 }), new GridViewConfig() { cellSize = 1f });
            gridInputSampler = new GridInputSampler();
            _inputHandler = SolveInputHandler();
            gridInputSampler.Setup(_inputHandler, gridView, Camera.main);
            gridInputSampler.Enable(true);
        }

        public InputHandler SolveInputHandler()
        {
#if UNITY_EDITOR
            return gameObject.AddComponent<MouseInputHandler>();
#else
            return gameObject.AddComponent<TouchInputHandler>()
#endif
        }
    }
}