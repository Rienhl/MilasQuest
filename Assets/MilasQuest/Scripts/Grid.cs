using System;
using TMPro.EditorUtilities;

namespace MilasQuest.Grid
{
    public class Grid
    {
        public PointInt2D Dimensions { get; private set; }

        //Jagged Arrays performance in tight loops outweigh the readability provided by Multidimensional Arrays.
        //This is recommended both by Unity and Microsoft. (More info: https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity8.html)
        public Cell[][] Cells { get; private set; }

        private PointInt2D aux;

        public Grid(PointInt2D dimensions)
        {
            this.Dimensions = dimensions;
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            Cells = new Cell[Dimensions.X][];
            for (int x = 0; x < Dimensions.X; x++)
            {
                Cells[x] = new Cell[Dimensions.Y];
                for (int y = 0; y < Dimensions.Y; y++)
                {
                    Cells[x][y] = new Cell(x, y);
                }
            }
        }

        private void SwapPosition(Cell cellA, Cell cellB)
        {
            aux = cellA.Index;
            cellA.UpdateIndex(cellB.Index.X, cellB.Index.Y);
            cellB.UpdateIndex(aux.X, aux.Y);

        }
    }
}