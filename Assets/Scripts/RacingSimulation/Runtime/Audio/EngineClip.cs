using UnityEngine;

namespace RacingSimulation.Runtime.Audio
{
    [System.Serializable]
    public class EngineClip
    {
        [SerializeField] private AudioClip clipOff;
        [SerializeField] private AudioClip clipOn;
        
        //RPM at which the clip is at 100% volume (NOTE: Spacing between audio clips MUST be the same (i.e if the first clip has refRPM of 1000 and the second is 1500, the third must be 2000, and so on in 500 RPM increments))
        private float _refRpm;
        private AudioSource _sourceOff;
        private AudioSource _sourceOn;
        
        //Tick to prevent audio clip's volume from falling off
        private bool _noFallingEdge;

        public void Init(AudioSource sourceOff, AudioSource sourceOn, float rpm, bool noFallingEdge)
        {
            //Setters
            _refRpm = rpm;
            _noFallingEdge = noFallingEdge;
            _sourceOff = sourceOff;
            _sourceOn = sourceOn;

            //Set sources' clips to the audio clips
            _sourceOff.clip = clipOff;
            _sourceOn.clip = clipOn;

            //Set the sources to loop
            _sourceOff.loop = true;
            _sourceOn.loop = true;

            //Set sources to play on startup
            _sourceOff.Play();
            _sourceOn.Play();

            //Set volume to 0 on startup
            _sourceOff.volume = 0.0f;
            _sourceOn.volume = 0.0f;

            //-----Add more init. params. for 3D audio settings here-----//
        }

        public void KeepPlay()
        {
            if (!_sourceOn.isPlaying)
                _sourceOn.Play();
            
            if (!_sourceOff.isPlaying)
                _sourceOff.Play();
        }

        public void SetPitchByRpm(float rpm)
        {
            float pitch = rpm / _refRpm;
            
            _sourceOn.pitch = pitch;
            _sourceOff.pitch = pitch;
        }
        
        public void SetOnVolume(float volume)
        {
            _sourceOn.volume = volume;
        }

        public void SetOffVolume(float volume)
        {
            _sourceOff.volume = volume;
        }

        public void SetVolume(float volume)
        {
            SetOnVolume(volume);
            SetOffVolume(volume);
        }

        public float GetVolume(float rpm, float range)
        {
            float min = _refRpm - range;
            float max = _refRpm + range;
            
            //Set to zero if outside range
            if ((rpm < min || rpm > max) && !_noFallingEdge)
            {
                return 0.0f;
            }
            
            //Rising Edge
            if (rpm <= _refRpm)
            {
                return Mathf.Max((rpm - min) / (_refRpm - min), 0.0f);
            }
            
            //Falling Edge
            if (!_noFallingEdge)
            {
                return Mathf.Max((max - rpm) / (max - _refRpm), 0.0f);
            }
            else
            { 
                return 1.0f;
            }
        }
    }
}