using RacingSimulation.Runtime.Physics;
using UnityEngine;
using UnityEngine.Serialization;

namespace RacingSimulation.Runtime.Audio
{
    public class EngineAudio : MonoBehaviour
    {
        //Input in ascending order (ex: 1koff, 1kon -> 2koff, 2kon -> ....)
        [SerializeField] private EngineClip[] engineClips;
        [SerializeField] private float rpmInterval; //must be the same between all clips!!!

        public void Init()
        {
            for (int i = 0; i < engineClips.Length; i++)
            {
                AudioSource newSourceOff = gameObject.AddComponent<AudioSource>();
                AudioSource newSourceOn = gameObject.AddComponent<AudioSource>();

                engineClips[i].Init(newSourceOff, newSourceOn, (i + 1) * rpmInterval, 
                    i == engineClips.Length - 1);
            }
        }

        public void UpdateAudio(float rpm, float throttle) //Use LateUpdate for processes that come after input and game logic (i.e audio, visuals, particles, etc.)
        {
            foreach (var engineClip in engineClips)
            {
                //Reset source volume
                engineClip.SetVolume(0f);

                //Set audio pitch based off of its own reference
                engineClip.SetPitchByRpm(rpm);

                //Set the volume of a clip based off the current engine speed and throttle input
                engineClip.SetOnVolume(engineClip.GetVolume(rpm, rpmInterval) * throttle);
                engineClip.SetOffVolume(engineClip.GetVolume(rpm, rpmInterval) * (1.0f - throttle));

                //Keep all sources (even silent ones) playing due to audio clipping issues when trying to pause or stop sources and replay them
                engineClip.KeepPlay();
            }
        }
    }
}