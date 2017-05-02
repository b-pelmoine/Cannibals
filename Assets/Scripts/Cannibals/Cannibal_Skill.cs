using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_Skill : MonoBehaviour {


    public CannibalObject m_cannibalObject { get; private set; }

    [SerializeField]
    Transform cannibalObjectParent;
    [SerializeField]
    UIManager UIManager;

    /// <summary>
    /// Take an cannibal object or exchange if the cannibal has already an object
    /// </summary>
    /// <param name="cannibalObject"></param>
    public void TakeCannibalObject(CannibalObject cannibalObject)
    {
        if (m_cannibalObject)
        {
            m_cannibalObject.Exchange(cannibalObject);
            m_cannibalObject.Release();
        }


       cannibalObject.Take(cannibalObjectParent);
       m_cannibalObject = cannibalObject;
    }

    public void LooseCannibalObject()
    {
        m_cannibalObject.Transform.SetParent(null);
        m_cannibalObject = null;
    }

    public void TriggerHuntSense(string playerID, bool state)
    {
        UIManager.triggerHuntSense(playerID, state);
    }

}
