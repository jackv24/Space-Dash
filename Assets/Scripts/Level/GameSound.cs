using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class GameSound : MonoBehaviour
{
    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (OptionsManager.instance.gameMixerGroup)
            source.outputAudioMixerGroup = OptionsManager.instance.gameMixerGroup;
    }
}
