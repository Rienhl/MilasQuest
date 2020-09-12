using DG.Tweening;
using MilasQuest.Stats;
using TMPro;
using UnityEngine;

namespace MilasQuest.UI
{
    public class StatPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text txt_statValue;

        protected Stat _storedStat;

        public void Setup(Stat stat)
        {
            _storedStat = stat;
            txt_statValue.text = stat.CurrentValue.ToString("F0");
            _storedStat.OnStatUpdated += HandleStatUpdated;
        }

        public void Unsetup()
        {
            _storedStat.OnStatUpdated -= HandleStatUpdated;
        }

        private void HandleStatUpdated(Stat stat)
        {
            txt_statValue.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 5, 0.3f);
            txt_statValue.text = stat.CurrentValue.ToString("F0");
        }
    }
}