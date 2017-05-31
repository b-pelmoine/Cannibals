using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomManager : MonoBehaviour {

    [SerializeField]
    public List<Vector3> champPos = new List<Vector3>();

    public float RefreshRate = 10;

    public GameObject mushroom;
    public GameObject mushroomHolder;


    public LayerMask objectLayer;

	// Use this for initialization
	void Start () {
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("Champignon")) {
            champPos.Add(o.transform.position);
        }
        StartCoroutine("RefreshMushroom");
	}

    IEnumerator RefreshMushroom() {
        while (true) {
            foreach (Vector3 v in champPos) {
                if(!Physics.CheckSphere(v,2, objectLayer)){
                    GameObject o = Instantiate(mushroom, v, Quaternion.FromToRotation(Vector3.forward,Vector3.up));
                    o.transform.parent = mushroomHolder.transform;
                    
                    //Pour décaler les Check dans le temps, au cas où
                    yield return new WaitForFixedUpdate();
                }
            }
            yield return new WaitForSeconds(RefreshRate);
        }
    }
}
