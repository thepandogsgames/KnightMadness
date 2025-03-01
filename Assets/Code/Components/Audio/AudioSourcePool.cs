using System.Collections.Generic;
using UnityEngine;

namespace Code.Components.Audio
{
    public class AudioSourcePool
    {
        private readonly Queue<AudioSource> _pool;
        private readonly GameObject _poolParent;
        private int _poolSize;

        public AudioSourcePool(int startPoolSize)
        {
            _pool = new Queue<AudioSource>();
            _poolParent = new GameObject("AudioSourcePool");
            InitializePool(startPoolSize);
        }

        public AudioSource GetAudioSource()
        {
            AudioSource source = _pool.Count > 0 ? _pool.Dequeue() : CreateAudioSource();
            source.gameObject.SetActive(true);
            return source;
        }

        public void ReturnAudioSource(AudioSource source)
        {
            source.Stop();
            source.clip = null;
            source.gameObject.SetActive(false);
            _pool.Enqueue(source);
        }

        private void InitializePool(int startSize)
        {
            for (int i = 0; i < startSize; i++)
            {
                var poolSource = CreateAudioSource();
                _pool.Enqueue(poolSource);
            }
        }

        private AudioSource CreateAudioSource()
        {
            GameObject obj = new GameObject("PooledAudioSource_" + _poolSize);
            _poolSize += 1;
            obj.transform.parent = _poolParent.transform;
            var newAudioSource = obj.AddComponent<AudioSource>();
            newAudioSource.gameObject.SetActive(false);
            return newAudioSource;
        }
        
    }
}
