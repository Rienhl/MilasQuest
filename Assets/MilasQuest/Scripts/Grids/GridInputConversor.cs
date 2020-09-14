using MilasQuest.InputManagement;
using System;
using UnityEngine;

namespace MilasQuest.Grids
{
    public class GridInputConversor
    {
        private InputHandler _inputHandler;
        private GridView _gridView;
        private Camera _cam;
        private bool _isEnabled;

        private Vector3 _worldPosition;
        private GridInputInfo _inputInfo;

        public Action<PointInt2D> OnGridInputStarted;
        public Action<PointInt2D> OnGridInputUpdated;
        public Action<PointInt2D> OnGridInputEnded;
        public Action OnGridInputCancelled;

        public GridInputConversor(InputHandler inputHandler, Camera gridCam) 
        {
            _inputHandler = inputHandler;
            _cam = gridCam;
        }

        public void SetGridView(GridView gridView)
        {
            _gridView = gridView;
        }

        public void Unsetup()
        {
            _gridView = null;
            Enable(false);
        }

        public void Enable(bool enable)
        {
            if (_isEnabled == enable)
                return;

            _isEnabled = enable;

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
            StoreCurrentPoint(input);
            if (_inputInfo.ratioToCenter <= _gridView.ActiveInputRadius)
                OnGridInputStarted?.Invoke(_inputInfo.point);
        }

        private void HandleOnInputUpdated(Vector2 input)
        {
            StoreCurrentPoint(input);
            if (_inputInfo.ratioToCenter <= _gridView.ActiveInputRadius)
                OnGridInputUpdated?.Invoke(_inputInfo.point);
        }

        private void HandleOnInputEnded(Vector2 input)
        {
            StoreCurrentPoint(input);
            OnGridInputEnded?.Invoke(_inputInfo.point);
        }

        private void HandleOnInputCancelled(Vector2 input)
        {
            OnGridInputCancelled?.Invoke();
        }

        private Vector3 ConvertToWorldPosition(Vector2 input)
        {
            return _cam.ScreenToWorldPoint(input);
        }

        //This stores out of bounds points on the grid.
        //Even though it might sound wrong at first, it is not this class' responsibility to decide if out of bounds input should be sampled or not.
        //This will be a responsibility of this class' consumer, some interesting gameplay can arise from allowing out of bounds input
        private void StoreCurrentPoint(Vector2 rawInput)
        {
            _worldPosition = ConvertToWorldPosition(rawInput);
            _inputInfo = GridUtils.SampleGridInputInfo(_worldPosition, _gridView.GridBounds.min.x, _gridView.GridBounds.min.y, _gridView.CellSize);
        }
    }
}