using ExactCoverDLX.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExactCoverDLX.DLX
{
    public class ColumnNode : DancingNode
    {
        #region attributes
        public int Size { get; set; }
        public String Name { get; set; }
        #endregion

        #region constructors
        public ColumnNode(String name) : base()
        {
            this.Size = 0;
            this.Name = name;
            this.Column = this;
        }
        #endregion

        #region cover & uncover functions
        public void Cover() // O(N)
        {
            Logger.Log("Cover: covering column: " + Name);
            RemoveLeftRight(); // O(1)

            int b = 0, r = 0, rc = 0;

            for (DancingNode i = this.Bottom; i != this; i = i.Bottom) // O(N)
            {
                b++;
                r = 0;
                for (DancingNode j = i.Right; j != i; j = j.Right) // O(1)
                {
                    r++;
                    j.RemoveTopBottom(); // O(1)
                    j.Column.Size--;
                }
                Logger.Log("Cover: covered row : " + b + ", items in row: " + r);
                rc += r;
            }
            Logger.Log("Cover: rows : " + b + ", total items in row" + rc);
        }

        public void Uncover() // O(N)
        {
            Logger.Log("Unover: uncovering column: " + Name);
            int t = 0, l = 0, lu = 0;

            for (DancingNode i = this.Top; i != this; i = i.Top) // O(N)
            {
                t++;
                l = 0;
                for (DancingNode j = i.Left; j != i; j = j.Left) // O(1)
                {
                    l++;
                    j.Column.Size++;
                    j.ReinsertTopBottom(); // O(1)
                }
                Logger.Log("Uncover: uncovered row : " + t + ", items in row: " + l);
                lu += l;
            }

            ReinsertLeftRight(); // O(1)
            Logger.Log("Uncover: rows : " + t + ", total items in row" + lu);
        }
        #endregion
    }
}
