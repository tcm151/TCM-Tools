using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace HexMap
{
    [RequireComponent(typeof(MeshRenderer))]
    public class HexGrid : MonoBehaviour
    {
        public Cell cellPrefab;
        public Chunk chunkPrefab;

        public int xChunks, zChunks;
        public int xCells = 10, zCells = 10;

        public List<Color> colors;

        [SerializeField] public Chunk[] chunks;
        [SerializeField] public Cell[] cells;
        [SerializeField] public bool asset = false;

        //> INITIALIZE THE GRID
        private void Awake()
        {
            if (asset) return;

            Metrics.colors = colors;   // set the colors for the hex metrics
            CreateMap(xCells, zCells); // create a new map with dimensions
        }

        //> CREATE A NEW MAP
        public bool CreateMap(int x, int z)
        {
            // check if the map is of valid size
            if ((x <= 0) || (x % Metrics.XChunks != 0) || (z <= 0) || (z % Metrics.ZChunks != 0))
            {
                //? REFACTOR to closest multiple of chunk resolution...

                Debug.LogError("Unsupported map size. :(");
                return false; // exit when not supported
            }

            // clear the map data if one already exists
            if (chunks != null)
            {
                foreach (Chunk chunk in chunks) Destroy(chunk.gameObject);
            }

            // setup the map dimensions
            xCells = x;
            zCells = z;
            xChunks = xCells / Metrics.XChunks;
            zChunks = zCells / Metrics.ZChunks;

            CreateChunks(); // build all the chunks
            CreateCells();  // the build all the cells

            return true;
        }

        //> CREATE THE PROPER AMOUNT OF CHUNKS
        private void CreateChunks()
        {
            // initialize chunk array with proper dimensions
            chunks = new Chunk[xChunks * zChunks];

            // loop over 2 dimensions (x,z)
            for (int z = 0, i = 0; z < zChunks; z++) {
                for (int x = 0; x < xChunks; x++)
                {
                    chunks[i++] = Instantiate(chunkPrefab);     // instantiate a new chunk 
                    chunks[i++].transform.SetParent(transform); // assure a clean hierarchy
                    chunks[i++].name = $"Chunk.{i}";            // give it a name
                }
            }
        }

        //> CREATE EVERY CELL ACCORDING TO THE DIMENSIONS
        private void CreateCells()
        {
            // initialize the cells array with proper dimensions
            cells = new Cell[zCells * xCells];

            // loop over two dimensions (x,z)
            for (int z = 0, i = 0; z < zCells; z++) {
                for (int x = 0; x < xCells; x++)
                {
                    CreateCell(x, z, i++); // handle creation separately
                }
            }
        }

        //> CREATE A CELL
        private void CreateCell(int x, int z, int i)
        {
            // calculate the hexagonal position
            Vector3 position;
            position.x = Metrics.XPositionFromOffset(x, z);
            position.z = Metrics.ZPositionFromOffset(z);
            position.y = 0f;

            // instantiate a cell prefab, set it's position and provide it with hex coords
            cells[i] = Instantiate(cellPrefab);
            cells[i].transform.localPosition = position;
            cells[i].coords = Coordinates.FromOffset(x, z);
            cells[i].name = $"Cell.{cells[i].coords.ToString()}";

            // complicated neighbor association, don't need to understand
            if (x > 0) cells[i].SetNeighbor(Direction.W, cells[i - 1]);
            if (z > 0)
            {
                if ((z & 1) == 0)
                {
                    cells[i].SetNeighbor(Direction.SE, cells[i - xCells]);
                    if (x > 0) cells[i].SetNeighbor(Direction.SW, cells[i - xCells - 1]);
                }
                else
                {
                    cells[i].SetNeighbor(Direction.SW, cells[i - xCells]);
                    if (x < xCells - 1) cells[i].SetNeighbor(Direction.SE, cells[i - xCells + 1]);
                }
            }

            cells[i].Elevation = 0; // default elevation

            AddCellToChunk(x, z, cells[i]); // associate the cell to it's proper chunk
        }

        //> ADD CELL TO IT'S RESPECTIVE CHUNK
        private void AddCellToChunk(int x, int z, Cell cell)
        {
            // calculate the proper chunk 
            int chunkX = x / Metrics.XChunks;
            int chunkZ = z / Metrics.ZChunks;
            Chunk chunk = chunks[chunkX + chunkZ * xChunks];

            // add the cell to that chunk
            int localX = x - chunkX * Metrics.XChunks;
            int localZ = z - chunkZ * Metrics.ZChunks;
            chunk.AddCell(localX + localZ * Metrics.XChunks, cell);
        }

        //> GET A CELL FROM POSITION
        public Cell GetCell(Vector3 position)
        {
            // return the cell which was click upon
            position = transform.InverseTransformPoint(position);
            Coordinates coordinates = Coordinates.FromPosition(position);
            int index = coordinates.X + coordinates.Z * xCells + coordinates.Z / 2;
            return cells[index];
        }

        //> GET A CELL FROM COORDINATES
        public Cell GetCell(Coordinates coords)
        {
            int z = coords.Z;
            if (z < 0 || z >= zCells) return null; // ignore if out of bounds
            int x = coords.X + z / 2;
            if (x < 0 || x >= xCells) return null; // ignore if out of bounds

            return cells[x + z * xCells];
        }

        //> REFRESH ALL CHUNKS
        public void Refresh()
        {
            foreach (var chunk in chunks) chunk.Refresh();
        }

        //> SAVE THE MAP
        public void Save(BinaryWriter writer)
        {
            writer.Write(xCells);
            writer.Write(zCells);

            foreach (var cell in cells) cell.Save(writer);
        }

        //> LOAD A MAP
        public void Load(BinaryReader reader, int header)
        {
            int x = 20, z = 15;

            if (header >= 1)
            {
                x = reader.ReadInt32();
                z = reader.ReadInt32();
            }

            if (x != xCells || z != zCells)
            {
                if (!CreateMap(x, z)) return;
            }

            foreach (var cell in cells) cell.LoadFrom(reader);
            foreach (var chunk in chunks) chunk.Refresh();
        }
    }
}