using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectUI : MonoBehaviour {

    [Header("Set up infos")]
    public Cannibal[] cannibals;
    private List<GameObjectType> objects;
    public Image[] ObjectsImg;
    public Image[] ObjectsBG;
    public CanvasGroup canvas;

    [Header("Sprites")]
    public Sprite sprite_BG;
    public Sprite sprite_Bone;
    public Sprite sprite_Bottle;
    public Sprite sprite_Canette;
    public Sprite sprite_ChampiBon;
    public Sprite sprite_ChampiPoison;
    public Sprite sprite_Cookies;
    public Sprite sprite_PlasticDuck;

    [Header("UI Show Params")]
    public float showDuration;
    public float fadeInDur;
    public float fadeOutDur;

    public Color[] pColors;

    float elapsed = 0;
    bool showUI = false;

    // Use this for initialization
    void Start () {
        objects = new List<GameObjectType>();
        int i = 0;
		foreach(Cannibal c in cannibals)
        {
            CannibalObject obj = c.m_cannibalSkill.m_cannibalObject;
            if (obj)
                objects.Add(obj.m_info.type);
            else
                objects.Add(GameObjectType.NONE);
            ObjectsBG[i].sprite = sprite_BG;
            ObjectsImg[i].color = pColors[i];
            ObjectsImg[i].enabled = false;
            ++i;
        }

        canvas.alpha = 0;
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < cannibals.Length; i++)
        {
            GameObjectType objectType;
            CannibalObject obj = cannibals[i].m_cannibalSkill.m_cannibalObject;
            if (obj)
                objectType = obj.m_info.type;
            else
                objectType = GameObjectType.NONE;
            if (objectType != objects[i]) UpdateUI(i, objectType);
        }

        if(showUI)
        {
            elapsed += Time.deltaTime;
            if(elapsed < fadeInDur)
                canvas.alpha = Mathf.Lerp(0, 1, elapsed/fadeInDur);
            else
            {
                if (elapsed > fadeInDur) canvas.alpha = 1f;
                if (elapsed > fadeInDur + showDuration)
                    canvas.alpha = Mathf.Lerp(1, 0, (elapsed - (fadeInDur + showDuration)) / fadeOutDur);
            }
                
        }
    }

    void UpdateUI(int playerID, GameObjectType type)
    {
        objects[playerID] = type;
        Sprite newSprite = getSpriteFromGameObjectType(type);
        if (!newSprite) ObjectsImg[playerID].enabled = false;
        else
        {
            ObjectsImg[playerID].enabled = true;
            elapsed = 0;
            showUI = true;
        }
        ObjectsImg[playerID].sprite = newSprite;
    }

    Sprite getSpriteFromGameObjectType(GameObjectType type)
    {
        switch(type)
        {
            case GameObjectType.Bone: return sprite_Bone;
            case GameObjectType.Bottle: return sprite_Bottle;
            case GameObjectType.Canette: return sprite_Canette;
            case GameObjectType.ChampiBon: return sprite_ChampiBon;
            case GameObjectType.ChampiPoison: return sprite_ChampiPoison;
            case GameObjectType.Cookies: return sprite_Cookies;
            case GameObjectType.PlasticDuck: return sprite_PlasticDuck;
            default: return null;
        }
    }
}
