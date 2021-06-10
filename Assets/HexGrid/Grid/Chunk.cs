using UnityEngine;


namespace HexMap
{
    public class Chunk : MonoBehaviour
    {
        private HexMesh hexMesh;
        [SerializeField] private Cell[] cells;
        [SerializeField] public bool asset = false;

        //> INITIALIZE THE CHUNK
        private void Awake()
        {
            if (asset) return;
            
            hexMesh = GetComponentInChildren<HexMesh>();
            cells = new Cell[Metrics.XChunks * Metrics.ZChunks];
        }

        //> ADD A CELL TO THIS CHUNK
        public void AddCell(int index, Cell newCell)
        {
            // link & assure proper hierarchy
            newCell.chunk = this;
            cells[index] = newCell;
            newCell.transform.SetParent(hexMesh.transform, false);
        }

        //> UPDATE THIS CHUNK*
        public void Refresh() => enabled = true;

        //> UPDATE THIS CHUNK FOR REAL
        public void LateUpdate()
        {
            hexMesh.TriangulateAllCells(cells);
            enabled = false;
        }
    }
}