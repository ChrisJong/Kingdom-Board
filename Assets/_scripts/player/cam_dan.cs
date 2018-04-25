using UnityEngine;

public class cam_dan : MonoBehaviour {

    public float panSpeed = 20f;
    public float panBorderThickness = 5f;
    public float scrollSpeed = 10f;
    private float panLimitX;
    private Vector2 panLimitY;
    private float zoomPanLimitRatioX = 10f;

    private float currentZoom = 1f;
    public float maxZoom = 2.25f;
    public float minZoom = 0.5f;

    public GameObject mainCamera;

	
	// Update is called once per frame
	void Update () {
        //Detect input to move camera
        Vector3 pos = transform.position;

        if (Input.GetKey("w") || (Input.mousePosition.y >= Screen.height - panBorderThickness)) {
            pos.z += panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("s") || (Input.mousePosition.y <= panBorderThickness))
        {
            pos.z -= panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("a") || (Input.mousePosition.x <= panBorderThickness))
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("d") || (Input.mousePosition.x >= Screen.width - panBorderThickness))
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        //Clamps camera to game region
        pos.x = Mathf.Clamp(pos.x, -panLimitX, panLimitX);
        pos.z = Mathf.Clamp(pos.z, panLimitY.x, panLimitY.y);
        //Move camera call function
        MoveCamera(pos);

        //Detect input to zoom camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        //Scroll up means zoom in
        if (scroll > 0) {
            currentZoom += scroll;
            if (currentZoom > maxZoom)
            {
                currentZoom = maxZoom;
            }
            else {
                ZoomCamera(scroll);
            }
        }
        //Scroll down means zoom out
        if (scroll < 0) {
            currentZoom += scroll;
            if (currentZoom < minZoom)
            {
                currentZoom = minZoom;
            }
            else {
                ZoomCamera(scroll);
            }
        }

        //Update camera pan limit to suit zoom level
        panLimitX = currentZoom * zoomPanLimitRatioX;
        panLimitY.x = ((5f + (currentZoom * 2.5f)) - (currentZoom * 7.5f));
        panLimitY.y = ((5f + (currentZoom * 2.5f)) + (currentZoom * 7.5f));
    }

    void MoveCamera (Vector3 moveDirection) {
        transform.position = moveDirection;
    }

    void ZoomCamera (float zoomDirection) {
        mainCamera.transform.Translate(0, 0, zoomDirection * scrollSpeed);
    }
}
