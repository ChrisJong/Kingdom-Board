using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shatter_test : MonoBehaviour {

    [SerializeField] private GameObject[] unitGfx;
    [SerializeField] private GameObject shatteredPrefab;
    [SerializeField] private Camera playerCam;
    [SerializeField] private LayerMask unitLayer;

    private GameObject shatteredObj;
    private bool shattered;

    public float explosiveRadius;
    public float explosiveForce;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !shattered)
        {
            ShatterUnit();
        }

        if (Input.GetKeyDown(KeyCode.Space) && shattered)
        {
            ResetGfx();
        }
    }

    private void ShatterUnit()
    {
        RaycastHit hit;
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitLayer))
        {
            CreateShatteredUnit();
            CreateExplosionForce(hit.point);
        }
    }

    private void CreateShatteredUnit()
    {
        shattered = true;

        ToggleUnitGfx(false);

        shatteredObj = Instantiate(shatteredPrefab, transform.position, transform.rotation);
    }

    private void CreateExplosionForce(Vector3 _explosivePos)
    {
        Rigidbody[] rigidBodies = shatteredObj.GetComponentsInChildren<Rigidbody>();
        Vector3 explosiveDirection = (transform.position - _explosivePos).normalized;
        explosiveDirection.y = 0;

        for (int i = 0; i < rigidBodies.Length; i++)
        {
            rigidBodies[i].AddForceAtPosition(explosiveDirection * explosiveForce, _explosivePos);

            //rigidBodies[i].AddExplosionForce(explosiveForce, transform.position, explosiveRadius, 100f);
        }
    }

    private Vector3 GetForceVector(Vector3 _rbPos, Vector3 _explosivePos)
    {
        Vector3 direction = (_rbPos - _explosivePos).normalized;

        if (direction.y < 0)
        {
            direction.y *= -1;
        }

        return direction * explosiveForce;
    }

    private void ToggleUnitGfx(bool _input)
    {
        for (int i = 0; i < unitGfx.Length; i++)
        {
            unitGfx[i].SetActive(_input);
        }
    }

    private void ResetGfx()
    {
        shattered = false;

        ToggleUnitGfx(true);

        Destroy(shatteredObj);
        if (shatteredObj != null)
        {
            shatteredObj = null;
        }
    }
}
