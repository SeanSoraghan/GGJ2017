using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameDetector : MonoBehaviour {
	SquishScale wobbler; //dedicated to Robin Baumgarten <3
	// Use this for initialization
	void Start () {
		wobbler = GetComponent <SquishScale> ();
	}

	void Update()
	{
		if (Input.GetKeyDown ("a"))
		{
			StartCoroutine (WobbleFlame ());
		}
	}

	public void TriggerWobbleFlame()
	{
		StartCoroutine (WobbleFlame ());
	}

	IEnumerator WobbleFlame()
	{
		wobbler.amount = 0.6f;
		yield return new WaitForSeconds (0.2f);
		wobbler.amount = 0.1f;
		yield return null;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log ("AAAAARGH!"); 
	}
}
