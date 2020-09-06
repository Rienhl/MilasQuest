using MilasQuest.Grids;
using UnityEngine;

namespace MilasQuest.InputManagement
{
    public class GridInputSampler
    {
        private InputHandler _inputHandler;
        private GridView _gridView;
        private Camera _cam;

        private bool _isEnabled;
        private Vector3 _currentWorldPosition;

        public void Setup(InputHandler inputHandler, GridView gridView, Camera gridCam)
        {
            _inputHandler = inputHandler;
            _gridView = gridView;
            _cam = gridCam;
        }

        public void Enable(bool enable)
        {
            if (_isEnabled == enable)
                return;

            if (enable)
                RegisterToInputHandler();
            else
                UnregisterFromInputHandler();
        }

        private void RegisterToInputHandler()
        {
            _inputHandler.OnInputStarted += HandleOnInputStarted;
            _inputHandler.OnInputUpdated += HandleOnInputUpdated;
            _inputHandler.OnInputEnded += HandleOnInputEnded;
            _inputHandler.OnInputCanceled += HandleOnInputCancelled;
        }

        private void UnregisterFromInputHandler()
        {
            _inputHandler.OnInputStarted -= HandleOnInputStarted;
            _inputHandler.OnInputUpdated -= HandleOnInputUpdated;
            _inputHandler.OnInputEnded -= HandleOnInputEnded;
            _inputHandler.OnInputCanceled -= HandleOnInputCancelled;
        }

        private void HandleOnInputStarted(Vector2 input)
        {
            Debug.Log("Input Started");
            _currentWorldPosition = ConvertToWorldPosition(input);
            if (IsPositionOutOfGridBounds(_currentWorldPosition))
                return;
            Debug.Log("GridPos: " + SampleAsGridPoint(_currentWorldPosition));
            //convert to cell index
            //check if it should be discarded
        }

        private void HandleOnInputUpdated(Vector2 input)
        {
            _currentWorldPosition = ConvertToWorldPosition(input);
            if (IsPositionOutOfGridBounds(_currentWorldPosition))
                return;
        }

        private void HandleOnInputEnded(Vector2 input)
        {
            
        }

        private void HandleOnInputCancelled(Vector2 input)
        {
            
        }

        private Vector3 ConvertToWorldPosition(Vector2 input)
        {
            return _cam.ScreenToWorldPoint(input);
        }

        private bool IsPositionOutOfGridBounds(Vector3 position)
        {
            return position.x < _gridView.GridBounds.min.x ||
                    position.x > _gridView.GridBounds.max.x ||
                    position.y < _gridView.GridBounds.min.y ||
                    position.y > _gridView.GridBounds.max.y;
        }

        private PointInt2D SampleAsGridPoint(Vector3 input)
        {
            return new PointInt2D()
            {
                X = Mathf.FloorToInt((input.x - _gridView.GridBounds.min.x) / _gridView.CellSize),
                Y = Mathf.FloorToInt((input.y - _gridView.GridBounds.min.y) / _gridView.CellSize)
            };
        }

    }
}