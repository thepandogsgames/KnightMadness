using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace Code.Components.Audio
{
    public class AudioController : IAudioController
    {
        private readonly AudioMixer _mixer;
        private readonly AudioMixerGroup _musicGroup;
        private readonly AudioMixerGroup _soundGroup;
        private readonly AudioSourcePool _audioSourcePool;

        public AudioController(AudioMixer mixer)
        {
            _mixer = mixer;
            _audioSourcePool = new AudioSourcePool(10);
            _musicGroup = _mixer.FindMatchingGroups("Music").FirstOrDefault();
            _soundGroup = _mixer.FindMatchingGroups("SFX").FirstOrDefault();
        }

        public AudioSource PlayMusic(AudioClip clip, bool loop)
        {
            AudioSource source = _audioSourcePool.GetAudioSource();
            source.outputAudioMixerGroup = _musicGroup;
            source.clip = clip;
            _ = ShouldBackToPool(source, loop);
            source.Play();
            return source;
        }

        public AudioSource PlaySound(AudioClip clip, bool loop)
        {
            AudioSource source = _audioSourcePool.GetAudioSource();
            source.outputAudioMixerGroup = _soundGroup;
            source.clip = clip;
            _ = ShouldBackToPool(source, loop);
            source.Play();
            return source;
        }

        public void StopAudio(AudioSource source)
        {
            _audioSourcePool.ReturnAudioSource(source);
        }

        public void PlaySoundWithRandomPitch(AudioClip clip, bool loop, float minPitch, float maxPitch)
        {
            AudioSource source = _audioSourcePool.GetAudioSource();
            source.outputAudioMixerGroup = _soundGroup;
            source.clip = clip;
            source.pitch = Random.Range(minPitch, maxPitch);
            _ = ShouldBackToPool(source, loop);
            source.Play();
        }

        public void SetMasterVolume(float volume)
        {
            _mixer.SetFloat("Master", volume);
        }

        public void SetMusicVolume(float volume)
        {
            _mixer.SetFloat("Music", volume);
        }

        public void SetSoundVolume(float volume)
        {
            _mixer.SetFloat("SFX", volume);
        }

        private async Task ShouldBackToPool(AudioSource source, bool shouldBack)
        {
            if (shouldBack)
            {
                source.loop = true;
            }
            else
            {
                source.loop = false;
                await ReturnAfterPlaying(source, source.clip.length);
            }
        }

        private async Task ReturnAfterPlaying(AudioSource source, float delay)
        {
            await Task.Delay((int)((delay + 0.15f) * 1000));
            _audioSourcePool.ReturnAudioSource(source);
        }
    }
}