﻿using ExactCoverDLX.Utils;
using System;
using System.Collections.Generic;
using System.IO;
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

        private int idProcess = 0;
        private void Process(int k) // O(N^(N^2))
        {
            int id = ++idProcess;
            Logger.Log("Process: begin id: " + id + ", k: "+k);

            if (_header.Right == _header) // covered all the headers (columns)
            {
                // End of Algorithm X
                // Result is copied in a result list
                Result = new LinkedList<DancingNode>(_answer);
                Logger.Log("Process: id: " + id + ", end of algorithm, result found");
            }
            else
            {
                // we choose column c
                Logger.Log("Process: id: " + id + ", selecting column");
                ColumnNode c = SelectColumnNodeHeuristic(); // O(N^2)
                Logger.Log("Process: id: " + id + ", selected column: " + c.Name);

                c.Cover(); // O(N)
                Logger.Log("Process: id: " + id + ", covered column: " + c.Name);

                int nodes = 0;
                for (DancingNode r = c.Bottom; r != c; r = r.Bottom) // N iterations * (N + recurse (depth = N^2)) -> O(N^(N^2))
                {
                    // We add r line to partial solution
                    _answer.Add(r);
                    Logger.Log("Process: id: " + id + ", choosing node (Bottom): " + ++nodes + " as partial answer");

                    // We cover columns
                    for (DancingNode j = r.Right; j != r; j = j.Right) // 4 * N -> O(N)
                    {
                        j.Column.Cover(); // O(N)
                        Logger.Log("Process: id: " + id + ", node: " + nodes + ", covered column (Right): " + j.Column.Name);
                    }

                    // recursive call to level k + 1
                    Logger.Log("Process: id: " + id + ", Recursive call of Process: " + (k+1));
                    Process(k + 1); // Recurse -> depth ~= N^2 (number of columns to cover)
                    if (Result != null) return;

                    // We go back
                    _answer.Remove(r);
                    Logger.Log("Process: id: " + id + ", removing node (Bottom): " + nodes + " as partial answer");
                    c = r.Column;


                    // We uncover columns
                    for (DancingNode j = r.Left; j != r; j = j.Left) // 4 * N -> O(N)
                    {
                        j.Column.Uncover(); // O(N)
                        Logger.Log("Process: id: " + id + ", node: " + nodes + ", uncovered column (Left): " + j.Column.Name);
                    }
                }

                c.Uncover(); // O(N)
                Logger.Log("Process: id: " + id + ", uncovered column: " + c.Name);
            }
            Logger.Log("Process: end id: " + id);
        }

        public void Solve()
        {
            _answer = new List<DancingNode>();
            Process(0);
        }

        private int idSelectColumnNodeHeuristic = 0;
        private ColumnNode SelectColumnNodeHeuristic() // O(N^2)
        {
            int id = ++idSelectColumnNodeHeuristic;
            Logger.Log("SelectColumnNodeHeuristic: begin execution id: " + id);

            int min = int.MaxValue;
            ColumnNode ret = null;
            for (ColumnNode column = (ColumnNode)_header.Right; column != _header; column = (ColumnNode) column.Right) // O(N^2)
            {
                if (column.Size < min) // O(1)
                {
                    min = column.Size;
                    ret = column;
                }
                if (min == 1) break;
                Logger.Log("SelectColumnNodeHeuristic: loop -> min:" + min + ", column: " + column.Name + ", size: " + column.Size);
            }
            Logger.Log("SelectColumnNodeHeuristic: end execution id: " + id + ", min column: "+ ret.Name);
            return ret;
        }

        #endregion
    }
}
