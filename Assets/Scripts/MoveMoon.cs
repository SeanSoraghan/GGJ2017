using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMoon : MonoBehaviour {

    public bool MoonShouldMove = false;
    public float moveSpeed = 0.5f;
    public float returnSpeed = 0.25f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.P) || MoonShouldMove)
        {
            gameObject.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
        else
        {
            if (gameObject.transform.localPosition.x < 0.01)
            {
                gameObject.transform.Translate(Vector3.right * returnSpeed * Time.deltaTime);
            }
        }

	}
}
