using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Kill : ActionTask {
        public BBParameter<GameObject> target;
        public BBParameter<float> distance;
        public BBParameter<float> maxAngle;

        protected override void OnExecute()
        {
            GameObject t = target.value;
            Vector3 planarDir = (t.transform.position - agent.transform.position);
            planarDir.y = 0;
            planarDir = planarDir.normalized;
            float angle = Vector3.Angle(agent.transform.forward, planarDir);
            if (Vector3.Distance(t.transform.position, agent.transform.position)<distance.value 
                && angle<Mathf.Abs(maxAngle.value))
            {
                AIAgent agent = t.GetComponent<AIAgent>();
                if (agent != null)
                {
                    agent.Kill();
                }
                else
                {
                    Cannibal cannibal = t.GetComponentInParent<Cannibal>();
                    if (cannibal != null)
                    {
                        cannibal.Kill();
                    }
                    else
                    {
                        Debug.LogError("Cible non valide");
                    }
                }
            }
            EndAction();
        }
    }
}
