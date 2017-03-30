using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_Skill : MonoBehaviour {


    public Call m_call;

    public CannibalObject m_cannibalObject { get; private set; }

    [SerializeField]
    Transform cannibalObjectParent;

    /// <summary>
    /// Take an cannibal object or exchange if the cannibal has already an object
    /// </summary>
    /// <param name="cannibalObject"></param>
    public void TakeCannibalObject(CannibalObject cannibalObject)
    {
        if (m_cannibalObject)
        {
            CannibalObject.Exchange(m_cannibalObject, cannibalObject);
        }
        else
        {
            cannibalObject.Transform.SetParent(cannibalObjectParent);
        }

        m_cannibalObject = cannibalObject;
    }

}
