using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderKeeper : MonoBehaviour {

    [System.NonSerialized]
    public List<GameObject> gameObjects;


    void Awake()
    {
        gameObjects = new List<GameObject>();
    }

    void OnTriggerEnter(Collider c)
    {
        gameObjects.Add(c.gameObject);
    }

    void OnTriggerExit(Collider c)
    {
        gameObjects.Remove(c.gameObject);
    }

    void OnColliderEnter(Collision c)
    {
        gameObjects.Add(c.gameObject);
    }

    void OnColliderExit(Collision c)
    {
        gameObjects.Remove(c.gameObject);
    }

}
