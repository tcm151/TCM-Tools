using System.Collections;
using UnityEngine;


namespace TCM
{
    public static class UnityExtensions
    {
        //> LAYER MASKS

        public static bool Contains(this LayerMask layerMask, int layer) => (layerMask == (layerMask | (1 << layer)));

        //> VECTORS

        public static void Remap(this ref Vector3 v, float inMin, float inMax, float outMin, float outMax)
        {
            v.x = Remap(inMin, inMax, outMin, outMax, v.x);
            v.y = Remap(inMin, inMax, outMin, outMax, v.y);
            v.z = Remap(inMin, inMax, outMin, outMax, v.z);
        }

        public static float Lerp(float a, float b, float v) => ((1f - v) * a) + (b * v);
        public static float InverseLerp(float a, float b, float v) => (v - a) / (b - a);
        public static float Remap(float inMin, float inMax, float outMin, float outMax, float v) => Lerp(outMin, outMax, InverseLerp(inMin, inMax, v));

        public static Vector3Int FloorToInt(this Vector3 v) => new Vector3Int {x = Mathf.FloorToInt(v.x), y = Mathf.FloorToInt(v.y), z = Mathf.FloorToInt(v.z)};

        // static public void Start(this MonoBehaviour s, IEnumerator activity) => s.StartCoroutine(activity);
    }
}