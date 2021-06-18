using UnityEditor;
using UnityEngine;


namespace TCM.MarchingCubes
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ChunkManager))]
    public class ChunkManagerInspector : Editor
    {
        private ChunkManager manager;
        
        private void OnEnable() => manager = target as ChunkManager;

        override public void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Generate")) manager.Initialize();
            // if (GUILayout.Button("Polygonalize")) manager.PolygonalizeChunks();
        }
    }
}