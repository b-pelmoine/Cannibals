using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EquilibreGames
{
    public class SimpleCharacter : MonoBehaviour {

        [SerializeField]
        CharacterControllerExt characterController2D;

        [SerializeField]
        Animator animator;

        [SerializeField]
        float jumpForce = 2f;

        [SerializeField]
        float speed = 1f;


        bool jump = false;
        bool walk = false;
        float timer = 0.6f;

        //All input are tested in Update
        void Update()
        {
            walk = false;

            if (characterController2D.IsGrounded && !jump)
            {
                characterController2D.velocity.y = 0;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    jump = true;
                    animator.SetTrigger("Jump_01");
                    timer = 0.6f;

                    characterController2D.velocity.x = 0;
                    characterController2D.acceleration.x = 0;
                    return;
                }

                if (Input.GetKey(KeyCode.Q))
                {
                    characterController2D.velocity.x = -speed;

                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        characterController2D.velocity.x *= 2f;
                    }

                    walk = true;
                    this.transform.localScale = new Vector3(1, 1, 1);
                }

                if (Input.GetKey(KeyCode.D))
                {
                    characterController2D.velocity.x = speed; 

                    if(Input.GetKey(KeyCode.LeftShift))
                    {
                        characterController2D.velocity *= 2f;
                    }

                    walk = true;
                    this.transform.localScale = new Vector3(-1, 1, 1);
                }


                if(Mathf.Abs(characterController2D.velocity.x) > speed)
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run_02"))
                        animator.SetTrigger("Run_02");
                }
                else if (Mathf.Abs(characterController2D.velocity.x) >= speed/2f)
                {
                    if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Run_01"))
                        animator.SetTrigger("Run_01");
                }
                else if (Mathf.Abs(characterController2D.velocity.x) >= speed/4f)
                {
                    if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk_01"))
                         animator.SetTrigger("Walk_01");
                }
                else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_01") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Jump_01"))
                    animator.SetTrigger("Idle_01");
            }
            else if (jump)
            {
                if (timer <= 0)
                {
                    characterController2D.velocity.y = jumpForce;
                    jump = false;
                }
                timer -= Time.deltaTime;
            }
        }


        //Apply modification on fixed Update
        void FixedUpdate()
        {        
            //Friction for the ground
            if(characterController2D.IsGrounded && !walk)
            {
                characterController2D.acceleration.x = -8f*characterController2D.velocity.x;
            }
        }

    }
}
