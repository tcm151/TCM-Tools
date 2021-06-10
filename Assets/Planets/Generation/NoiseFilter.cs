using UnityEngine;


namespace TCM.Planets
{
    public class NoiseFilter
    {
        //> NOISE TYPES
        public enum Type { Simple, Rigid };
        
        //> NOISE LAYER SETTINGS
        [System.Serializable] public class Settings
        {
            public bool enabled = true;
            public bool useMask = true;
        
            public Type noiseType;
        
            [Range( 1,  8)] public int octaves = 1;
            [Range( 0,  5)] public float strength = 1f;
            [Range( 0, 10)] public float baseRoughness = 1f;
            [Range( 0, 15)] public float roughness = 2f;
            [Range( 0,  5)] public float persistence = 0.5f;
            [Range(-5,  5)] public float minValue;
        
            public Vector3 center = Vector3.zero;
        }
        
        private readonly SimplexNoise generator = new SimplexNoise();

        //> GET A NOISE VALUE FROM A VECTOR3
        public float GetValue(Settings settings, Vector3 point)
        {
            float noiseValue = 0f;
            float roughness = settings.baseRoughness;
            float amplitude = 1;
            float weight = 1;

            switch (settings.noiseType)
            {
                case Type.Simple:
                {
                    for (int i = 0; i < settings.octaves; i++)
                    {
                        float value = generator.Generate(point * roughness + settings.center);
                        noiseValue += (value + 1) * 0.5f * amplitude;
                        roughness *= settings.roughness;
                        amplitude *= settings.persistence;
                    }
                    break;
                }

                case Type.Rigid:
                {
                    for (int i = 0; i < settings.octaves; i++)
                    {
                        float v = 1 - Mathf.Abs(generator.Generate(point * roughness + settings.center));
                        v *= v;
                        v *= weight;
                        weight = v; 
                        noiseValue += v * amplitude;
                        roughness *= settings.roughness;
                        amplitude *= settings.persistence;
                    }
                    break;
                }

                default:
                {
                    Debug.Log("This is an ERROR! Improper Noise type.");
                    break;
                }
            }
            
            noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
            return noiseValue * settings.strength;
        }
    }
}
