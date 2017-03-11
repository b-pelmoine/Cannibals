using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour {

    [System.Serializable]
    public enum CameraState
    {
        IDLE,
        MOVE_UP,
        MOVE_DOWN
    }

    public Transform playerOne;
    public Transform playerTwo;

    //flag to know if the camera can unzoom to see all players
    private bool cameraCanSeePlayers;

    [SerializeField]
    [Tooltip("Debug only (false in release)")]
    private bool forceUpdate = false;

    private Vector3 barycenter, prevBarycenter;
    private CameraState state;

    [Range(5, 15)]
    [Tooltip("How Close the camera can be")]
    public float closePlan;
    [Range(15, 30)]
    [Tooltip("How Far the camera can be")]
    public float farPlan;

    [Range(-5f, 5)]
    [Tooltip("pas trop en abuser")]
    public float cameraXOffset;
    public float cameraZOffset;

    public float cameraSpeed = 5f;
    private float prevDistance = 0f;

    Vector3 goToPosition;
    float distanceRemaining;

    void Start () {
        barycenter = Vector3.Lerp(playerOne.position, playerTwo.position, .5f);
        state = CameraState.IDLE;
    }

	void Update () {
        if(playerOne.hasChanged || playerTwo.hasChanged || forceUpdate)
        {
            prevBarycenter = barycenter;
            barycenter = Vector3.Lerp(playerOne.position, playerTwo.position, .5f);

            float distanceBetweenPlayers = Vector3.Distance(playerOne.position, playerTwo.position);
            float distanceBetweenCamBarycenter = Vector3.Distance(barycenter, transform.position);
            float distance = distanceBetweenPlayers - distanceBetweenCamBarycenter;

            cameraCanSeePlayers = (distanceBetweenCamBarycenter >= farPlan+closePlan) ? false : true;

            if (state == CameraState.IDLE && barycenter != prevBarycenter)
            {
                state = (distance > 0) ? CameraState.MOVE_UP : CameraState.MOVE_DOWN;
                moveCamera(distance);
            }
            else
            {
                state = CameraState.IDLE;
                if (transform.position != goToPosition)
                    moveCamera(distanceRemaining);
            }

            float verticalVariation = transform.position.y - barycenter.y;
            float Yoffset = (verticalVariation < closePlan) ? barycenter.y + closePlan : transform.position.y;
            Yoffset = (verticalVariation > farPlan) ? barycenter.y + farPlan : Yoffset;
            cameraZOffset = -Yoffset;
            transform.position = new Vector3(barycenter.x + cameraXOffset, Yoffset, barycenter.z + cameraZOffset);
            transform.LookAt(barycenter);

            prevDistance = distance;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = (!cameraCanSeePlayers) ? Color.red : Color.green;
        Gizmos.DrawLine(playerOne.position, playerTwo.position);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(barycenter, transform.position);
    }

    void moveCamera(float distance)
    {
        Vector3 oldPos = transform.position + Vector3.up * prevDistance * Time.deltaTime * cameraSpeed;
        goToPosition = transform.position + Vector3.up * distance * Time.deltaTime * cameraSpeed;
        float offset = Vector3.Distance(oldPos, goToPosition);
        transform.position = Vector3.Lerp(oldPos, goToPosition, offset/distance);
        distanceRemaining = distance;
    }
}
