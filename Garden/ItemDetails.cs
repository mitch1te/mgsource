using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetails : MonoBehaviour
{

    public enum ItemType{Plant, Decoration, Environment}
    public bool growable;
    public ItemType itemType;
    public Sprite inventoryImage;
    public Sprite itemPreview;
    public enum Rarity{Common, Uncommon, Rare, Mythical}
    public Rarity rarity;
    public GameObject model;
    public GameObject seededModel;
    public GameObject growingModel;
    public int value;
    public int beauty;
    public int timeLeft;
    public bool planted;
    
    public bool inGrowing;
    public bool fullyGrown;
    public int itemGrowingTime;
    [TextArea]
    public string bio;
    public bool selected;
    private Color savedColor;
    private bool switcher;
    public GameObject plantBoom;
    private bool playedBoom;

    void Start(){
        if(model != null){
            // savedColor = model.GetComponent<Renderer>().material.GetColor("_GlowColor");
        }
        if(planted && growable){
            UpdateGrowTime();
            if(timeLeft <= 0){
                inGrowing = false;
                fullyGrown = true;
                seededModel.SetActive(false);
                growingModel.SetActive(false);
                model.GetComponent<Renderer>().enabled = true;
            } else if(timeLeft <= (gameObject.GetComponent<ItemDetails>().itemGrowingTime/2)){
                inGrowing = true;
                seededModel.SetActive(false);
                growingModel.SetActive(true);
            }
        }
    }

    void Update(){
        if(fullyGrown && !switcher){
            growingModel.SetActive(false);
            model.GetComponent<Renderer>().enabled = true;
            switcher = true;
        }
        if(!fullyGrown && planted && growable){
            UpdateGrowTime();
            if(timeLeft <= 0){
                if(!fullyGrown){
                    plantBoom.GetComponent<ParticleSystem>().Play();
                }
                fullyGrown = true;
            }
            if(timeLeft <= (gameObject.GetComponent<ItemDetails>().itemGrowingTime/2)){
                if(!playedBoom){
                    plantBoom.GetComponent<ParticleSystem>().Play();
                    playedBoom = true;
                } 
                seededModel.SetActive(false);
                growingModel.SetActive(true);
            }
        }
    }

    // public void ChangeHighlight(){
    //     if(model != null){
    //         if(selected && model.GetComponent<Renderer>().material.HasProperty("_GlowPower")){
    //             model.GetComponent<Renderer>().material.SetFloat("_GlowPower", 20);
    //         } else {
    //             model.GetComponent<Renderer>().material.SetFloat("_GlowPower", 0);
    //         }
    //     }
    // }

    // public void ShowCollision(){
    //     if(model != null){
    //         model.GetComponent<Renderer>().material.SetFloat("_GlowPower", 20);
    //         if(gameObject.GetComponent<Item>().colliding){
    //             model.GetComponent<Renderer>().material.SetColor("_GlowColor", model.GetComponent<Renderer>().material.GetColor("_CollidingColor"));
    //         } else {
    //             model.GetComponent<Renderer>().material.SetFloat("_GlowPower", 0);
    //             model.GetComponent<Renderer>().material.SetColor("_GlowColor", savedColor);
    //         }
    //     }
    // }

    public void UpdateGrowTime(){
        // gameObject.GetComponent<Item>();
        timeLeft = (gameObject.GetComponent<ItemDetails>().itemGrowingTime) - (GetEpochTime() - gameObject.GetComponent<Item>().placedDatetime);
   
    }

    public int GetEpochTime(){
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Local);
        return (int)(System.DateTime.Now - epochStart).TotalSeconds;
    }
}
