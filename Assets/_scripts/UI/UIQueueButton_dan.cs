using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQueueButton_dan : MonoBehaviour {

    public UITest_dan uiCanvas;
    public int queueID; //unique ID to this queue button
    public int turnsToTrain = 1;
    public int turnsRemaining;

    private Vector3 UIPosition; //copy of the current position of this queue button
    private float positionToBe; //position this button wants to move to
    private float finishedPosition = 50; //y value of the position when finished
    private float cancelPosition = 50f; //x value of the position when cancelled
    private float moveSpeed = 8f; //speed this queue button moves at
    
    private Color colour; //the colour component of this queue button

    private bool needsToMove; //determines if this queue button needs to move
    private bool finished; //determines if this queue button has finished training its unit
    private bool cancelled; //determines if this queue button should cancel training its unit

    private float destroyTime = 0.2f; //determines how long this takes to be destroyed

    void Start() {
        //setting variables to instantiated prefab
        UIPosition = transform.localPosition;
        colour = GetComponent<Image>().color;

        turnsRemaining = turnsToTrain;

        finished = false;
        cancelled = false;
    }
    
    void Update() {
        //if this queue button has been told to move, it should move
        if (needsToMove == true) {
            MoveIntoPosition();
        }
    }

    public void UpdateUIPosition()
    {
        //if the queueID of this button has changed due to cancellations
        //or units finishing training then update position of this button
        positionToBe = -4f - (queueID * 72);
        needsToMove = true;
    }

    public void MoveIntoPosition() {
        //changes the position of the queue button to its proper position in the queue unless it has finished or has been cancelled
        if (!finished && !cancelled) {
            UIPosition.y = Mathf.Lerp(UIPosition.y, positionToBe, moveSpeed * Time.deltaTime);
        }
        //when finished, move it above the screen
        if (finished) {
            UIPosition.y = Mathf.Lerp(UIPosition.y, finishedPosition, moveSpeed * Time.deltaTime);
            colour.a = Mathf.Lerp(colour.a, 0f, moveSpeed * Time.deltaTime);
        }
        //moves this button to the right if it has been cancelled
        if (cancelled) {
            UIPosition.x = Mathf.Lerp(UIPosition.x, cancelPosition, moveSpeed * Time.deltaTime);
            colour.a = Mathf.Lerp(colour.a, 0f, moveSpeed * Time.deltaTime);
            GetComponent<Image>().color = colour;
        }
        //update the position of this button
        transform.localPosition = UIPosition;
        //once the button reaches its destination, check off its need to move        
        if (UIPosition.y == positionToBe) {
            needsToMove = false;
        }
    }

    public void NewTurn() {
        turnsRemaining = turnsRemaining - 1;
        if (turnsRemaining == 0) {
            FinishTraining();
        }
    }

    public void FinishTraining() {
        finished = true;
        needsToMove = true;
        Destroy(this, destroyTime);
    }

    public void CancelUnit() {
        cancelled = true;
        needsToMove = true;
        Destroy(this, destroyTime);
    }

    private void OnDestroy() {
        uiCanvas.RefreshQueue(queueID);
        Destroy(this.gameObject);
    }




}
