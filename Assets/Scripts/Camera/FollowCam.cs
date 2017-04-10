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

    [Range(.5f, 4)]
    [Tooltip("change the point of view angle")]
    public float verticalityFactor;

    [Range(.5f, 1f)]
    public float stickFactor;
    [Range(.5f, 5f)]
    public float dragFactor;

    [Range(-5f, 5)]
    [Tooltip("pas trop en abuser")]
    public float cameraXOffset;
    private float cameraZOffset;

    public float cameraSpeed = 5f;
    private float prevDistance = 0f;

    private Vector3 goToPosition;
    private float distanceRemaining;

    float distanceBetweenPlayers;
    
    // elements / characters / whatever that the camera must show to the players
    private List<GameObject> leadingElements;
    public float catchElementsRadius = 40f;

    void Awake()
    {
        goToPosition = transform.position;
    }

    public bool arePlayersTooFarAway(out float distance)
    {
        distance = distanceBetweenPlayers;
        return distanceBetweenPlayers > farPlan;
    }

    Vector3 getBarycenter()
    {
        Vector3 centerOfAttention = Vector3.Lerp(playerOne.position, playerTwo.position, .5f);
        int layerMask = 1 << LayerMask.NameToLayer("AI_Agent");
        Collider[] nearbyElements = Physics.OverlapSphere(centerOfAttention, catchElementsRadius, layerMask);
        int sumOfLevels = 1; //Importance level of both players
        Vector3 elementsBarycenter = centerOfAttention;
        foreach(Collider elt in nearbyElements)
        {
            int level = elt.GetComponent<LevelOfImportance>().getLevel();
            elementsBarycenter = Vector3.Lerp(elementsBarycenter, elt.transform.position, (float)level/(float)(sumOfLevels+level));
            sumOfLevels += level;
        }
        Vector3 newBarycenter = Vector3.Lerp(elementsBarycenter, centerOfAttention, stickFactor); 

        return newBarycenter;
    }

    void Start () {
        barycenter = Vector3.Lerp(playerOne.position, playerTwo.position, .5f);
        state = CameraState.IDLE;
    }

	void FixedUpdate () {
        if(playerOne.hasChanged || playerTwo.hasChanged || forceUpdate)
        {
            prevBarycenter = barycenter;
            barycenter = getBarycenter();
            barycenter = Vector3.Lerp(prevBarycenter, barycenter, Time.deltaTime* dragFactor);

            Vector3 playersBarycenter = Vector3.Lerp(playerOne.position, playerTwo.position, .5f);
            float distanceplayersBarycenter = Vector3.Distance(barycenter, playersBarycenter);
            distanceBetweenPlayers = Vector3.Distance(playerOne.position, playerTwo.position);
            float distanceBetweenCamPlayersBarycenter = Vector3.Distance(playersBarycenter, transform.position);
            float distance = distanceBetweenPlayers - distanceBetweenCamPlayersBarycenter + (distanceplayersBarycenter);

            cameraCanSeePlayers = (distanceBetweenPlayers >= farPlan+closePlan) ? false : true;

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
            cameraZOffset = Mathf.Round(cameraZOffset * 100f) / (100f*verticalityFactor);
            transform.position = new Vector3(barycenter.x + cameraXOffset, Yoffset, barycenter.z + cameraZOffset + distanceplayersBarycenter/5);
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
        Gizmos.DrawLine(barycenter, playerOne.position);
        Gizmos.DrawLine(barycenter, playerTwo.position);
        Gizmos.color = Color.grey - new Color(0, 0, 0, .5f);
        Gizmos.DrawWireSphere(barycenter, catchElementsRadius);
    }

    void moveCamera(float distance)
    {
        Vector3 oldPos = transform.position + Vector3.up * prevDistance * Time.deltaTime * cameraSpeed;
        goToPosition = transform.position + Vector3.up * distance * Time.deltaTime * cameraSpeed;
        float offset = Vector3.Distance(oldPos, goToPosition);
        transform.position = Vector3.Lerp(oldPos, goToPosition, (offset*1.5f)/distance);
        distanceRemaining = distance;
    }
}
