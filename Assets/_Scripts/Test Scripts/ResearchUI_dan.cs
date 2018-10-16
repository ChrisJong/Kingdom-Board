using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchUI_dan : MonoBehaviour {

    #region Singleton

    public static ResearchUI_dan instance;

    private void Awake()
    {
        if (instance != null)
        {
            //Debug.LogWarning("Error: Multiple Research UI Managers detected");
            return;
        }

        instance = this;
        //Debug.Log("Singleton Found: Research UI Manager Instance Initialised");
    }

    #endregion

    #region Variables

    [SerializeField] private ResearchCard_dan cardPrefab;

    public enum ShownCardsState { none, classes, classesFading, preparingSecondSet, units, upgrades, end };
    public ShownCardsState currentState = ShownCardsState.none;

    private ResearchCard_dan[] researchCards = new ResearchCard_dan[3];
    private int clickedCardID = -1;

    private int chosenClass = -1;
    private bool hasChoice = true;

    [SerializeField] private List<int> unitTurns = new List<int>();
    private bool isUnitTurn = false;


    [SerializeField] private Vector3[] threeOptionsPositions = new Vector3[3];
    [SerializeField] private Vector3[] twoOptionsPositions = new Vector3[2];

    [SerializeField] private Sprite[] classSprites;
    [SerializeField] private Sprite[] baseClassUnitSprites;
    [SerializeField] private Sprite[] unitSprites = new Sprite[6];
    [SerializeField] private Sprite[] upgradeSprites;

    private bool[] hasResearchedBaseClass = new bool[3];
    private int classesUnlocked = 0;

    private bool[] hasResearchedAdvancedUnit = new bool[6];

    private int turnCounter;
    [SerializeField] private Text turnCounterText;

    #endregion

    #region Functionality

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            hasResearchedBaseClass[i] = false;
        }

        turnCounter = 1;
        turnCounterText.text = "Turn: " + turnCounter;

        isUnitTurn = true;
    }

    public void CreateResearchCards()
    {
        if (currentState != ShownCardsState.none)
        {
            Debug.Log("Already showing cards");
            return;
        }

        currentState = ShownCardsState.classes;


        if (isUnitTurn || classesUnlocked == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                researchCards[i] = Instantiate(cardPrefab, this.transform);
                researchCards[i].SetUpCard(i, i);
                researchCards[i].GiveCardNewOppositeFace(classSprites[i]);
                researchCards[i].transform.position = threeOptionsPositions[i];
            }
        }
        else if (classesUnlocked == 1)
        {
            hasChoice = false;

            for (int c = 0; c < 3; c++)
            {
                if (!hasResearchedBaseClass[c])
                {
                    continue;
                }
                else
                {
                    researchCards[0] = Instantiate(cardPrefab, this.transform);
                    researchCards[0].SetUpCard(0, c);
                    researchCards[0].GiveCardNewOppositeFace(classSprites[c]);
                    researchCards[0].transform.position = threeOptionsPositions[1];

                    break;
                }
            }
        }
        else if (classesUnlocked == 2)
        {
            int newCardID = 0;

            for (int c = 0; c < 3; c++)
            {
                if (!hasResearchedBaseClass[c])
                {
                    continue;
                }
                else
                {
                    researchCards[newCardID] = Instantiate(cardPrefab, this.transform);
                    researchCards[newCardID].SetUpCard(newCardID, c);
                    researchCards[newCardID].GiveCardNewOppositeFace(classSprites[c]);
                    researchCards[newCardID].transform.position = twoOptionsPositions[newCardID];

                    newCardID++;

                    if (newCardID == 2)
                    {
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.Log("HUGE ERROR: Is Research Turn, but invalid amount of Classes Unlocked");
        }
    }

    public void ResolveResearchChoice()
    {
        string classType = null;
        if (chosenClass == 0)
        {
            classType = "Melee ";
        }
        else if (chosenClass == 1)
        {
            classType = "Ranged ";
        }
        else if (chosenClass == 2)
        {
            classType = "Magic ";
        }
        else
        {
            Debug.Log("HUGE ERROR: Invalid Class Type ID");
        }

        if (currentState == ShownCardsState.units)
        {
            string researchedUnit = null;

            if (researchCards[clickedCardID].IsBasicUnit())
            {
                hasResearchedBaseClass[clickedCardID] = true;
                classesUnlocked++;

                if (chosenClass == 0)
                {
                    researchedUnit = "Warrior";
                }
                else if (chosenClass == 1)
                {
                    researchedUnit = "Archer";
                }
                else if (chosenClass == 2)
                {
                    researchedUnit = "Mage";
                }
                else
                {
                    Debug.Log("HUGE ERROR: Invalid Basic Unit ID");
                }
            }
            else
            {
                if (chosenClass == 0)
                {
                    if (clickedCardID == 0)
                    {
                        researchedUnit = "Knight";
                    }
                    else if (clickedCardID == 1)
                    {
                        researchedUnit = "Guardian";
                    }
                    else
                    {
                        Debug.Log("HUGE ERROR: Invalid Unit ID");
                    }
                }
                else if (chosenClass == 1)
                {
                    if (clickedCardID == 0)
                    {
                        researchedUnit = "Crossbow";
                    }
                    else if (clickedCardID == 1)
                    {
                        researchedUnit = "Longbow";
                    }
                    else
                    {
                        Debug.Log("HUGE ERROR: Invalid Unit ID");
                    }
                }
                else if (chosenClass == 2)
                {
                    if (clickedCardID == 0)
                    {
                        researchedUnit = "Wizard";
                    }
                    else if (clickedCardID == 1)
                    {
                        researchedUnit = "Cleric";
                    }
                    else
                    {
                        Debug.Log("HUGE ERROR: Invalid Unit ID");
                    }
                }
                else
                {
                    Debug.Log("HUGE ERROR: Invalid Class ID");
                }
            }

            Debug.Log("You unlocked a new Unit: " + researchedUnit);
        }
        else if (currentState == ShownCardsState.upgrades)
        {
            string researchedUpgrade = null;

            if (researchCards[clickedCardID].UpgradeType() == 0)
            {
                researchedUpgrade = "Attack";
            }
            else if (researchCards[clickedCardID].UpgradeType() == 1)
            {
                researchedUpgrade = "Health";
            }
            else if (researchCards[clickedCardID].UpgradeType() == 2)
            {
                researchedUpgrade = "Stamina";
            }
            else
            {
                Debug.Log("HUGE ERROR: Invalid Upgrade ID");
            }
            Debug.Log("You upgraded " + classType + researchedUpgrade);
        }
        else
        {
            Debug.Log("HUGE ERROR: Resolving Choice in wrong phase");
        }
        for (int i = 0; i < 3; i++)
        {
            if (researchCards[i])
            {
                researchCards[i].PlayFadeCardAnimation();
            }
        }

        currentState = ShownCardsState.end;
    }

    #endregion

    #region Sequence

    public void CardWasClicked(int _cardID)
    {
        clickedCardID = _cardID;

        if (currentState == ShownCardsState.classes)
        {
            chosenClass = researchCards[clickedCardID].ClassType();

            for (int i = 0; i < 3; i++)
            {
                if (i != clickedCardID && researchCards[i])
                {
                    researchCards[i].PlayFadeCardAnimation();
                }
            }

            if (!hasChoice)
            {
                currentState = ShownCardsState.preparingSecondSet;

                StartCoroutine(researchCards[clickedCardID].BeginCardRotation());
            }
            else
            {
                currentState = ShownCardsState.classesFading;
            }

        }
        else if (currentState == ShownCardsState.units || currentState == ShownCardsState.upgrades)
        {
            ResolveResearchChoice();
        }
        
    }

    public void CardWasFaded(int _cardID)
    {
        researchCards[_cardID] = null;

        if (currentState == ShownCardsState.classesFading)
        {
            StartCoroutine(researchCards[clickedCardID].BeginCardRotation());

            currentState = ShownCardsState.preparingSecondSet;
        }
        else if (currentState == ShownCardsState.end)
        {
            currentState = ShownCardsState.none;
            NextTurn();
        }
    }

    public void CardReadyToChange(int _cardID)
    {
        if (currentState != ShownCardsState.preparingSecondSet)
        {
            return;
        }

        Sprite spriteToGive = null;

        if (isUnitTurn)
        {
            currentState = ShownCardsState.units;

            if (!hasResearchedBaseClass[clickedCardID])
            {
                spriteToGive = baseClassUnitSprites[clickedCardID];
                hasChoice = false;

                researchCards[clickedCardID].ChangeToBasicUnitCard(clickedCardID, true);
            }
            else
            {
                spriteToGive = unitSprites[chosenClass * 2];
                hasChoice = true;

                if (clickedCardID != 0)
                {
                    researchCards[0] = researchCards[clickedCardID];
                    researchCards[clickedCardID] = null;

                    clickedCardID = 0;
                }

                researchCards[0].ChangeToUnitCard(0, 0);

                researchCards[1] = Instantiate(cardPrefab, this.transform);
                researchCards[1].ChangeToUnitCard(1, (chosenClass * 2) + 1);
                researchCards[1].ImmediateHalfRotateWithSpriteChange(unitSprites[(chosenClass * 2) + 1]);
                researchCards[1].transform.position = researchCards[0].transform.position;
            }
        }
        else
        {
            currentState = ShownCardsState.upgrades;

            spriteToGive = upgradeSprites[clickedCardID];
            hasChoice = true;

            researchCards[clickedCardID].ChangeToUpgradeCard(clickedCardID, clickedCardID);

            for (int i = 0; i < 3; i++)
            {
                if (researchCards[i])
                {
                    continue;
                }
                else
                {
                    researchCards[i] = Instantiate(cardPrefab, this.transform);
                    researchCards[i].ChangeToUpgradeCard(i, i);
                    researchCards[i].ImmediateHalfRotateWithSpriteChange(upgradeSprites[i]);
                    researchCards[i].transform.position = researchCards[clickedCardID].transform.position;
                }
            }
        }

        researchCards[clickedCardID].GiveCardNewOppositeFace(spriteToGive);
    }

    public void CardFinishedRotating(int _cardID)
    {
        if (!hasChoice)
        {
            researchCards[_cardID].PlayClickCardAnimation();
        }

        if (!isUnitTurn && currentState == ShownCardsState.classes)
        {
            if (classesUnlocked == 1)
            {
                StartCoroutine(researchCards[_cardID].MoveCardToPosition(threeOptionsPositions[0].x));
            }
            else if (classesUnlocked == 2)
            {
                StartCoroutine(researchCards[_cardID].MoveCardToPosition(twoOptionsPositions[_cardID].x));
            }
        }

        if (hasChoice && currentState == ShownCardsState.units)
        {
            StartCoroutine(researchCards[_cardID].MoveCardToPosition(twoOptionsPositions[_cardID].x));
        }

        if (currentState == ShownCardsState.upgrades)
        {
            if (researchCards[_cardID].transform.position.x != threeOptionsPositions[_cardID].x)
            {
                StartCoroutine(researchCards[_cardID].MoveCardToPosition(threeOptionsPositions[_cardID].x));
            }
        }
    }

    #endregion

    #region Functionality

    public void NextTurn()
    {
        turnCounter++;
        turnCounterText.text = "Turn: " + turnCounter;

        isUnitTurn = unitTurns.Contains(turnCounter) ? true : false;

        clickedCardID = -1;
        hasChoice = true;
    }

    public void ResetUI()
    {
        if (currentState == ShownCardsState.none)
        {
            Debug.Log("Cards not showing");
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            if (researchCards[i] != null)
            {
                Destroy(researchCards[i].gameObject);
                researchCards[i] = null;
            }
        }

        clickedCardID = -1;
        hasChoice = true;

        currentState = ShownCardsState.none;
    }

    #endregion
}
