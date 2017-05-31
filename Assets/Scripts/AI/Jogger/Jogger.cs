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

	    new void Start () {
            base.Start();
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
            Play(root);
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
            ActionTask task = new ActionTask();
            task.OnExecute = () =>
            {
                if(MoveTo(startPosition, 2))
                {
                    Chasseur.alert = true;
                    Destroy();
                }
            };
        }

        public bool IsKnifeVulnerable()
        {
            return true;
        }

        public void KnifeKill()
        {
            animator.Play("Die");
            StopAll();
        }

        public void ShowKnifeIcon()
        {
            icon.Show();
        }
    }
}
