using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalController : MonoBehaviour
{
    static GameObject GlobalControllerObject;

    void Awake()
    {
        DontDestroyOnLoad (transform.gameObject);
    }

	// Use this for initialization
	void Start ()
    {}
	
    public void PlayWinSound()
    {
        AudioSource source = GetComponent<AudioSource>();
        if (source != null && !source.isPlaying)
            source.Play();
    }

	// Update is called once per frame
	void Update ()
    {
		
	}

    public static GlobalController GetGlobalController()
    {
        if (GlobalControllerObject == null)
        { 
            GlobalControllerObject = new GameObject();
            GlobalControllerObject.AddComponent<GlobalController>();
            GlobalControllerObject.AddComponent<AudioSource>();
            GlobalControllerObject.GetComponent<AudioSource>().clip = Resources.Load<AudioClip> ("WinningMusic");
            GlobalControllerObject.GetComponent<AudioSource>().volume = 0.9f;
        }

        if (GlobalControllerObject != null)
        {
            GlobalController controller = GlobalControllerObject.GetComponent<GlobalController>();
            if (controller != null)
                return controller;
        }
        return null;
    }
}
