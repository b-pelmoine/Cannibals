using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

[Category("Physics")]
public class CheckSphereOverlapCondition<T> : ConditionTask<Transform> {

    public BBParameter<List<T>> savedList;
    public LayerMask layerMask;
    public BBParameter<float> radius;

#if UNITY_EDITOR
    public Color debugColor = Color.cyan;
#endif

    protected override bool OnCheck()
    {
       savedList.value.Clear();
       Collider[] colliders =  Physics.OverlapSphere(agent.position, radius.value);

        foreach(Collider c in colliders)
        {
            T script = c.gameObject.GetComponentInParent<T>();

            if(script != null && !savedList.value.Contains(script))
            {
                savedList.value.Add(script);
            }
        }

        SpecialCondition(savedList.value);

        return savedList.value.Count > 0;
    }


    protected virtual void SpecialCondition(List<T> savedList)
    {

    }
#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = debugColor;
        Gizmos.DrawSphere(agent.position, radius.value);
    }
    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
    }
#endif
}
