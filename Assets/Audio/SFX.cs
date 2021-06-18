
namespace TCM.Audio
{
    //> CONTAINER FOR ALL IN GAME SOUND EFFECTS
    [System.Serializable] public class SFX
    {
        public string name; 
        public float pitch;
        public float volume;
        
        //- RAW AUDIO FILE
        public UnityEngine.AudioClip audio;
    }
}