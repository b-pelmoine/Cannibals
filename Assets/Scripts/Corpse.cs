using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EquilibreGames;

public class Corpse : MonoBehaviour {

    public List<Cannibal> cannibals = new List<Cannibal>();

    public Transform m_transform;

    public Rigidbody m_rigidbody;

    [SerializeField]
    BoxCollider collision;

    [SerializeField]
    Vector3 centerPoint = Vector3.zero;

    [SerializeField]
    Vector3 rightPoint;

    [SerializeField]
    Vector3 leftPoint;

    [SerializeField]
    CharacterControllerExt characterControllerExt;

    [SerializeField]
    Vector3 cannibalTakeOffset = new Vector3(0, -1.5f, 0);

    [SerializeField]
    Vector3 orientationOffset = new Vector3(0, 90, 0);

    void Awake()
    {
        characterControllerExt.enabled = false;
        m_rigidbody.isKinematic = true;
        StartCoroutine(HackPhysic());
    }

    IEnumerator HackPhysic()
    {
        yield return new WaitForFixedUpdate();
        m_rigidbody.isKinematic = false;
    }


    void LateUpdate()
    {
        if (cannibals.Count >= 2)
        {

            m_transform.position = (cannibals[0].m_cannibalSkill.CannibalObjectParent.position + cannibals[1].m_cannibalSkill.CannibalObjectParent.position)*0.5f;
            m_transform.rotation = Quaternion.LookRotation(cannibals[0].m_cannibalSkill.CannibalObjectParent.position - cannibals[1].m_cannibalSkill.CannibalObjectParent.position) *  Quaternion.Euler(orientationOffset);

            CharacterControllerExt c1 = cannibals[0].m_cannibalMovement.CharacterControllerEx;
            CharacterControllerExt c2 = cannibals[1].m_cannibalMovement.CharacterControllerEx;
            Vector3 c1VelocityMemory = c1.velocity;

            c1.velocity += ExtendedMath.ProjectVectorOnLine((c1.CharacterTransform.position - c2.CharacterTransform.position).normalized, c2.velocity);
            c2.velocity += ExtendedMath.ProjectVectorOnLine((c1.CharacterTransform.position - c2.CharacterTransform.position).normalized, c1VelocityMemory);
            characterControllerExt.velocity = (c1.velocity + c2.velocity) * 0.5f;
        }
        else if ((cannibals.Count >= 1))
        {
            m_transform.position = cannibals[0].m_cannibalSkill.CannibalObjectParent.position;
            characterControllerExt.velocity = cannibals[0].m_cannibalMovement.CharacterControllerEx.velocity;
            m_transform.rotation = Quaternion.Slerp(m_transform.rotation, cannibals[0].m_cannibalAppearence.m_appearenceTransform.rotation * Quaternion.Euler(0, 90, 0), 20f*Time.deltaTime);
        }
    }


    public void BeTaken(Cannibal cannibal)
    {
        cannibals.Add(cannibal);
        characterControllerExt.enabled = true;
        characterControllerExt.LinkTo(cannibal.m_cannibalMovement.CharacterControllerEx);

        m_rigidbody.isKinematic = true;
        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.angularVelocity = Vector3.zero;

        //MAL FAIT A REFAIRE NIVEAU CONCEPTION
        if (cannibals.Count == 1)
        {
            collision.enabled = false;
            cannibal.m_cannibalSkill.pointOnCorpse = centerPoint;
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
            m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
        }
        else if (cannibals.Count > 1)
        {
            cannibal.m_cannibalSkill.pointOnCorpse = NearPoint(cannibal.m_cannibalAppearence.m_appearenceTransform.position);

            if (cannibal.m_cannibalSkill.pointOnCorpse == rightPoint)
            {
                cannibals[0].m_cannibalSkill.pointOnCorpse = leftPoint;
            }
            else
                cannibals[0].m_cannibalSkill.pointOnCorpse = rightPoint;
    
                cannibals[0].m_cannibalMovement.CharacterControllerEx.CharacterTransform.position = m_transform.TransformPoint(cannibals[0].m_cannibalSkill.pointOnCorpse) + cannibalTakeOffset;       
                cannibal.m_cannibalMovement.CharacterControllerEx.CharacterTransform.position = m_transform.TransformPoint(cannibal.m_cannibalSkill.pointOnCorpse) + cannibalTakeOffset;
         }
    }

    public void BeReleased(Cannibal cannibal)
    {
        cannibals.Remove(cannibal);
        characterControllerExt.Unlink(cannibal.m_cannibalMovement.CharacterControllerEx);

        if (cannibals.Count == 1)
        {
            cannibals[0].m_cannibalSkill.pointOnCorpse = centerPoint;
            cannibals[0].m_cannibalMovement.CharacterControllerEx.CharacterTransform.position = m_transform.TransformPoint(centerPoint) + cannibalTakeOffset;
        }
        else if (cannibals.Count == 0)
        {
            collision.enabled = true;
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
            m_rigidbody.constraints = RigidbodyConstraints.None;
            characterControllerExt.enabled = false;
            m_rigidbody.isKinematic = false;
        }
    }

    Vector3 NearPoint(Vector3 position)
    {
        Vector3 b = Vector3.zero;
        float minDistance = float.MaxValue;

        if ((m_transform.TransformPoint(rightPoint) - position).magnitude < minDistance)
        {
            minDistance = (m_transform.TransformPoint(rightPoint) - position).magnitude;
            b = rightPoint;
        }

        if ((m_transform.TransformPoint(leftPoint) - position).magnitude < minDistance)
            b = leftPoint;

        return b;
    }


#if UNITY_EDITOR || EQUILIBRE_GAMES_DEBUG
    /// <summary>
    /// Draw the debug function of Your circle collider character controller
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_transform.TransformPoint(rightPoint), 0.1f);
        Gizmos.DrawWireSphere(m_transform.TransformPoint(leftPoint), 0.1f);
    }
#endif
}
