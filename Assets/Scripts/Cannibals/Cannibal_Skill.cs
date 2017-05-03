using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_Skill : MonoBehaviour {


    public CannibalObject m_cannibalObject { get; private set; }
    public Corpse m_corpse { get; private set; }

    [SerializeField]
    Cannibal m_cannibal;

    [SerializeField]
    Transform cannibalObjectParent;

    [SerializeField]
    Transform corpseTransform;

    [SerializeField]
    UIManager UIManager;

    public BoxCollider corpseTakenCollider;

    void Update()
    {
        if (m_corpse != null)
        {
            if(m_corpse.cannibals.Count == 1)
            {
                m_corpse.m_transform.position = corpseTransform.position - corpseTakenCollider.center;
            }
        else if (m_cannibal.m_rewiredInput.number == 0)
            {
                m_corpse.m_transform.position = corpseTransform.position - corpseTakenCollider.center;
            }
            else
                m_corpse.m_transform.position = corpseTransform.position - corpseTakenCollider.center;
        }

    }

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

    public void ReleaseCorpse()
    {
        m_corpse.BeReleased(this.m_cannibal);
        m_corpse = null;
    }

    public void TakeCorpse(Corpse corpse)
    {
        corpse.BeTaken(this.m_cannibal);
        m_corpse = corpse;
    }

    public void TriggerHuntSense(string playerID, bool state)
    {
        UIManager.triggerHuntSense(playerID, state);
    }

}
