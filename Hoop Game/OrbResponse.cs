using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbResponse : MonoBehaviour
{

public GameObject gameManager;
public bool active;

public bool collided;
    public void ReadyUp(){
        active = true;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
    }

    void OnCollisionEnter(Collision col){
        if(col.gameObject.layer == LayerMask.NameToLayer("HoopGameObjects")){
            col.gameObject.GetComponent<HoopResponse>().collided = true;
        }  
    }
    
    void OnTriggerExit(Collider col){
        if(col.gameObject.layer == LayerMask.NameToLayer("HoopGameObjects")){
            col.transform.parent.gameObject.GetComponent<HoopResponse>().CloudDestroyed();
        }    
    }       
}
