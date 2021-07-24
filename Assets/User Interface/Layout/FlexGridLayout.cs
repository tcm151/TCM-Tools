using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TCM.UI
{
    public class FlexGridLayout : LayoutGroup
    {
        //> FIT TYPE OF GRID
        public enum FitType { Uniform, Height, Width, FixedRows, FixedColumns };

        //> LOCAL VARIABLES
        public FitType fit;
        public int rows, columns;
        public bool expandX, expandY;
        public Vector2 cellSize, spacing;

        //> LAYOUT CREATION
        override public void CalculateLayoutInputVertical()
        {
            base.CalculateLayoutInputHorizontal();

            if (fit == FitType.Width || fit == FitType.Height || fit == FitType.Uniform)
            {
                expandX = expandY = true;
                
                float squareRoot = Mathf.Sqrt(transform.childCount);
                rows = Mathf.CeilToInt(squareRoot);
                columns = Mathf.CeilToInt(squareRoot);
            }
            

            if (fit == FitType.Width || fit == FitType.FixedColumns)
                rows = Mathf.CeilToInt(transform.childCount / (float)columns);
            
            if (fit == FitType.Height || fit == FitType.FixedRows)
                columns = Mathf.CeilToInt(transform.childCount / (float)rows);

            var rect = rectTransform.rect;
            
            float cellHeight = (rect.height / rows) 
                             - ((spacing.y / rows) * (rows - 1))
                             - (padding.top / (float)rows)
                             - (padding.bottom / (float)rows);
            
            float cellWidth = (rect.width / columns)
                            - ((spacing.x / columns) * (columns - 1))
                            - (padding.left / (float)columns)
                            - (padding.right / (float)columns);

            cellSize.y = expandY ? cellHeight : cellSize.y;
            cellSize.x = expandX ? cellWidth : cellSize.x;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                int rowCount = i / columns;
                int columnCount = i % columns;

                var item = rectChildren[i];
                var yPos = (cellSize.y *  rowCount  ) + (spacing.y *  rowCount  ) + padding.top;
                var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
                
                SetChildAlongAxis(item, 1, yPos, cellSize.y);
                SetChildAlongAxis(item, 0, xPos, cellSize.x);
            }
        }

        override public void SetLayoutHorizontal() { }
        override public void SetLayoutVertical() { }
    }
}
