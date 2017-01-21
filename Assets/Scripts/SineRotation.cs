using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineRotation : MonoBehaviour {
	public float frequency;
	public float magnitude;
	private Quaternion rot;
	private Vector3 axis;
	// Use this for initialization
	void Start () {
		rot = transform.rotation;
		axis = transform.forward;
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Euler (rot.eulerAngles + axis * Mathf.Sin (Time.time*frequency) * magnitude);
	}
}
