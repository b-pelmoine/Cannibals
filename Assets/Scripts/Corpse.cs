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
    BoxCollider normalCollider;

    [SerializeField]
    BoxCollider rightCollider;

    [SerializeField]
    BoxCollider leftCollider;

    [SerializeField]
    CharacterControllerExt characterControllerExt;

    void Awake()
    {
        characterControllerExt.enabled = false;
    }

    public void BeTaken(Cannibal cannibal)
    {
        cannibals.Add(cannibal);
        characterControllerExt.enabled = true;
        characterControllerExt.LinkTo(cannibal.m_cannibalMovement.CharacterControllerEx);

        //MAL FAIT A REFAIRE NIVEAU CONCEPTION
        if (cannibals.Count == 1)
        {
            collision.enabled = false;
            cannibal.m_cannibalSkill.corpseTakenCollider = normalCollider;
            //m_rigidbody.isKinematic = true;
            m_rigidbody.useGravity = false;
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
            m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX;

            normalCollider.enabled = false;
            rightCollider.enabled = true;
            leftCollider.enabled = true;

            cannibal.m_cannibalSkill.corpseJoint.connectedBody = this.m_rigidbody;
            cannibal.m_cannibalSkill.corpseJoint.connectedAnchor = normalCollider.center;
        }
        else if (cannibals.Count > 1)
        {
            cannibal.m_cannibalSkill.corpseTakenCollider = NearCollider(cannibal.m_cannibalAppearence.m_appearenceTransform.position);

            if (cannibal.m_cannibalSkill.corpseTakenCollider == rightCollider)
            {
                cannibals[0].m_cannibalSkill.corpseTakenCollider = leftCollider;
            }
            else
                cannibals[0].m_cannibalSkill.corpseTakenCollider = rightCollider;

            cannibals[0].m_cannibalMovement.CharacterControllerEx.CharacterTransform.position = cannibals[0].m_cannibalSkill.corpseTakenCollider.center + cannibals[0].m_cannibalSkill.corpseTakenCollider.transform.position;
            cannibal.m_cannibalMovement.CharacterControllerEx.CharacterTransform.position = cannibal.m_cannibalSkill.corpseTakenCollider.center + cannibal.m_cannibalSkill.corpseTakenCollider.transform.position;

            cannibals[0].m_cannibalSkill.corpseJoint.connectedBody = this.m_rigidbody;
            cannibals[0].m_cannibalSkill.corpseJoint.connectedAnchor = cannibals[0].m_cannibalSkill.corpseTakenCollider.center;

            cannibal.m_cannibalSkill.corpseJoint.connectedBody = this.m_rigidbody;
            cannibal.m_cannibalSkill.corpseJoint.connectedAnchor = cannibal.m_cannibalSkill.corpseTakenCollider.center;
        }
    }

    public void BeReleased(Cannibal cannibal)
    {
        cannibals.Remove(cannibal);
        characterControllerExt.Unlink(cannibal.m_cannibalMovement.CharacterControllerEx);
        cannibal.m_cannibalSkill.corpseJoint.connectedBody = null;

        if (cannibals.Count == 1)
        {
            cannibals[0].m_cannibalSkill.corpseTakenCollider = normalCollider;
            cannibals[0].m_cannibalMovement.CharacterControllerEx.CharacterTransform.position = cannibals[0].m_cannibalSkill.corpseTakenCollider.center + cannibals[0].m_cannibalSkill.corpseTakenCollider.transform.position;
        }
        else if (cannibals.Count == 0)
        {
            collision.enabled = true;
            // m_rigidbody.isKinematic = false;
            m_rigidbody.constraints = RigidbodyConstraints.None;
            m_rigidbody.useGravity = true;
            normalCollider.enabled = true;
            rightCollider.enabled = false;
            leftCollider.enabled = false;
            characterControllerExt.enabled = false;
        }
    }

    BoxCollider NearCollider(Vector3 position)
    {
        BoxCollider b = null;
        float minDistance = float.MaxValue;

        if ((rightCollider.transform.position + rightCollider.center - m_transform.position).magnitude < minDistance)
        {
            minDistance = (rightCollider.center + m_transform.position).magnitude;
            b = rightCollider;
        }
        
        if ((leftCollider.transform.position + leftCollider.center - m_transform.position).magnitude < minDistance)
            b = leftCollider;

        return b;
    }
}
