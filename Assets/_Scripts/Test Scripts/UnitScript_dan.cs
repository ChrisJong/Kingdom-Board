using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UnitScript_dan : MonoBehaviour {

    public int ownerID;
    public bool selected = false;

    private NavMeshAgent agent;
    private Animator anim;

    [SerializeField] private Camera playerCam;
    [SerializeField] private LayerMask terrainMask;

    [SerializeField] private Canvas canvas;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image redHpBar;
    [SerializeField] private Image staminaBar;

    public float redHpBarLerpRate = 0.5f;

    public float maxHp;
    private float currentHp;

    public float maxStamina;
    private float currentStamina;

    public bool canAttack;
    public float attackRange;

    public bool canMove()
    {
        if (currentStamina > 0 && !isMoving && !agent.pathPending)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isMoving;

    private NavMeshPath path;
    private float tempStamina;
    private bool outOfStamina;
    private Vector3 stoppingPoint;

    private UnitScript_dan target;
    private bool targetInRange = false;

    Renderer[] rends;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        currentHp = maxHp;
        currentStamina = maxStamina;

        canAttack = true;
        isMoving = false;
        path = new NavMeshPath();

        rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in rends)
        {
            if (ownerID == 1)
            {
                rend.material.SetColor("_Color", Color.white);
            }
            else if (ownerID == 2)
            {
                rend.material.SetColor("_Color", Color.black);
            }
        }
    }

    private void Update()
    {
        //UTILITIES
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetStamina();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            ResetHP();
        }

        //FUNCTIONALITIES

        UIFaceCamera();

        //ACTIONS
        if (!selected)
            return;

        /*if (canMove())
        {
            GetPath();
        }

        if (Input.GetMouseButtonDown(1) && path.status == NavMeshPathStatus.PathComplete)
        {
            SetMovement();
        }*/
    }

    #region Selection

    public void SelectUnit()
    {
        selected = true;
        HighlightAllyUnit();
    }

    public void DeselectUnit()
    {
        selected = false;
        UnhighlightUnit();
    }

    #endregion

    #region Commands

    public void MoveUnit()
    {
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            SetMovement();
        }
    }

    public void AttackUnit(UnitScript_dan _target)
    {
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            target = _target;
            SetAttack();
        }
    }


    #endregion

    #region UI

    public void HighlightAllyUnit()
    {
        foreach (Renderer rend in rends)
        {
            rend.material.SetFloat("_Outline", 0.03f);
            rend.material.SetColor("_OutlineColor", Color.green);
        }
    }

    public void HighlightEnemyUnit()
    {
        foreach (Renderer rend in rends)
        {
            rend.material.SetFloat("_Outline", 0.03f);
            rend.material.SetColor("_OutlineColor", Color.red);
        }
    }

    public void UnhighlightUnit()
    {
        foreach (Renderer rend in rends)
        {
            rend.material.SetFloat("_Outline", 0f);
        }
    }

    private void UIFaceCamera()
    {
        canvas.transform.LookAt(playerCam.transform);
        Vector3 canvasRotation = canvas.transform.rotation.eulerAngles;
        canvasRotation.y = 180f; //this will need to be 0 for player 2
        canvas.transform.rotation = Quaternion.Euler(canvasRotation);
    }

    private void UpdateHPBar(bool _reset)
    {
        hpBar.fillAmount = currentHp / maxHp;
        StartCoroutine(LerpRedHPBar(_reset));
    }

    IEnumerator LerpRedHPBar(bool _reset)
    {
        if(!_reset)
        yield return new WaitForSeconds(1f);

        while (true)
        {
            redHpBar.fillAmount -= redHpBarLerpRate;

            if (redHpBar.fillAmount < hpBar.fillAmount)
            {
                redHpBar.fillAmount = hpBar.fillAmount;
                break;
            }

            yield return new WaitForSeconds(0.05f);
        }

        yield return null;
    }

    private void UpdateStaminaBar()
    {
        staminaBar.fillAmount = currentStamina / maxStamina;
    }

    #endregion

    #region PathFinding

    public Vector3[] GetPathToPoint(Vector3 _targetPos)
    {
        if (agent.CalculatePath(_targetPos, path))
        {
            return FinalizePath(false);
        }
        else
        {
            return null;
        }
    }

    public Vector3[] GetPathToUnit(Vector3 _unitPos)
    {
        if (agent.CalculatePath(_unitPos, path))
        {
            return FinalizePath(true);
        }
        else
        {
            return null;
        }
    }

    private Vector3[] FinalizePath(bool _attack)
    {
        Stack<Vector3> linePoints = new Stack<Vector3>();
        float castLerpSize = 0.5f;

        tempStamina = currentStamina;
        outOfStamina = false;

        linePoints.Push(path.corners[0]);

        if (_attack)
        {
            Vector3 direction = (path.corners[path.corners.Length - 2] - path.corners[path.corners.Length - 1]).normalized;
            Vector3 attackPoint = path.corners[path.corners.Length - 1] + (direction * attackRange);
            path.corners[path.corners.Length - 1] = attackPoint;
        }

        for (int i = 1; i < path.corners.Length; i++)
        {
            if (Mathf.Abs(path.corners[i].y - path.corners[i-1].y) <= 0.5f)
            {
                float distance = Vector3.Distance(path.corners[i], path.corners[i - 1]);

                if (tempStamina > distance)
                {
                    linePoints.Push(path.corners[i]);
                    tempStamina -= distance;
                }
                else
                {
                    Vector3 direction = (path.corners[i] - path.corners[i - 1]).normalized;
                    Vector3 endPoint = path.corners[i - 1] + (direction * tempStamina);
                    linePoints.Push(endPoint);
                    tempStamina = 0;
                    outOfStamina = true;
                }
            }
            else
            {
                Vector3 target = path.corners[i];
                Vector3 direction = (target - path.corners[i - 1]).normalized;

                bool reachedNextPoint = false;

                while (true)
                {
                    Vector3 previousPoint = linePoints.Peek();
                    float remainingCastDistance = Vector3.Distance(previousPoint, target);
                    Vector3 nextCastPoint = previousPoint + (direction * castLerpSize);

                    if (remainingCastDistance < castLerpSize)
                    {
                        reachedNextPoint = true;
                        nextCastPoint = target;
                    }

                    Vector3 foundPoint = GetPointOnGround(nextCastPoint);

                    float distanceThisIteration = Vector3.Distance(previousPoint, foundPoint);

                    if (tempStamina > distanceThisIteration)
                    {
                        tempStamina -= distanceThisIteration;
                    }
                    else
                    {
                        linePoints.Pop();
                        nextCastPoint = previousPoint + (direction * tempStamina);
                        foundPoint = GetPointOnGround(nextCastPoint);

                        outOfStamina = true;
                        tempStamina = 0;
                    }

                    linePoints.Push(foundPoint);

                    if (outOfStamina || reachedNextPoint)
                    {
                        break;
                    }
                }
            }

            if (outOfStamina)
            {
                break;
            }
        }

        Vector3[] finalLine = GetFinalLine(linePoints);
        stoppingPoint = finalLine[0];

        return finalLine;
    }

    private Vector3 GetPointOnGround(Vector3 _point)
    {
        RaycastHit hit;
        Ray ray = new Ray(_point + (Vector3.up * 4), Vector3.down);

        Physics.Raycast(ray, out hit, Mathf.Infinity, terrainMask);

        return hit.point;
    }

    private Vector3[] GetFinalLine(Stack<Vector3> _stack)
    {
        Vector3[] stackArray = _stack.ToArray();

        for (int i = 0; i < _stack.Count; i++)
        {
            stackArray[i].y += 0.2f;
        }

        return stackArray;
    }

    private void SetMovement()
    {
        if (isMoving)
        {
            Debug.Log("Already moving");
            return;
        }

        if (path.status != NavMeshPathStatus.PathComplete)
        {
            Debug.Log("Invalid Path");
            return;
        }

        if (currentStamina <= 0)
        {
            Debug.Log("Out of Stamina");
            return;
        }

        if(outOfStamina)
        {
            currentStamina = 0;
        }
        else
        {
            currentStamina = tempStamina;
        }

        isMoving = true;

        UpdateStaminaBar();

        agent.SetDestination(stoppingPoint);
        StartCoroutine(CheckAgentStopped());
    }

    private void SetAttack()
    {
        if (isMoving)
        {
            Debug.Log("Currently moving");
            return;
        }

        if (!canAttack)
        {
            Debug.Log("Already attacked");
            return;
        }

        if (path.status != NavMeshPathStatus.PathComplete)
        {
            Debug.Log("Invalid Path");
        }

        if (currentStamina <= 0)
        {
            Debug.Log("Out of Stamina");
        }

        if (outOfStamina)
        {
            currentStamina = 0;
        }
        else
        {
            currentStamina = tempStamina;
        }

        float distance = (target.transform.position - transform.position).magnitude;

        if (distance > attackRange)
        {
            isMoving = true;
            UpdateStaminaBar();
            agent.SetDestination(stoppingPoint);
            StartCoroutine(TryGetInRangeToAttack());
        }
        else
        {
            Attack();
        }
    }

    private void Attack()
    {
        canAttack = false;
        anim.Play("Attack");
        target.TakeDamage();
    }

    IEnumerator CheckAgentStopped()
    {
        yield return new WaitForSeconds(1f);

        while (agent.velocity.sqrMagnitude > 0)
        {
            yield return new WaitForSeconds(0.2f);
        }

        isMoving = false;

        yield return null;
    }

    IEnumerator TryGetInRangeToAttack()
    {
        yield return new WaitForSeconds(1f);

        while (agent.velocity.sqrMagnitude > 0)
        {
            yield return new WaitForSeconds(0.2f);
        }

        isMoving = false;

        float distance = (target.transform.position - transform.position).magnitude;
        if (distance < attackRange)
        {
            Attack();
        }
        else
        {
            Debug.Log("Target was too far away");
        }

        yield return null;
    }



    #endregion

    #region Utilities

    public void TakeDamage()
    {
        currentHp -= 10f;
        UpdateHPBar(false);
    }

    private void ResetHP()
    {
        currentHp = maxHp;
        UpdateHPBar(true);
    }

    private void ResetStamina()
    {
        if (isMoving)
        {
            Debug.Log("Currently moving, wait until Agent stops");
            return;
        }

        currentStamina = maxStamina;
        canAttack = true;

        UpdateStaminaBar();
    }

    #endregion
}