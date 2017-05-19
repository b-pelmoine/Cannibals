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

    [SerializeField]
    float speedToRecoverCorpsePoint = 10f;

    public Transform CannibalObjectParent
    {
        get { return cannibalObjectParent; }
    }

    [SerializeField]
    float maxDistanceToCorpsePoint = 1.5f;

    public Vector3 pointOnCorpse;


    private void LateUpdate()
    {
        if (m_corpse != null && Vector3.Distance(cannibalObjectParent.position, m_corpse.m_transform.TransformPoint( m_cannibal.m_cannibalSkill.pointOnCorpse)) > maxDistanceToCorpsePoint)
        {
                m_cannibal.m_cannibalMovement.CharacterControllerEx.CharacterTransform.position = m_cannibal.m_cannibalMovement.CharacterControllerEx.CharacterTransform.position - (cannibalObjectParent.position -
                                                                                                  Vector3.Lerp(cannibalObjectParent.position,
                                                                                                             m_corpse.m_transform.TransformPoint(m_cannibal.m_cannibalSkill.pointOnCorpse),
                                                                                                             speedToRecoverCorpsePoint * Time.deltaTime) );
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

        if (cannibalObject.linkedCannibal != null && cannibalObject.linkedCannibal != this.m_cannibal)
            cannibalObject.linkedCannibal.m_cannibalSkill.LooseCannibalObject();

       cannibalObject.Take(this.m_cannibal, cannibalObjectParent);
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
