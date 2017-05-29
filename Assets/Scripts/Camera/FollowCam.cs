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

    [Header("Vars")]
    public Transform playerOne;
    private Cannibal cannibalOne;
    private Vector3 pOnePos;
    public Transform playerTwo;
    private Cannibal cannibalTwo;
    private Vector3 pTwoPos;

    //flag to know if the camera can unzoom to see all players
    private bool cameraCanSeePlayers;

    private Vector3 barycenter, prevBarycenter;
    private CameraState state;

    [Header("Camera settings")]
    [Range(5, 15)]
    [Tooltip("How Close the camera can be")]
    public float closePlan;
    [Range(15, 30)]
    [Tooltip("How Far the camera can be")]
    public float farPlan;
    private float farplanStatic;

    [Range(15, 30)]
    [Tooltip("How Far the camera can be when only one player is alive")]
    public float CamDistanceOnePlayer;

    [Range(.5f, 4)]
    [Tooltip("change the point of view angle")]
    public float verticalityFactor;

    [Range(0f, 10f)]
    public float openViewFactor;

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

    bool bothPlayersDead;
    bool bothPlayersAlive;
    bool pOneAlive, pTwoAlive;
    //cam distance when only one player is alive
    
    // elements / characters / whatever that the camera must show to the players
    private List<GameObject> leadingElements;
    public float catchElementsRadius = 40f;

    void Awake()
    {
        goToPosition = transform.position;
        bothPlayersDead = false;
    }

    public bool arePlayersTooFarAway(out float distance)
    {
        distance = distanceBetweenPlayers;
        if (!bothPlayersAlive) return false;
        return distanceBetweenPlayers > farPlan;
    }

    public Vector3 getPlayersBarycenter()
    {
        return barycenter;
    }

    Vector3 getBarycenter()
    {
        //center of attention vary if one player die
        Vector3 centerOfAttention;
        pOneAlive = !cannibalOne.IsDead();
        pTwoAlive = !cannibalTwo.IsDead();
        bothPlayersAlive = pOneAlive && pTwoAlive;
        if (pOneAlive && pTwoAlive)
            centerOfAttention = Vector3.Lerp(playerOne.position, playerTwo.position, .5f);
        else
        {
            if (!pOneAlive && !pTwoAlive && !bothPlayersDead) bothPlayersDead = true;
            else bothPlayersDead = false;
            centerOfAttention = (pOneAlive) ? playerOne.position : playerTwo.position;
        }
        //Collider[] nearbyElements = Physics.OverlapSphere(centerOfAttention, catchElementsRadius, layerMask);
        List<GameObject> agents = AIAgentManager.getActiveAgents().FindAll(x => (x.transform.position-centerOfAttention).sqrMagnitude<Mathf.Pow(catchElementsRadius,2));
        int sumOfLevels = 1; //Importance level of both players
        Vector3 elementsBarycenter = centerOfAttention;
        foreach(GameObject a in agents)
        {
            int level = a.GetComponent<AI.AIAgent>().GetLevel();
            elementsBarycenter = Vector3.Lerp(elementsBarycenter, a.transform.position, (float)level/(float)(sumOfLevels+level));
            sumOfLevels += level;
        }
        Vector3 newBarycenter = Vector3.Lerp(elementsBarycenter, centerOfAttention, stickFactor); 

        return newBarycenter;
    }
    
    void Start () {
        barycenter = Vector3.Lerp(playerOne.position, playerTwo.position, .5f);
        state = CameraState.IDLE;

        farplanStatic = farPlan;

        cannibalOne = playerOne.parent.GetComponent<Cannibal>();
        cannibalTwo = playerTwo.parent.GetComponent<Cannibal>();
    }

	void FixedUpdate () {
        pOnePos = playerOne.position + Vector3.Normalize(playerOne.position - playerTwo.position) * openViewFactor;
        pTwoPos = playerTwo.position + Vector3.Normalize(playerTwo.position - playerOne.position) * openViewFactor;

        prevBarycenter = barycenter;
        barycenter = getBarycenter();
        barycenter = Vector3.Lerp(prevBarycenter, barycenter, Time.deltaTime* dragFactor);

        farPlan = (bothPlayersAlive) ? farplanStatic : CamDistanceOnePlayer;

        Vector3 playersBarycenter;
        if (bothPlayersAlive)
            playersBarycenter = Vector3.Lerp(playerOne.position, playerTwo.position, .5f);
        else
            playersBarycenter = (pOneAlive) ? playerOne.position : playerTwo.position;

        float distanceplayersBarycenter = Vector3.Distance(barycenter, playersBarycenter);
        distanceBetweenPlayers = (bothPlayersAlive) ? Vector3.Distance(pOnePos, pTwoPos) : 0;
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
        transform.LookAt(barycenter+Vector3.back*verticalVariation/8);

        prevDistance = distance;
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
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerOne.position, pOnePos);
        Gizmos.DrawLine(playerTwo.position, pTwoPos);
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
