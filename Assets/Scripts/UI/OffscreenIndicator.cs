using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffscreenIndicator : MonoBehaviour {

    public Texture tex_AI;
    public Texture tex_Target;
    public Texture tex_Player;

    public Camera mCamera;
    private Vector2 center;

    public GameObject[] AIAgents; //public for tests
    private List<Vector3> AIOnScreenPositions;
    private GameObject[] Players; // assigned automatically
    private List<Vector3> PlayersOnScreenPositions;
    public GameObject Target; // assigned at runtime / may vary over time
    private Vector3 TargetOnScreenPosition;

    private bool showAIAgents;
    private bool showTarget; // automatique ? offscreen = enabled

    Vector3 tempPos = Vector3.zero;
    Vector3 tempCoordScreen = Vector3.zero;

    void Start () {
        //AIAgents = GameObject.FindGameObjectsWithTag("Agent");
        Players = GameObject.FindGameObjectsWithTag("Player");
        if(!mCamera)
            mCamera = Camera.main;
        center = new Vector2(mCamera.pixelWidth / 2, mCamera.pixelHeight / 2);

        AIOnScreenPositions = new List<Vector3>();
        TargetOnScreenPosition = Vector3.zero;
        PlayersOnScreenPositions = new List<Vector3>();

        showAIAgents = false;
        showTarget = false;
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

                tempPos = mCamera.WorldToViewportPoint(go.transform.position);
                tempCoordScreen = Camera.main.WorldToScreenPoint(go.transform.position);

                if (tempPos.x > 0f && tempPos.x < 1 && tempPos.y > 0 && tempPos.y < 1)
                {
                     // on Screen
                }
                else // off Screen
                {
                    if (tempPos.x > 1)
                    {
                        GUI.DrawTexture(new Rect(Screen.width - tex_AI.width, Screen.height - tempCoordScreen.y, iconWidth / 2, iconHeight / 2), EnemySymbol);
                    }
                    if (tempPos.x < 0)
                    {
                        GUI.DrawTexture(new Rect(0, Screen.height - enemyWorldToScreenPoint[i].y, iconWidth / 2, iconHeight / 2), EnemySymbol);

                    }
                    if (tempPos.y > 1)
                    {
                        GUI.DrawTexture(new Rect(enemyWorldToScreenPoint[i].x, 0, iconWidth / 2, iconHeight / 2), EnemySymbol);
                    }
                    if (tempPos.y < 0)
                    {
                        GUI.DrawTexture(new Rect(enemyWorldToScreenPoint[i].x, Screen.height - EnemySymbol.height, iconWidth / 2, iconHeight / 2), EnemySymbol);
                    }
                }
            }
        }

    }
}
