using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSmoke : MonoBehaviour
{

        public float speed;
        public float spriteWidth;
        private float screenWidth;
        private float trainLength;
        public float numberOfClones;


        // Use this for initialization
        void Start()
        {

            screenWidth = Screen.width;
            spriteWidth = gameObject.GetComponent<SpriteRenderer>().sprite.rect.width;
            trainLength = numberOfClones * (spriteWidth / 100);



        }

        // Update is called once per frame
        void Update()
        {

            transform.Translate(Vector3.right * speed * Time.deltaTime);

            if (gameObject.transform.position.y > 30)
            {

                gameObject.transform.Translate(-trainLength, 0, 0);
            }

        }
    }


