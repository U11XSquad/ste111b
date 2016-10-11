using UnityEngine;
using System.Collections;

public class SoundEffect : MonoBehaviour
{

    static SoundEffect instance;
    static public SoundEffect Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<SoundEffect>();
                if (!instance)
                {
                    throw new UnityException("缺乏SoundEffect");
                }
            }
            return instance;
        }
    }

    static public AudioSource Source
    {
        get
        {
            return Instance.GetComponent<AudioSource>();
        }
    }

    static public void Play(AudioClip clip)
    {
        Source.clip = clip;
        Source.Play();
    }
}
