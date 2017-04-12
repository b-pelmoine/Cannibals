using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.StateMachines;


public class Bush : MonoBehaviour {

    [SerializeField]
    FSMOwner m_statemachine;

    [SerializeField]
    LayerMask m_cannibalLayerMask;

    List<Cannibal> m_cannibalInBush;

    void Awake()
    {
        m_cannibalInBush = new List<Cannibal>();
    }

    /// <summary>
    /// Will move the bush for detecting that something is in it.
    /// </summary>
    public bool Move()
    {
        return ((Bush_State)m_statemachine.behaviour.currentState).Move();
    }

    public bool IsMoving()
    {
        return ((Bush_State)m_statemachine.behaviour.currentState).IsMoving();
    }

    /// <summary>
    /// Will kill a cannibal that can be killed
    /// </summary>
    /// <returns>false if no cannibal killed</returns>
    public bool KillACannibal()
    {
        foreach(Cannibal c in m_cannibalInBush)
        {
            if (c.Kill())
                return true;
        }

        return false;
    }

    void OnTriggerEnter(Collider c)
    {
        if (((1 << c.gameObject.layer) & m_cannibalLayerMask) != 0)
            m_cannibalInBush.Add(c.gameObject.GetComponentInParent<Cannibal>());
    }

}
