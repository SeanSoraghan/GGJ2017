using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalController : MonoBehaviour
{
    static GameObject GlobalControllerObject;

    public string[] Levels = /*IntroductionScene*/ { "MoonScene", "JellyfishScene", "FlowerScene", "MothScene", "CityScene"}; /*WinScreen*/
    private int LevelIndex = 0;

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

    private void NextLevel()
    {
        LevelIndex++;
        if (LevelIndex >= Levels.Length)
            LevelIndex = 0;
        if (LevelIndex > -1 && LevelIndex < Levels.Length)
            SceneManager.LoadScene (Levels[LevelIndex]);
    }

    public void CurrentLevelCompleted()
    {
        PlayWinSound();
        LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        StartCoroutine (LoadNextLevelAfterDelay());
    }
	// Update is called once per frame
	IEnumerator LoadNextLevelAfterDelay()
	{
		yield return new WaitForSeconds (2);
	    NextLevel ();
		yield return null;
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
