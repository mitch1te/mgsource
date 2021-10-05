using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Vault Info")]
    public int itemID;
    public string itemName;
    [Header("In game info")]
    public int uid;
    public Vector3 itemLocation;
    public Vector3 itemRotation;
    
    public int placedDatetime;
    public bool colliding;
    public bool moving;
    public bool movingValid;

    void OnCollisionExit(Collision col){
        colliding = false;
        movingValid = true;
        // gameObject.GetComponent<ItemDetails>().ChangeHighlight();
    }

    void OnCollisionEnter(){
        // gameObject.GetComponent<ItemDetails>().ShowCollision();
    }
    void OnCollisionStay(Collision col){
        if(moving){
            // gameObject.GetComponent<ItemDetails>().model.GetComponent<Renderer>().material.SetFloat("_GlowPower", 20);
            if(gameObject.GetComponent<ItemDetails>().itemType == ItemDetails.ItemType.Plant || gameObject.GetComponent<ItemDetails>().itemType == ItemDetails.ItemType.Decoration){
                if(col.gameObject.layer != LayerMask.NameToLayer("Soil") ){
                    colliding = true;
                    // gameObject.GetComponent<ItemDetails>().ShowCollision();
                } 
                // if(col.gameObject.layer != LayerMask.NameToLayer("Soil")){
                //     colliding = true;
                //     gameObject.GetComponent<ItemDetails>().ShowCollision();
                // }
                if(col.gameObject.layer == LayerMask.NameToLayer("Soil")){
                    colliding = false;
                    movingValid = true;
                    // gameObject.GetComponent<ItemDetails>().ShowCollision();
                    // gameObject.GetComponent<ItemDetails>().ChangeHighlight();
                } else {
                    movingValid = false;
                }
            }

            // if(gameObject.GetComponent<ItemDetails>().itemType == ItemDetails.ItemType.Pot){
            //    movingValid = false;
            // }
        }

    
    }
}
