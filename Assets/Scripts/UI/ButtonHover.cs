using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Sprite ImageHover;
    public Sprite ImageNormal;

    public Image buttonImage;

    private SceneController controller;

    public Animator anim;

    private bool state;

    public TriggerPopUp article;
    public TriggerPopUp credits;
    public TriggerPopUp controls;

    public void Awake()
    {
        StartCoroutine(skipFirstAnim());
        state = EventSystem.current.currentSelectedGameObject == gameObject;
        controller = GameObject.FindObjectOfType<SceneController>();
    }

    public void Start()
    {
        if(gameObject.name == "quit")
        {
            Button btn = GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { FindObjectOfType<GameManager>().Quit(); });
        }
    }

    public void Update()
    {
        bool newState = EventSystem.current.currentSelectedGameObject == gameObject;
        if (state != newState)
        {
            PointerEventData data = new PointerEventData(EventSystem.current);
            if (newState)
                OnPointerEnter(data);
            else
                OnPointerExit(data);
            state = newState;
        }
    }

    public void displayForSelected(string selected)
    {
        switch(selected)
        {
            case "play":
                {
                    article.setBool(true);
                    controls.setBool(false);
                    credits.setBool(false);
                }
                break;
            case "controls":
                {
                    article.setBool(false);
                    credits.setBool(false);
                    controls.setBool(true);
                }
                break;
            case "credit":
                {
                    article.setBool(false);
                    credits.setBool(true);
                    controls.setBool(false);
                }
                break;
            case "quit":
                {
                    article.setBool(false);
                    credits.setBool(false);
                    controls.setBool(false);
                }
                break;
        }
    }

    IEnumerator skipFirstAnim()
    {
        anim.speed = 50000000f;
        yield return new WaitForSeconds(0.01f);
        anim.speed = 1f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AkSoundEngine.PostEvent("menu_scroll", gameObject);
        anim.SetBool("Hover", true);
        buttonImage.sprite = ImageHover;
        displayForSelected(gameObject.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.SetBool("Hover", false);
        buttonImage.sprite = ImageNormal;
    }
}
