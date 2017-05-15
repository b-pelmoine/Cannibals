using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Cannibal_Skill : MonoBehaviour {

    public static Action<Cannibal> OnStartUseHunterSense;
    public static Action<Cannibal> OnStopUseHunterSense;

    public CannibalObject m_cannibalObject { get; private set; }
    public Corpse m_corpse { get; private set; }

    [SerializeField]
    Cannibal m_cannibal;

    [SerializeField]
    Transform cannibalObjectParent;

    public Joint corpseJoint;
    public Vector3 pointOnCorpse;

    private void LateUpdate()
    {
        if (m_corpse != null && Vector3.Distance(corpseJoint.transform.position, m_cannibal.m_cannibalSkill.pointOnCorpse + m_corpse.m_transform.position) > 2f)
        {
                m_cannibal.m_cannibalMovement.CharacterControllerEx.CharacterTransform.position = m_cannibal.m_cannibalMovement.CharacterControllerEx.CharacterTransform.position - (corpseJoint.transform.position -
                                                                                                  Vector3.Lerp(corpseJoint.transform.position,
                                                                                                            m_cannibal.m_cannibalSkill.pointOnCorpse + m_corpse.m_transform.position,
                                                                                                             0.1f) );
        }

        if(m_corpse != null && m_corpse.cannibals.Count == 1)
        {
            m_corpse.m_transform.rotation =Quaternion.Slerp(m_corpse.m_transform.rotation, m_cannibal.m_cannibalAppearence.m_appearenceTransform.rotation * Quaternion.Euler(0, 90, 0),0.1f);
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

        foreach(Collider c in m_corpse.GetComponentsInChildren<Collider>())
            m_cannibal.m_cannibalMovement.CharacterControllerEx.ignoredColliders3D.Remove(c);


        m_corpse = null;
    }

    public void TakeCorpse(Corpse corpse)
    {
        corpse.BeTaken(this.m_cannibal);
        m_corpse = corpse;

        m_cannibal.m_cannibalMovement.CharacterControllerEx.ignoredColliders3D.AddRange(corpse.GetComponentsInChildren<Collider>());
    }
}
