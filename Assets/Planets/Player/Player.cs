using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


namespace TCM.Planets
{
    public class Player : MonoBehaviour
    {
        public Planet planet;
        
        private void Start()
        {
            Vector3 pointOnUnitSphere;
            do pointOnUnitSphere = Random.insideUnitSphere.normalized;
            while (Mathf.Abs(Vector3.Dot(pointOnUnitSphere, Vector3.up)) > 0.5f);


            var closestChunk = planet.GenerateClosestChunks(pointOnUnitSphere);
            transform.position = closestChunk.GetElevation(pointOnUnitSphere) * 1.0001f;
        
        //     StartCoroutine(RefreshChunksCoroutine(new WaitForSeconds(15f)));
        }
        
        
        // private IEnumerator RefreshChunksCoroutine(WaitForSeconds delay, bool repeat = true)
        // {
        //     Debug.Log("Starting Chunk Refresh!");
        //     
        //     List<Chunk> closestChunks = planet.GenerateClosestChunks(transform.position);
        //
        //     //- Remove the chunk were standing on
        //     if (Physics.Raycast(transform.position + transform.up * 0.1f, Vector3.down, out RaycastHit hit))
        //     {
        //         var chunk = hit.transform.GetComponent<Chunk>();
        //         if (chunk && closestChunks.Contains(chunk))
        //             closestChunks.Remove(chunk);
        //     }
        //     
        //     //- triangulate each chunk
        //     foreach (var chunk in closestChunks)
        //     {
        //         chunk.Triangulate(64);
        //         chunk.Apply();
        //     }
        //
        //     yield return delay;
        //     if (repeat) StartCoroutine(RefreshChunksCoroutine(delay));
        // }
        
    }
}