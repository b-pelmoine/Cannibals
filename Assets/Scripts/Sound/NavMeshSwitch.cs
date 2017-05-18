using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshSwitch : MonoBehaviour {
    public string switchName = "";
    [System.Serializable]
    public class SwitchState
    {
        public string name = "";
        public string aera = "";
        public int mask = 0;
    }
    public List<SwitchState> switchStates = new List<SwitchState>();

    int lastMask = 0;

	// Use this for initialization
	void Start () {
        Debug.Log(switchName);
		foreach(SwitchState st in switchStates)
        {
            st.mask = 1 << NavMesh.GetAreaFromName(st.aera);
            Debug.Log(st.name);
        }
	}
	
	// Update is called once per frame
	void Update () {
        NavMeshHit hit;
        if(NavMesh.SamplePosition(transform.position, out hit, 5, ~0))
        {
            if((hit.mask & lastMask) == 0)
            {
                Debug.Log("lol3");
                foreach (SwitchState st in switchStates)
                    if ((hit.mask & st.mask) != 0)
                        AkSoundEngine.SetSwitch(switchName, st.name, gameObject);
            }
            lastMask = hit.mask;
        }
	}
}
