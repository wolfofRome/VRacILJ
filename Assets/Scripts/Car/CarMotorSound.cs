using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMotorSound : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip explosionSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.PlayOneShot(explosionSound);
    }
}
