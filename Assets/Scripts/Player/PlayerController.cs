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

    private Animator anim;
    private Rigidbody rb;

    public float jumpForce = 0.8f; 
    public float moveSpeed = 10f;

    private Vector3 movePosition;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

	void Start () {
		
	}
	
	void Update () {

        //update players inputs
        switch(id)
        {
            case PlayerID.PLAYER_ONE:
                {
                    movePosition = new Vector3(Input.GetAxis("Horizontal"), rb.velocity.y, Input.GetAxis("Vertical"));
                }
                break;
            case PlayerID.PLAYER_TWO:
                {
                    movePosition = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                }
                break;
        }
        if (Input.GetButtonDown("Jump"))
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        if (movePosition != Vector3.zero && id != PlayerID.PLAYER_ONE)
            rb.MovePosition(transform.position + movePosition * moveSpeed * Time.deltaTime);

        if (rb.velocity.sqrMagnitude == 0)
            transform.hasChanged = false;
        else
            transform.hasChanged = true;
    }
}
