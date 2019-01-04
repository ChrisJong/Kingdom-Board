using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class unitTraining_test : MonoBehaviour
{
    public static unitTraining_test singleton;

    private void Awake()
    {
        if (!singleton)
            singleton = this;
        else
            Destroy(this);
    }

    [SerializeField] private RectTransform unitTrainingPanel;
    private bool isShowingUnitTrainingPanel = false;

    private float showPanelXPos = 420f;
    private float hidePanelXPos = -290f;

    private float panelMoveSpeed = 50f;
    private bool panelIsMoving = false;

    List<unitTraining_ribbon> unitsQueued = new List<unitTraining_ribbon>();

    [SerializeField] private unitTraining_ribbon unitRibbonPrefab;
    [SerializeField] private RectTransform unitsQueuedPanel;

    private float ribbonStartingYPos = 0f;
    private float ribbonHeight = 60f;
    private float ribbonVerticalPadding = 5f;
    private float ribbonMoveSpeed = 10f;

    [SerializeField] private Sprite[] unitSprites = new Sprite[9];
    [SerializeField] private string[] unitNames = new string[9];

    [SerializeField] private Color meleeColor;
    [SerializeField] private Color rangedColor;
    [SerializeField] private Color magicColor;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleTrainUnitsPanel();
        }
    }

    private void ToggleTrainUnitsPanel()
    {
        if (!isShowingUnitTrainingPanel)
            isShowingUnitTrainingPanel = true;
        else
            isShowingUnitTrainingPanel = false;

        if (panelIsMoving)
            StopCoroutine(MoveUnitTrainingPanel());

        StartCoroutine(MoveUnitTrainingPanel());
    }

    private IEnumerator MoveUnitTrainingPanel()
    {
        panelIsMoving = true;

        if (isShowingUnitTrainingPanel)
        {
            while (unitTrainingPanel.position.x < showPanelXPos)
            {
                Vector3 panelPos = unitTrainingPanel.position;
                panelPos.x += panelMoveSpeed;
                unitTrainingPanel.position = panelPos;

                if (panelPos.x >= showPanelXPos)
                {
                    panelPos.x = showPanelXPos;
                    unitTrainingPanel.position = panelPos;

                    break;
                }

                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (unitTrainingPanel.position.x > hidePanelXPos)
            {
                Vector3 panelPos = unitTrainingPanel.position;
                panelPos.x -= panelMoveSpeed;
                unitTrainingPanel.position = panelPos;

                if (panelPos.x <= hidePanelXPos)
                {
                    panelPos.x = hidePanelXPos;
                    unitTrainingPanel.position = panelPos;

                    break;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        panelIsMoving = false;
        yield return null;
    }

    public void TrainNewUnit(int _unitId)
    {
        //Unit ID List
        //0 - Warrior
        //1 - Archer
        //2 - Mage
        //3 - Guardian
        //4 - Knight
        //5 - Crossbow
        //6 - Longbow
        //7 - Wizard
        //8 - Cleric

        unitTraining_ribbon newUnit = Instantiate(unitRibbonPrefab, unitsQueuedPanel);

        float yPos = ribbonStartingYPos - unitsQueued.Count * (ribbonHeight + ribbonVerticalPadding);

        Color classColor;
        if (_unitId == 0 || _unitId == 3 || _unitId == 4)
            classColor = meleeColor;
        else if (_unitId == 1 || _unitId == 5 || _unitId == 6)
            classColor = rangedColor;
        else if (_unitId == 2 || _unitId == 7 || _unitId == 8)
            classColor = magicColor;
        else
            classColor = Color.white;

        newUnit.SetupRibbon(unitNames[_unitId], unitSprites[_unitId], classColor, yPos);
        unitsQueued.Add(newUnit);
    }

    public void CancelTrainingUnit(unitTraining_ribbon _unitToCancel)
    {
        unitsQueued.Remove(_unitToCancel);
        Destroy(_unitToCancel.gameObject);

        SortRibbons();
    }

    private void SortRibbons()
    {
        for (int i = 0; i < unitsQueued.Count; i++)
        {
            float yPos = ribbonStartingYPos - i * (ribbonHeight + ribbonVerticalPadding);

            StartCoroutine(unitsQueued[i].MoveRibbon(yPos, ribbonMoveSpeed));
        }
    }
}
