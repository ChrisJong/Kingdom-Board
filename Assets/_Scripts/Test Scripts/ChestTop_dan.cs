namespace Testing
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ChestTop_dan : MonoBehaviour
    {
        private Animator anim;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        //Opens the chest tops to reveal the game board
        //should be automatically played after the menu
        //is hidden during the main menu match-making
        public void OpenBoard()
        {
            anim.Play("ChestTop_Open");
        }

        //Closes the chest tops once the match is over
        //and return the players to the main menu
        //should automatically trigger show menu
        //once the animation is finished
        public void CloseBoard()
        {
            anim.Play("ChestTop_Close");
        }

        //slides open the sliders
        //to show the menu buttons
        public void ShowMenu()
        {
            anim.Play("ChestTop_ShowMenu");
        }

        //slides close the sliders
        //to hide the menu buttons
        //should automatically trigger opening the
        //chest tops after the sliders close
        //NOTE: in the event that we want to close the sliders
        //if the player loses connection to mm servers
        //will need to rework this to not automatically
        //open the chest tops!
        public void HideMenu()
        {
            anim.Play("ChestTop_HideMenu");
        }

        public void RevealFindGameButton()
        {
            anim.Play("ChestTop_RevealFindGameButton");
        }

        public void HideFindGameButton()
        {
            anim.Play("ChestTop_HideFindGameButton");
        }

        public void ShowFindGameButton()
        {
            anim.Play("ChestTop_ShowFindGameButton");
        }

        public void ShowCancelButton()
        {
            anim.Play("ChestTop_ShowCancelFindGameButton");
        }
    }

}
