using DG.Tweening;
using MilasQuest.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace MilasQuest.UI
{
    public class StatBar : StatPanel
    {
        [Header("Anim Settings")]
        [SerializeField] private Color color_normal;
        [SerializeField] private Color color_complete;
        [Range(0,2)]
        [SerializeField] private float barFillTime = 0.7f;
        [SerializeField] private Ease barFillEaseType = Ease.InOutSine;

        [Header("UI References")]
        [SerializeField] private Image img_bar;

        public override void Setup(Stat stat, bool invertValue = false)
        {
            base.Setup(stat, invertValue);
            img_bar.fillAmount = _storedStat.CurrentValue / _storedStat.MaxValue;
            img_bar.color = color_normal;
        }

        protected override void HandleStatUpdated(Stat _)
        {
            base.HandleStatUpdated(_);
        }
        
        protected override void AnimateUI(string overrideValue = "")
        {
            base.AnimateUI(overrideValue);
            img_bar.DOComplete();
            img_bar.DOFillAmount(_storedStat.CurrentValue / _storedStat.MaxValue, barFillTime).SetEase(barFillEaseType).OnComplete(() => { if (img_bar.fillAmount == 1) img_bar.color = color_complete; });
        }
    }
}