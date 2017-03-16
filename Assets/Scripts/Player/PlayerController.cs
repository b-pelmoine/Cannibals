using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PlayerController : MonoBehaviour {

    [System.Serializable]
    public enum PlayerID
    {
        ALL,
        PLAYER_ONE,
        PLAYER_TWO
    }

    [SerializeField]
    private PlayerID id;
    [SerializeField]
    private Transform otherPlayer;

    private Animator anim;
    private Rigidbody rb;

    public float jumpForce = 0.8f; 
    public float moveSpeed = 10f;

    [SerializeField]
    [Range(15, 35)]
    private float maxDistanceBetweenPlayers;

    private Vector3 movePosition;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
	
	void Update () {

        //update players inputs
        switch(id)
        {
            case PlayerID.PLAYER_ONE:
                {
                    movePosition = new Vector3(Input.GetAxis("Horizontal_P1"), 0, Input.GetAxis("Vertical_P1"));
                }
                break;
            case PlayerID.PLAYER_TWO:
                {
                    movePosition = new Vector3(Input.GetAxis("Horizontal_P2"), 0, Input.GetAxis("Vertical_P2"));
                }
                break;
        }
    }

    void checkMovePosition()
    {
        Vector3 targetPoint = getMovePosition();
        if(movePosition != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
        float newDistance = Vector3.Distance(targetPoint, otherPlayer.position);
        float distance = Vector3.Distance(transform.position, otherPlayer.position);
        if (newDistance > distance && newDistance > maxDistanceBetweenPlayers)
            movePosition = Vector3.zero;
    }

    Vector3 getMovePosition()
    {
        return transform.position + movePosition * moveSpeed * Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        Gizmos.color = Color.yellow;
    }

    void FixedUpdate()
    {
        //prevent player from going too far from the over one
        checkMovePosition();

        if (movePosition != Vector3.zero)
            rb.MovePosition(getMovePosition());

        if (rb.velocity.sqrMagnitude == 0)
            transform.hasChanged = false;
        else
            transform.hasChanged = true;
    }
}
