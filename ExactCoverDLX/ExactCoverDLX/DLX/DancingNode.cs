using System;
using System.Collections.Generic;
using System.Text;

namespace ExactCoverDLX.DLX
{
    public class DancingNode
    {

        #region attributes
        // 4 directions pointer
        public DancingNode Left { get; set; } 
        public DancingNode Right { get; set; } 
        public DancingNode Top { get; set; } 
        public DancingNode Bottom { get; set; }

        // column pointer
        public ColumnNode Column { get; set; }
        #endregion

        #region constructors
        public DancingNode()
        {
            Left = Right = Top = Bottom = this;
        }

        public DancingNode(ColumnNode column) : this()
        {
            this.Column = column;
        }
        #endregion

        #region link operations
        public DancingNode LinkDown(DancingNode node)
        {
            node.Bottom = this.Bottom;
            node.Bottom.Top = node;
            node.Top = this;
            this.Bottom = node;
            return node;
        }

        public DancingNode LinkRight(DancingNode node)
        {
            node.Right = this.Right;
            node.Right.Left = node;
            node.Left = this;
            this.Right = node;
            return node;
        }

        public void RemoveLeftRight()
        {
            this.Left.Right = this.Right;
            this.Right.Left = this.Left;
        }

        public void ReinsertLeftRight()
        {
            this.Left.Right = this;
            this.Right.Left = this;
        }

        public void RemoveTopBottom()
        {
            this.Top.Bottom = this.Bottom;
            this.Bottom.Top = this.Top;
        }

        public void ReinsertTopBottom()
        {
            this.Top.Bottom = this;
            this.Bottom.Top = this;
        }
        #endregion
    }
}
