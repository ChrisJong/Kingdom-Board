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

    private enum ShownCardsState { none, classes, classesFading, preparingSecondSet, units, upgrades, backToClasses, end };
    private static ShownCardsState currentState = ShownCardsState.none;

    private ResearchCard_dan[] researchCards = new ResearchCard_dan[3];

    [SerializeField] private Button backButton;
    private Animation backButtonAnim;
    private bool backButtonIsActive = false;

    private int clickedCardID = -1;

    private int chosenClassID = -1;
    private int chosenUnitID = -1;
    private int chosenUpgradeID = -1;

    [SerializeField] private List<int> unitTurns = new List<int>();
    private bool isUnitTurn = false;

    [SerializeField] private Vector3[] threeOptionsPositions = new Vector3[3];
    [SerializeField] private Vector3[] twoOptionsPositions = new Vector3[2];

    [SerializeField] private Sprite[] classSprites;
    [SerializeField] private Sprite[] baseClassUnitSprites;
    [SerializeField] private Sprite[] unitSprites = new Sprite[6];
    [SerializeField] private Sprite[] upgradeSprites;

    [SerializeField] private Toggle[] researchedUnitsToggles = new Toggle[9];

    private int[] researchedUpgradeCounts = new int[9];
    [SerializeField] private Text[] researchedUpgradeTexts = new Text[9];

    private bool[] hasResearchedBaseClass = new bool[3];
    private int classesUnlocked()
    {
        int classes = 0;

        for (int i = 0; i < 3; i++)
        {
            if (hasResearchedBaseClass[i])
            {
                classes++;
            }
        }

        return classes;
    }

    private int lastResearchedClass = -1;
    private bool[] classIsLockedForThisTurn = new bool[3];

    private int currentOptions()
    {
        int count = 0;

        for (int i = 0; i < 3; i++)
        {
            if (researchCards[i])
            {
                count++;
            }
        }

        return count;
    }
    private bool hasChoice()
    {
        bool multipleChoices = true;

        if (currentOptions() == 1)
        {
            multipleChoices = false;
        }

        return multipleChoices;
    }

    private bool hadChoice = true;
    private int hadOptions = 0;
    private float xPosWhenClicked;
    private int cardsRemaining = 0;

    private int hadOptionsWhenBack = 0;

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

        for (int i = 0; i < 6; i++)
        {
            hasResearchedAdvancedUnit[i] = false;
        }

        for (int i = 0; i < 9; i++)
        {
            researchedUpgradeCounts[i] = 0;
            researchedUpgradeTexts[i].text = "0";
        }

        for (int i = 0; i < 3; i++)
        {
            classIsLockedForThisTurn[i] = false;
        }

        backButtonAnim = backButton.GetComponent<Animation>();

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

        int newCardID = 0;

        if (isUnitTurn)
        {
            for (int c = 0; c < 3; c++)
            {
                if (turnCounter <= unitTurns[1] && (classIsLockedForThisTurn[c]) || (hasResearchedAdvancedUnit[c * 2] && hasResearchedAdvancedUnit[(c * 2) + 1]))
                {
                    continue;
                }
                else
                {
                    researchCards[newCardID] = (CreateClassCard(newCardID, c));
                    newCardID++;
                }
            }
        }
        else
        {
            for (int c = 0; c < 3; c++)
            {
                if (!hasResearchedBaseClass[c])
                {
                    continue;
                }
                else
                {
                    researchCards[newCardID] = (CreateClassCard(newCardID, c));
                    newCardID++;
                }
            }
        }

        MoveCardsToInitialPositions();
    }

    private ResearchCard_dan CreateClassCard(int _cardID, int _classID)
    {
        ResearchCard_dan newCard = Instantiate(cardPrefab, this.transform);
        newCard.SetUpCard(_cardID, _classID, true);
        newCard.GiveCardNewOppositeFace(classSprites[_classID]);

        return newCard;
    }

    private ResearchCard_dan CreateClassCardNoSpawnAnim(int _cardID, int _classID)
    {
        ResearchCard_dan newCard = Instantiate(cardPrefab, this.transform);
        newCard.SetUpCard(_cardID, _classID, false);
        newCard.ImmediateHalfRotateWithSpriteChange(classSprites[_classID]);

        return newCard;
    }

    private ResearchCard_dan CreateUnitCard(int _cardID, int _unitID)
    {
        ResearchCard_dan newCard = Instantiate(cardPrefab, this.transform);
        newCard.SetUpCard(_cardID, chosenClassID, false);
        newCard.ChangeToUnitCard(_cardID, _unitID);
        newCard.ImmediateHalfRotateWithSpriteChange(unitSprites[(chosenClassID * 2) + _unitID]);

        return newCard;
    }

    private ResearchCard_dan CreateUpgradeCard(int _cardID, int _upgradeID)
    {
        ResearchCard_dan newCard = Instantiate(cardPrefab, this.transform);
        newCard.SetUpCard(_cardID, chosenClassID, false);
        newCard.ChangeToUpgradeCard(_cardID, _upgradeID);
        newCard.ImmediateHalfRotateWithSpriteChange(upgradeSprites[_upgradeID]);

        return newCard;
    }

    

    private void MoveCardsToInitialPositions()
    {
        if (currentOptions() == 1)
        {
            researchCards[0].transform.position = threeOptionsPositions[1];
        }
        else if (currentOptions() == 2)
        {
            for (int i = 0; i < 2; i++)
            {
                researchCards[i].transform.position = twoOptionsPositions[i];
            }
        }
        else if (currentOptions() == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                researchCards[i].transform.position = threeOptionsPositions[i];
            }
        }
        else
        {
            Debug.Log("Invalid amount of options!");
        }
    }

    private void MoveCardsToProperPositions()
    {
        if (currentState == ShownCardsState.units || currentState == ShownCardsState.upgrades || currentState == ShownCardsState.backToClasses)
        {
            if (currentState == ShownCardsState.backToClasses)
            {
                currentState = ShownCardsState.classes;
            }

            if (currentOptions() == 1)
            {
                int onlyOptionIndex = -1;

                for (int i = 0; i < 3; i++)
                {
                    if (researchCards[i])
                    {
                        if (researchCards[i].IsBasicUnit())
                        {
                            return;
                        }

                        onlyOptionIndex = i;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (onlyOptionIndex != -1)
                {
                    StartCoroutine(researchCards[onlyOptionIndex].MoveCardToPosition(threeOptionsPositions[1].x));
                }
                else
                {
                    Debug.Log("HUGE ERROR: Could not find only option card index");
                }
            }
            else if (currentOptions() == 2)
            {
                Debug.Log("moving the two cards to twoOptionsPositions");
                for (int i = 0; i < 2; i++)
                {
                    StartCoroutine(researchCards[i].MoveCardToPosition(twoOptionsPositions[i].x));
                }
            }
            else if (currentOptions() == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    StartCoroutine(researchCards[i].MoveCardToPosition(threeOptionsPositions[i].x));
                }
            }
            else
            {
                Debug.Log("Invalid amount of options!");
            }
        }
    }

    public void ResolveResearchChoice()
    {
        string classType = null;
        if (chosenClassID == 0)
        {
            classType = "Melee ";
        }
        else if (chosenClassID == 1)
        {
            classType = "Ranged ";
        }
        else if (chosenClassID == 2)
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

                if (chosenClassID == 0)
                {
                    researchedUnit = "Warrior";
                    ToggleUnitResearchedUI(0);
                }
                else if (chosenClassID == 1)
                {
                    researchedUnit = "Archer";
                    ToggleUnitResearchedUI(1);
                }
                else if (chosenClassID == 2)
                {
                    researchedUnit = "Mage";
                    ToggleUnitResearchedUI(2);
                }
                else
                {
                    Debug.Log("HUGE ERROR: Invalid Basic Unit ID");
                }

                if (turnCounter <= unitTurns[1])
                {
                    if (lastResearchedClass != -1)
                    {
                        classIsLockedForThisTurn[lastResearchedClass] = false;
                    }

                    lastResearchedClass = chosenClassID;
                    classIsLockedForThisTurn[lastResearchedClass] = true;
                }
            }
            else
            {
                if (chosenClassID == 0)
                {
                    if (researchCards[clickedCardID].UnitType() == 0)
                    {
                        researchedUnit = "Knight";

                        hasResearchedAdvancedUnit[0] = true;
                        ToggleUnitResearchedUI(3);
                    }
                    else if (researchCards[clickedCardID].UnitType() == 1)
                    {
                        researchedUnit = "Guardian";

                        hasResearchedAdvancedUnit[1] = true;
                        ToggleUnitResearchedUI(4);
                    }
                    else
                    {
                        Debug.Log("HUGE ERROR: Invalid Unit ID");
                    }
                }
                else if (chosenClassID == 1)
                {
                    if (researchCards[clickedCardID].UnitType() == 0)
                    {
                        researchedUnit = "Crossbow";

                        hasResearchedAdvancedUnit[2] = true;
                        ToggleUnitResearchedUI(5);
                    }
                    else if (researchCards[clickedCardID].UnitType() == 1)
                    {
                        researchedUnit = "Longbow";

                        hasResearchedAdvancedUnit[3] = true;
                        ToggleUnitResearchedUI(6);
                    }
                    else
                    {
                        Debug.Log("HUGE ERROR: Invalid Unit ID");
                    }
                }
                else if (chosenClassID == 2)
                {
                    if (researchCards[clickedCardID].UnitType() == 0)
                    {
                        researchedUnit = "Wizard";

                        hasResearchedAdvancedUnit[4] = true;
                        ToggleUnitResearchedUI(7);
                    }
                    else if (researchCards[clickedCardID].UnitType() == 1)
                    {
                        researchedUnit = "Cleric";

                        hasResearchedAdvancedUnit[5] = true;
                        ToggleUnitResearchedUI(8);
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

            int upgradeID = (chosenClassID * 3) + researchCards[clickedCardID].UpgradeType();
            researchedUpgradeCounts[upgradeID]++;
            researchedUpgradeTexts[upgradeID].text = researchedUpgradeCounts[upgradeID].ToString();
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
        hadChoice = hasChoice();
        hadOptions = currentOptions();
        xPosWhenClicked = researchCards[clickedCardID].transform.position.x;

        Debug.Log("had " + hadOptions + " options when clicked");

        if (currentState == ShownCardsState.classes)
        {
            chosenClassID = researchCards[clickedCardID].ClassType();

            if (!hasChoice())
            {
                currentState = ShownCardsState.preparingSecondSet;

                StartCoroutine(researchCards[clickedCardID].BeginCardRotation());

                return;
            }
            else
            {
                currentState = ShownCardsState.classesFading;

                for (int i = 0; i < 3; i++)
                {
                    if (i != clickedCardID && researchCards[i])
                    {
                        researchCards[i].PlayFadeCardAnimation();
                    }
                }
            }
        }
        else if (currentState == ShownCardsState.units || currentState == ShownCardsState.upgrades)
        {
            ResolveResearchChoice();

            HideBackButton();
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
        if (currentState != ShownCardsState.preparingSecondSet && currentState != ShownCardsState.backToClasses)
        {
            return;
        }

        Sprite spriteToGive = null;

        if (currentState == ShownCardsState.preparingSecondSet)
        {
            if (isUnitTurn)
            {
                currentState = ShownCardsState.units;

                if (!hasResearchedBaseClass[chosenClassID])
                {
                    spriteToGive = baseClassUnitSprites[chosenClassID];

                    researchCards[_cardID].ChangeToBasicUnitCard(_cardID, true);
                }
                else
                {
                    int options = 0;
                    int onlyOptionID = -1;

                    if (!hasResearchedAdvancedUnit[chosenClassID * 2])
                    {
                        options++;
                        onlyOptionID = 0;
                    }
                    if (!hasResearchedAdvancedUnit[(chosenClassID * 2) + 1])
                    {
                        options++;
                        if (onlyOptionID == -1)
                        {
                            onlyOptionID = 1;
                        }
                    }

                    if (options == 2)
                    {
                        if (clickedCardID == 0)
                        {
                            spriteToGive = unitSprites[chosenClassID * 2];

                            researchCards[0].ChangeToUnitCard(0, 0);
                            researchCards[1] = CreateUnitCard(1, 1);
                            researchCards[1].transform.position = researchCards[0].transform.position;
                        }
                        else
                        {
                            spriteToGive = unitSprites[(chosenClassID * 2) + 1];

                            researchCards[clickedCardID].ChangeToUnitCard(1, 1);
                            if (clickedCardID == 2)
                            {
                                researchCards[1] = researchCards[2];
                                researchCards[2] = null;
                                clickedCardID = 1;
                            }

                            researchCards[0] = CreateUnitCard(0, 0);
                            researchCards[0].transform.position = researchCards[1].transform.position;
                        }
                    }
                    else if (options == 1 && onlyOptionID != -1)
                    {
                        spriteToGive = unitSprites[(chosenClassID * 2) + onlyOptionID];

                        researchCards[clickedCardID].ChangeToUnitCard(clickedCardID, onlyOptionID);
                    }
                    else
                    {
                        Debug.Log("HUGE ERROR: Could not find valid options for this class");
                    }
                    
                }
            }
            else
            {
                currentState = ShownCardsState.upgrades;

                spriteToGive = upgradeSprites[clickedCardID];

                researchCards[clickedCardID].ChangeToUpgradeCard(clickedCardID, clickedCardID);

                for (int i = 0; i < 3; i++)
                {
                    if (!researchCards[i])
                    {
                        researchCards[i] = CreateUpgradeCard(i, i);
                        researchCards[i].transform.position = researchCards[clickedCardID].transform.position;
                    }
                }
            }

            if(hadChoice)
            {
                ShowBackButton();
            }
        }
        else if (currentState == ShownCardsState.backToClasses)
        {
            if (currentOptions() == 1)
            {
                int onlyOptionIndex = -1;

                for (int i = 0; i < 3; i++)
                {
                    if (researchCards[i])
                    {
                        onlyOptionIndex = i;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (onlyOptionIndex != -1)
                {
                    int onlyOptionClassID = researchCards[onlyOptionIndex].ClassType();

                    spriteToGive = classSprites[onlyOptionClassID];

                    for (int i = 0; i < 3; i++)
                    {
                        if (!researchCards[i])
                        {
                            researchCards[i] = CreateClassCardNoSpawnAnim(i, i);
                            researchCards[i].transform.position = researchCards[onlyOptionIndex].transform.position;
                        }
                    }

                    if (onlyOptionClassID == 0)
                    {
                        ReorderCards(2, 1, 0);
                    }
                    else if (onlyOptionClassID == 1)
                    {
                        ReorderCards(2, 0, 1);
                    }
                    else if (onlyOptionClassID == 2)
                    {
                        ReorderCards(0, 1, 2);
                    }
                    else
                    {
                        Debug.Log("HUGE ERROR: Invalid ClassID for Only Option");
                    }
                }
                else
                {
                    Debug.Log("HUGE ERROR: Could not find card index for only option available");
                }
            }
            else if (currentOptions() == 3)
            {
                if (hadOptions == 2)
                {
                    spriteToGive = classSprites[researchCards[clickedCardID].ClassType()];
                }
            }
        }
        else
        {
            Debug.Log("HUGE ERROR: Entered ready to change phase in incorrect state");
        }

        researchCards[clickedCardID].GiveCardNewOppositeFace(spriteToGive);
        
    }

    public void CardFinishedRotating(int _cardID)
    {
        if (currentState == ShownCardsState.classes && !hasChoice())
        {
            researchCards[0].PlayClickCardAnimation();
            return;
        }
        else if (currentState == ShownCardsState.units && !hasChoice())
        {
            if (!hadChoice)
            {
                researchCards[0].PlayClickCardAnimation();
            }
            return;
        }

        MoveCardsToProperPositions();
    }

    public void CardFinishedMoving(int _cardID)
    {
        if (currentState != ShownCardsState.backToClasses)
        {
            return;
        }

        if (_cardID != clickedCardID)
        {
            researchCards[_cardID].DestroyCard();

            if (researchCards[_cardID])
            {
                researchCards[_cardID] = null;
            }

            cardsRemaining--;

            if (cardsRemaining == 1 && !researchCards[clickedCardID].IsMoving())
            {
                StartCoroutine(researchCards[clickedCardID].BeginCardRotation());
            }
        }
        else
        {
            if (cardsRemaining == 1)
            {
                StartCoroutine(researchCards[clickedCardID].BeginCardRotation());
            }
        }
    }

    private void GoBackToClasses()
    {
        currentState = ShownCardsState.backToClasses;

        

        if (currentOptions() == 1)
        {
            int onlyOptionIndex = -1;

            for (int i = 0; i < 3; i++)
            {
                if (researchCards[i])
                {
                    onlyOptionIndex = i;
                    break;
                }
                else
                {
                    continue;
                }
            }

            if (onlyOptionIndex != -1)
            {
                StartCoroutine(researchCards[onlyOptionIndex].BeginCardRotation());
            }
            else
            {
                Debug.Log("HUGE ERROR: Could not find card index for only option available");
            }
        }
        else if (currentOptions() == 3)
        {
            cardsRemaining = 3;

            for (int i = 0; i < 3; i++)
            {
                StartCoroutine(researchCards[i].MoveCardToPosition(xPosWhenClicked));
            }
        }
    }

    #endregion

    #region Functionality

    private void ReorderCards(int _firstID, int _secondID, int _thirdID)
    {
        researchCards[_firstID].transform.SetSiblingIndex(1);
        researchCards[_secondID].transform.SetSiblingIndex(2);
        researchCards[_thirdID].transform.SetSiblingIndex(3);
    }

    private void ShowBackButton()
    {
        if(backButtonIsActive != true)
        {
            backButtonIsActive = true;
            backButtonAnim.Play("buttonFadeIn");
        }
    }

    private void HideBackButton()
    {
        if (backButtonIsActive != false)
        {
            backButtonIsActive = false;
            backButtonAnim.Play("buttonFadeOut");
        }
    }

    public void BackButton()
    {
        if (currentState == ShownCardsState.units || currentState == ShownCardsState.upgrades)
        {
            hadOptionsWhenBack = currentOptions();

            GoBackToClasses();

            HideBackButton();
        }
        else
        {
            Debug.Log("back button pressed during " + currentState + " state");
            return;
        }
    }

    private void ToggleUnitResearchedUI(int _unitID)
    {
        researchedUnitsToggles[_unitID].isOn = true;
    }

    public void NextTurn()
    {
        turnCounter++;
        turnCounterText.text = "Turn: " + turnCounter;

        isUnitTurn = unitTurns.Contains(turnCounter) ? true : false;

        clickedCardID = -1;
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

        currentState = ShownCardsState.none;
    }

    #endregion
}
