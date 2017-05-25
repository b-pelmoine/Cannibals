using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Scout : AIAgent {
        Vector3 basePosition;
        public Transform scoutHand;
        public GameObject cookie;
	    // Use this for initialization
	    new void Start () {
            base.Start();
            basePosition = transform.position;

            ActionTask root = new ActionTask();
            root.OnExecute = () =>
            {
                if (WanderAround(basePosition, 4))
                {
                    Wait(1).callbacks.Add(10, EchangerAvecMamie);
                }
            };
            root.callbacks.Add(10, EchangerAvecMamie);
            Play(root);
	    }
	
	    // Update is called once per frame
	    new void Update () {
            base.Update();
            animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
	    }

        public void Call(Mamie mamie)
        {
            CurrentAction.callData = mamie;
            Call(10);
        }

        protected void EchangerAvecMamie()
        {
            Debug.Log("Echange avec mamie");
            Mamie mamie = (CurrentAction.callData as Mamie);
            ActionTask task = new ActionTask();
            task.OnExecute = () =>
            {
                if (MoveTo(mamie.transform.position + mamie.transform.forward * 1.5f, 0.1f))
                {
                    LookAt(mamie.transform.position);
                    animator.Play("Give");
                    mamie.Call(0);
                    Wait(0.1f).Next = () =>
                     {
                         Wait(0).callbacks.Add(10, () =>
                         {
                             Stop();
                             GameObject c = Instantiate(cookie);
                             c.transform.parent = scoutHand;
                             c.GetComponent<Cookies>().m_rigidbody.isKinematic = true;
                             c.transform.localPosition = Vector3.zero;
                             animator.Play("Give");
                             Wait(0.1f).Next = () =>
                             {
                                 anim_call_count = 0;
                                 ActionTask give = Wait(animator.GetCurrentAnimatorStateInfo(0).length);
                                 give.callbacks.Add(0, () =>
                                 {
                                     mamie.Take(c);
                                 });
                                 give.Next = Stop;
                             };
                         });
                     };
                }
            };
            Play(task);
        }

        public void Take(GameObject can)
        {
            can.transform.parent = scoutHand;
            can.SetActive(false);
        }
    }
}
