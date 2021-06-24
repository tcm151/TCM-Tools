using System;
using System.IO;
using UnityEngine;


namespace TCM.HexGrid
{
    public class Cell : MonoBehaviour
    {
        [Serializable] public struct Data //@ JOB-ABLE
        {
            public float elevation;
            public int   terrain;
            public bool  buildable;
            public bool  occupied;
        }
        
        private Data data;

        public Chunk chunk;
        public Coordinates coords;
        [SerializeReference] private Cell[] neighbours = new Cell[6];
        
        //> ELEVATION
        public float Elevation
        {
            get => data.elevation;
            set
            {
                if (Math.Abs(Elevation - value) < 0.0001f) return;
                Elevation = value;

                UpdatePosition();
                UpdateChunks();
            }
        }
        
        //> COLOR
        public Color Color => Metrics.colors[data.terrain]; //@ to be updated

        //> TERRAIN
        public int Terrain
        {
            get => data.terrain;
            set
            {
                if (data.terrain == value) return;
                data.terrain = value;
                UpdateChunks();
            }
        }
        
        //> BUILDABLE
        public bool Buildable
        {
            get => data.buildable;
            set
            {
                if (data.buildable == value) return;
                data.buildable = value;
                UpdateChunks();
            }
        }

        //> OCCUPIED
        public bool Occupied
        {
            get => data.occupied;
            set
            {
                if (data.buildable == value) return;
                data.occupied = value;
                UpdateChunks();
            }
        }

        //> GET THE NEIGHBOR IN THE PROVIDED DIRECTION
        public Cell GetNeighbor(Direction direction) => neighbours[(int)direction];
        
        //> SET THE NEIGHBOR IN THE PROVIDED DIRECTION
        public void SetNeighbor(Direction direction, Cell cell)
        {
            neighbours[(int)direction] = cell;
            cell.neighbours[(int)direction.Opposite()] = this;
        }
        
        //> UPDATE THE ELEVATION OF THE HEX TILE
        private void UpdatePosition()
        {
            Vector3 position = transform.localPosition;
            position.y = Elevation * Metrics.ElevationStep;
            transform.localPosition = position;
        }

        //> UPDATE THE MESH
        private void UpdateChunks()
        {
            if (!chunk) return;
            chunk.Refresh();

            // check if every neighbor exists, and update the neighbor chunk
            foreach (Cell neighbor in neighbours)
            {
                if (neighbor && neighbor.chunk != this.chunk) neighbor.chunk.Refresh();
            }
        }

        //> SAVE THIS HEX CELL
        public void Save(BinaryWriter writer)
        {
            writer.Write(Terrain);
            writer.Write(Elevation);
            writer.Write(Buildable);
            writer.Write(Occupied);
        }

        //> LOAD THIS HEX CELL
        public void LoadFrom(BinaryReader reader)
        {
            Terrain   = reader.ReadInt32();
            Elevation = reader.ReadInt32();
            Buildable = reader.ReadBoolean();
            Occupied  = reader.ReadBoolean();
            UpdatePosition(); // required or death
        }
    }
}