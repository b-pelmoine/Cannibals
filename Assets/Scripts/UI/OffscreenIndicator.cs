using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct IndicatorData
{
    public IndicatorData(Rect pos,float angle) { _pos = pos; _angle = angle; }
    public Rect _pos;
    public float _angle;
}

public class OffscreenIndicator : MonoBehaviour {

    public Texture tex_AI;
    public Texture tex_Target;
    public Texture tex_Player;

    public Camera mCamera;
    private Vector3 center; // center of the screen
    private Vector3 bounds;

    public GameObject[] AIAgents; //public for tests
    private List<IndicatorData> AIOnScreenPositions;
    private GameObject[] Players; // assigned automatically
    private List<IndicatorData> PlayersOnScreenPositions;
    public GameObject Target; // assigned at runtime / may vary over time
    private IndicatorData TargetOnScreenPosition;

    public bool showAIAgents;
    public bool showTarget; // automatique ? offscreen = enabled

    Vector3 screenPos = Vector3.zero;

    void Start () {
        //AIAgents = GameObject.FindGameObjectsWithTag("Agent");
        Players = GameObject.FindGameObjectsWithTag("Player");
        if(!mCamera)
            mCamera = Camera.main;
        center = new Vector3(Screen.width, Screen.height, 0)/2;
        bounds = center * 0.9f;

        AIOnScreenPositions = new List<IndicatorData>();
        TargetOnScreenPosition = new IndicatorData();
        PlayersOnScreenPositions = new List<IndicatorData>();
	}

    public void triggerAgentIndicator(bool state)
    {
        showAIAgents = state;
    }
    public void triggerTargetIndicator(bool state)
    {
        showTarget = state;
    }

    void Update () {
        if(showAIAgents)
        {
            //loop through AI_Agents
            AIOnScreenPositions.Clear();
            foreach (GameObject go in AIAgents)
            {
                //jump to the next one if not active + TODO if dead
                if (!go.activeInHierarchy) continue;

                screenPos = Camera.main.WorldToScreenPoint(go.transform.position);

                if (screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height && screenPos.z > 0)
                {
                     // on Screen
                }
                else // off Screen
                {
                    //translate coordinate
                    screenPos -= center;
                    if (screenPos.z < 0) screenPos *= -1;

                    float angle = Mathf.Atan2(screenPos.y, screenPos.x);
                    angle -= 90 * Mathf.Deg2Rad;

                    float sin = -Mathf.Sin(angle);
                    float cos = Mathf.Cos(angle);

                    screenPos = center + new Vector3(sin * 150, cos * 150, 0);

                    float m = cos / sin;

                    if (cos > 0)
                        screenPos = new Vector3(bounds.y / m, -bounds.y, 0);
                    else
                        screenPos = new Vector3(-bounds.y / m, bounds.y, 0);

                    if (screenPos.x > bounds.x)
                        screenPos = new Vector3(bounds.x, -bounds.x*m, 0);
                    else if(screenPos.x < -bounds.x)
                        screenPos = new Vector3(-bounds.x, bounds.x*m, 0);

                    //undo translation
                    screenPos += center;

                    AIOnScreenPositions.Add(new IndicatorData(
                        new Rect(screenPos.x, screenPos.y, Screen.width/20, Screen.height/20),
                        angle
                        ));
                }
            }
        }
    }

    void OnGUI()
    {
        if(showAIAgents)
        {
            foreach (IndicatorData data in AIOnScreenPositions)
            {
                GUI.DrawTexture(data._pos, tex_AI, ScaleMode.ScaleToFit);
            }
        }
    }
}
