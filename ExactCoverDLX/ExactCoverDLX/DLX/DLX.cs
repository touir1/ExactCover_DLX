using System;
using System.Collections.Generic;
using System.Text;

namespace ExactCoverDLX.DLX
{
    public class DLX
    {
        #region attributes
        private ColumnNode _header;
        private List<DancingNode> _answer;
        public LinkedList<DancingNode> Result { get; set; }
        #endregion

        #region constructors
        public DLX(int[,] cover)
        {
            _header = CreateDLXList(cover);
        }
        #endregion

        #region DLX functions
        private ColumnNode CreateDLXList(int[,] grid)
        {
            /*
            int nbOnes = 0,count, nbZeros = 0;
            for(int i = 0; i < grid.GetLength(1); i++)
            {
                count = 0;
                for(int j = 0; j < grid.GetLength(0); j++)
                {
                    if (grid[j, i] == 1) count++;
                }
                if (count == 1) nbOnes++;
                if(count==0) nbZeros++;
            }
            */

            Console.WriteLine("cover matrix size: {0} x {1}", grid.GetLength(0), grid.GetLength(1));
            //Console.WriteLine("(start)number of columns with zeros: {0}",nbZeros);
            //Console.WriteLine("(start)number of columns with ones: {0}",nbOnes);



            int nbColumns = grid.GetLength(1);
            ColumnNode headerNode = new ColumnNode("header");
            List<ColumnNode> columnNodes = new List<ColumnNode>();

            for (int i = 0; i < nbColumns; i++)
            {
                ColumnNode node = new ColumnNode(i + "");
                columnNodes.Add(node);
                headerNode = (ColumnNode)headerNode.LinkRight(node);
            }

            headerNode = headerNode.Right.Column;

            for (int i=0; i<grid.GetLength(0); i++)
            {
                DancingNode prev = null;

                for (int j = 0; j < nbColumns; j++)
                {
                    if (grid[i,j] == 1)
                    {
                        ColumnNode col = columnNodes[j];
                        DancingNode newNode = new DancingNode(col);

                        if (prev == null)
                            prev = newNode;

                        col.Top.LinkDown(newNode);
                        prev = prev.LinkRight(newNode);
                        col.Size++;
                    }
                }
            }

            headerNode.Size = nbColumns;

            return headerNode;
        }

        //private int minCount = int.MaxValue;
        //private System.Diagnostics.Stopwatch stopwatch;

        private void Process(int k)
        {
            /*
            if(k==0) stopwatch = System.Diagnostics.Stopwatch.StartNew();

            int count = 0;
            for (ColumnNode column = (ColumnNode)_header.Right; column != _header; column = (ColumnNode)column.Right)
            {
                count++;
            }
            if(count < minCount)
            {
                minCount = count;
                Console.WriteLine("[{0}]header mninimum count: " + minCount, stopwatch.Elapsed);
            }
            */
            

            if (_header.Right == _header)
            {
                // End of Algorithm X
                // Result is copied in a result list
                Result = new LinkedList<DancingNode>(_answer);
            }
            else
            {
                // we choose column c
                ColumnNode c = SelectColumnNodeHeuristic();
                c.Cover();

                for (DancingNode r = c.Bottom; r != c; r = r.Bottom)
                {
                    // We add r line to partial solution
                    _answer.Add(r);

                    // We cover columns
                    for (DancingNode j = r.Right; j != r; j = j.Right)
                    {
                        j.Column.Cover();
                    }

                    // recursive call to level k + 1
                    Process(k + 1);

                    // We go back
                    _answer.Remove(r);
                    c = r.Column;

                    // We uncover columns
                    for (DancingNode j = r.Left; j != r; j = j.Left)
                    {
                        j.Column.Uncover();
                    }
                }

                c.Uncover();
            }
        }

        public void Solve()
        {
            _answer = new List<DancingNode>();
            Process(0);
        }

        private ColumnNode SelectColumnNodeHeuristic()
        {
            int min = int.MaxValue;
            ColumnNode ret = null;
            for (ColumnNode column = (ColumnNode)_header.Right; column != _header; column = (ColumnNode) column.Right)
            {
                if (column.Size < min)
                {
                    min = column.Size;
                    ret = column;
                }
            }
            return ret;
        }

        #endregion
    }
}
