using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentAdjust : MonoBehaviour {

    Transform player;

    void OnEnable()
    {
        player = Camera.main.GetComponent<CameraController>().target;
    }

    void FixedUpdate()
    {
        transform.position -= player.position;
        player.position = Vector3.zero;
    }




    //// move the world so that the camera is always at 0, 0, 0
    //Transform mainCamera;
    //void OnEnable()
    //{
    //    mainCamera = Camera.main.transform.parent;
    //}

    //void FixedUpdate()
    //{
    //    transform.position = -mainCamera.localPosition;
    //}
}
