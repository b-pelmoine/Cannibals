using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

[Category("Physics")]
public class CheckBoxOverlapCondition<T> : ConditionTask<Transform> {

    public BBParameter<List<T>> savedList;
    public LayerMask layerMask;
    public int maxItems = 10;

    public BBParameter<Vector3> boxSize;
    public BBParameter<Vector3> positionOffset;
    public BBParameter<Vector3> rotationOffset;

    Collider[] colliders;

#if UNITY_EDITOR
    public Color debugColor = Color.cyan;
#endif

    protected override string OnInit()
    {
        colliders = new Collider[maxItems];
        return base.OnInit();
    }

    protected override bool OnCheck()
    {
       savedList.value.Clear();
       int count = Physics.OverlapBoxNonAlloc(agent.TransformPoint(positionOffset.value), boxSize.value, colliders, agent.rotation*Quaternion.Euler(rotationOffset.value));

        for(int i = count -1; i >=0; i--)
        {
            T script = colliders[i].gameObject.GetComponentInParent<T>();

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

        if (agent != null)
        {
            Gizmos.color = debugColor;
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(agent.TransformPoint(positionOffset.value), agent.rotation * Quaternion.Euler(rotationOffset.value), agent.lossyScale);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawWireCube(Vector3.zero, boxSize.value);
        }
    }
    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
    }
#endif

}
