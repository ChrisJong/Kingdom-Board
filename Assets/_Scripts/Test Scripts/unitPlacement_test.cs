using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitPlacement_test : MonoBehaviour
{
    ////////////////////////This script is only used for testing purposes
    ////////////////////////It will be place on the player, which contains the Castle model
    ////////////////////////Implementation will depend on how the already written scripts function

    public unitToBePlaced_test[] unitsToBePlacedPrefabs = new unitToBePlaced_test[9]; //these are the prefabs which contain the transparent units which will be spawned when the player must place down a unit
    public GameObject[] actualUnits = new GameObject[9]; //using generic game object in place of units so i dont have to write a unit script
    public LayerMask environmentMask; //the layer mask for the environment cause im lazy

    bool placingUnit = false; //a check for when the player is placing down a unit
    unitToBePlaced_test unitToBePlaced; //the representation of the unit to be placed
    int unitId = -1; //id of unit, used to remember which unit we're placing, default to -1 for debugging
    
    Vector3 hideUnitPosition = new Vector3(0, 100, 0);//the position we use to hide the unit if the raycast misses

    //simple region class to see if a position is within a rectangle
    //if this is useful, then extract into another script
    public class Region
    {
        Vector3 origin;
        float topBoundary, bottomBoundary, rightBoundary, leftBoundary;

        public Region(Vector3 _origin, float zHeight, float xWidth)
        {
            origin = _origin;
            topBoundary = origin.z + zHeight;
            bottomBoundary = origin.z - zHeight;
            rightBoundary = origin.x + xWidth;
            leftBoundary = origin.x - xWidth;
        }

        public bool IsWithinRegion(Vector3 _position)
        {
            if (_position.z <= topBoundary && _position.z >= bottomBoundary && _position.x <= rightBoundary &&_position.x >= leftBoundary )
                return true;
            else
                return false;
        }
    }

    //the offset for spawn region height and width
    public float spawnBoundaryHeight, spawnBoundaryWidth;
    //since we cannot reference transform position in declaration
    //we create a reference to spawn region and assign it in awake
    Region spawnRegion;

    private void Awake()
    {
        spawnRegion = new Region(transform.position, spawnBoundaryHeight, spawnBoundaryWidth);
    }

    //we only use the update function to check whether the mouse was clicked or not
    //fixed update can skip some frames so we use update instead
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (placingUnit)
                PlaceUnit();
        }

        if (Input.GetMouseButtonDown(1))
            if (placingUnit)
                CancelUnit();
    }

    //This function is used to determine whether we need to update the unit to be placed's position and rotation
    private IEnumerator UpdateUnitToBePlacedPositionRotation()
    {
        Vector3 previousMousePosition = Input.mousePosition; //remember where the player's mouse position was the previous frame, use this so that we only need to calculate things if the player's mouse is moving
        bool wasWithinSpawnRegion = true; //initialize this bool to be true due to the first frame the unit is created, is within the castle

        while (placingUnit)
        {
            //dont calculate anything if the player hasn't moved their mouse since the last fixed update
            if (Input.mousePosition == previousMousePosition)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }
            else
            {
                //set the position to where our mouse is pointing
                Vector3 position = GetWorldCoordAtMouseCoord();
                unitToBePlaced.transform.position = position;

                //rotate the unit to face away from castle
                Vector3 direction = GetDirectionFromCastle(position);
                unitToBePlaced.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

                //check if we're within the spawn region
                bool isWithinRegion = spawnRegion.IsWithinRegion(position);

                //change the color if iswithinregion changed
                //so we dont need to access the unit and all its renderers each frame
                if (isWithinRegion != wasWithinSpawnRegion)
                {
                    wasWithinSpawnRegion = isWithinRegion;
                    unitToBePlaced.ChangeColor(isWithinRegion);
                }
            }

            //update the previous mouse position for the next fixed update
            previousMousePosition = Input.mousePosition;
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    //retrieves a world coordinate from raycasting the screen from mouse position
    private Vector3 GetWorldCoordAtMouseCoord()
    {
        RaycastHit hit;
        Ray ray = (Camera.main.ScreenPointToRay(Input.mousePosition));

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, environmentMask))
            return hit.point;
        else
            return hideUnitPosition; //if the raycast misses the environment, place the unit high into the air to hide it
    }

    //retrieves a displacement vector from the castle to the unit
    private Vector3 GetDirectionFromCastle(Vector3 _position)
    {
        if (_position == hideUnitPosition)
            return Vector3.forward;

        _position.y = transform.position.y;

        Vector3 direction = _position - transform.position;

        return direction;
    }

    //This function begins the placement of a unit
    public void InitiateUnitPlacement(int _unitId)
    {
        //check if valid unitId
        if (_unitId < 0 || _unitId > 8)
        {
            Debug.Log("Invalid Unit ID");
            return;
        }

        //check if we are already placing a unit
        if (placingUnit)
        {
            //can replace this section if implementing a way to place the current unit being placed back into the queue
            Debug.Log("Already placing a unit! Finish placing the unit to place another unit");
            return;
        }

        //set up our safety checks and instantiate the unit within the castle for now
        //then begin updating the position of the unit to the mouse position
        placingUnit = true;
        unitId = _unitId;
        unitToBePlaced = Instantiate(unitsToBePlacedPrefabs[unitId], transform.position, transform.rotation);
        StartCoroutine(UpdateUnitToBePlacedPositionRotation());
    }

    //this function replaces the unit representation with the actual unit
    //requires heavy changes here to fully integrate into current system
    private void PlaceUnit()
    {
        Vector3 pos = unitToBePlaced.transform.position;

        //if the unit wasn't within the spawn region just ignore the rest
        //could add warning, or place back in the queue, or just cancel the unit
        if (!spawnRegion.IsWithinRegion(pos))
            return;

        Quaternion rot = unitToBePlaced.transform.rotation;

        Instantiate(actualUnits[unitId], pos, rot);

        //reset the script
        placingUnit = false;
        unitId = -1;

        //destroy last to be safe
        Destroy(unitToBePlaced.gameObject);
        unitToBePlaced = null;
    }

    private void CancelUnit()
    {
        //basically identical to the bottom of place unit function
        placingUnit = false;
        unitId = -1;

        Destroy(unitToBePlaced.gameObject);
        unitToBePlaced = null;
    }

    //gizmo used to represent the spawn region
    //use whatever replacement that is preferred
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + (Vector3.up * 0.5f), new Vector3(spawnBoundaryWidth * 2, 0, spawnBoundaryHeight * 2));
    }
}
