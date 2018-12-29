using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class turnbanner_dan : MonoBehaviour
{
    [Header("Turn Banner Panel Objects")]
    [SerializeField] private Image bannerImageObj;
    [SerializeField] private Image frontRibbonObj;
    [SerializeField] private Image phaseTextObj;
    
    [Space]
    [Header("Turn Banner Panel Objects")]
    [SerializeField] private Sprite attackBannerImage;
    [SerializeField] private Sprite defenceBannerImage;
    [SerializeField] private Sprite attackPhaseTextImage;
    [SerializeField] private Sprite defencePhaseTextImage;

    private Animation bannerImageAnim;
    private Animation frontRibbonAnim;
    private Animation phaseTextAnim;

    private enum PhaseState { attack, defence };
    private PhaseState currentState = PhaseState.attack;

    private bool isAnimating = false;

    public float bannerLingerTimer = 1f;

    private void Start()
    {
        bannerImageAnim = bannerImageObj.GetComponent<Animation>();
        frontRibbonAnim = frontRibbonObj.GetComponent<Animation>();
        phaseTextAnim = phaseTextObj.GetComponent<Animation>();

        //PlaySpawnAnimation();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isAnimating)
            {
                Debug.Log("Still animating!");
                return;
            }

            PlaySpawnAnimation();
        }
    }

    private void PlaySpawnAnimation()
    {
        Debug.Log("Starting Spawn Animation");

        isAnimating = true;

        bannerImageAnim.Play("BannerSpawn");
        frontRibbonAnim.Play("RibbonFadeIn");
        phaseTextAnim.Play("PhaseTextFadeIn");

        float spawnAnimTimer = phaseTextAnim.GetClip("PhaseTextFadeIn").length;

        Invoke("PlayEndAnimation", spawnAnimTimer + bannerLingerTimer);
    }

    private void PlayEndAnimation()
    {
        Debug.Log("Starting Fade Out Animation");

        bannerImageAnim.Play("TurnBannerFadeOut");
        frontRibbonAnim.Play("TurnBannerFadeOut");
        phaseTextAnim.Play("TurnBannerFadeOut");

        float endAnimationTimer = bannerImageAnim.GetClip("TurnBannerFadeOut").length;

        Invoke("OnEndAnimation", endAnimationTimer);
    }

    private void OnEndAnimation()
    {
        Debug.Log("Animation Has Ended");

        if (currentState == PhaseState.attack)
        {
            currentState = PhaseState.defence;

            bannerImageObj.sprite = defenceBannerImage;
            phaseTextObj.sprite = defencePhaseTextImage;
        }
        else if (currentState == PhaseState.defence)
        {
            currentState = PhaseState.attack;

            bannerImageObj.sprite = attackBannerImage;
            phaseTextObj.sprite = attackPhaseTextImage;
        }
        else
        {
            Debug.Log("Error: Current State out of index");
        }

        isAnimating = false;
    }
}
