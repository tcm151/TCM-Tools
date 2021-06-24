using System;
using UnityEngine;
using UnityEditor;


namespace TCM.MarchingCubes
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ChunkGrid))]
    public class ChunkGridInspector : Editor
    {
        private bool autoUpdate;
        private ChunkGrid chunkGrid;

        //> ON AWAKE
        private void Awake() => autoUpdate = true;

        //> ON ENABLE
        private void OnEnable() => chunkGrid = target as ChunkGrid;

        //> INSPECTOR GUI
        override public void OnInspectorGUI()
        {
            using var check = new EditorGUI.ChangeCheckScope();
            base.OnInspectorGUI();

            autoUpdate = EditorGUILayout.Toggle("Auto Update", autoUpdate);
            if (check.changed && autoUpdate) chunkGrid.Generate(false);

            EditorGUILayout.Space(8);
            if (GUILayout.Button("Generate")) chunkGrid.Generate(true);
        }
    }
}