using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResearchCard_dan : MonoBehaviour, IPointerDownHandler {

    private ResearchUI_dan researchUI;

    private int cardID;

    private int classType = -1;
    private bool isBasicUnit = false;
    private int unitType = -1;
    private int upgradeType = -1;

    private Sprite oppositeCardFace;

    private Animation anim;

    private float moveSpeed = 15f;

    private float currentY = 0f;
    private float rotateSpeed = 5f;
    private Vector3 currentRotation;

    private bool isSpawning = true;
    private bool isRotating = false;
    private bool isFading = false;


    #region Functionality

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isSpawning && !isRotating && !isFading)
        {
            PlayClickCardAnimation();
        }
    }

    public void GiveCardNewOppositeFace(Sprite _newCardFace)
    {
        oppositeCardFace = _newCardFace;
    }

    public void ImmediatelyChangeCardFace(Sprite _newCardFace)
    {
        Image img = GetComponent<Image>();
        img.sprite = _newCardFace;
    }

    #endregion

    #region Setup

    private void Awake()
    {        
        anim = GetComponent<Animation>();
        researchUI = ResearchUI_dan.instance;
    }

    public void SetUpCard(int _cardID, int _classID)
    {
        cardID = _cardID;
        classType = _classID;

        string classString = null;
        if (classType == 0)
        {
            classString = "Melee Class";
        }
        else if (classType == 1)
        {
            classString = "Ranged Class";
        }
        else
        {
            classString = "Magic Class";
        }

        name = "Card " + cardID + ": " + classString;

        PlaySpawnCardAnimation();
    }

    public void ChangeToBasicUnitCard(int _cardID, bool _isBasicUnit)
    {
        cardID = _cardID;
        isBasicUnit = _isBasicUnit;

        string unitString = null;
        if (unitType == 0)
        {
            unitString = "Warrior";
        }
        else if (unitType == 1)
        {
            unitString = "Archer";
        }
        else
        {
            unitString = "Mage";
        }

        name = "Card " + cardID + ": " + unitString;
    }

    public void ChangeToUnitCard(int _cardID, int _unitID)
    {
        cardID = _cardID;
        unitType = _unitID;

        string unitString = null;
        if (classType == 0)
        {
            if (unitType == 0)
            {
                unitString = "Knight";
            }
            else
            {
                unitString = "Guardian";
            }
        }
        else if (classType == 1)
        {
            if (unitType == 0)
            {
                unitString = "Crossbow";
            }
            else
            {
                unitString = "Longbow";
            }
        }
        else
        {
            if (unitType == 0)
            {
                unitString = "Wizard";
            }
            else
            {
                unitString = "Cleric";
            }
        }

        name = "Card " + cardID + ": " + unitString;
    }

    public void ChangeToUpgradeCard(int _cardID, int _upgradeID)
    {
        cardID = _cardID;
        upgradeType = _upgradeID;

        string classString = null;
        if (classType == 0)
        {
            classString = "Melee Class";
        }
        else if (classType == 1)
        {
            classString = "Ranged Class";
        }
        else
        {
            classString = "Magic Class";
        }

        string upgradeString = null;
        if (upgradeType == 0)
        {
            upgradeString = " Attack";
        }
        else if (upgradeType == 1)
        {
            upgradeString = " Health";
        }
        else
        {
            upgradeString = " Stamina";
        }

        name = "Card " + cardID + ": " + classString + upgradeString;
    }

    public int ClassType()
    {
        return classType;
    }

    public bool IsBasicUnit()
    {
        return isBasicUnit;
    }

    public int UnitType()
    {
        return unitType;
    }

    public int UpgradeType()
    {
        return upgradeType;
    }

    #endregion

    #region Animations

    public void PlayClickCardAnimation()
    {
        if (anim)
        {
            anim.Play("clickResearchCard");
        }
    }

    public void FinishedPlayingClickCardAnimation()
    {
        researchUI.CardWasClicked(cardID);
    }

    public void PlayFadeCardAnimation()
    {
        if (anim)
        {
            isFading = true;
            anim.Play("fadeResearchCard");
        }
    }

    public void FinishedPlayingFadeCardAnimation()
    {
        isFading = false;
        researchUI.CardWasFaded(cardID);

        Destroy(gameObject);
    }

    public void PlaySpawnCardAnimation()
    {
        if (anim)
        {
            isSpawning = true;
            anim.Play("spawnResearchCard");
        }
    }

    public void FinishedPlayingSpawnCardAnimation()
    {
        isSpawning = false;
        StartCoroutine(BeginCardRotation());
    }

    #endregion

    #region Generated Animations

    private void FlipXScale()
    {
        Vector3 newScale = new Vector3(transform.localScale.x * -1, 1, 1);

        transform.localScale = newScale;
    }

    private void ChangeCardFace()
    {
        Image img = GetComponent<Image>();
        img.sprite = oppositeCardFace;
    }

    private void ChangeCardFace(Sprite _newSprite)
    {
        Image img = GetComponent<Image>();
        img.sprite = _newSprite;
    }

    private void RotateCard()
    {
        currentY += rotateSpeed;
        currentRotation.y = currentY;

        transform.rotation = Quaternion.Euler(currentRotation);
    }

    private void RotateCard(float _yValue)
    {
        currentY = _yValue;
        currentRotation.y = currentY;

        transform.rotation = Quaternion.Euler(currentRotation);
    }

    public IEnumerator BeginCardRotation()
    {
        isRotating = true;

        while (currentY < 90)
        {
            RotateCard();

            yield return new WaitForEndOfFrame();
        }

        researchUI.CardReadyToChange(cardID);

        ChangeCardFace();
        FlipXScale();

        StartCoroutine(FinishCardRotation());

        yield return null;
    }

    private IEnumerator FinishCardRotation()
    {
        while (currentY < 180)
        {
            RotateCard();

            yield return new WaitForEndOfFrame();
        }

        RotateCard(0f);
        FlipXScale();

        isRotating = false;

        researchUI.CardFinishedRotating(cardID);

        yield return null;
    }

    public void ImmediateHalfRotateWithSpriteChange(Sprite _newSprite)
    {
        ChangeCardFace(_newSprite);
        FlipXScale();
        RotateCard(90);

        isSpawning = false;
        isRotating = true;

        StartCoroutine(FinishCardRotation());
    }

    public IEnumerator MoveCardToPosition(float _xToMoveTo)
    {
        Vector3 startPos = transform.position;

        float distanceRemaining = Mathf.Abs(startPos.x - _xToMoveTo);

        int rightLeftFactor = (startPos.x < _xToMoveTo) ? 1 : -1;

        while (distanceRemaining > 0)
        {
            distanceRemaining = Mathf.Abs(transform.position.x - _xToMoveTo);
            Vector3 posToMoveTo = transform.position;

            if (distanceRemaining < moveSpeed)
            {
                posToMoveTo.x = _xToMoveTo;
                transform.position = posToMoveTo;
                break;
            }
            else
            {
                posToMoveTo.x += moveSpeed * rightLeftFactor;

                transform.position = posToMoveTo;
            }            

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    #endregion
}