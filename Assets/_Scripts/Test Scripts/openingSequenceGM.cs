namespace Testing
{

    using System;
    using UnityEngine;

    /// -----------------------------------------------------Notes:---------------------------------------------------------------------------------------
    /// Rather than a game manager type class, this should be more like a lobby manager or some other similar manager which utilises match making services
    /// I have combined both scripts (lobby and game managers into this one script because lazy)
    /// But lobby manager should handle the chest board top animations etc.
    /// Whilst the opening sequence with the coin and stuff should be left to the game manager
    /// All function names are arbitrary and should be replaced with appropriate function names from PUN library
    /// Will need to include Unity.SceneManagement
    /// 
    /// 
    /// --------------------------------------------------------------------------------------------------------------------------------------------------


    public class openingSequenceGM : MonoBehaviour
    {

        #region Singleton

        //Surely, something will want to reference this
        //In the event there are multiple instances of this
        //Destroy the old copy and replace with the new
        //If thats desirable or not can be easily changed
        //By destroying the new version instead

        public static openingSequenceGM Singleton;

        private void Awake()
        {
            if (Singleton != null)
                Destroy(Singleton.gameObject);

            Singleton = this;
        }

        #endregion

        #region LobbyManagerSection

        public ChestTop_dan chestTop;

        //when the game finishes loading
        //pop out the menu from the chest top
        public void OnGameLoaded()
        {
            playerCam.PlayOpenSequenceMoveToTop();
            Invoke("ShowChestTopMenu", 5f);
        }

        public void ShowChestTopMenu()
        {
            chestTop.ShowMenu();
        }

        //should be an override on PUN similar to this
        //use this to show the menu buttons once the player
        //has connected to match making servers
        public void OnConnectedToMaster()
        {
            chestTop.RevealFindGameButton();
        }

        //should be an override on PUN similar to this
        //use when disconnected from the server
        //and hide the find game button
        public void OnDisconnectedFromMaster()
        {
            chestTop.HideFindGameButton();
        }

        //some function when the find game button is
        //pressed to show the cancel find game button
        public void FindGame()
        {
            chestTop.ShowCancelButton();
        }

        //some function when the cancel find game button
        //is pressed to show the find game button again
        public void CancelFindGame()
        {
            chestTop.ShowFindGameButton();
        }

        //some function when PUN matches the player with
        //another player, not sure of the override so
        //using some random function name here
        public void OnOpponentFound()
        {
            chestTop.HideMenu();
        }

        //some overide on PUN similar to this
        //use this to hide the menu once the player
        //has found an opponent and can no longer cancel
        //once the slider has finished closing
        //the controller automatically transitions into
        //opening the chest board tops
        public void OnSceneLoaded()
        {
            chestTop.OpenBoard();
            Invoke("OnGameStart", 2f); //using invoke here cause i dont have any callbacks or animation events currently
        }

        //some function which notifies the server that
        //the match is over and should return both players
        //to the main menu and close the chest board tops
        //once the tops close, the controller auto shows menu
        public void OnMatchEnded()
        {
            chestTop.CloseBoard();
        }

        #endregion

        #region GameManagerSection

        /// These variables should probably be controlled by the player
        /// and the gm tells the player to do these stuffs
        public coinTest coin; //the coin which will be flipped on the player's screens.. can be given different targeted RPC arguments to show different animations
        public openSeqCamera playerCam; //the player camera script, currently using this for opening sequence only
        public turnbanner_dan banner; //the banner which we will control here because not creating player script
        public SettingsMenuDan settingsMenu; //the settings menu

        private bool settingsMenuShowing = false;

        private bool playerOneIsAttacking; //replace this with however which player is first is implemented: maybe something like players[i].IsTurn or something

        public void OnGameStart()
        {
            Debug.Log("OnGameStart() called");

            int rng = UnityEngine.Random.Range(1, 100);
            if (rng < 51)
                playerOneIsAttacking = true;
            else
                playerOneIsAttacking = false;

            OpenSequence();
        }

        /// using delegate here so i dont need to put animation events everywhere
        public delegate void GMCallBacks();

        private void OpenSequence()
        {
            StartCoroutine(coin.BeginFlipSequence(playerOneIsAttacking, MoveCamera));
        }

        private void MoveCamera()
        {
            playerCam.PlayOpenSequenceMoveToCastle(playerOneIsAttacking, PlayBannerAnimation);
        }

        private void PlayBannerAnimation()
        {
            banner.PlayBannerAnimation(playerOneIsAttacking);
        }

        private void ToggleSettingsMenu()
        {
            settingsMenuShowing = !settingsMenuShowing;

            if (settingsMenuShowing)
                settingsMenu.MoveToCamera();
            else
                settingsMenu.ReturnToBoard();
        }

        #endregion

        //using update here for now since we don't have PUN imported
        //only for testing purposes to play the animations
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                OnGameLoaded();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                OnConnectedToMaster();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                OnDisconnectedFromMaster();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                FindGame();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                CancelFindGame();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                OnOpponentFound();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                OnSceneLoaded();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                OnMatchEnded();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ToggleSettingsMenu();
            }
        }
    }

}
