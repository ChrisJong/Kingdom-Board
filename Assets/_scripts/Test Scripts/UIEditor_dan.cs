using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(UITest_dan))]
public class UIEditor_dan : Editor {
    //Replaces the UI in Unity's Inspector panel to reduce clutter of multiple array tables corresponding to each Train Unit button

    private SerializedProperty unitPanelProperty; //the Unit Panel which holds the Units to Train Panel and Units Queue Panel's property
    private SerializedProperty unitsQueuedPanelProperty; //the Units Queue Panel's property
    private SerializedProperty unitOrdersPanelProperty; //the Unit Orders Panel's property

    private SerializedProperty unitsToTrainButtonsProperty; //the imbedded Train Unit buttons which train a unit's property
    private SerializedProperty unitsToTrainButtonLockImagesProperty; //the imbedded Train Unit buttons' lock sprite's property
    private SerializedProperty unitsToQueueButtonPrefabsProperty; //the imbedded Train Unit buttons' queue button prefab's property
    
    private SerializedProperty playerCameraProperty; //the player camera's property
    private SerializedProperty turnCountTextProperty; //the turn count number text's property

    private bool[] showUnitButtons = new bool[UITest_dan.numUnits]; //bool array which determines if each Train Unit button's envelope should be opened

    private const string unitPanelName = "unitPanel"; //variable name of the Unit Panel
    private const string unitsQueuedPanelName = "unitsQueuedPanel"; //variable name of the Units Queued Panel
    private const string unitOrdersPanelName = "unitOrdersPanel"; //variable name of the Unit Orders Panel

    private const string unitsToTrainButtonName = "unitToTrainButtons"; //variable name of the imbedded Train Unit buttons
    private const string unitsToTrainButtonLockImageName = "unitToTrainButtonLockImages"; //variable name of the imbedded Train Unit buttons' lock sprite
    private const string unitsToQueueButtonPrefabsName = "unitToQueueButtonPrefabs"; //variable name of the imbedded Train Unit buttons' prefab
    
    private const string playerCameraName = "playerCamera"; //variable name of the player camera
    private const string turnCountTextName = "turnCountText"; //variable name of the turn counter number text

    private void OnEnable() {
        unitPanelProperty = serializedObject.FindProperty(unitPanelName); //finds the property of the UnitPanel according to its variable name
        unitsQueuedPanelProperty = serializedObject.FindProperty(unitsQueuedPanelName); //finds the property of the Units Queued Panel according to its variable name
        unitOrdersPanelProperty = serializedObject.FindProperty(unitOrdersPanelName); //finds the property of the Unit Orders Panel according to its variable name

        unitsToTrainButtonsProperty = serializedObject.FindProperty(unitsToTrainButtonName); //finds the property of the imbedded Train Unit buttons according to its variable name
        unitsToTrainButtonLockImagesProperty = serializedObject.FindProperty(unitsToTrainButtonLockImageName); //finds the property of the imbedded Train Unit buttons' lock sprite according to its variable name
        unitsToQueueButtonPrefabsProperty = serializedObject.FindProperty(unitsToQueueButtonPrefabsName); //finds the property of the imbedded Train Unit buttons' prefab according to its variable name
        
        playerCameraProperty = serializedObject.FindProperty(playerCameraName); //finds the property of the player camera according to its variable name
        turnCountTextProperty = serializedObject.FindProperty(turnCountTextName); //finds the property of the turn count text according to its variable name
    }

    public override void OnInspectorGUI() { //when the game object this script is attached to is selected in Unity       

        //shows the property field of each serialized property stated above
        serializedObject.Update();

        EditorGUILayout.PropertyField(unitPanelProperty);
        EditorGUILayout.PropertyField(unitsQueuedPanelProperty);
        EditorGUILayout.PropertyField(unitOrdersPanelProperty);
        EditorGUILayout.PropertyField(playerCameraProperty);
        EditorGUILayout.PropertyField(turnCountTextProperty);

        //creates a seperate property field for each Train Unit button
        for (int i = 0; i < UITest_dan.numUnits; i++) {
            UnitButtonSlotGUI(i);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void UnitButtonSlotGUI(int index) {
        EditorGUILayout.BeginVertical(GUI.skin.box); //creates an envelope box for each Train Unit button 
        EditorGUI.indentLevel++; //indents box in the inspector

        //toggle the visibility of the clicked envelope box
        showUnitButtons[index] = EditorGUILayout.Foldout(showUnitButtons[index], "Unit Button " + index);
        if (showUnitButtons[index]) {
            EditorGUILayout.PropertyField(unitsToTrainButtonsProperty.GetArrayElementAtIndex(index));
            EditorGUILayout.PropertyField(unitsToTrainButtonLockImagesProperty.GetArrayElementAtIndex(index));
            EditorGUILayout.PropertyField(unitsToQueueButtonPrefabsProperty.GetArrayElementAtIndex(index));
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }
}