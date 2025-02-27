using UnityEngine;

namespace Code.Components.Audio
{
    public interface IAudioController
    {
        AudioSource PlayMusic(AudioClip clip, bool loop);
        AudioSource PlaySound(AudioClip clip, bool loop);
        
        void StopAudio(AudioSource source);
        void PlaySoundWithRandomPitch(AudioClip clip, bool loop, float minPitch, float maxPitch);
        void SetMasterVolume(float volume);
        void SetMusicVolume(float volume);
        void SetSoundVolume(float volume);
    }
}