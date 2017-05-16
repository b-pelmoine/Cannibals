using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum AIType{
    UNKNOWN, //default
    Target_Alive,
    Target_Dead,
    PlayerOne,
    PlayerTwo,
    Hunter,
    Dog,
    Scout,
    Runner,
    EndPoint
}

public struct IndicatorData
{
    public IndicatorData(Rect pos,float angle, AIType type) { _pos = pos; _angle = angle; _type = type; }
    public Rect _pos;
    public float _angle;
    public AIType _type;
}

[System.Serializable]
public struct RawGO
{
    public RawGO(GameObject go_t, AIType type_t) { go = go_t; type = type_t;}
    public GameObject go;
    public AIType type;
}

public class OffscreenIndicator : MonoBehaviour {
    [Range(0.5f, 1.0f)]
    public float distanceFromScreenBounds;

    [Range(0.5f, 50.0f)]
    public float pulseSpeed;

    [Header("Textures")]
    public Texture tex_default;
    public Texture tex_AI_Hunter;
    public Texture tex_AI_Dog;
    public Texture tex_AI_Scout;
    public Texture tex_AI_Runner;
    public Texture tex_Target_Alive;
    public Texture tex_Target_Dead;
    public Texture tex_Player_One;
    public Texture tex_Player_Two;
    public Texture tex_Village;

    public Camera mCamera;
    private Vector3 center; // center of the screen
    private Vector3 bounds;

    public List<RawGO> AIAgents; //public for tests
    [Range(0.5f, 2f)]
    public float AIAgentsSizeMultiplier = 1.0f;
    public bool pulse_AIAgents = false;
    private List<IndicatorData> AIOnScreenPositions;
    public RawGO[] Players; // assigned automatically
    public Cannibal[] cannibalsRef;
    [Range(0.5f, 2f)]
    public float PlayersSizeMultiplier = 1.0f;
    public bool pulse_Players = false;
    private List<IndicatorData> PlayersOnScreenPositions;
    public RawGO[] Targets; // assigned at runtime / may vary over time
    [Range(0.5f, 2f)]
    public float TargetsSizeMultiplier = 1.0f;
    public bool pulse_Targets = false;
    private List<IndicatorData> TargetOnScreenPositions;
    private List<IndicatorData> OldTargetOnScreenPositions;

    public RawGO[] endPosition;
    private List<IndicatorData> endPositions;

    private bool showAIAgents;
    private bool showTarget; // automatique ? offscreen = enabled
    private bool showPlayers;

    [Header("Params :")]
    private float elapsedTime = 0f;
    private float elapsedLastPop = 0f;
    private int lastAIID = 0;
    [Range(0.2f,5f)]
    public float transitionOut = 0f;
    [Range(0.2f, 5f)]
    public float transitionIn = 0f;
    private List<RawGO> oldAgents;

    Vector3 screenPos = Vector3.zero;

    void Start () {
        if(!mCamera)
            mCamera = Camera.main;
        center = new Vector3(Screen.width, Screen.height, 0)/2;

        AIAgents = new List<RawGO>();
        oldAgents = new List<RawGO>();
        
        AIOnScreenPositions = new List<IndicatorData>();
        TargetOnScreenPositions = new List<IndicatorData>();
        OldTargetOnScreenPositions = new List<IndicatorData>();
        PlayersOnScreenPositions = new List<IndicatorData>();
        endPositions = new List<IndicatorData>();

        showTarget = true;
        showPlayers = true;
        showAIAgents = true;
        //showPlayers = true;
    }

    //test if not already registered
    public void AddAgentIndicator(GameObject go, AIType type)
    {
        int instanceID = go.GetInstanceID();
        //don't add if already in the list
        foreach (RawGO RawgameObject in AIAgents){if (instanceID == RawgameObject.go.GetInstanceID()) return;}
        //remove newly discovered from old list
        foreach (RawGO RawgameObject in oldAgents) { if (instanceID == RawgameObject.go.GetInstanceID()) { oldAgents.Remove(RawgameObject); break; } }
        AIAgents.Add(new RawGO(go, type));
        lastAIID = AIAgents[AIAgents.Count - 1].go.GetInstanceID();
        elapsedLastPop = 0f;
    }

    public void triggerAgentIndicator(bool state)
    {
        if(!state)
        {
            if(elapsedTime > transitionOut)
                elapsedTime = 0;
            foreach (RawGO go in AIAgents) oldAgents.Add(go);
            AIAgents.Clear();
        }
    }

    //return the appropriate texture according to AIType
    Texture getTextureFromType(AIType type)
    {
        switch(type)
        {
            case AIType.Hunter: return tex_AI_Hunter;
            case AIType.Dog: return tex_AI_Dog;
            case AIType.Scout: return tex_AI_Scout;
            case AIType.Runner: return tex_AI_Runner;
            case AIType.PlayerOne: return tex_Player_One;
            case AIType.PlayerTwo: return tex_Player_Two;
            case AIType.Target_Alive: return tex_Target_Alive;
            case AIType.Target_Dead: return tex_Target_Dead;
            case AIType.EndPoint: return tex_Village;
            default:    return tex_default;
        }
    }

    void OnGUI()
    {
        if(showAIAgents)
        {
            foreach (IndicatorData data in AIOnScreenPositions)
            {
                GUI.DrawTexture(data._pos, getTextureFromType(data._type), ScaleMode.ScaleToFit);
            }
            if (elapsedTime < transitionOut)
            {
                GUI.color = Color.white*(transitionOut - elapsedTime);
                foreach (IndicatorData data in OldTargetOnScreenPositions)
                {
                    GUI.DrawTexture(data._pos, getTextureFromType(data._type), ScaleMode.ScaleToFit);
                }
            }
            else
            {
                oldAgents.Clear();
            }
        }
        GUI.color = Color.white;
        if (showTarget)
        {
            
            foreach (IndicatorData data in TargetOnScreenPositions)
            {
                GUI.DrawTexture(data._pos, getTextureFromType(data._type), ScaleMode.ScaleToFit);
            }
        }
        if(showPlayers)
        {
            foreach (IndicatorData data in PlayersOnScreenPositions)
            {
                GUI.DrawTexture(data._pos, getTextureFromType(data._type), ScaleMode.ScaleToFit);
            }
        }
        //proto uiui
        foreach (IndicatorData data in endPositions)
        {
            GUI.DrawTexture(data._pos, getTextureFromType(data._type), ScaleMode.ScaleToFit);
        }
    }

    void Update () {
        elapsedTime += Time.deltaTime;
        bounds = center * distanceFromScreenBounds;
        float pSpeed;
        if (showAIAgents)
        {
            elapsedLastPop += Time.deltaTime;
            pSpeed = (pulse_AIAgents) ? pulseSpeed : 0;
            addIndicatorForGameObjects(AIOnScreenPositions, AIAgents.ToArray(), AIAgentsSizeMultiplier, true, pSpeed);
            addIndicatorForGameObjects(OldTargetOnScreenPositions, oldAgents.ToArray(), AIAgentsSizeMultiplier, true, pSpeed);
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
            List<RawGO> deadPlayers = new List<RawGO>();
            foreach(RawGO player in Players)
            {
                Cannibal c;
                if (player.type == AIType.PlayerOne)
                    c = cannibalsRef[0];
                else
                    c = cannibalsRef[1];
                if (c.IsDead()) deadPlayers.Add(player);
            }
            addIndicatorForGameObjects(PlayersOnScreenPositions, deadPlayers.ToArray(), PlayersSizeMultiplier , true, pSpeed);
        }

        addIndicatorForGameObjects(endPositions, endPosition, 2);
    }

    //add IndicatorData for the given GameObjects into the list
    void addIndicatorForGameObjects(List<IndicatorData> list, RawGO[] targets, float sizeMultiplier = 1.0f, bool displayOnScreen = false, float pulseSpeed = 0f)
    {
        Vector3 playersCenter = Vector3.Lerp(Players[0].go.transform.position, Players[1].go.transform.position, .5f);
        //loop through AI_Agents
        list.Clear();
        foreach (RawGO go in targets)
        {
            //jump to the next one if not active + TODO if dead
            if (!go.go.activeInHierarchy) continue;

            screenPos = Camera.main.WorldToScreenPoint(go.go.transform.position);

            if (screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height && screenPos.z > 0)
            {
                if(displayOnScreen)
                {
                    float alterDimensionPop = 1f;
                    //pop offset change the value (5) to increase the pop factor
                    if (lastAIID == go.go.GetInstanceID() && elapsedLastPop < transitionIn)
                        alterDimensionPop = 1 + (4 * (transitionIn - elapsedLastPop));

                    Vector2 dimensions = new Vector2(Screen.width / 40, Screen.height / 40)
                        * (sizeMultiplier + 0.1f * (Mathf.Sin(pulseSpeed * Time.time)))
                        * alterDimensionPop;
                    //Vector2 dimensions = new Vector2(Screen.width / 40, Screen.height / 40) * (sizeMultiplier + 0.1f*(Mathf.Sin(pulseSpeed*Time.time)));
                    Vector2 position = new Vector2(screenPos.x, (Screen.height-screenPos.y)) - dimensions / 2;

                    list.Add(new IndicatorData(
                        new Rect(position, dimensions),
                        0,
                        go.type
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

                float distFromPlayers = Vector3.Distance(playersCenter, go.go.transform.position);

                float alterDimensionPop = 1f;
                //pop offset change the value (5) to increase the pop factor
                if (lastAIID == go.go.GetInstanceID() && elapsedLastPop < transitionIn)
                    alterDimensionPop = 1 + (4 * (transitionIn - elapsedLastPop));

                Vector2 dimensions = new Vector2(Screen.width / 20, Screen.height / 20) 
                    * Mathf.Clamp(20 / distFromPlayers, 0.5f, 3) 
                    * (sizeMultiplier + 0.1f * (Mathf.Sin(pulseSpeed*Time.time)))
                    * alterDimensionPop;
                

                Vector2 position = new Vector2(screenPos.x, screenPos.y) - dimensions / 2;

                list.Add(new IndicatorData(
                    new Rect(position, dimensions),
                    angle,
                    go.type
                    ));
            }
        }
    }
}
