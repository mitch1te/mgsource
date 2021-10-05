using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscResponse : MonoBehaviour
{
    public GameObject bad;
    public GameObject good;

    void Update(){
        if(gameObject.transform.localPosition.y < -1100){
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision col){
        foreach(ContactPoint contact in col.contacts){
            // bad
            if(contact.thisCollider.name == bad.name){
                // col.gameObject.GetComponent<OrbResponse>().gameManager.GetComponent<WindHoopChallenge>().ChangeAusp(-10);
                Destroy(gameObject);
            } 
            // good
            else if (contact.thisCollider.name == good.name){
                col.gameObject.GetComponent<OrbResponse>().gameManager.GetComponent<WindHoopChallenge>().discs += 1;
                // col.gameObject.GetComponent<OrbResponse>().gameManager.GetComponent<WindHoopChallenge>().ChangeAusp(5);
                Destroy(gameObject);
            }
        }
    }
}
