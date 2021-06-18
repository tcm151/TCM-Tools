using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


namespace HexMap.UI
{
    public class SaveAsAsset : MonoBehaviour
    {
        #if UNITY_EDITOR

        public InputField nameInput;

        public GameObject hexGrid;

        public void Open() => gameObject.SetActive(true);
        public void Close() => gameObject.SetActive(false);

        public void Save()
        {
            string mapName = nameInput.text;
            if (mapName.Length == 0) return;

            Debug.Log("NAME: " + mapName);

            string mapFolder, meshFolder;

            if (!AssetDatabase.IsValidFolder("Assets/Maps/" + mapName))
            {
                Debug.Log("Assets/Maps" + mapName);
                Debug.Log("CREATED NEW FOLDER");
                mapFolder = AssetDatabase.CreateFolder("Assets/Maps", mapName);
                meshFolder = AssetDatabase.CreateFolder("Assets/Maps/" + mapName, "Meshes");

                mapFolder = AssetDatabase.GUIDToAssetPath(mapFolder) + "/";
                meshFolder = AssetDatabase.GUIDToAssetPath(meshFolder) + "/";

                Debug.Log(mapFolder);
                Debug.Log(meshFolder);
            }
            else
            {
                mapFolder = "Assets/Maps/" + mapName + "/";
                meshFolder = mapFolder + "/Meshes/";
            }

            string mapPrefabName = mapName + ".prefab";
            GameObject mapGameObject = hexGrid;
            //! hexGrid.GetComponent<HexGrid>().IsAsset = true;
            GameObject mapPrefab = PrefabUtility.SaveAsPrefabAsset(mapGameObject, mapFolder + "/" + mapPrefabName);
            Chunk[] mapPrefabChunks = mapPrefab.GetComponent<HexGrid>().chunks;

            Chunk[] chunks = hexGrid.GetComponent<HexGrid>().chunks;

            for (int i = 0; i < chunks.Length; i++)
            {
                string chunkAssetName = "chunk" + i.ToString() + ".asset";
                Mesh chunkMesh = chunks[i].GetComponentInChildren<MeshFilter>().mesh;
                Mesh chunkMeshToSave = Object.Instantiate(chunkMesh);

                AssetDatabase.CreateAsset(chunkMeshToSave, meshFolder + chunkAssetName);
                // AssetDatabase.SaveAssets();

                Mesh savedMesh = (Mesh)AssetDatabase.LoadAssetAtPath(meshFolder + chunkAssetName, typeof(Mesh));
                if (savedMesh == null) Debug.Log("NULL MESH " + i);
                mapPrefabChunks[i].GetComponentInChildren<MeshFilter>().mesh = savedMesh;
                mapPrefabChunks[i].GetComponentInChildren<MeshCollider>().sharedMesh = savedMesh;
                mapPrefabChunks[i].GetComponent<Chunk>().asset = true;
                mapPrefabChunks[i].GetComponentInChildren<HexMesh>().asset = true;
            }
            AssetDatabase.SaveAssets();
            PrefabUtility.SavePrefabAsset(mapPrefab);

            Close();
        }
        
        #endif
    }
}