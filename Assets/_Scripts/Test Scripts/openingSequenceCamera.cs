using UnityEngine;

public class openingSequenceCamera : MonoBehaviour
{
    //can put this script inside player camera
    //starting positions are
    //p1: ( 20, 16, -26) (55,   0, 0)
    //p2: (-20, 16,  26) (55, 180, 0)
    private Animation anim;

    private void Start()
    {
        anim = GetComponent<Animation>();
    }

    public void MoveCameraToStartingPos(int _id)
    {
        if (_id == 0)
            anim.Play("OpeningLerpToPlayerOne");
        else
            anim.Play("OpeningLerpToPlayerTwo");
    }
}
