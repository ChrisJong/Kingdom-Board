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

    private void Update()
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
    }

    IEnumerator FadeCoin(bool _in)
    {
        //Get color of the material on coin
        Color newColor = mat.color;

        //Wait a bit before fading out, if we're fading out
        if (!_in)
            yield return new WaitForSeconds(fadeDelay);

        //Loop to progressively fade in or out
        if (_in)
        {
            while (newColor.a < 1)
            {
                newColor.a += (fadeSpeed * Time.fixedDeltaTime);
                if (newColor.a > 1)
                    newColor.a = 1;

                mat.color = newColor;

                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (newColor.a > 0)
            {
                newColor.a -= (fadeSpeed * Time.fixedDeltaTime);
                if (newColor.a < 0)
                    newColor.a = 0;

                mat.color = newColor;

                yield return new WaitForFixedUpdate();
            }
        }

        //If we faded in, then we've finished fading in so flip the coin
        if (_in)
            FlipCoin();

        //If we faded out, reset the coin. currently for testing
        if (!_in)
            ResetCoin();

        yield return null;
    }

    void FlipCoin()
    {
        //0 - 1 defend and 1.000001 - 2 is attack or whatever floating point
        //Remove this and use preferred RNG
        string animationName;

        int rng = UnityEngine.Random.Range(1, 100);
        Debug.Log(rng);

        if (rng > 51)
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

        StartCoroutine(FadeCoin(false));
    }
}
