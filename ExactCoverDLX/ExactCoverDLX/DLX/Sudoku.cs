using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExactCoverDLX.DLX
{
    public class Sudoku
    {
        #region attributes
        // Grid size
        private int _size;
        // Box size
        private int _boxSize;
        private int _emptyCell;
        // 4 constraints : cell, line, column, boxes
        private int _constraint;
        // Values for each cells
        private int _minValue;
        private int _maxValue;
        // Starting index for cover matrix
        private int _coverStartIndex;

        private int[,] _grid;
        private int[,] _gridSolved;
        #endregion

        #region constructors
        public Sudoku(int[,] grid) : this(9, 3, grid){ } // initializing size to a 3x3 sudoku grid by default

        public Sudoku(int size, int boxSize, int[,] grid)
        {
            // initializing variables
            _size = size;
            _boxSize = boxSize;
            _emptyCell = 0;
            _constraint = 4;
            _minValue = 1;
            _maxValue = _size;
            _coverStartIndex = 1;

            // copying grid
            _grid = new int[_size, _size];
            for (int i = 0; i < _size; i++)
                for (int j = 0; j < _size; j++)
                    _grid[i,j] = grid[i,j];
        }
        #endregion

        #region Exact Cover Matrix functions
        private int IndexInCoverMatrix(int row, int column, int num)
        {
            return (row - 1) * _size * _size + (column - 1) * _size + (num - 1);
        }

        private int[,] CreateCoverMatrix()
        {
            int[,] coverMatrix = new int[_size * _size * _maxValue,_size * _size * _constraint];

            int header = 0;
            header = CreateCellConstraints(coverMatrix, header);
            header = CreateRowConstraints(coverMatrix, header);
            header = CreateColumnConstraints(coverMatrix, header);
            CreateBoxConstraints(coverMatrix, header);

            return coverMatrix;
        }

        private int[,] ConvertInCoverMatrix(int[,] grid)
        {
            int[,] coverMatrix = CreateCoverMatrix();

            // Taking into account the values already entered in Sudoku's grid instance
            for (int row = _coverStartIndex; row <= _size; row++)
            {
                for (int column = _coverStartIndex; column <= _size; column++)
                {
                    int n = grid[row - 1,column - 1];

                    if (n != _emptyCell)
                    {
                        for (int num = _minValue; num <= _maxValue; num++)
                        {
                            if (num != n)
                            {
                                int idxInCoverMatrix = IndexInCoverMatrix(row, column, num);
                                for (int i = 0; i < coverMatrix.GetLength(1); i++)
                                    coverMatrix[idxInCoverMatrix, i] = 0;
                                
                            }
                        }
                    }
                }
            }

            return coverMatrix;
        }

        private int[,] ConvertDLXListToGrid(LinkedList<DancingNode> answer)
        {
            int[,] result = new int[_size,_size];

            foreach (DancingNode n in answer)
            {
                DancingNode rcNode = n;
                int min = int.Parse(rcNode.Column.Name);

                for (DancingNode tmp = n.Right; tmp != n; tmp = tmp.Right)
                {
                    int val = int.Parse(tmp.Column.Name);

                    if (val < min)
                    {
                        min = val;
                        rcNode = tmp;
                    }
                }

                // we get line and column
                int ans1 = int.Parse(rcNode.Column.Name);
                int ans2 = int.Parse(rcNode.Right.Column.Name);
                int r = ans1 / _size;
                int c = ans1 % _size;
                // and the affected value
                int num = (ans2 % _size) + 1;
                // we affect that on the result grid
                result[r,c] = num;
            }

            return result;
        }

        public void Solve()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            int[,] cover = ConvertInCoverMatrix(_grid);
            //printCoverMatrix(cover);
            DLX dlx = new DLX(cover);
            dlx.Solve();
            _gridSolved = ConvertDLXListToGrid(dlx.Result);

            watch.Stop();
            TimeSpan elapsed = watch.Elapsed;

            Console.WriteLine("Grid solved with dancing links in {0}", elapsed);
            DisplaySolution();
        }

        public void DisplayGrid()
        {
            int maxLength = _grid.Cast<int>().Max().ToString().Length;

            Console.WriteLine("\n " + new String('-',_size * (maxLength+1) + (_size/_boxSize)*2 - 1));
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (j % _boxSize == 0)
                        Console.Write("| ");
                    Console.Write(_grid[i, j] + " ");
                    
                }
                Console.Write("|\n");
            }
            Console.WriteLine(" " + new String('-', _size * (maxLength + 1) + (_size / _boxSize) * 2 - 1));
        }

        public void DisplaySolution()
        {
            int maxLength = _gridSolved.Cast<int>().Max().ToString().Length;

            Console.WriteLine("\n " + new String('-', _size * (maxLength + 1) + (_size / _boxSize) * 2 - 1));
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (j % _boxSize == 0)
                        Console.Write("| ");
                    Console.Write(_gridSolved[i, j] + " ");

                }
                Console.Write("|\n");
            }
            Console.WriteLine(" " + new String('-', _size * (maxLength + 1) + (_size / _boxSize) * 2 - 1));
        }
        #endregion

        #region constraints

        // box constraint
        private int CreateBoxConstraints(int[,] matrix, int header)
        {
            for (int row = _coverStartIndex; row <= _size; row += _boxSize)
            {
                for (int column = _coverStartIndex; column <= _size; column += _boxSize)
                {
                    for (int n = _coverStartIndex; n <= _size; n++, header++)
                    {
                        for (int rowDelta = 0; rowDelta < _boxSize; rowDelta++)
                        {
                            for (int columnDelta = 0; columnDelta < _boxSize; columnDelta++)
                            {
                                int index = IndexInCoverMatrix(row + rowDelta, column + columnDelta, n);
                                matrix[index,header] = 1;
                            }
                        }
                    }
                }
            }

            return header;
        }

        // column constraint
        private int CreateColumnConstraints(int[,] matrix, int header)
        {
            for (int column = _coverStartIndex; column <= _size; column++)
            {
                for (int n = _coverStartIndex; n <= _size; n++, header++)
                {
                    for (int row = _coverStartIndex; row <= _size; row++)
                    {
                        int index = IndexInCoverMatrix(row, column, n);
                        matrix[index,header] = 1;
                    }
                }
            }

            return header;
        }

        // row constraint
        private int CreateRowConstraints(int[,] matrix, int header)
        {
            for (int row = _coverStartIndex; row <= _size; row++)
            {
                for (int n = _coverStartIndex; n <= _size; n++, header++)
                {
                    for (int column = _coverStartIndex; column <= _size; column++)
                    {
                        int index = IndexInCoverMatrix(row, column, n);
                        matrix[index,header] = 1;
                    }
                }
            }

            return header;
        }

        // cell constraint
        private int CreateCellConstraints(int[,] matrix, int header)
        {
            for (int row = _coverStartIndex; row <= _size; row++)
            {
                for (int column = _coverStartIndex; column <= _size; column++, header++)
                {
                    for (int n = _coverStartIndex; n <= _size; n++)
                    {
                        int index = IndexInCoverMatrix(row, column, n);
                        matrix[index,header] = 1;
                    }
                }
            }

            return header;
        }
        #endregion


    }
}
