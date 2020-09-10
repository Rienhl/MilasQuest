using MilasQuest.Grids;
using System.Collections.Generic;

namespace MilasQuest
{
    public class ScoreSolver
    {
        private readonly ScoreValuesData _scoreValuesData;

        public ScoreSolver(ScoreValuesData scoreValuesData)
        {
            _scoreValuesData = scoreValuesData;
        }

        public float SolveScore(List<Cell> pickedCells)
        {
            return _scoreValuesData.basicChainScore * (1 + (pickedCells.Count - _scoreValuesData.minLinkCount) * _scoreValuesData.extraChainMultiplier);
        }

    }
}