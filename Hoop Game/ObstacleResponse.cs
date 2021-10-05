using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleResponse : MonoBehaviour
{    
    
    void Update(){
        if(gameObject.transform.localPosition.y < -1800){
            Destroy(gameObject);
        }
    }


}
