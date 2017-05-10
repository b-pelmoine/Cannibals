using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class Chasseur : AIAgent {
        public float waypointDistance = 2;
        public float shootingRange = 1;
        public float detectTime = 3;
        public Animator animator;
        int anim_speedId = Animator.StringToHash("Speed");

        private LineOfSight los;
        

        public bool alerte = false;

        Waypoint patrouille;

        enum ChasseurTask
        {
            Normal,
            Chase,
            Defend,
            Look
        }

	    // Use this for initialization
	    new void Start () {
            base.Start();
            type = AIType.Hunter;
            Bottle.OnBottleShaked += OnBottleShaked;
            los = GetComponent<LineOfSight>();
            patrouille = GetComponent<Waypoint>();
            tasks.Push(new Task((int)ChasseurTask.Normal));
	    }
	
	    // Update is called once per frame
	    new void Update () {
            base.Update();
		    if(animator != null && agent!=null)
            {
                animator.SetFloat(anim_speedId, agent.velocity.magnitude);
            }
            else
            {
                Debug.Log("Chasseur.cs: animator ou navmeshagent non présent");
            }

            if (los.Updated)
            {
                foreach(GameObject obj in los.sighted)
                {
                    Cannibal cannibal = obj.GetComponentInParent<Cannibal>();
                    if (cannibal!=null && CurrentTask.id==(int)ChasseurTask.Normal)
                    {
                        agent.ResetPath();
                        Debug.Log("Target in sight");
                        tasks.Push(new Task((int)ChasseurTask.Look, obj));
                    }
                }
            }
            //Manage tasks
            switch (CurrentTask.id)
            {
                case (int)ChasseurTask.Normal:
                    //Follow waypoints
                    if (patrouille != null)
                    {
                        if(CurrentTask.elapsed > 4 && MoveTo(patrouille.getCurrentDestination(), 5))
                        {
                            patrouille.Next();
                            CurrentTask.elapsed = 0;
                        }
                    }
                    break;

                case (int)ChasseurTask.Chase:
                    if((CurrentTask.target.transform.position - transform.position).sqrMagnitude >= Mathf.Pow((los.camera.farClipPlane), 2) || MoveTo(CurrentTask.target.transform.position, shootingRange))
                    {
                        
                        agent.ResetPath();
                        tasks.Pop();
                    }
                    break;

                //Regarde vers le joueur jusqu'à détection
                case (int)ChasseurTask.Look:
                    agent.transform.LookAt(CurrentTask.target.transform);
                    //Si le joueur est en vue
                    if (los.sighted.Contains(CurrentTask.target)) { 
                        //Si le joueur est détecté -> poursuite
                       if((CurrentTask.target.transform.position-transform.position).sqrMagnitude < Mathf.Pow((CurrentTask.elapsed/detectTime)*(los.camera.farClipPlane),2)){
                            GameObject target = CurrentTask.target;
                            tasks.Pop();
                            Debug.Log("Chasing");
                            tasks.Push(new Task((int)ChasseurTask.Chase, target));
                       }
                    }
                    else //Si le joueur n'est plus en vue, on diminue le timer
                    {
                        CurrentTask.elapsed -= Time.deltaTime*2;
                        //Si le temps est négatif, on abandonne
                        if (CurrentTask.elapsed < 0)
                        {
                            tasks.Pop();
                        }
                    }
                    break;

                case (int)ChasseurTask.Defend:

                    break;
            }

	    }

        void OnBottleShaked(Bottle bot)
        {
            /*if(Vector3.SqrMagnitude(bot.transform.position-this.transform.position)< range*range && !sawBottle)
            {
                bottle = bot.transform.position;
                sawBottle = true;
            }*/
        }

        
    }
}
