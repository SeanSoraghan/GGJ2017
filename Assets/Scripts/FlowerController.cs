using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerController : MonoBehaviour {
	public Transform[] targetAngles;
	public Transform[] petals;
	public Quaternion[] petalsInitialRotations;
	public BeeMovement bee;

	public float speedOpening=0.4f;
	public float speedClosing=1;

	float interpolator;
	// Use this for initialization
	void Start () {
		interpolator = 0;
		petalsInitialRotations = new Quaternion [petals.Length];
		for (int i=0; i<petals.Length; i++)
		{
			petalsInitialRotations [i] = petals [i].rotation;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		Debug.Log (interpolator);

		if (interpolator<=1)
		{
			if (Input.GetKey ("a"))
				interpolator += Time.deltaTime*speedOpening;
			else
			{
				interpolator = Mathf.Clamp (interpolator - Time.deltaTime*speedClosing, 0, 1);
			}

			for (int i=0; i<petals.Length; i++)
			{
				petals [i].rotation = Quaternion.Slerp (petalsInitialRotations[i], targetAngles [i].rotation, interpolator);
			}
		}

		else
		{
			if (bee.enabled==false)
			{
				bee.enabled = true;
			}
		}

	}
}
