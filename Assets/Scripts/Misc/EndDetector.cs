using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDetector : MonoBehaviour {
    private List<Cannibal> cannibals;
    private Corpse m_corpse;

    private bool end = false;
    private bool calledOnce = false;

    void Start()
    {
        cannibals = new List<Cannibal>();
        m_corpse = null;
    }

    void Update()
    {
        if(m_corpse)
        {
            if (cannibals.Count == 2) end = true;
        }

        if(end)
        {
            if(!calledOnce)
            {
                GameManager manager = GameObject.FindObjectOfType<GameManager>();
                if(manager)
                    manager.setEndConditionState(true);
                calledOnce = true;
            }
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if(c.name == "GlobalCollider")
        {
            Cannibal can = c.transform.parent.parent.GetComponent<Cannibal>();
            if (!cannibals.Contains(can))
                cannibals.Add(can);
        }
        else
        {
            Corpse corpse = c.GetComponent<Corpse>();
            if (corpse)
            {
                m_corpse = corpse;
            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.name == "GlobalCollider")
        {
            Cannibal can = c.transform.parent.parent.GetComponent<Cannibal>();
            if (cannibals.Contains(can))
                cannibals.Remove(can);
        }
        else
        {
            Corpse corpse = c.GetComponent<Corpse>();
            if (corpse)
            {
                m_corpse = null;
            }
        }
    }
}
