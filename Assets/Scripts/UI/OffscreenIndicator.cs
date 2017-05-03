﻿using System.Collections;
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

    [Range(0.5f, 1.0f)]
    public float distanceFromScreenBounds;

    [Range(0.5f, 50.0f)]
    public float pulseSpeed;

    public Texture tex_AI;
    public Texture tex_Target;
    public Texture tex_Player;
    public Texture tex_Village;

    public Camera mCamera;
    private Vector3 center; // center of the screen
    private Vector3 bounds;

    public GameObject[] AIAgents; //public for tests
    [Range(0.5f, 2f)]
    public float AIAgentsSizeMultiplier = 1.0f;
    public bool pulse_AIAgents = false;
    private List<IndicatorData> AIOnScreenPositions;
    public GameObject[] Players; // assigned automatically
    [Range(0.5f, 2f)]
    public float PlayersSizeMultiplier = 1.0f;
    public bool pulse_Players = false;
    private List<IndicatorData> PlayersOnScreenPositions;
    public GameObject[] Targets; // assigned at runtime / may vary over time
    [Range(0.5f, 2f)]
    public float TargetsSizeMultiplier = 1.0f;
    public bool pulse_Targets = false;
    private List<IndicatorData> TargetOnScreenPositions;

    public GameObject[] endPosition;
    private List<IndicatorData> endPositions;

    private bool showAIAgents;
    private bool showTarget; // automatique ? offscreen = enabled
    private bool showPlayers;

    Vector3 screenPos = Vector3.zero;

    void Start () {
        if(!mCamera)
            mCamera = Camera.main;
        center = new Vector3(Screen.width, Screen.height, 0)/2;

        AIOnScreenPositions = new List<IndicatorData>();
        TargetOnScreenPositions = new List<IndicatorData>();
        PlayersOnScreenPositions = new List<IndicatorData>();
        endPositions = new List<IndicatorData>();

        showTarget = true;

    }

    public void triggerAgentIndicator(bool state)
    {
        showAIAgents = state;
    }
    public void triggerTargetIndicator(bool state)
    {
        showTarget = state;
    }

    public void triggerPlayersIndicator(bool state)
    {
        showPlayers = state;
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
        if(showTarget)
        {
            foreach (IndicatorData data in TargetOnScreenPositions)
            {
                GUI.DrawTexture(data._pos, tex_Target, ScaleMode.ScaleToFit);
            }
        }
        if(showPlayers)
        {
            foreach (IndicatorData data in PlayersOnScreenPositions)
            {
                GUI.DrawTexture(data._pos, tex_Player, ScaleMode.ScaleToFit);
            }
        }
        //proto uiui
        foreach (IndicatorData data in endPositions)
        {
            GUI.DrawTexture(data._pos, tex_Village, ScaleMode.ScaleToFit);
        }
    }

    void Update () {
        bounds = center * distanceFromScreenBounds;
        float pSpeed;
        if (showAIAgents)
        {
            pSpeed = (pulse_AIAgents) ? pulseSpeed : 0;
            addIndicatorForGameObjects(AIOnScreenPositions, AIAgents, AIAgentsSizeMultiplier, false, pSpeed);
        }
        if(showTarget)
        {
            pSpeed = (pulse_Targets) ? pulseSpeed : 0;
            //checke if alive then display
            addIndicatorForGameObjects(TargetOnScreenPositions, Targets, TargetsSizeMultiplier, false, pSpeed);
        }
        if(showPlayers)
        {
            pSpeed = (pulse_Players) ? pulseSpeed : 0;
            List<GameObject> deadPlayers = new List<GameObject>();
            foreach(GameObject player in Players)
            {
                if(player.GetComponent<Cannibal>().IsDead())
                    deadPlayers.Add(player);
            }

            addIndicatorForGameObjects(PlayersOnScreenPositions, deadPlayers.ToArray(), PlayersSizeMultiplier , true, pSpeed);
        }

        addIndicatorForGameObjects(endPositions, endPosition, 2);
    }

    //add IndicatorData for the given GameObjects into the list
    void addIndicatorForGameObjects(List<IndicatorData> list, GameObject[] targets, float sizeMultiplier = 1.0f, bool displayOnScreen = false, float pulseSpeed = 0f)
    {
        Vector3 playersCenter = Vector3.Lerp(Players[0].transform.position, Players[1].transform.position, .5f);
        //loop through AI_Agents
        list.Clear();
        foreach (GameObject go in targets)
        {
            //jump to the next one if not active + TODO if dead
            if (!go.activeInHierarchy) continue;

            screenPos = Camera.main.WorldToScreenPoint(go.transform.position);

            if (screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height && screenPos.z > 0)
            {
                if(displayOnScreen)
                {
                    Vector2 dimensions = new Vector2(Screen.width / 20, Screen.height / 20) * (sizeMultiplier + 0.1f*(Mathf.Sin(pulseSpeed*Time.time)));
                    Vector2 position = new Vector2(screenPos.x, (Screen.height-screenPos.y)) - dimensions / 2;

                    list.Add(new IndicatorData(
                        new Rect(position, dimensions),
                        0
                        ));
                }
                // on Screen, outline first
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
                    screenPos = new Vector3(bounds.x, -bounds.x * m, 0);
                else if (screenPos.x < -bounds.x)
                    screenPos = new Vector3(-bounds.x, bounds.x * m, 0);

                //undo translation
                screenPos += center;

                float distFromPlayers = Vector3.Distance(playersCenter, go.transform.position);

                Vector2 dimensions = new Vector2(Screen.width / 20, Screen.height / 20) 
                    * Mathf.Clamp(20 / distFromPlayers, 0.5f, 5) * (sizeMultiplier + 0.1f * (Mathf.Sin(pulseSpeed*Time.time)));
                Vector2 position = new Vector2(screenPos.x, screenPos.y) - dimensions / 2;

                list.Add(new IndicatorData(
                    new Rect(position, dimensions),
                    angle
                    ));
            }
        }
    }
}
