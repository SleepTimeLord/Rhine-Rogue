using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cam;
    private Vector3 clickorigin;

    // Update is called once per frame
    void Update()
    {
        cameraclickmove();
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
    }
}
