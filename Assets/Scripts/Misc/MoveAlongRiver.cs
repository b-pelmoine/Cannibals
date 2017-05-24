using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongRiver : MonoBehaviour {

    public Transform AudioListener;
    public Transform CheckpointsParent;
    private Vector3[] CheckpointPositions;

    private Vector3[] nearests = new Vector3[2];
    private float[] nearestDistSqr = new float[2];

    private Vector3 prevPosition;
    public float dragfactor;

    void Start () {
        List<Vector3> positions = new List<Vector3>();
        foreach(Transform t in CheckpointsParent)
        {
            positions.Add(t.position);
            Destroy(t.gameObject);
        }
        CheckpointPositions = positions.ToArray();
        prevPosition = transform.position;
    }
	
	void FixedUpdate () {
        for (int i = 0; i < 2; i++)
        {
            nearests[i] = Vector3.up*1000f;
            nearestDistSqr[i] = Mathf.Infinity;
        }
        float distSqr;
        int nearestIndex = -1;
        int secondNearestIndex = -1;
        for (int i = 0; i < CheckpointPositions.Length; i++)
        {
            distSqr = (CheckpointPositions[i] - AudioListener.position).sqrMagnitude;
            if(distSqr < nearestDistSqr[0])
            {
                float tmp = nearestDistSqr[0];
                Vector3 tmpP = nearests[0];
                nearestDistSqr[0] = distSqr;
                nearests[0] = CheckpointPositions[i];
                if (nearestDistSqr[1] < tmp)
                {
                    nearestDistSqr[1] = tmp;
                    nearests[1] = tmpP;
                    secondNearestIndex = nearestIndex;
                }
                nearestIndex = i;
            }
            else
            {
                if(distSqr < nearestDistSqr[1])
                {
                    nearestDistSqr[1] = distSqr;
                    nearests[1] = CheckpointPositions[i];
                    secondNearestIndex = i;
                }
            }
        }
        if (Mathf.Abs(nearestIndex - secondNearestIndex) > 1.5f)
        {
            float distToNearUp, distToNearDown;
            if (nearestIndex - 1 >= 0)
                distToNearUp = (CheckpointPositions[nearestIndex - 1] - CheckpointPositions[nearestIndex]).sqrMagnitude;
            else
                distToNearUp = Mathf.Infinity;
            if (nearestIndex + 1 < CheckpointPositions.Length)
                distToNearDown = (CheckpointPositions[nearestIndex + 1] - CheckpointPositions[nearestIndex]).sqrMagnitude;
            else
                distToNearDown = Mathf.Infinity;

            nearests[1] = (distToNearUp > distToNearDown) ? CheckpointPositions[nearestIndex - 1] : CheckpointPositions[nearestIndex + 1] ;
        }
        transform.position = Vector3.Lerp(getPosOnLine(nearests),prevPosition,Time.deltaTime*dragfactor);
        prevPosition = transform.position;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(nearests[0], nearests[1]);
        Gizmos.DrawWireSphere(nearests[0], .3f);
        Gizmos.DrawWireSphere(nearests[1], .3f);
    }

    Vector3 getPosOnLine(Vector3[] points)
    {
        Vector3 onLineProjection = Vector3.Project((AudioListener.position-points[0]),(points[1]- points[0])) + points[0];
        float distSqrPt1Project = (onLineProjection - points[0]).sqrMagnitude;
        float distSqrPt2Project = (onLineProjection - points[1]).sqrMagnitude;
        float distSqrPt1Pt2 = (points[0] - points[1]).sqrMagnitude;
        if(distSqrPt1Pt2 < distSqrPt1Project)
        {
            onLineProjection = points[1];
        }
        else
        {
            if (distSqrPt1Pt2 < distSqrPt2Project) onLineProjection = points[0];
        }
        return onLineProjection;
    }
}
