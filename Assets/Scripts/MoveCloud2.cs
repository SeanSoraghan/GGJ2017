using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCloud2 : MonoBehaviour {

    public float moveSpeed = 0.5f;
    public float returnSpeed = 0.25f;
    private float originalPosition;
    public bool CloudShouldMove = false;
	// Use this for initialization
	void Start () {
        originalPosition = gameObject.transform.localPosition.x;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.P) || CloudShouldMove)
        {
            gameObject.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
        else
        {
            if (gameObject.transform.localPosition.x > originalPosition)
            {
                gameObject.transform.Translate(Vector3.right * returnSpeed * Time.deltaTime);
            }
        }

	}
}
