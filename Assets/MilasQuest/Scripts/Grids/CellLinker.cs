using MilasQuest.Grids.LinkableRules;
using System.Collections.Generic;
using UnityEngine;

namespace MilasQuest.Grids
{
    public class CellLinker
    {
        public List<Cell> LinkedCells { get; private set; }
        private List<ILinkableRule> _linkingRules;

        public CellLinker(PointInt2D gridDimensions, CELL_LINKING_RULE[] conditions)
        {
            LinkedCells = new List<Cell>();
            _linkingRules = new List<ILinkableRule>();

            for (int i = 0; i < conditions.Length; i++)
            {
                _linkingRules.Add(LinkableRulesFactory.GetRule(conditions[i]));
            }
        }

        public void AddRule(CELL_LINKING_RULE newRule)
        {
            for (int i = 0; i < _linkingRules.Count; i++)
            {
                if (_linkingRules[i].GetRuleType() == newRule)
                    return;
            }
            _linkingRules.Add(LinkableRulesFactory.GetRule(newRule));
        }

        public void RemoveRule(CELL_LINKING_RULE ruleToRemove)
        {
            for (int i = _linkingRules.Count - 1; i >= 0; i--)
            {
                if (_linkingRules[i].GetRuleType() == ruleToRemove)
                {
                    _linkingRules.RemoveAt(i);
                }
            }
        }

        public bool AddCell(Cell newCell)
        {
            CELL_EVALUATION_OUTPUT currentOutput = EvaluateCell(newCell);
            switch (currentOutput)
            {
                case CELL_EVALUATION_OUTPUT.ADD:
                    newCell.SetAsSelected();
                    LinkedCells.Add(newCell);
                    return true;
                case CELL_EVALUATION_OUTPUT.DONT_ADD:
                    break;
                case CELL_EVALUATION_OUTPUT.REMOVE_PREVIOUS:
                    LinkedCells[LinkedCells.Count - 1].SetAsSelected(false);
                    LinkedCells.RemoveAt(LinkedCells.Count - 1);
                    break;
                default:
                    break;
            }
            return false;
        }

        public void ClearLink()
        {
            for (int i = 0; i < LinkedCells.Count; i++)
            {
                LinkedCells[i].SetAsSelected(false);
            }
            LinkedCells.Clear();
        }

        private CELL_EVALUATION_OUTPUT EvaluateCell(Cell newCell)
        {
            CELL_EVALUATION_OUTPUT currentOutput = CELL_EVALUATION_OUTPUT.ADD;
            for (int i = 0; i < _linkingRules.Count; i++)
            {
                currentOutput = _linkingRules[i].CheckRule(LinkedCells, newCell);
                if (currentOutput != CELL_EVALUATION_OUTPUT.ADD)
                    break;
            }
            return currentOutput;
        }
    }

}