using UnityEngine;


namespace TCM.Planets.Generation
{
    public class TerrainProfileUI : MonoBehaviour
    {
        [SerializeField] private Vector2[] vertices;
        [SerializeField] private int resolution;

        public void Render()
        {
            foreach (var v in vertices)
            {
                
            }
        }
    }
}