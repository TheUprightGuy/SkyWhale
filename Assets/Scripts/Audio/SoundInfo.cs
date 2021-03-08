using UnityEngine;

namespace Audio
{
    public class SoundInfo
    {
        public AudioSource AudioSource;
        public float PitchDefault;
        public float VolumeDefault;
        public void InitialiseSound(string name)
        {
            this.AudioSource = GameObject.Find(name)
                .GetComponent<AudioSource>();
            this.PitchDefault = this.AudioSource.pitch;
            this.VolumeDefault = this.AudioSource.volume;
        }
        
        public void InitialiseSound()
        {
            this.PitchDefault = this.AudioSource.pitch;
            this.VolumeDefault = this.AudioSource.volume;
        }

        public void Reset()
        {
            //Reset altered pitch and volume to defaults
            this.AudioSource.pitch = this.PitchDefault;
            this.AudioSource.volume = this.VolumeDefault;
        }
        
        public void Reset(float newVolume)    //Resets audio source to default before adjusting it by the new volume
        {
            //Reset altered pitch and volume to defaults
            this.AudioSource.pitch = this.PitchDefault;
            this.AudioSource.volume = this.VolumeDefault * newVolume;
        }
    }   
}