using System;
using System.Collections;
using UnityEngine;

public class coinTest : MonoBehaviour
{
    Animator anim;
    Material mat;

    private void Start()
    {
        anim = GetComponent<Animator>();
        mat = GetComponent<Renderer>().material;
    }

    public IEnumerator BeginFlipSequence(bool _isAtk)
    {
        FadeCoin(true);
        yield return new WaitForSeconds(0.25f);
        FlipCoin(_isAtk);

        yield return new WaitForSeconds(2.75f);

        FadeCoin(false);

        yield return null;
    }

    private void FadeCoin(bool _in)
    {
        string fade = (_in) ? "in" : "out";
        Debug.Log("Fading " + fade);

        string animationName;

        if (_in)
            animationName = "CoinFadeIn";
        else
            animationName = "CoinFadeOut";

        anim.Play(animationName, 0);
    }

    void FlipCoin(bool _isAtk)
    {
        string animationName;

        if (_isAtk)
            animationName = "CoinAttack";
        else
            animationName = "CoinDefend";

        anim.Play(animationName, 1);
    }
}
