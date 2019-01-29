using System;
using System.Collections;
using UnityEngine;

public class coinTest : MonoBehaviour
{
    Animator anim;  //animation component of the coin
    Material mat;   //material component of the coin

    bool hasBeenFlipped = false;    //quick check to see if this coin has been flipped yet

    bool hasFadedOut = false;   //redundant bool to check if the coin has finished fading out, only use for reset function

    float fadeSpeed = 1f;   //how quickly the coin fades in or out
    float fadeDelay = 1f;   //delay after the coin lands before we begin to fade out

    private void Start()
    {
        //get the components we need to work with

        anim = GetComponent<Animator>();
        mat = GetComponent<Renderer>().material;
    }

    /*private void Update()
    {
        //press space to begin fading in the coin

        if (Input.GetKeyDown(KeyCode.Space) && !hasBeenFlipped)
        {
            StartCoroutine(FadeCoin(true));
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCoin();
        }
    }*/

    public void BeginFlipSequence(bool _isAtk)
    {
        StartCoroutine(FadeInCoin(_isAtk));
    }

    IEnumerator FadeInCoin(bool _isAtk)
    {
        Color newColor = mat.color;

        while (newColor.a < 1)
        {
            newColor.a += (fadeSpeed * Time.fixedDeltaTime);
            if (newColor.a > 1)
                newColor.a = 1;

            mat.color = newColor;

            yield return new WaitForFixedUpdate();
        }

        FlipCoin(_isAtk);
    }

    IEnumerator FadeOutCoin()
    {
        yield return new WaitForSeconds(fadeDelay);

        //Get color of the material on coin
        Color newColor = mat.color;
        
        while (newColor.a > 0)
        {
            newColor.a -= (fadeSpeed * Time.fixedDeltaTime);
            if (newColor.a < 0)
                newColor.a = 0;

            mat.color = newColor;

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    void FlipCoin(bool _isAtk)
    {
        //0 - 1 defend and 1.000001 - 2 is attack or whatever floating point
        //Remove this and use preferred RNG
        string animationName;

        /*int rng = UnityEngine.Random.Range(1, 100);
        Debug.Log(rng);

        if (rng > 51)
            animationName = "CoinAttack";
        else
            animationName = "CoinDefend";
        */

        if (_isAtk)
            animationName = "CoinAttack";
        else
            animationName = "CoinDefend";

        anim.Play(animationName);
    }

    void ResetCoin()
    {
        if (!hasFadedOut)
            return;

        hasBeenFlipped = false;
    }

    public void OnAnimationComplete()
    {
        //callback for when the animation finishes
        //this script must be placed on the coin itself
        //for this callback to work
        //insert extra code here for any callback functionalities required

        StartCoroutine(FadeOutCoin());
    }
}
