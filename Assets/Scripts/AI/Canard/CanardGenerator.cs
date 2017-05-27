using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class CanardGenerator : MonoBehaviour {
        public GameObject canard;
        public Transform feedPosition;
        public Transform deadPosition;
        public Transform createPosition;
        public int numberOfCanard = 4;

        List<Canard> instances = new List<Canard>();

	    // Use this for initialization
	    void Start () {

        }
	
	    // Update is called once per frame
	    void Update () {
            instances.RemoveAll(x => x.transform.parent != transform);
            while (instances.Count < numberOfCanard)
                AddCanard();
	    }

        void AddCanard()
        {
            Canard can = (Instantiate(canard)).GetComponent<Canard>();

            can.transform.parent = transform;
            can.gameObject.GetComponent<NavMeshAgent>().Warp(createPosition.position);
            can.wanderPosition = feedPosition;
            can.deadPosition = deadPosition;
            instances.Add(can);
        }
    }
}
