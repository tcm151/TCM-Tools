using UnityEngine;
using UnityEditor;


namespace TCM.Planets
{
    [CustomPropertyDrawer(typeof(Noise.Layer))]
    public class NoiseLayerPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty enabled, mask, type, octaves, strength,
                                   baseRoughness, roughness, persistence, localZero, offset;
        
        //> INSPECTOR GUI
        override public void OnGUI(Rect rect, SerializedProperty property, GUIContent title)
        {
            enabled = property.FindPropertyRelative("enabled");
            mask = property.FindPropertyRelative("useMask");
            type = property.FindPropertyRelative("noiseType");
            octaves = property.FindPropertyRelative("octaves");
            strength = property.FindPropertyRelative("strength");
            baseRoughness = property.FindPropertyRelative("baseRoughness");
            roughness = property.FindPropertyRelative("roughness");
            persistence = property.FindPropertyRelative("persistence");
            localZero = property.FindPropertyRelative("localZero");
            offset = property.FindPropertyRelative("offset");

            const int lineHeight = 18;
            EditorGUI.BeginProperty(rect, title, property);
            {
                Rect enabledRect = new Rect(rect.x, rect.y + 0*lineHeight, rect.width, lineHeight);
                EditorGUI.PropertyField(enabledRect, enabled, new GUIContent("Enabled"));
                
                if (enabled.boolValue)
                {
                    Rect maskRect = new Rect(rect.x, rect.y + 1*lineHeight, rect.width, lineHeight);
                    EditorGUI.PropertyField(maskRect, mask, new GUIContent("Mask"));
                    
                    Rect typeRect = new Rect(rect.x, rect.y + 2*lineHeight, rect.width, lineHeight);
                    EditorGUI.PropertyField(typeRect, type, new GUIContent("Noise Type"));
                    
                    Rect octavesRect = new Rect(rect.x, rect.y + 3*lineHeight, rect.width, lineHeight);
                    EditorGUI.PropertyField(octavesRect, octaves, new GUIContent("Octaves"));
                    
                    Rect strengthRect = new Rect(rect.x, rect.y + 4*lineHeight, rect.width, lineHeight);
                    EditorGUI.PropertyField(strengthRect, strength, new GUIContent("Strength"));
                    
                    Rect baseRoughnessRect = new Rect(rect.x, rect.y + 5*lineHeight, rect.width, lineHeight);
                    EditorGUI.PropertyField(baseRoughnessRect, baseRoughness, new GUIContent("Base Roughness"));
                    
                    Rect roughnessRect = new Rect(rect.x, rect.y + 6*lineHeight, rect.width, lineHeight);
                    EditorGUI.PropertyField(roughnessRect, roughness, new GUIContent("Roughness"));
                    
                    Rect persistenceRect = new Rect(rect.x, rect.y + 7*lineHeight, rect.width, lineHeight);
                    EditorGUI.PropertyField(persistenceRect, persistence, new GUIContent("Persistence"));
                    
                    Rect localZeroRect = new Rect(rect.x, rect.y + 8*lineHeight, rect.width, lineHeight);
                    EditorGUI.PropertyField(localZeroRect, localZero, new GUIContent("LocalZero"));
                    
                    Rect offsetRect = new Rect(rect.x, rect.y + 9*lineHeight, rect.width, lineHeight);
                    EditorGUI.PropertyField(offsetRect, offset, new GUIContent("Offset"));
                }
            }
            EditorGUI.EndProperty();
        }
        
        //> DEFINE PROPERTY HEIGHT
        override public float GetPropertyHeight(SerializedProperty property, GUIContent title)
        {
            enabled = property.FindPropertyRelative("enabled");

            if (enabled.boolValue) return EditorGUIUtility.singleLineHeight * 9 + 18;
            else return EditorGUIUtility.singleLineHeight;

        }
    }
}