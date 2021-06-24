using UnityEngine;
using UnityEngine.UI;


namespace TCM.HexGrid.UI
{
    public class SaveLoadEntry : MonoBehaviour
    {
        public SaveLoadMenu menu;
        private string mapName;
        
        public string MapName
        {
            get => MapName;
            
            set
            {
                mapName = value;
                transform.GetChild(0).GetComponent<Text>().text = value;
            }
        }
        
        
        public void Select() => menu.SelectItem(mapName);
    }
}