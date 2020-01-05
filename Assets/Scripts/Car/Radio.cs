using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Radio : MonoBehaviour
{
    public AudioSource audioSource;
    private List<AudioClip> m_Tracks;
    void Awake()
    {
        m_Tracks = new List<AudioClip>();
        string dir = (Directory.GetCurrentDirectory()+"\\Assets\\Audio\\radio");
        string[] paths = Directory.GetFiles(dir);
        foreach(string path in paths) {
            Debug.Log(path);
            m_Tracks.Add((AudioClip)Resources.Load(path));
        }
        audioSource.PlayOneShot(m_Tracks[0]);
        
    }

    void Update()
    {
        
    }
}
