using UI;
using UnityEngine;
using UnityEngine.EventSystems;


namespace HexMap.UI
{
    public class MapEditorPanel : UIPanel
    {
        new private Camera camera;
        private HexGrid hexes;

        private Color color;
        private int brushSize;
        private bool continuous;
        
        private int terrain;
        private bool editingTerrain;

        private int elevation;
        private bool editingElevation;
        
        private bool buildable;
        private bool editingBuildable;


        override protected void Awake()
        {
            base.Awake();
            
            camera = Camera.main;
            hexes = GetComponent<HexGrid>();
        }

        private void Update()
        {
            if (continuous)
            {
                if (Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject()) HandleInput();
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) HandleInput();
            }
        }

        override public void GoBack()
        {
            //@ do something in the future
        }

        //> HANDLE THE INPUT
        private void HandleInput()
        {
            Ray inputRay = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(inputRay, out RaycastHit hit)) EditCells(hexes.GetCell(hit.point));
        }
        
        //> EDIT A CELL WITH CURRENT SETTINGS
        private void EditCell(Cell cell)
        {
            if (!cell) return;
            
            if (terrain != -1 && !editingBuildable) cell.Terrain = terrain;
            if (editingElevation) cell.Elevation = elevation;
            if (editingBuildable) cell.Buildable = buildable;
        }
        
        //> EDIT CELLS BY BRUSH SIZE
        private void EditCells(Cell centerCell)
        {
            int centerX = centerCell.coords.X;
            int centerZ = centerCell.coords.Z;

            for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++) {
                for (int x = centerX - r; x <= centerX + brushSize; x++)
                {
                    EditCell(hexes.GetCell(new Coordinates(x, z)));
                }

            }
            for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++) {
                for (int x = centerX - brushSize; x <= centerX + r; x++)
                {
                    EditCell(hexes.GetCell(new Coordinates(x, z)));
                }
            }

        }
        
        public void SetTerrainType(int index) => terrain = index;
        public void SetElevation(float newElevation) => newElevation = (int)newElevation;
        public void EditingElevation(bool toggle) => editingElevation = toggle;
        public void SetBrushSize(float size) => brushSize = (int)size;
        public void SetContinuous(bool toggle) => continuous = toggle;
        public void SetBuildable(bool toggle) => buildable = toggle;
        
        public void EditingBuildable(bool toggle) 
        {
            editingBuildable = toggle;
            // hexes.EditingBuildable(toggle);
            hexes.Refresh();
        }
    }
    
}