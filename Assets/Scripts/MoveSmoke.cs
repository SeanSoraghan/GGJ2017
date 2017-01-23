using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSmoke : MonoBehaviour
{

        public float speed;
        public float spriteHeight;
        private float screenWidth;
        private float trainLength;
        public float numberOfClones;


        // Use this for initialization
        void Start()
        {

            screenWidth = Screen.width;
            spriteHeight = gameObject.GetComponent<SpriteRenderer>().sprite.rect.height;
            trainLength = (numberOfClones - 1) * (spriteHeight / 100);



        }

        // Update is called once per frame
        void Update()
        {

            transform.Translate(Vector3.up * speed * Time.deltaTime);

            if (gameObject.transform.position.y > 30)
            {

                gameObject.transform.Translate(0, -trainLength, 0);
            }

        }
    }


