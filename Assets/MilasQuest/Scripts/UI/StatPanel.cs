using DG.Tweening;
using MilasQuest.Stats;
using TMPro;
using UnityEngine;

namespace MilasQuest.UI
{
    public class StatPanel : MonoBehaviour
    {
        [SerializeField] protected TMP_Text txt_statValue;

        protected Stat _storedStat;

        public virtual void Setup(Stat stat, bool invertValue = false)
        {
            _storedStat = stat;
            txt_statValue.text = invertValue ? (stat.MaxValue - stat.CurrentValue).ToString("F0") : stat.CurrentValue.ToString("F0");
            _storedStat.OnStatUpdated += HandleStatUpdated;
        }

        public virtual void Unsetup()
        {
            if (_storedStat != null)
                _storedStat.OnStatUpdated -= HandleStatUpdated;
        }

        protected virtual void HandleStatUpdated(Stat _)
        {
            AnimateUI();
        }

        protected virtual void AnimateUI(string overrideValue = "")
        {
            txt_statValue.transform.DOComplete();
            txt_statValue.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 5, 0.3f);
            txt_statValue.text = string.IsNullOrEmpty(overrideValue) ? _storedStat.CurrentValue.ToString("F0") : overrideValue;
        }
    }
}