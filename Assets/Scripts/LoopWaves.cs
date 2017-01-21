using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopWaves : MonoBehaviour {

    public Sprite sprite;
    public int numberOfClones = 5;
    public float speed = 5;

	// Use this for initialization
	void Start () {



        sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        CloneWaves();
	}


    void CloneWaves()
    {

        for(int i = 1; i < numberOfClones; i++)
        {
            GameObject go = new GameObject();
            go.AddComponent<SpriteRenderer>();
            go.GetComponent<SpriteRenderer>().sprite = sprite;
            go.transform.position = gameObject.transform.position;
            go.transform.rotation = gameObject.transform.rotation;

            float pos = sprite.rect.width / 100;
            pos = pos *= i;
            go.transform.Translate(-pos, 0, 0);

            go.AddComponent<MoveWaves>();
            go.GetComponent<MoveWaves>().speed = this.speed;
            go.GetComponent<MoveWaves>().numberOfClones = this.numberOfClones;
        }
    }

	// Update is called once per frame
	void Update () {

        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (gameObject.transform.position.x > 30)
        {
            
            gameObject.transform.Translate(-(numberOfClones * (sprite.rect.width/ 100)), 0, 0);
        }

    }
}
