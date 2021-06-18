using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TCM.Tools
{
    public class Factory : ScriptableObject
    {
        //> CREATE AND INSTANCE ON PREFAB
        public static T Instantiate<T>(ref Scene scene, T prefab) where T : MonoBehaviour
        {
            if (!scene.isLoaded) scene = SceneManager.CreateScene(scene.name);

            T instance = Instantiate(prefab);
            SceneManager.MoveGameObjectToScene(instance.gameObject, scene);
            return instance;
        }
    }
}