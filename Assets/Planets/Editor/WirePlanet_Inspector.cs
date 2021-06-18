using UnityEngine;
using UnityEditor;


namespace TCM.Planets
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(WirePlanet))]
    public class WirePlanetInspector : Editor
    {
        private WirePlanet planet;
        private bool autoUpdate;
        
        //> ON ENABLE
        private void OnEnable() => planet = target as WirePlanet;

        //> INSPECTOR GUI
        override public void OnInspectorGUI()
        {
            using var check = new EditorGUI.ChangeCheckScope();
            {
                base.OnInspectorGUI();
                
                autoUpdate = EditorGUILayout.Toggle("Auto Update", autoUpdate);
                if (check.changed && autoUpdate) planet.Initialize();

                EditorGUILayout.Space(8);
                
                if (GUILayout.Button("Generate Planet")) planet.Initialize();
            }
        }
    }
}