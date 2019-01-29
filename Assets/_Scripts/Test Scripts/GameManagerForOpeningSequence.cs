using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerForOpeningSequence : MonoBehaviour
{
    public GameObject[] boardTops = new GameObject[2]; //the board covers, need to flip these when ready

    public GameObject playerOne; //using public here to not have to create extra scripts
    public coinTest playerOneCoin; //the coin which belongs to player one
    public openingSequenceCamera playerOneCamera; //the camera which belongs to player one, replace this with actual camera
    public turnbanner_dan playerOneTurnBanner; //the turn banner which belongs to player one

    public float openBoardDuration = 3f;
    public float coinFlipDuration = 2f;
    public float moveCamerasDuration = 1.2f;

    int currentTurnPlayerId = -1;

    private void Awake()
    {
        RegisterPlayers();
    }

    private void RegisterPlayers()
    {
        //Some function I assume you're using to get the players

        OnPlayersRegistered();
    }

    private void OnPlayersRegistered()
    {
        //Do whatever initializations required and get their camera component
        //Alternatively, you could call a function on the camera script to do the opening sequence
        //But I would like to time the camera as its finishing panning to begin the phase banners

        OpenBoard();
    }

    private void OpenBoard()
    {
        for (int i = 0; i < boardTops.Length; i++)
        {
            Animator anim = boardTops[i].GetComponent<Animator>();
            anim.Play("OpenBoard");
        }

        Invoke("DetermineFirstPlayer", openBoardDuration);
    }

    private void DetermineFirstPlayer()
    {
        //figure out who goes first
        if (Random.Range(1, 100) > 50)
            currentTurnPlayerId = 0;
        else
            currentTurnPlayerId = 1;

        FlipCoin();
    }

    private void FlipCoin()
    {
        switch (currentTurnPlayerId)
        {
            case 0:
                playerOneCoin.BeginFlipSequence(true);
                //playerTwoCoin.BeginFlipSequence(false);
                break;
            case 1:
                playerOneCoin.BeginFlipSequence(false);
                //playerTwoCoin.BeginFlipSequence(true);
                break;
        }

        Invoke("MoveCameras", coinFlipDuration);
    }

    private void MoveCameras()
    {
        switch (currentTurnPlayerId)
        {
            case 0:
                playerOneCamera.MoveCameraToStartingPos(0);
                //playerTwoCamera.MoveCameraToStartingPos(1);
                break;
            case 1:
                playerOneCamera.MoveCameraToStartingPos(1);
                //playerTwoCamera.MoveCameraToStartingPos(0);
                break;
        }

        Invoke("StartGame", moveCamerasDuration);
    }

    private void StartGame()
    {
        switch (currentTurnPlayerId)
        {
            case 0:
                playerOneTurnBanner.PlayBannerAnimation(true);
                //playerTwoTurnBanner.PlayBannerAnimation(false);
                break;
            case 1:
                playerOneTurnBanner.PlayBannerAnimation(false);
                //playerTwoTurnBanner.PlayBannerAnimation(true);
                break;
        }

        //continue game as normal
    }
}
