using UnityEngine;
using UnityEngine.Serialization;


namespace TCM.Planets
{
    public static class Noise
    {
        //> NOISE TYPES
        public enum Type { Simple, Rigid };
        
        //> NOISE LAYER SETTINGS
        [System.Serializable] public class Layer
        {
            public bool enabled = true;
            public bool useMask = true;
        
            public Type noiseType;
        
            [Range( 1,  8)] public int octaves = 1;
            [Range( 0,  5)] public float strength = 1f;
            [Range( 0, 10)] public float baseRoughness = 1f;
            [Range( 0, 15)] public float roughness = 2f;
            [Range( 0,  5)] public float persistence = 0.5f;
            [Range(-5,  5)] public float localZero = 0f;
        
            public Vector3 offset = Vector3.zero;
        }
        
        //> GET A NOISE VALUE FROM A VECTOR3
        public static float GenerateValue(Layer layer, Vector3 vector3)
        {
            float noiseValue = 0f;
            float roughness = layer.baseRoughness;
            float amplitude = 1;
            float weight = 1;

            switch (layer.noiseType)
            {
                //> SIMPLE NOISE FILTER
                case Type.Simple:
                {
                    for (int i = 0; i < layer.octaves; i++)
                    {
                        float value = SimplexNoise.Generate(vector3 * roughness + layer.offset);
                        noiseValue += value * amplitude;
                        roughness *= layer.roughness;
                        amplitude *= layer.persistence;
                    }
                    break;
                }

                //> MOUNTAINOUS NOISE FILTER
                case Type.Rigid:
                {
                    for (int i = 0; i < layer.octaves; i++)
                    {
                        float v = 1 - Mathf.Abs(SimplexNoise.Generate(vector3 * roughness + layer.offset));
                        v *= v;
                        v *= weight;
                        weight = v; 
                        noiseValue += v * amplitude;
                        roughness *= layer.roughness;
                        amplitude *= layer.persistence;
                    }
                    break;
                }
            }
            
            // noiseValue = Mathf.Max(0, noiseValue - layer.localZero);
            return noiseValue * layer.strength;
        }
    }
}
