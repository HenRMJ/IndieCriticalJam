using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Audio[] sounds;

    private Dictionary<SoundType, AudioSource> _audioSourceDictionary = new Dictionary<SoundType, AudioSource>();

    private void Awake()
    {
        foreach (Audio sound in sounds)
        {
            if (_audioSourceDictionary.ContainsKey(sound.soundType))
            {
                Debug.LogError($"You have a duplicate {sound.soundType} in your sounds array");
                return;
            }

            AudioSource aS = gameObject.AddComponent<AudioSource>();
            aS.hideFlags = HideFlags.HideInInspector;
            aS.clip = sound.sound;
            aS.volume = sound.volume;
            
            _audioSourceDictionary.Add(sound.soundType, aS);
        }
    }

    public void PlaySound(SoundType type)
    {
        if (!_audioSourceDictionary.ContainsKey(type))
        {
            Debug.LogError("This Sound type is missing in your sounds array");
            return;
        }

        AudioSource audioSource = _audioSourceDictionary[type];

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.Play();
    }
}

[Serializable]
public class Audio
{
    public SoundType soundType;
    [Range(0f, 1f)]
    public float volume;
    public AudioClip sound;
}

[Serializable]
public enum SoundType
{
    TextRise,
    Swoosh,
    TextFall,
    Type
}