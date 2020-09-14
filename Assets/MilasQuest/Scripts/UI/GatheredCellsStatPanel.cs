using DG.Tweening;
using MilasQuest.GameData;
using MilasQuest.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MilasQuest.UI
{
    public class GatheredCellsStatPanel : StatPanel
    {
        [SerializeField] private Color color_normal;
        [SerializeField] private Color color_complete;
        [SerializeField] private TMP_Text txt_targetValue;
        [SerializeField] private Image img_cellIcon;

        private RectTransform _imageRectTransform;
        private float _targetDifference;

        public void Setup(GatheredCellsStat stat, float targetValue)
        {
            base.Setup(stat);
            img_cellIcon.sprite = CellDataProvider.Instance.GetCellTypeData(stat.CellType).sprite;
            txt_targetValue.text =  targetValue.ToString("F0");
            txt_statValue.color = color_normal;
            txt_targetValue.color = color_normal;
            _imageRectTransform = img_cellIcon.GetComponent<RectTransform>();
            _storedStat.OnStatReachedMax += HandleOnStatReachedMax;
        }

        public override void Unsetup()
        {
            base.Unsetup();
            if (_storedStat != null)
            {
                _storedStat.OnStatReachedMax -= HandleOnStatReachedMax;
            }
        }

        private void HandleOnStatReachedMax(Stat _)
        {
            _storedStat.OnStatReachedMax -= HandleOnStatReachedMax;
            txt_statValue.color = color_complete;
            txt_targetValue.color = color_complete; 
        }

        public Vector3 GetImagePosition()
        {
            //Uncomment if he UI canvas gets changed to Screen Space - Overlay 
            //return Camera.main.ViewportToWorldPoint(Camera.main.ScreenToViewportPoint(_imageRectTransform.position));
            return _imageRectTransform.position;
        }

        public void DoAnim()
        {
            if (_targetDifference < _storedStat.CurrentValue)
            {
                _targetDifference++;
                AnimateUI(_targetDifference.ToString("F0"));
            }
        }

        protected override void HandleStatUpdated(Stat stat)
        {
            _targetDifference = stat.PrevValue;
        }

        protected override void AnimateUI(string newValue)
        {
            img_cellIcon.transform.DOComplete();
            img_cellIcon.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5, 0.3f);
            base.AnimateUI(newValue);
        }

    }
}