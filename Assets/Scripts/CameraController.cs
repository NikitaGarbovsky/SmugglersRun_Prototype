
using Unity.Collections;
using UnityEngine;


/// <summary>
/// Control the main camera functions that help the player manipulate and control where they are looking in the game world.
/// </summary>

public class CameraController : MonoBehaviour
{
    //Global variables
    private Camera m_Cam;
    [SerializeField]
    public Transform m_CameraPivotTransform;
    private Vector3 m_DragOrigin;
    
    // Rotating the camera Variables
    private bool bIsRotating = false;
    private float fTargetAngle;
    [SerializeField]
    private float fRotationSpeed = 10f;

    // Tune-able variables shown to user
    [Header("Camera Variables")] 
    [SerializeField]
    private float zoomAmount;
    [SerializeField]
    private float minCamSize;
    [SerializeField]
    private float maxCamSize;
    
    // Sometimes it may be confusing to figure out which direction in the game world you are facing, 
    // This enum represents where that is. 
    // TODO make this field display in the editor but readonly (not changeable at run time)
    CameraFacingDirection TCameraFacingDirection;
    private enum CameraFacingDirection
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }
    
    private void Update()
    {
        PanCamera();
        ZoomCamera();
        IsCameraCurrentlyRotating();
    }

    private void IsCameraCurrentlyRotating()
    {
        // Handles the rotation of the camera using a lerp. 
        if (bIsRotating)
        {
            // Rotate towards the target rotation using lerp
            m_CameraPivotTransform.rotation = Quaternion.Lerp(m_CameraPivotTransform.rotation, Quaternion.Euler(0f, fTargetAngle, 0f), fRotationSpeed * Time.deltaTime);

            // Check if we are close enough to the target rotation
            if (Quaternion.Angle(m_CameraPivotTransform.rotation, Quaternion.Euler(0f, fTargetAngle, 0f)) < 0.1f)
            {
                m_CameraPivotTransform.rotation = Quaternion.Euler(0f, fTargetAngle, 0f); // Set exact rotation
                bIsRotating = false; // Stop rotating
            }
        }
        else
        {
            // Handle rotation input
            if (Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.LeftControl))
            {
                RotateCameraWithThisAngle(45f); // Rotate 45 degrees clockwise
            }
            else if (Input.GetKeyDown(KeyCode.A) && Input.GetKey(KeyCode.LeftControl))
            {
                RotateCameraWithThisAngle(-45f); // Rotate 45 degrees counterclockwise
            }
        }
    }
    // Rotates the camera while keeping orthographic perspective
    private void RotateCameraWithThisAngle(float angle)
    {
        // Calculate target angle based on current rotation and desired angle
        float currentAngle = m_CameraPivotTransform.rotation.eulerAngles.y;
        fTargetAngle = Mathf.Round(currentAngle / 45f) * 45f + angle; // Round to nearest multiple of 45 and add desired angle
        bIsRotating = true; // Start rotating
    }
    private void Start()
    {
        m_Cam = GetComponent<Camera>();
        // Upon startup, sets the camera direction to front
        TCameraFacingDirection = CameraFacingDirection.North;
    }
    
    
    
    private void PanCamera() //Pans camera on primary mouse button click & hold (DOES NOT ROTATE CAMERA)
    {
        //Save position of mouse in world space when drag start (first time clicked)

        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl))
        {
            m_DragOrigin = m_Cam.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log( "Starting position" +"origin " + dragOrigin + " newPosition " + cam.ScreenToWorldPoint(Input.mousePosition));
        }
        
        
        //Calculate distance between drag origin and new position if it is still held down
        
        if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl))
        {
            Vector3 difference = m_DragOrigin - m_Cam.ScreenToWorldPoint(Input.mousePosition);

            // Because the camera is not actually moving up or down, just horizontally, we need to use that y difference
            // the player is using to move the camera, which changes depending on the direction the camera is facing
            switch (TCameraFacingDirection)
            {
                // TODO, add cases for NW,SW,SE,NE
                case CameraFacingDirection.North:
                {
                    difference.z += difference.y;
                    break;
                }
                case CameraFacingDirection.West:
                {
                    difference.x += difference.y;
                    break;
                }
                case CameraFacingDirection.South:
                {
                    difference.z -= difference.y;
                    break;
                }
                case CameraFacingDirection.East:
                {
                    difference.x -= difference.y;
                    break;
                }
            }
            
            difference.y = 0f;
            
            m_CameraPivotTransform.position += (difference);
        }
    }
    //Zooms the camera in and out using the scroll-wheel
    public void ZoomCamera()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && Input.GetKey(KeyCode.LeftControl))
        {
            ZoomIn();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0 && Input.GetKey(KeyCode.LeftControl))
        { 
            ZoomOut();
        }
    }
    //Camera Zoom in & out functions 
    public void ZoomIn()
    {
        float newSize = m_Cam.orthographicSize + zoomAmount;

        m_Cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
    }
    public void ZoomOut()
    {
        float newSize = m_Cam.orthographicSize - zoomAmount;
        m_Cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
    }
}
