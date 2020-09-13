using Microsoft.Win32.SafeHandles;
using MilasQuest.GameData;
using MilasQuest.InputManagement;
using MilasQuest.Pools;
using MilasQuest.UI;
using System;
using System.Linq;
using UnityEngine;

namespace MilasQuest
{
    public class Initializer : MonoBehaviour
    {
        [SerializeField] private PoolData[] pools;
        [SerializeField] private LevelController levelController;
        [SerializeField] private LevelData[] levelDatas;

        [SerializeField] private MainMenuPanel mainMenuPanel;
        [SerializeField] private LevelPassedPanel levelPassedPanel;
        [SerializeField] private LevelFailedPanel levelFailedPanel;

        private InputHandler _inputHandler;
        private int _currentLevelIndex;

        private void Awake()
        {
            for (int i = 0; i < pools.Length; i++)
            {
                Pool.CreatePool(pools[i]);
            }
            _inputHandler = SolveInputHandler();
        }

        private void Start()
        {
            mainMenuPanel.OnLevelSelected += HandleOnLevelSelected;
            mainMenuPanel.Show(levelDatas.Length);
        }

        private void HandleOnLevelSelected(int selectedLevelIndex)
        {
            _currentLevelIndex = selectedLevelIndex;
            levelController.SetupLevel(_inputHandler, levelDatas[_currentLevelIndex]);
            levelController.OnSuccess += HandleOnLevelSuccess;
            levelController.OnFailure += HandleOnLevelFailure;
            mainMenuPanel.Hide();
        }

        private void HandleOnLevelSuccess()
        {
            _currentLevelIndex++;
            PlayerPrefs.SetInt("LEVEL_PROGRESS", _currentLevelIndex);
            if (_currentLevelIndex >= levelDatas.Length)
            {
                levelPassedPanel.OnContinue += HandleOnExitLevel;
                levelPassedPanel.Show();
            }
            else
            {
                levelPassedPanel.OnContinue += LoadNextLevel;
                levelPassedPanel.Show();
            }
        }

        private void LoadNextLevel()
        {
            levelPassedPanel.OnContinue -= LoadNextLevel;
            levelController.ResetLevel();
            levelController.SetupLevel(_inputHandler, levelDatas[_currentLevelIndex]);
        }

        private void HandleOnLevelFailure()
        {
            levelFailedPanel.OnRetryLevel += HandleOnRetryLevel;
            levelFailedPanel.OnExitLevel += HandleOnExitLevel;
            levelFailedPanel.Show();
        }

        private void HandleOnRetryLevel()
        {
            levelController.ResetLevel();
            levelController.SetupLevel(_inputHandler, levelDatas[_currentLevelIndex]);
        }

        private void HandleOnExitLevel()
        {
            levelPassedPanel.OnContinue -= HandleOnExitLevel;
            levelFailedPanel.OnExitLevel -= HandleOnExitLevel;
            levelController.ResetLevel();
            mainMenuPanel.Show(levelDatas.Length);
        }


        private InputHandler SolveInputHandler()
        {
#if UNITY_EDITOR
            return new GameObject("_Mouse Input Handler_").AddComponent<MouseInputHandler>();
#else
            return new GameObject("_Touch Input Handler_").AddComponent<TouchInputHandler>();
#endif
        }
    }
}