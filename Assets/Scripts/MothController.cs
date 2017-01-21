using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothController : MonoBehaviour {

	private SineMovement mothToLight;
	private LinearMovement mothRunAway;
	private SpriteRenderer mothSprite;

	private int attemptCounter;


	// Use this for initialization
	void Start () {
		mothToLight = GetComponent <SineMovement> ();
		mothRunAway = GetComponent <LinearMovement> ();
		mothSprite = GetComponent <SpriteRenderer> ();
		attemptCounter = 0;
		GoToLight ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown)
		{
			RunAway ();
		}
	}



	void RunAway()
	{		
		mothToLight.enabled = false;
		mothRunAway.enabled = true;
		mothSprite.flipY = true;
		if (attemptCounter<=3)
		{
			StartCoroutine (WaitAndRestart());
		}
		else
		{
			Debug.Log ("YAAAAY!");
		}
	}

	void GoToLight()
	{
		attemptCounter++;
		mothToLight.enabled = true;
		mothRunAway.enabled = false;
		mothSprite.flipY = false;
	}

	IEnumerator WaitAndRestart()
	{
		yield return new WaitForSeconds (4);
		GoToLight ();
		yield return null;
	}
}
