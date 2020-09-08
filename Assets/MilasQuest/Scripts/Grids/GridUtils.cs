using UnityEngine;

namespace MilasQuest.Grids
{
    public static class GridUtils
    {
        private static PointInt2D[] surroundingPoints;
        private static GridInputInfo inputInfo;

        public static GridInputInfo SampleGridInputInfo(Vector3 input, float minX, float minY, float cellSize)
        {
            inputInfo = new GridInputInfo();
            inputInfo.point = new PointInt2D()
            {
                X = Mathf.FloorToInt((input.x - minX) / cellSize),
                Y = Mathf.FloorToInt((input.y - minY) / cellSize)
            };
            inputInfo.ratioToCenter = Vector3.Distance(input, new Vector3(minX + cellSize * 0.5f + inputInfo.point.X * cellSize, minY + cellSize * 0.5f + inputInfo.point.Y * cellSize, input.z)) / cellSize * 0.5f;
            return inputInfo;
        }

        public static bool IsPositionOutOfGridBounds(Vector3 position, float minX, float maxX, float minY, float maxY)
        {
            return position.x < minX ||
                    position.x > maxX ||
                    position.y < minY ||
                    position.y > maxY;
        }

        public static bool IsPointOutOfGridBounds(PointInt2D point, int xCount, int yCount)
        {
            return point.X < 0 ||
                point.Y < 0 ||
                point.X >= xCount ||
                point.Y >= yCount;
        }

        public static PointInt2D[] GetSurroundingPoints(PointInt2D origin)
        {
            surroundingPoints = new PointInt2D[8];
            surroundingPoints[0] = new PointInt2D { X = origin.X - 1, Y = origin.Y + 1 };
            surroundingPoints[1] = new PointInt2D { X = origin.X, Y = origin.Y + 1 };
            surroundingPoints[2] = new PointInt2D { X = origin.X + 1, Y = origin.Y + 1 };
            surroundingPoints[3] = new PointInt2D { X = origin.X - 1, Y = origin.Y};
            surroundingPoints[4] = new PointInt2D { X = origin.X + 1, Y = origin.Y};
            surroundingPoints[5] = new PointInt2D { X = origin.X - 1, Y = origin.Y - 1};
            surroundingPoints[6] = new PointInt2D { X = origin.X, Y = origin.Y - 1};
            surroundingPoints[7] = new PointInt2D { X = origin.X + 1, Y = origin.Y - 1};
            return surroundingPoints;
        }
    }
}