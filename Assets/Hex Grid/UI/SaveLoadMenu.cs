using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace TCM.HexGrid.UI
{
        public class SaveLoadMenu : MonoBehaviour
    {
        public HexGrid hexGrid;
        public Text menuLabel, buttonLabel;
        public InputField nameInput;

        public RectTransform listContent;
        public SaveLoadEntry itemPrefab;

        private bool saveMode;

        private string mapsPath = "Assets/Maps/-Editable/";

        public void Open(bool saveMode)
        {
            this.saveMode = saveMode;

            if (saveMode)
            {
                menuLabel.text = "Save Map";
                buttonLabel.text = "Save";
            }
            else
            {
                menuLabel.text = "Load Map";
                buttonLabel.text = "Load";
            }

            FillList();
            gameObject.SetActive(true);
        }

        public void Close() => gameObject.SetActive(false);

        private string GetSelectedPath()
        {
            string mapName = nameInput.text;
            if (mapName.Length == 0) return null;
            return Path.Combine(mapsPath, mapName + ".map");
        }

        public void Action()
        {
            string path = GetSelectedPath();

            if (path == null) return;

            if (saveMode) Save(path);
            if (!saveMode) Load(path);

            Close();
        }

        public void SelectItem(string name)
        {
            nameInput.text = name;
        }

        private void FillList()
        {
            for (int i = 0; i < listContent.childCount; i++)
            {
                Destroy(listContent.GetChild(i).gameObject);
            }

            string[] paths = Directory.GetFiles(mapsPath, "*.map");
            Array.Sort(paths);

            for (int i = 0; i < paths.Length; i++)
            {
                SaveLoadEntry item = Instantiate(itemPrefab, listContent, false);
                item.menu = this;
                item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
            }
        }

        public void Delete()
        {
            string path = GetSelectedPath();
            if (path == null) return;

            if (File.Exists(path)) File.Delete(path);

            nameInput.text = "";
            FillList();
        }

        //> SAVE THE MAP & WRITE IT TO FILE
        public void Save(string path)
        {
            using(BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(1);
                hexGrid.Save(writer);
            }
        }

        //> READ MAP FROM FILE & LOAD IT INTO SCENE
        public void Load(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log("Map file does not exist! :(");
                return;
            }

            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                int header = reader.ReadInt32(); // read the first byte of the file

                if (header <= 1) hexGrid.Load(reader, header); // if the header matches, load the map
                else Debug.Log("Incompatible Map Save: " + header); // map is not compatible
            }
        }

    }
}