using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour {
	public Transform fishTarget;
	public Transform otherTarget;
	private Transform perceivedTarget;
	// Use this for initialization
	void Start () {
		perceivedTarget = fishTarget;

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 dir = (perceivedTarget.position - transform.position);
		float angle = (Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg)-90;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.position += transform.up * Time.deltaTime;

		if (Input.anyKeyDown)
		{
			perceivedTarget = otherTarget;
		}
	}
}
