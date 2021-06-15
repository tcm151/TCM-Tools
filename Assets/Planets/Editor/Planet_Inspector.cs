using UnityEngine;
using UnityEditor;


namespace TCM.Planets
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Planet))]
    public class PlanetInspector : Editor
    {
        private Planet planet;
        private bool autoUpdate;
        
        //> ON ENABLE
        private void OnEnable() => planet = target as Planet;

        //> INSPECTOR GUI
        override public void OnInspectorGUI()
        {
            using var check = new EditorGUI.ChangeCheckScope();
            
            base.OnInspectorGUI();

            autoUpdate = EditorGUILayout.Toggle("Auto Update", autoUpdate);
            if (check.changed && autoUpdate) planet.TriangulateChunks();

            EditorGUILayout.Space(8);
            
            if (GUILayout.Button("Generate Planet")) planet.GeneratePlanet(false);
            // if (GUILayout.Button("Apply Colors")) planet.ApplyColors();
        }
    }
}