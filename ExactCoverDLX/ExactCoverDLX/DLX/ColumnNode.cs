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
        public void Cover()
        {
            RemoveLeftRight();

            for (DancingNode i = this.Bottom; i != this; i = i.Bottom)
            {
                for (DancingNode j = i.Right; j != i; j = j.Right)
                {
                    j.RemoveTopBottom();
                    j.Column.Size--;
                }
            }
        }

        public void Uncover()
        {
            for (DancingNode i = this.Top; i != this; i = i.Top)
            {
                for (DancingNode j = i.Left; j != i; j = j.Left)
                {
                    j.Column.Size++;
                    j.ReinsertTopBottom();
                }
            }

            ReinsertLeftRight();
        }
        #endregion
    }
}
