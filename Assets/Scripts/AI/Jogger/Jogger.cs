using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Jogger : AIAgent, IKnifeKillable {
        public Waypoint waypoints;
        public IconDisplayer icon;
        Vector3 startPosition;

        public float baseSpeed = 3.0f;
        public float fleeSpeed = 6.0f;

        public float startTime = 3;

	    new void Start () {
            base.Start();
            agent.speed = baseSpeed;
            startPosition = transform.position;
            if (waypoints == null || waypoints.points.Count==0)
            {
                Debug.LogError("Jogger: Pas de waypoints");
            }
            ActionTask root = new ActionTask();
            root.OnExecute = () =>
            {
                if(MoveTo(waypoints.getCurrentDestination(), 3))
                {
                    waypoints.Next();
                }
            };
            root.AddReaction(SeeDanger, Panic);
            Wait(startTime).Next = () =>
            {
                Play(root);
            };
	    }

        new void Update()
        {
            base.Update();
            animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
        }

        bool SeeDanger()
        {
            SightInfo si = los.FindNearest(x =>
            {
                Cannibal c = x.target.GetComponentInParent<Cannibal>();
                return x.target.GetComponent<Corpse>() != null || (c != null && c.isCoverOfBlood);
            });
            if (si != null)
                return true;
            return false;
        }

        void Panic()
        {
            agent.speed = fleeSpeed;
            AkSoundEngine.PostEvent("jogger_scared", gameObject);
            ActionTask task = new ActionTask();
            task.OnExecute = () =>
            {
                if(MoveTo(startPosition, 2))
                {
                    Chasseur.alert = true;
                    Destroy();
                }
            };
            Play(task);
        }

        public bool IsKnifeVulnerable()
        {
            return true;
        }

        public override void Kill()
        {
            base.Kill();
            animator.Play("Die");
            AkSoundEngine.PostEvent("jogger_death", gameObject);
            ResetPath();
            StopAll();
        }

        public void KnifeKill()
        {
            Kill();
        }

        public void ShowKnifeIcon()
        {
            icon.Show();
        }
    }
}
