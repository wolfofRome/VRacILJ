using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Radio : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioClip> m_Tracks;
    private bool m_radioOn = true;
    private float m_oldf3 = 0;
    
    void Awake()
    {
    }

    void Update()
    {
        float f3 = Input.GetAxis("Fire3");
        if(m_oldf3 != f3)
        {
            if (f3 == 1) { m_radioOn = !m_radioOn; }
            m_oldf3 = f3;
        }
        if (!audioSource.isPlaying && m_radioOn)
        {
            PlayRandomTrack();
        }
        else if(m_radioOn)
        {
            audioSource.Stop();
        }
    }

    void PlayRandomTrack()
    {
        int randId = Random.Range(0, m_Tracks.Count);
        audioSource.clip = m_Tracks[randId];
        audioSource.Play();
    }
}
