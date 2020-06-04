﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // do not need to be public
    Transform saveTransform; // used to save original camera position and location

    // target to view
    public GameObject BattleManager;
    public GameObject CameraTarget;

    public Transform targetView; //  this is the transfor to use when viewing target

    // set the offset and rotation of camera
    public Vector3 offsetPosition;
    public float smoothSpeed = 0.15f;

    // Start is called before the first frame update
    void Start()
    {
        saveTransform = transform; // save original position and rotation
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // called once per frame after update
    void LateUpdate()
    {
        // if camera folloing is true
        if(BattleManager.GetComponent<BattleManager>().cameraFollowing)
        {
            transform.position = CameraTarget.transform.position + offsetPosition; // moves camera close to target
            transform.LookAt(CameraTarget.transform); // makes camera look at target
        }
    }
}