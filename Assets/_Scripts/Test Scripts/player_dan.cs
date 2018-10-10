using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_dan : MonoBehaviour {

    private UnitScript_dan selectedUnit;
    private UnitScript_dan hoveredUnit;
    public int playerID;

    [SerializeField] private Camera playerCam;
    [SerializeField] private LayerMask unitsLayerMask;
    private cursor_dan cursorController;
    private LineRenderer lineRenderer;
    [SerializeField] private Transform xLocator;

    private void Start()
    {
        cursorController = GetComponent<cursor_dan>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        //SeeWhatImHovering();

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitsLayerMask))
            {
                shatter_test unitToShatter = hit.transform.GetComponent<shatter_test>();
                if (unitToShatter)
                {
                    unitToShatter.ShatterUnit(hit.point);
                }
            }
        }

        /*if (Input.GetMouseButtonDown(0))
        {
            DeselectUnit();
            TrySelectUnit();
        }

        if (Input.GetMouseButtonDown(1))
        {
            SeeWhatIClicked();
        }*/
    }

    private void SeeWhatImHovering()
    {
        RaycastHit hit;
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.tag == "Unit")
            {
                SetHoveredUnit(hit.transform.GetComponent<UnitScript_dan>());
            }
            else
            {
                UnsetHoveredUnit();
            }

            if (selectedUnit && hit.collider.tag == "Environment")
            {
                if (selectedUnit.canMove())
                {
                    EnablePathHighlight();
                    HighlightUnitPathToPoint(hit.point);

                    cursorController.SetCursorMoveReady();
                }
                else if (selectedUnit.isMoving)
                {
                    DisablePathHighlight();

                    cursorController.SetCursorDefault();
                }
                else
                {
                    DisablePathHighlight();

                    cursorController.SetCursorMoveNotReady();
                }
            }
        }
    }

    private void SeeWhatIClicked()
    {
        if (selectedUnit)
        {
            RaycastHit hit;
            Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "Unit")
                {
                    UnitScript_dan clickedUnit = hit.transform.GetComponent<UnitScript_dan>();
                    if (clickedUnit.ownerID != playerID)
                    {
                        SendUnitAttackOrder(clickedUnit);
                    }
                    else
                    {
                        Debug.Log("Cannot attack friendly units");
                    }
                }
                else if (hit.collider.tag == "Environment")
                {
                    SendUnitMoveOrder();
                }
                else
                {
                    Debug.Log("Clicked out of bounds");
                }
            }
        }
        else
        {
            Debug.Log("No unit selected");
        }
    }

    #region UI

    private void SetHoveredUnit(UnitScript_dan _unit)
    {
        hoveredUnit = _unit;

        if (hoveredUnit.ownerID == playerID)
        {
            hoveredUnit.HighlightAllyUnit();

            DisablePathHighlight();
            cursorController.SetCursorDefault();
        }
        else
        {
            hoveredUnit.HighlightEnemyUnit();
            if (selectedUnit)
            {
                if (selectedUnit.canAttack)
                {
                    cursorController.SetCursorAttackReady();

                    if (selectedUnit.canMove())
                    {
                        EnablePathHighlight();
                        HighlightUnitPathToEnemy(hoveredUnit);
                    }
                    else if (selectedUnit.isMoving)
                    {
                        DisablePathHighlight();
                        cursorController.SetCursorAttackNotReady();
                    }
                }
                else
                {
                    cursorController.SetCursorAttackNotReady();
                }
            }
        }
    }

    private void UnsetHoveredUnit()
    {
        if(hoveredUnit && hoveredUnit != selectedUnit)
        {
            hoveredUnit.UnhighlightUnit();
        }
    }

    private void DeselectUnit()
    {
        if (selectedUnit)
        {
            selectedUnit.DeselectUnit();
            selectedUnit = null;
        }

        DisablePathHighlight();
        cursorController.SetCursorDefault();
    }

    private void TrySelectUnit()
    {
        RaycastHit hit;
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.tag == "Unit")
            {
                UnitScript_dan clickedUnit = hit.transform.GetComponent<UnitScript_dan>();
                if (clickedUnit.ownerID == playerID)
                {
                    selectedUnit = clickedUnit;
                    selectedUnit.SelectUnit();
                }
            }
        }
    }

    private void HighlightUnitPathToPoint(Vector3 _targetPos)
    {
        Vector3[] unitPath = selectedUnit.GetPathToPoint(_targetPos);
        if (unitPath != null)
        {
            lineRenderer.positionCount = unitPath.Length;
            lineRenderer.SetPositions(unitPath);
            xLocator.position = unitPath[0];
        }
    }

    private void HighlightUnitPathToEnemy(UnitScript_dan _enemyUnit)
    {
        Vector3[] unitPath = selectedUnit.GetPathToUnit(_enemyUnit.transform.position);
        if (unitPath != null)
        {
            lineRenderer.positionCount = unitPath.Length;
            lineRenderer.SetPositions(unitPath);
            xLocator.position = unitPath[0];
        }
    }

    public void EnablePathHighlight()
    {
        lineRenderer.enabled = true;
        xLocator.gameObject.SetActive(true);
    }

    public void DisablePathHighlight()
    {
        lineRenderer.enabled = false;
        xLocator.gameObject.SetActive(false);
    }

    #endregion

    #region Commands

    private void SendUnitMoveOrder()
    {
        selectedUnit.MoveUnit();
    }

    private void SendUnitAttackOrder(UnitScript_dan _target)
    {
        selectedUnit.AttackUnit(_target);
    }

    #endregion
}
