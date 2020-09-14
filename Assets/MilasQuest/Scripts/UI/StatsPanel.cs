using MilasQuest.Stats;
using System.Collections.Generic;
using UnityEngine;

namespace MilasQuest.UI
{
    public class StatsPanel : MonoBehaviour
    {
        [SerializeField] private StatBar statBar_score;
        [SerializeField] private StatPanel statPanel_score;
        [SerializeField] private StatPanel statPanel_totalMoves;
        [SerializeField] private GatheredCellsStatPanel[] statPanels_gatheredCells;

        public Dictionary<int, GatheredCellsStatPanel> ActiveGatheredCellsPanels { get; private set; }

        public void Setup(LevelStats levelStats)
        {
            SetupScorePanels(levelStats);
            SetupGatheredCellsPanels(levelStats);
            statPanel_totalMoves.Setup(levelStats.Stats[STAT_TYPE.TOTAL_MOVES]);
        }

        private void SetupScorePanels(LevelStats levelStats)
        {
            Stat score = levelStats.Stats[STAT_TYPE.TOTAL_SCORE];
            if (score.MaxValue != Mathf.Infinity)
            {
                statPanel_score.gameObject.SetActive(false);
                statBar_score.gameObject.SetActive(true);
                statBar_score.Setup(score);
            }
            else
            {
                statPanel_score.gameObject.SetActive(true);
                statPanel_score.Setup(score);
                statBar_score.gameObject.SetActive(false);
            }
        }

        private void SetupGatheredCellsPanels(LevelStats levelStats)
        {
            if (ActiveGatheredCellsPanels == null)
                ActiveGatheredCellsPanels = new Dictionary<int, GatheredCellsStatPanel>();
            else
                ActiveGatheredCellsPanels.Clear();

            int currentIndex = 0;
            foreach (GatheredCellsStat stat in levelStats.GatheredCells.Values)
            {
                if (currentIndex == statPanels_gatheredCells.Length)
                {
                    Debug.LogError("Not enough GatheredCellsStatPanels avaialble to setup!", this.gameObject);
                    return;
                }
                statPanels_gatheredCells[currentIndex].gameObject.SetActive(true);
                statPanels_gatheredCells[currentIndex].Setup(stat, stat.MaxValue);
                ActiveGatheredCellsPanels.Add((int)stat.CellType, statPanels_gatheredCells[currentIndex]);
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
            statBar_score.Unsetup();
            statPanel_totalMoves.Unsetup();
            for (int i = 0; i < statPanels_gatheredCells.Length; i++)
            {
                statPanels_gatheredCells[i].Unsetup();
            }
        }
    }
}