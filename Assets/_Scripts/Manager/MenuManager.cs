namespace KingdomBoard.Manager {

    using UnityEngine;
	using UnityEngine.UI;
    using UnityEngine.EventSystems;

	using Constants;
    using Extension;
	using MainMenu;
	using UI;

    public class MenuManager : SingletonMono<MenuManager> {

		#region FIELDS

		[SerializeField] private MenuCamera _menuCamera = null;

		[SerializeField] private bool _isSettingOpen = false;
		[SerializeField] private bool _isFindingGame = false;

		[SerializeField] private Transform _chestTop;
		[SerializeField] private Transform _menuButtons = null;
        [SerializeField] private Transform _findGame = null;
        [SerializeField] private Transform _aiGame = null;

        [Space]
        [SerializeField] private Camera _camera = null;

        [Space]
        [SerializeField] private Animator _settingAnimator = null;
        [SerializeField] private Animator _chestTopAniamtor = null;

		[SerializeField] private Canvas _mainUI = null;
		[SerializeField] private Button _connectBTN = null;
		[SerializeField] private ConnectButton _connect = null;

		public delegate void MainMenuCallBack();

        #endregion

        #region UNITY

        protected override void Awake() {
            base.Awake();

			DontDestroyOnLoad(this);

            this.Init();

			this.GameOpen();
        }

        public void Update() {
            this.UpdateClass();
        }

        #endregion

        #region CLASS
        public override void Init() {

			if(this._mainUI == null) {
				if(this.transform.Find(UIValues.UI_SUFFIX).GetComponent<Canvas>() == null)
					Debug.LogError("No Canvas Component Or Main UI Found within the Menu Manager!");
				else
					this._mainUI = this.transform.Find(UIValues.UI_SUFFIX).GetComponent<Canvas>();
			}

			if(this._connectBTN == null) {
				if(this._mainUI.transform.Find("Connect" + UIValues.BUTTON_SUFFIX).GetComponent<Button>() == null)
					Debug.LogError("No Button Component Or Connect Button Found within the Main UI");
				else {
					this._connectBTN = this._mainUI.transform.Find("Connect" + UIValues.BUTTON_SUFFIX).GetComponent<Button>();
					if(this._connect == null) {
						if(this._connectBTN.GetComponent<ConnectButton>() == null) {
							Debug.Log("No Connect Button Script attached to the connect button");
							this._connect = this._connectBTN.gameObject.AddComponent<ConnectButton>();
						} else
							this._connect = this._connectBTN.GetComponent<ConnectButton>();
					}
				}
			}
			this._connectBTN.gameObject.SetActive(false);

            if(this._settingAnimator == null) {
                if(this.transform.Find("SettingsMenu").GetComponent<Animator>() == null)
                    Debug.LogError("No Settings Menu Found in the MenuManager");
                else
                    this._settingAnimator = this.transform.Find("SettingsMenu").GetComponent<Animator>();
            }

            if(this._camera == null) {
				if(this.transform.Find("Camera").GetComponent<Camera>() == null)
					Debug.Log("Creating Menu Camera");
				else {
					this._camera = this.transform.Find("Camera").GetComponent<Camera>();

					if(this._camera.GetComponent<MenuCamera>() == null)
						this._menuCamera = this._camera.gameObject.AddComponent<MenuCamera>();
					else
						this._menuCamera = this._camera.GetComponent<MenuCamera>();
				}

            } else {
				this._menuCamera = this._camera.GetComponent<MenuCamera>();
			}

			if(this._chestTop == null) {
				if(GameObject.FindGameObjectWithTag("ChestTop") == null) {
					Debug.LogError("No Chest Top Exists in the Scene!");
				} else {
					this._chestTop = GameObject.FindGameObjectWithTag("ChestTop").transform;
				}
			}

            if(this._chestTopAniamtor == null) {
                if(GameObject.FindGameObjectWithTag("ChestTop") == null)
                    Debug.LogError("No Chest Top Exists in the Scene!");
                else
                    this._chestTopAniamtor = GameObject.FindGameObjectWithTag("ChestTop").GetComponent<Animator>();
            }

			if(this._menuButtons == null) {
				if(this._chestTop.Find("MenuButtons") == null) {
					Debug.LogError("No Menu buttons found on the scene!");
				} else {
					this._menuButtons = this._chestTop.Find("MenuButtons");

					if(this._aiGame == null) {
						if(this._menuButtons.Find("AIGameButton") == null) {
							Debug.LogError("No AI Game Button Found!");
						} else {
							this._aiGame = this._menuButtons.Find("AIGameButton"); 
						}
					}

					if(this._findGame == null) {
						if(this._menuButtons.Find("FindGameButton") == null) {
							Debug.LogError("No Find Game Button Found!");
						} else {
							this._findGame = this._menuButtons.Find("FindGameButton");
						}
					}
				}
			}

        }

        public void UpdateClass() {
            if(Input.GetMouseButtonUp(0)) {
                this.CastRay();
            }
        }

        public void InActive() {
            if(this.gameObject.activeSelf)
                this.gameObject.SetActive(false);
            else
                Debug.LogError("Main Menu Already De-Active");
        }

        public void Active() {
            if(!this.gameObject.activeSelf)
                this.gameObject.SetActive(true);
            else
                Debug.LogError("Main Menu Already Active");
        }

		public void ConnectToServer() {
			if(NetworkManager.instance != null)
				NetworkManager.instance.ConnectToServer();
		}

        public void ConnectedToServer() {
			if(instance != null)
				this.RevealFindGameButton();
		}

        public void DisonnectedToServer() {
			if(instance != null)
				this.HideFindGameButtons();
        }

		public void OfflineMode() {
			this._connectBTN.gameObject.SetActive(true);
		}

        public void Search() {
			if(instance != null) {
				this._isFindingGame = true;
				this.ShowCancelButton();
				NetworkManager.instance.SearchGameRoom();
			}
		}

        public void CancelSearch() {
			if(instance != null) {
				this._isFindingGame = false;
				this.ShowFindGameButtons();
				NetworkManager.instance.CancelSearchGameRoom();
			}
        }

		public void StartGame() {
			this.HideMenu();
			Invoke("OpenBoard", 3.0f);
			Invoke("OnStartGame", 5.0f);
		}

		public void OnStartGame() {
			if(NetworkManager.instance != null)
				NetworkManager.instance.StartGame();

			if(NetworkManager.instance.isMasterClient) {
				// RNG Sequence goes here.
				int rng = UnityEngine.Random.Range(1, 100);
				if(rng <= 50)
					Debug.Log("Player On Attack");
				else
					Debug.Log("Player on Defense");
			}

			this.OpeningSequence();
		}

		public void OpeningSequence() {

		}

        private bool CastRay() {

            RaycastHit hitInfo;
            Ray ray = this._camera.ScreenPointToRay(Input.mousePosition);

            if(EventSystem.current.IsPointerOverGameObject())
                return false;

            Debug.DrawRay(ray.origin, ray.direction * 50.0f, Color.red);

            Physics.Raycast(ray, out hitInfo, 100.0f);

            // Check to see if we hit the settings panel.
            if(hitInfo.transform == this._settingAnimator.transform)
                this.ToggleSettings();

            // Check To see if we hit any of the buttons.
            if(hitInfo.transform == this._findGame) {

				if(!this._isFindingGame)
					this.Search();
				else
					this.CancelSearch();
            }

            if(hitInfo.transform == this._aiGame) {
                Debug.Log("AI GAME PLAY");
            }

            return true;
        }
        #endregion

        #region SETTINGS_MENU
        private void ToggleSettings() {
            if(!this._isSettingOpen)
                this.OpenSettings();
            else
                this.CloseSettings();
        }

        private void OpenSettings() {
            if(this._settingAnimator != null) {
                this._settingAnimator.Play("MoveToCamera");
                this._isSettingOpen = true;
            } else
                Debug.LogError("No Settings Animator/Menu Found!");
        }

        private void CloseSettings() {
            if(this._settingAnimator != null) {
                this._settingAnimator.Play("ReturnToBoard");
                this._isSettingOpen = false;
            } else
                Debug.LogError("No Settings Animator/Menu Found!");
        }
        #endregion

        #region CHEST_TOP
		private void GameOpen() {
			this._menuCamera.PlayAnimOpeningSequence();
			Invoke("ShowMenu", 5.0f);
		}

        private void OpenBoard() {
            this._chestTopAniamtor.Play("ChestTop_Open");
        }

        private void CloseBoard() {
            this._chestTopAniamtor.Play("ChestTop_Close");
        }

        public void ShowMenu() {
            this._chestTopAniamtor.Play("ChestTop_ShowMenu");
			Invoke("ConnectToServer", 2.0f);
        }

        public void HideMenu() {
            this._chestTopAniamtor.Play("ChestTop_HideMenu");
        }

        private void RevealFindGameButton() {
            this._chestTopAniamtor.Play("ChestTop_RevealFindGameButton");
        }

        private void ShowFindGameButtons() {
            this._chestTopAniamtor.Play("ChestTop_ShowFindGameButton");
        } 

        private void HideFindGameButtons() {
            this._chestTopAniamtor.Play("ChestTop_HideFindGameButton");
        }

        private void ShowCancelButton() {
            this._chestTopAniamtor.Play("ChestTop_ShowCancelFindGameButton");
        }

        #endregion
    }
}