using UnityEngine;
using UnityEditor;


namespace TCM.UI
{
    //> PLACEHOLDER
    public class Stat { }
    
    [CustomPropertyDrawer(typeof(Stat))]
    public class StatPropertyDrawer: PropertyDrawer
    {
        SerializedProperty value, upgradeAmount, upgradeCost, costMultiplier, upgradeable;
        // float scale;

        override public void OnGUI(Rect rect, SerializedProperty property, GUIContent title)
        {
            value = property.FindPropertyRelative("defaultValue");
            upgradeAmount = property.FindPropertyRelative("upgradeAmount");
            upgradeCost = property.FindPropertyRelative("defaultUpgradeCost");
            costMultiplier = property.FindPropertyRelative("costMultiplier");
            upgradeable = property.FindPropertyRelative("upgradeable");


            EditorGUI.BeginProperty(rect, title, property);
            GUIStyle bold = new GUIStyle {richText = true};
            string statName = $"<b><color=white>{title.text}</color></b>";
            Rect prefixRect = new Rect(rect.x, rect.y+2, rect.width, 18);
            Rect titleRect = EditorGUI.PrefixLabel(prefixRect, new GUIContent(statName), bold);
            Rect valueRect = new Rect(titleRect.x, rect.y+2, titleRect.width*9/10, 18);
            EditorGUI.PropertyField(valueRect, value, GUIContent.none);
            Rect upgradableRect = new Rect(titleRect.x + titleRect.width*9/10+4, rect.y+2, titleRect.width*1/10, 18);
            EditorGUI.PropertyField(upgradableRect, upgradeable, GUIContent.none);

            EditorGUI.indentLevel++;

            // Rect foldoutRect = new Rect(rect.x, rect.y+20, rect.width, 18);
            if (upgradeable.boolValue)
            {
                Rect upgradeAmountRect = new Rect(rect.x, rect.y+22, rect.width, 18);
                EditorGUI.PropertyField(upgradeAmountRect, upgradeAmount, new GUIContent("Upgrade Amount"));
                
                Rect upgradeCostRect = new Rect(rect.x, rect.y+42, rect.width, 18);
                EditorGUI.PropertyField(upgradeCostRect, upgradeCost, new GUIContent("Upgrade Cost"));

                Rect costMultiplierRect = new Rect(rect.x, rect.y+62, rect.width, 18);
                costMultiplier.floatValue = EditorGUI.Slider(costMultiplierRect, new GUIContent("Cost Multiplier"), costMultiplier.floatValue, 1f, 2f);
            }
            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();


        }

        override public float GetPropertyHeight(SerializedProperty property, GUIContent title)
        {
            upgradeable = property.FindPropertyRelative("upgradeable");

            if (upgradeable.boolValue) return EditorGUIUtility.singleLineHeight * 4 + 8;
            else return EditorGUIUtility.singleLineHeight;
            
        }

    }
}
