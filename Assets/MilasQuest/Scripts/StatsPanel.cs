using MilasQuest.Stats;
using UnityEngine;

namespace MilasQuest.UI
{
    public class StatsPanel : MonoBehaviour
    {
        [SerializeField] StatPanel statPanel_score;
        [SerializeField] StatPanel statPanel_totalMoves;
        [SerializeField] GatheredCellsStatPanel[] statPanels_gatheredCells;

        public void Setup(LevelStats levelStats)
        {
            statPanel_score.Setup(levelStats.Stats[STAT_TYPE.TOTAL_SCORE]);
            statPanel_totalMoves.Setup(levelStats.Stats[STAT_TYPE.TOTAL_MOVES]);
            int currentIndex = 0;
            foreach (GatheredCellsStat stat in levelStats.GatheredCells.Values)
            {
                if (currentIndex == statPanels_gatheredCells.Length)
                {
                    Debug.LogError("Not enough GatheredCellsStatPanels avaialble to setup!", this.gameObject);
                    return;
                }
                statPanels_gatheredCells[currentIndex].gameObject.SetActive(true);
                statPanels_gatheredCells[currentIndex].Setup(stat, 10);
                currentIndex++;
            }

            for (int i = currentIndex; i < statPanels_gatheredCells.Length; i++)
            {
                statPanels_gatheredCells[i].gameObject.SetActive(false);
            }
        }

        public void Unsetup()
        {
            statPanel_score.Unsetup();
            statPanel_totalMoves.Unsetup();
            //statPanel_gatheredCells.Unsetup();
        }
    }
}