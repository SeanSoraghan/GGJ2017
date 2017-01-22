using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBoat : MonoBehaviour {

    public float moveSpeed = 0.5f;
    public float stopPosition = 1.5f;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        if (gameObject.transform.localPosition.x > stopPosition)
        {
            gameObject.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }

        else
        {
            if (GameObject.Find("Cloud 1").transform.position.x > 5)
            {
                gameObject.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            }
        }

    }
}
