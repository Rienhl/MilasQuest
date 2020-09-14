using DG.Tweening;
using MilasQuest.GameData;
using MilasQuest.Pools;
using System;
using TMPro;
using UnityEngine;

namespace MilasQuest.UI
{
    public class MainMenuPanel : MonoBehaviour
    {
        public Action<int> OnLevelSelected;

        [Header("UI References")]
        [SerializeField] private GameObject mainMenuHolder;
        [SerializeField] private TMP_Text txt_title;
        [SerializeField] private TMP_Text txt_levelSelect;
        [SerializeField] private RectTransform rt_levelSelect;
        [SerializeField] private Transform t_levelsHolder;

        [Header("Anim Values")]
        [Range(0, 5)]
        [SerializeField] private float titleFadeInTime = 0.4f;
        [Range(0, 5)]
        [SerializeField] private float levelSelectFadeInTime = 0.4f;
        [Range(0, 5)]
        [SerializeField] private float levelSelectGridScaleTime = 0.4f;
        [Range(0, 1)]
        [SerializeField] private float buttonScaleTime = 0.1f;
        [Range(0, 1)]
        [SerializeField] private float delayBetweenButtons = 0.05f;

        [Header("Data References")]
        [SerializeField] private PoolData levelSelectionUIPoolData;

        private LevelSelectionPanel[] _levelSelectionPanels;
        private Sequence _sequence;

        public void Show(int levelsCount)
        {
            int currentProgress = PlayerPrefs.GetInt("LEVEL_PROGRESS", 0);
            _levelSelectionPanels = new LevelSelectionPanel[levelsCount];
            for (int i = 0; i < levelsCount; i++)
            {
                LevelSelectionPanel selectionPanel = Pool.GetPool(levelSelectionUIPoolData).Spawn(t_levelsHolder).GetComponent<LevelSelectionPanel>();
                selectionPanel.SetLevel(i, (currentProgress > i) ? LEVEL_STATUS.PASSED : ((currentProgress < i) ? LEVEL_STATUS.LOCKED : LEVEL_STATUS.AVAILABLE));
                selectionPanel.OnLevelSelected += HandleOnLevelSelected;
                _levelSelectionPanels[i] = selectionPanel;
            }

            _sequence = DOTween.Sequence();
            _sequence.Append(txt_title.DOFade(0, titleFadeInTime).From());
            _sequence.Append(txt_levelSelect.DOFade(0, levelSelectFadeInTime).From());
            _sequence.Append(rt_levelSelect.DOScale(Vector3.zero, levelSelectGridScaleTime).From());
            for (int i = 0; i < _levelSelectionPanels.Length; i++)
            {
                if (i == 0)
                    _sequence.Append(_levelSelectionPanels[i].transform.DOScale(Vector3.zero, buttonScaleTime).From());
                else
                    _sequence.Join(_levelSelectionPanels[i].transform.DOScale(Vector3.zero, buttonScaleTime).From().SetDelay(delayBetweenButtons * i));
            }
            mainMenuHolder.SetActive(true);
            _sequence.Play();
        }

        public void Hide()
        {
            for (int i = _levelSelectionPanels.Length - 1; i >= 0; i--)
            {
                _levelSelectionPanels[i].OnLevelSelected -= HandleOnLevelSelected;
                _levelSelectionPanels[i].GetComponent<PoolObject>().Despawn();
            }
            _sequence.Complete();
            _sequence.Kill();
            mainMenuHolder.SetActive(false);
        }

        private void HandleOnLevelSelected(int selectedLevelIndex)
        {
            OnLevelSelected?.Invoke(selectedLevelIndex);
        }
    }
}