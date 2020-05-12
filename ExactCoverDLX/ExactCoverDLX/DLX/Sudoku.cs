using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
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

        private bool[,,] _eliminated;
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

            // initializing eliminated
            _eliminated = new bool[_size, _size, _size];

            // copying grid and setting eliminated values
            _grid = new int[_size, _size];
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    _grid[i, j] = grid[i, j];
                    if (grid[i, j] != _emptyCell)
                    {
                        for (int k = 0; k < _size; k++)
                        {
                            if(k!=j) _eliminated[i, k, grid[i, j] - 1] = true;
                            if(k!=i) _eliminated[k, j, grid[i, j] - 1] = true;
                            if(k / _boxSize + (i / _boxSize) * _boxSize != i && k % _boxSize + (j / _boxSize) * _boxSize != j) 
                                _eliminated[k / _boxSize + (i/_boxSize)*_boxSize, k % _boxSize + (j / _boxSize) * _boxSize, grid[i, j] - 1] = true;
                            
                        }
                    }
                }
            }

            /*
            for(int i = 0; i < _size; i++)
            {
                for(int j = 0; j < _size; j++)
                {
                    //Console.Write("eliminated[{0},{1}]: ", i, j);
                    for (int v = 0; v < _size; v++)
                    {
                        Console.Write(_eliminated[i, j, v]?"1":"0");
                    }
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            */

            
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

        private int[,] CreateCoverMatrix(int[,] grid)
        {
            int[,] coverMatrix = new int[_size * _size * _maxValue, _size * _size * _constraint];

            int header = 0;
            header = CreateCellConstraints(coverMatrix, header,grid);
            header = CreateRowConstraints(coverMatrix, header,grid);
            header = CreateColumnConstraints(coverMatrix, header,grid);
            CreateBoxConstraints(coverMatrix, header,grid);

            return coverMatrix;
        }

        private int[,] ConvertInCoverMatrix(int[,] grid)
        {
            //int[,] coverMatrix = CreateCoverMatrix();
            int[,] coverMatrix = CreateCoverMatrix(grid);

            // Taking into account the values already entered in Sudoku's grid instance
            /*
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
            */
            
            //Console.WriteLine("end updating cover matrix");


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


            Console.WriteLine("building cover matrix...");
            int[,] cover = ConvertInCoverMatrix(_grid);
            //PrintCoverMatrix(cover);

            DLX dlx = new DLX(cover);

            Console.WriteLine("finished building cover matrix.\nSolving...");
            dlx.Solve();
            _gridSolved = ConvertDLXListToGrid(dlx.Result);

            watch.Stop();
            TimeSpan elapsed = watch.Elapsed;

            Console.WriteLine("Grid solved with Algorithm X (dancing links) in {0}", elapsed);
            DisplaySolution();
        }

        public void DisplayGrid()
        {
            int maxLength = _grid.Cast<int>().Max().ToString().Length;

            //Console.WriteLine("\n " + new String('-',_size * (maxLength+1) + (_size/_boxSize)*2 - 1));
            for (int i = 0; i < _size; i++)
            {
                if (i % _boxSize == 0)
                    Console.WriteLine(" " + new String('-', _size * (maxLength + 1) + (_size / _boxSize) * 2 - 1));
                for (int j = 0; j < _size; j++)
                {
                    if (j % _boxSize == 0)
                        Console.Write("| ");
                    Console.Write(_grid[i, j].ToString().PadLeft(maxLength,' ') + " ");
                    
                }
                Console.Write("|\n");
                
            }
            Console.WriteLine(" " + new String('-', _size * (maxLength + 1) + (_size / _boxSize) * 2 - 1));
        }

        public void PrintCoverMatrix(int[,] cover)
        {
            string matrix = "";
            for(int i = 0; i < cover.GetLength(0); i++)
            {
                for(int j = 0; j < cover.GetLength(1); j++)
                {
                    matrix += cover[i, j];
                }
                matrix += "\n";
            }
            File.WriteAllText("./exactCoverMatrix.txt",matrix);
        }

        public void DisplaySolution()
        {
            int maxLength = _gridSolved.Cast<int>().Max().ToString().Length;

            //Console.WriteLine("\n " + new String('-', _size * (maxLength + 1) + (_size / _boxSize) * 2 - 1));
            for (int i = 0; i < _size; i++)
            {
                if (i % _boxSize == 0)
                    Console.WriteLine(" " + new String('-', _size * (maxLength + 1) + (_size / _boxSize) * 2 - 1));
                for (int j = 0; j < _size; j++)
                {
                    if (j % _boxSize == 0)
                        Console.Write("| ");
                    Console.Write(_gridSolved[i, j].ToString().PadLeft(maxLength, ' ') + " ");

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
        private int CreateBoxConstraints(int[,] matrix, int header, int[,] grid)
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
                                if (
                                    grid[row + rowDelta - 1, column + columnDelta - 1] != _emptyCell && n != grid[row + rowDelta - 1, column + columnDelta - 1]
                                    || _eliminated[row + rowDelta - 1, column + columnDelta - 1, n - 1]
                                    ) continue;

                                int index = IndexInCoverMatrix(row + rowDelta, column + columnDelta, n);
                                matrix[index, header] = 1;
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
        private int CreateColumnConstraints(int[,] matrix, int header,int[,] grid)
        {
            for (int column = _coverStartIndex; column <= _size; column++)
            {
                for (int n = _coverStartIndex; n <= _size; n++, header++)
                {
                    for (int row = _coverStartIndex; row <= _size; row++)
                    {
                        if (
                            grid[row - 1, column - 1] != _emptyCell && n != grid[row - 1, column - 1]
                            || _eliminated[row - 1, column - 1, n - 1]
                            ) continue;

                        int index = IndexInCoverMatrix(row, column, n);
                        matrix[index, header] = 1;
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
        private int CreateRowConstraints(int[,] matrix, int header, int[,] grid)
        {
            for (int row = _coverStartIndex; row <= _size; row++)
            {
                for (int n = _coverStartIndex; n <= _size; n++, header++)
                {
                    for (int column = _coverStartIndex; column <= _size; column++)
                    {
                        if (
                            grid[row - 1, column - 1] != _emptyCell && n != grid[row - 1, column - 1]
                            || _eliminated[row - 1, column - 1, n - 1]
                            ) continue;

                        int index = IndexInCoverMatrix(row, column, n);
                        matrix[index, header] = 1;
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
        private int CreateCellConstraints(int[,] matrix, int header, int[,] grid)
        {
            for (int row = _coverStartIndex; row <= _size; row++)
            {
                for (int column = _coverStartIndex; column <= _size; column++, header++)
                {
                    for (int n = _coverStartIndex; n <= _size; n++)
                    {
                        if (
                            grid[row - 1, column - 1] != _emptyCell && n != grid[row - 1, column - 1]
                            || _eliminated[row - 1, column - 1, n - 1]
                            ) continue;

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
