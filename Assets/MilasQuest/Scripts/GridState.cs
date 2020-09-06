namespace MilasQuest.Grids
{
    /// <summary>
    /// Think similar to MVC, this class contains the model part of the grid, where it's current state is stored
    /// independent from cell size, sprites, grid position in the world, etc...
    /// </summary>
    public class GridState
    {
        public PointInt2D CellCount { get; private set; }

        //Jagged Arrays performance in tight loops outweigh the readability provided by Multidimensional Arrays.
        //This is recommended both by Unity and Microsoft. (More info: https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity8.html)
        public Cell[][] Cells { get; private set; }

        private PointInt2D aux;

        public GridState(PointInt2D cellCount)
        {
            this.CellCount = cellCount;
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            Cells = new Cell[CellCount.X][];
            for (int x = 0; x < CellCount.X; x++)
            {
                Cells[x] = new Cell[CellCount.Y];
                for (int y = 0; y < CellCount.Y; y++)
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