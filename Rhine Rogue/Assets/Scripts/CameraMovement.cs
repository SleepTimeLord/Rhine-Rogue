using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CameraMovement : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    private CameraControlActions cameraActions;
    private InputAction movement;
    private InputAction rotateInGeneral;
    private InputAction zoomIn;
    [SerializeField]
    private float moveSpeed = 10f;

    [SerializeField]
    private float rotSpeed = 10f;
    private float rotDirection = 0f;

    [SerializeField]
    private int edgeScrollSize = 10;
    public bool edgeScrollingActive = false;
    [SerializeField]
    private float zoomSpeed = 50f;
    [SerializeField]
    private float followOffsetMin = 5f;
    [SerializeField]
    private float followOffsetMax = 50f;
    private Vector3 followOffset;
    public GameObject grid;

    public int maxX;
    public int maxY;
    public int minX;
    public int minY;
    public int extraRange = 2;
    // Start is called before the first frame update
    private void Awake()
    {
        transform.position = new Vector3 (0, -5, 0);
        followOffset = virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;


    }
    private void Start()
    {
        FindMapMax();
    }

    private void FindMapMax()
    {
        if (grid == null)
        {
            print("no grid assigned");
        }

        List<int> xPosition = new List<int>();
        List<int> yPosition = new List<int>();

        foreach (Transform block in grid.transform)
        {
            TileProperties tileProperties = block.GetComponent<TileProperties>();
            if (block == null) continue;
            if (tileProperties == null) continue;

            int xNumbers = tileProperties.position.x.ConvertTo<int>();
            int yNumbers = tileProperties.position.y.ConvertTo<int>();
 
            xPosition.Add(xNumbers);
            yPosition.Add(yNumbers);

            maxX = xPosition.Max() + extraRange;
            maxY = yPosition.Max() + extraRange;
            minX = xPosition.Min() - extraRange;
            minY = yPosition.Min() - extraRange;
        }
        print(maxY);
    }
    private void OnEnable()
    {
        cameraActions = new CameraControlActions();
        movement = cameraActions.Camera.Movement;

        rotateInGeneral = cameraActions.Camera.RotateCamera;
        rotateInGeneral.performed += CameraRotation;
        rotateInGeneral.canceled += CameraRotation;


        zoomIn = cameraActions.Camera.ZoomCamera;
        zoomIn.performed += ZoomCamera;

        cameraActions.Camera.Enable(); // Enable the camera actions
    }

    // Make sure to disable the actions when the object is disabled
    private void OnDisable()
    {
        cameraActions.Camera.Disable(); // Disable the camera actions
    }

    void Update()
    {


        KeyBoardMovement();
        Rotate();
        if (edgeScrollingActive)
        {
            EdgeScrolling();
        }
    }

    void KeyBoardMovement()
    {
        Vector3 position = transform.position;
        Vector3 readValue = movement.ReadValue<Vector2>();
        Vector3 inputValue = new Vector3(readValue.x, 0, readValue.y);
        inputValue = inputValue.normalized;

        // Use local directions for relative movement (based on object rotation)
        Vector3 moveDir = transform.forward * inputValue.z + transform.right * inputValue.x;

        // Stop movement if at boundaries, but keep using the transformed movement direction
        if (position.x >= maxX && moveDir.x > 0)
        {
            moveDir.x = 0;
            print("reached maxX");
        }
        if (position.x <= minX && moveDir.x < 0)
        {
            moveDir.x = 0;
            print("reached minX");
        }
        if (position.z >= maxY && moveDir.z > 0)
        {
            moveDir.z = 0;
            print("reached maxY");
        }
        if (position.z <= minY && moveDir.z < 0)
        {
            moveDir.z = 0;
            print("reached minY");
        }

        // Apply movement
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }



    void EdgeScrolling()
    {
        Vector3 inputValue = new Vector3 (0, 0, 0);

        if (Input.mousePosition.x > Screen.width - edgeScrollSize) 
        {
            //print("right");
            inputValue.x = +1f;
        }
        if (Input.mousePosition.y > Screen.height - edgeScrollSize) 
        {
            //print("up");
            inputValue.z = +1f;
        }
        if (Input.mousePosition.x < edgeScrollSize) 
        {
            //print("left");
            inputValue.x = -1f;
        }
        if (Input.mousePosition.y < edgeScrollSize) 
        {
            //print("down");
            inputValue.z = -1f;
        }

        Vector3 moveDir = transform.forward * inputValue.z + transform.right * inputValue.x;

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    void CameraRotation(InputAction.CallbackContext context)
    {
        float readValue = context.ReadValue<float>();
        
        if (context.performed && readValue > 0f)
        {
            rotDirection = -1f;
        }
        else if (context.canceled)
        {
            rotDirection = 0f;
        }
        else
        {
            rotDirection = 1f;
        }
    }
    void Rotate()
    {
        transform.eulerAngles += new Vector3(0, rotDirection * rotSpeed * Time.deltaTime, 0);
    }

    void ZoomCamera(InputAction.CallbackContext context)
    {

        float readValue = context.ReadValue<Vector2>().y;
        readValue = Mathf.Clamp01(readValue);

        Vector3 zoomDir = followOffset.normalized;

        if (readValue >= 1f)
        {
            followOffset += zoomDir;
            //print(readValue + " is greater than 1");
        }
        else if (readValue <= 0f)
        {
            followOffset -= zoomDir;
            //print(readValue + " is less than 1");
        }

        if (followOffset.magnitude < followOffsetMin)
        {
            //print("is at the minima");
            followOffset = zoomDir * followOffsetMin;
        }

        if (followOffset.magnitude > followOffsetMax)
        {
            //print("is at the maxima");
            followOffset = zoomDir * followOffsetMax;
        }
        
        Vector3 smoothZoom = Vector3.Lerp(virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, zoomSpeed * Time.deltaTime);
        virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = smoothZoom;

    }
}
