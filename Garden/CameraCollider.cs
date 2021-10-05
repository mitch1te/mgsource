using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollider : MonoBehaviour
{
    public bool colliding;
    void OnTriggerEnter(){
        Debug.Log("cam colliding");
        colliding = true;
    }
    void OnTriggerExit(){
        Debug.Log("Leaving collision");
        colliding = false;
    }
}
