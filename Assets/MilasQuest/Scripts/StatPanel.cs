using DG.Tweening;
using MilasQuest.Stats;
using TMPro;
using UnityEngine;

namespace MilasQuest.UI
{
    public class StatPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text txt_statName;
        [SerializeField] private TMP_Text txt_statValue;

        private Stat _storedStat;

        public void Setup(Stat stat)
        { 
            _storedStat = stat;
            txt_statName.text = stat.GetStatType().ToString();
            txt_statValue.text = stat.GetCurrentValue().ToString("F0");
            _storedStat.OnStatUpdated += HandleStatUpdated;
        }

        public void Unsetup()
        {
            _storedStat.OnStatUpdated -= HandleStatUpdated;
        }

        private void HandleStatUpdated(Stat stat)
        {
            txt_statValue.transform.DOPunchScale(Vector3.one * 1.1f, 0.2f, 5, 0);
            txt_statValue.text = stat.GetCurrentValue().ToString("F0");
        }
    }
}