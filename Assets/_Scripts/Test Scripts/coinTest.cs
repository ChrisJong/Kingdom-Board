using UnityEngine;

public class coinTest : MonoBehaviour
{
    public GameObject coin;
    Animator anim;

    bool atk = true;

    private void Start()
    {
        anim = coin.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (atk)
            {
                anim.Play("CoinAttack");
                atk = false;
            }
            else
            {
                anim.Play("CoinDefend");
                atk = true;
            }
        }
    }
}
