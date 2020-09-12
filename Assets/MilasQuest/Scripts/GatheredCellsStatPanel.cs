using MilasQuest.GameData;
using MilasQuest.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MilasQuest.UI
{
    public class GatheredCellsStatPanel : StatPanel
    {
        [SerializeField] private TMP_Text txt_targetValue;
        [SerializeField] private Image img_cellIcon;

        public void Setup(GatheredCellsStat stat, float targetValue)
        {
            base.Setup(stat);
            img_cellIcon.sprite = CellDataProvider.Instance.GetCellTypeData(stat.CellType).sprite;
            txt_targetValue.text =  targetValue.ToString("F0");
        }

    }
}