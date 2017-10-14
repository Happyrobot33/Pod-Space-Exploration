using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    CanvasController canvas;

    [Header("Following")]
    public Transform target;
    public float distanceToTarget = 3f;

    [Header("Rotate")]
    public bool canRotate = true;
    public float rotateSpeed = 0.5f;

    [Header("Zoom")]
    public bool canZoom = true;
    public float zoomSpeed = 0.001f;
    public float zoomSmoothing = 0.25f;


    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<CanvasController>();
        transform.localPosition = new Vector3(0, 0, -distanceToTarget);
    }

    public enum CameraMode
    {
        Free,
        Fixed
    }
    public CameraMode mode;


    // messy pc support
    Vector3 oldMousePos = Vector3.zero;
    bool first = true;

    class MultiplatformTouch
    {
        public Vector2 position;
        public Vector2 deltaPosition;
        public Vector2 oldPosition;

        public MultiplatformTouch(Vector2 _position, Vector2 _deltaPosition, Vector2 _oldPosition)
        {
            position = _position;
            deltaPosition = _deltaPosition;
            oldPosition = _oldPosition;
        }
    }

    public List<Collider> touchedColliders = new List<Collider>();
    public float cameraRotationOffset = 0f;
    void Update ()
    {
        // find which touches are suitable
        float rightTouchLimit = canvas.edgeMarker.extScreenPosition().x;
        //Debug.Log(rightTouchLimit + " " + Screen.width);
        List<MultiplatformTouch> validTouches = new List<MultiplatformTouch>();
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.touches[i].position.x < rightTouchLimit)
                validTouches.Add(new MultiplatformTouch(
                    Input.touches[i].position,
                    Input.touches[i].deltaPosition,
                    Input.touches[i].position - Input.touches[i].deltaPosition) );
        }

        // add pc functionality
        first = false;
        if (first)
            oldMousePos = Input.mousePosition;
        Input.simulateMouseWithTouches = false;
        if (Input.GetMouseButton(0) && Input.mousePosition.x < rightTouchLimit)
        {
            MultiplatformTouch touch = new MultiplatformTouch(
                    Input.mousePosition,
                    Input.mousePosition - oldMousePos,
                    oldMousePos);
            validTouches.Add(touch);
        }   
        oldMousePos = Input.mousePosition;
        // remove this
        if(Input.GetKey(KeyCode.Z))
            distanceToTarget -= 8 * zoomSpeed * distanceToTarget;
        if(Input.GetKey(KeyCode.X))
            distanceToTarget += 8 * zoomSpeed * distanceToTarget;





        ////// camera controls



        // lock camera to target
        transform.parent.position = target.position;

        // allow these for all camera modes
        Vector3 eulerRotation = Vector3.zero;
        if (validTouches.Count == 1)
        {
            if(canRotate)
            {
                // rotate on 1-finger swipe
                float deltaX = rotateSpeed * validTouches[0].deltaPosition.x;
                float deltaY = rotateSpeed * validTouches[0].deltaPosition.y;
                eulerRotation += new Vector3(-deltaY, deltaX, 0);
            }
            
        }
        if (validTouches.Count == 2)
        {
            if(canZoom)
            {
                // 2-finger zoom
                float oldMag = (validTouches[1].oldPosition - validTouches[0].oldPosition).magnitude;
                float newMag = (validTouches[1].position - validTouches[0].position).magnitude;
                float touchMagnitude = oldMag - newMag;
                distanceToTarget += touchMagnitude * zoomSpeed * distanceToTarget; // multuply by self because exponential
                if (distanceToTarget == 0)
                    distanceToTarget = 0.001f;
            }
            
            if(canRotate)
            {
                //// 2-finger spin
                //Vector2 oldDirection = (validTouches[1].oldPosition - validTouches[0].oldPosition);
                //Vector2 newDirection = (validTouches[1].position - validTouches[0].position);
                //Vector2 deltaDirection = newDirection - oldDirection;
                //float touchSpin = Mathf.Atan2(deltaDirection.y, deltaDirection.x);
                //eulerRotation += new Vector3(0, 0, touchSpin);
            }

        }

        // these camera controls are mode-dependent
        if (mode == CameraMode.Free)
        {

        }
        else if(mode == CameraMode.Fixed)
        {
            if(validTouches.Count == 0)
            {
                Vector3 targetRotation = target.eulerAngles + new Vector3(cameraRotationOffset, 0f, 0f);
                transform.parent.rotation = Quaternion.Slerp(
                    transform.parent.rotation,
                    Quaternion.Euler(targetRotation),
                    0.1f);
            }
        }


        // set camera's position relative to parent
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            new Vector3(0, 0, -distanceToTarget),
            zoomSmoothing);

        // rotate
        transform.parent.Rotate(eulerRotation);
        eulerRotation = Vector3.zero;




    }
}

