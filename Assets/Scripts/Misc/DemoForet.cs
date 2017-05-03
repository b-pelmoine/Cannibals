using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoForet : MonoBehaviour {

    [SerializeField]
    Cannibal[] cannibals;

    Dictionary<string, bool> cannibalsInTheEnd;
    [Space]
    [SerializeField]
    GameObject target;
    bool targetReachedTheExit;

	void Start () {
        targetReachedTheExit = false;
        cannibalsInTheEnd = new Dictionary<string, bool>();
        //init both cannibals
        for (int i=0; i<2; ++i)
        {
            cannibalsInTheEnd[cannibals[i].GetComponent<RewiredInput>().playerInputID] = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
		if(!BothCannibalsAreDead())
        {
            if(targetReachedTheExit)
            {
                Debug.Log("gg wp");
            }
        }
        else
        {
            Debug.Log("so bad");
        }
        //magic button
        if (Input.GetKeyDown(KeyCode.R)) UnityEngine.SceneManagement.SceneManager.LoadScene("Integration");
    }

    bool BothCannibalsAreDead()
    {
        return cannibals[0].IsDead() && cannibals[1].IsDead();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.Equals(target))
        {
            targetReachedTheExit = true;
        }
        RewiredInput cIN = other.GetComponent<RewiredInput>();
        if (cIN != null) cannibalsInTheEnd[cIN.playerInputID] = true;
    }

    void OnTriggerExit(Collider other)
    {
        RewiredInput cIN = other.GetComponent<RewiredInput>();
        if (cIN != null) cannibalsInTheEnd[cIN.playerInputID] = false;
    }
}
