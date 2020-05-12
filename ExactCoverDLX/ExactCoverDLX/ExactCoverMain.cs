using ExactCoverDLX.DLX;
using ExactCoverDLX.Utils;
using System;
using System.IO;

namespace ExactCoverDLX
{
    public class ExactCoverMain
    {
        public static void Main(string[] args)
        {
            Logger.DoLog = true;
            Logger.DoAppend = false;

            SudokuInput sudokuInput = SudokuInput.GetSudokuFromFile(16, 4, @"./sudoku_grids/sudoku_16x16_1.txt");

            Sudoku sudoku = new Sudoku(sudokuInput.Size, sudokuInput.BoxSize, sudokuInput.Grid);
            sudoku.DisplayGrid();
            sudoku.Solve();

            Logger.Destroy();
        }

        public class SudokuInput
        {
            public int Size { get; set; }
            public int BoxSize { get; set; }
            public int[,] Grid { get; set; }

            public SudokuInput() : this(9,3,new int[9,9]) { }

            public SudokuInput(int[,] grid) : this(9,3,grid) { }

            public SudokuInput(int size, int boxSize, int[,] grid)
            {
                Size = size;
                BoxSize = boxSize;
                Grid = grid;
            }

            public static SudokuInput GetSudokuFromFile(int size,int boxSize, string path)
            {
                int[,] grid = new int[size,size];

                string[] lines = File.ReadAllLines(path);

                for (int i=0;i<size;i++)
                {
                    string[] line = lines[i].Split(" ");
                    for(int j = 0; j < size; j++)
                    {
                        grid[i, j] = int.Parse(line[j]);
                    }
                    
                }

                return new SudokuInput(size, boxSize, grid);
            }
        }
    }
}
