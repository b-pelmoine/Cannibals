using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Chasseur : AIAgent {
        public float range = 30;
        public bool sawBottle = false;
        public Vector3 bottle;


	    // Use this for initialization
	    void Start () {
            Bottle.OnBottleShaked += OnBottleShaked;
	    }
	
	    // Update is called once per frame
	    void Update () {
		
	    }

        void OnBottleShaked(Bottle bot)
        {
            if(Vector3.SqrMagnitude(bot.transform.position-this.transform.position)< range*range && !sawBottle)
            {
                bottle = bot.transform.position;
                sawBottle = true;
            }
        }

        
    }
}
