using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cam;
    private Vector3 clickorigin;
    private float zoom;
    private float zoomSpeed = 4f;
    private float maxZoom = 8f;
    private float minZoom = 2f;
    private float velocity = 0f;
    private float smoothTime = .2f;
    private void Start()
    {
        zoom = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        cameraclickmove();
        camerazoom();
    }

    private void cameraclickmove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickorigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        // held down pos
        if (Input.GetMouseButton(0))
        {
            Vector3 difference = clickorigin - cam.ScreenToWorldPoint(Input.mousePosition);

            cam.transform.position += difference;
        }
    }
    
    private void camerazoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoom -= scroll *= zoomSpeed;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, smoothTime);
    }
}
