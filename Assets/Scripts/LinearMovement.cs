using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMovement : MonoBehaviour {
	public Transform target;
	public float speed;

	private float startTime;
	private float journeyLength;
	// Use this for initialization
	void OnEnable () {
		startTime = Time.time;
		journeyLength = Vector3.Distance(transform.position, target.position);
	}
	
	// Update is called once per frame
	void Update () {
		float distCovered = (Time.time - startTime) * speed;
		float fracJourney = distCovered / journeyLength;
		transform.position = Vector3.Lerp(transform.position, target.position, fracJourney);
	}
}
