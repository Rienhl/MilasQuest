using System;
using UnityEngine;

namespace MilasQuest.Grids
{
    public struct GridInputInfo : IEquatable<GridInputInfo>
    {
        public PointInt2D point;
        public float ratioToCenter;

        public bool Equals(GridInputInfo other)
        {
            return this.point == other.point && this.ratioToCenter == other.ratioToCenter;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            return this.Equals((GridInputInfo)obj);
        }

        public override int GetHashCode()
        {
            return point.GetHashCode() * Mathf.RoundToInt(ratioToCenter * 100f);
        }
    }
}