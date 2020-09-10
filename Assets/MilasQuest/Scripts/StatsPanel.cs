using MilasQuest.Stats;
using UnityEngine;

namespace MilasQuest.UI
{
    public class StatsPanel : MonoBehaviour
    {
        [SerializeField] StatPanel statPanel_score;
        [SerializeField] StatPanel statPanel_totalMoves;
        [SerializeField] StatPanel statPanel_gatheredCells;

        public void Setup(LevelStats levelStats)
        {
            statPanel_score.Setup(levelStats.Stats[STAT_TYPE.TOTAL_SCORE]);
            statPanel_totalMoves.Setup(levelStats.Stats[STAT_TYPE.TOTAL_MOVES]);
            statPanel_gatheredCells.Setup(levelStats.Stats[STAT_TYPE.GATHERED_CELLS_OF_TYPE]);
        }

        public void Unsetup()
        {
            statPanel_score.Unsetup();
            statPanel_totalMoves.Unsetup();
            statPanel_gatheredCells.Unsetup();
        }
    }
}