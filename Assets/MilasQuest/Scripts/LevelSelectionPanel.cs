using DG.Tweening;
using MilasQuest.GameData;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MilasQuest.UI
{
    public class LevelSelectionPanel : MonoBehaviour
    {
        public Action<int> OnLevelSelected;

        [SerializeField] private TMP_Text txt_levelNumber;
        [SerializeField] private TMP_Text txt_status;
        [SerializeField] private Button btn_select;

        private int _storedLevelIndex;
        private LEVEL_STATUS _levelStatus;

        public void SetLevel(int levelIndex, LEVEL_STATUS status)
        {
            _storedLevelIndex = levelIndex;
            _levelStatus = status;

            txt_levelNumber.text = (_storedLevelIndex + 1).ToString();
            txt_status.text = _levelStatus.ToString();

            if (status != LEVEL_STATUS.LOCKED)
            {
                btn_select.interactable = true;
                btn_select.onClick.AddListener(HandleOnButtonClicked);
            }
            else
            {
                btn_select.interactable = false;
                btn_select.onClick.RemoveListener(HandleOnButtonClicked);
            }
        }

        private void HandleOnButtonClicked()
        {
            btn_select.onClick.RemoveListener(HandleOnButtonClicked);
            btn_select.interactable = false;
            btn_select.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 8, 0.2f).OnComplete(() => OnLevelSelected?.Invoke(_storedLevelIndex));
        }
    }
}
