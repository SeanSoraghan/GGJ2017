using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeMovement : MonoBehaviour {
	public float MoveSpeed;

	public float initialFrequency;  // Speed of sine movement
	public float initialMagnitude;   // Size of sine movement
	public bool frequencyNoise;
	public bool magnitudeNoise;
	public Transform target;

	private Vector3 pos;
	private Vector3 axis;
	private float frequency;
	private float magnitude;

	// Use this for initialization
	void Awake ()
	{

	}


	void OnEnable () {
		frequency = initialFrequency;
		magnitude = initialMagnitude;
		pos = target.position;
		axis = transform.up;
	}

	// Update is called once per frame
	void Update () {
		pos -= transform.right * Time.deltaTime * MoveSpeed;
		if (frequencyNoise)
		{
			frequency = initialFrequency * Mathf.PerlinNoise (Time.time, 0);
		}

		if (magnitudeNoise)
		{
			magnitude = initialMagnitude * Mathf.PerlinNoise (Time.time, 0);	
		}

		transform.position = pos + axis * (Mathf.Sin(Time.time*4) + Mathf.Sin (frequency) * magnitude);
		//Debug.Log (frequency); 
	}
}
