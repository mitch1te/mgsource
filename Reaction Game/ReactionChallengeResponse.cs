using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ReactionChallengeResponse : MonoBehaviour
{
 
    private ReactionChallenge reactionChallenge;
    public bool isDragging;
    
    void Awake(){
        reactionChallenge = GameObject.Find("GameManager").gameObject.GetComponent<ReactionChallenge>();
        RevealSpirit();
    }
    void Update(){
        if(isDragging){  
            reactionChallenge.chainLine.GetComponent<LineRenderer>().positionCount = reactionChallenge.chainPos + 1;
            try{
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit)){ 
                    reactionChallenge.chainLine.GetComponent<LineRenderer>().SetPosition(reactionChallenge.chainPos, hit.point);
                    // Debug.Break();
                    if(hit.transform.gameObject.name.Contains(gameObject.name) && !reactionChallenge.chainedSpirits.Contains(hit.transform.gameObject)){
                        reactionChallenge.chainedSpirits.Add(hit.transform.gameObject);
                        // reactionChallenge.chainLine.GetComponent<LineRenderer>().positionCount += 1;
                        reactionChallenge.chainLine.GetComponent<LineRenderer>().SetPosition(reactionChallenge.chainPos, hit.transform.TransformPoint(hit.transform.localPosition));
                        reactionChallenge.chainPos += 1;
                    }
                }
            } catch (Exception e){
                Debug.Log(e);
            }
     
        }
    }

    void RevealSpirit(){
        LeanTween.moveLocalY(gameObject, 1, 1f);
        StartCoroutine(HideSpirit());
    }

    public void DestroyOne(){
        reactionChallenge.chainedSpirits.Clear();
        MakeHoleViable(gameObject);
        Debug.Log("Destroyed One");
        Destroy(gameObject);
        reactionChallenge.chainPos = 0;
    }
    
    public void DestroyMultiple(){
        foreach(GameObject spirit in reactionChallenge.chainedSpirits){
            MakeHoleViable(spirit);
            reactionChallenge.chainLength += 1;
            Destroy(spirit.gameObject);
        }
        Debug.Log("Chained " + reactionChallenge.chainLength + " spirits");
        reactionChallenge.chainPos = 0;;
        reactionChallenge.chainedSpirits.Clear();

    }
    void OnMouseUp(){
        reactionChallenge.chainLine.GetComponent<LineRenderer>().positionCount = 0;
        isDragging = false;
        if(reactionChallenge.chainedSpirits.Count > 1){
            DestroyMultiple();
        } else {
            DestroyOne();
        }
        reactionChallenge.chainLength = 0;
    }
    // void HideLine(){
    //     Debug.Log("hiding");
    //     reactionChallenge.chainLine.GetComponent<LineRenderer>().startWidth = 0;
    //     reactionChallenge.chainLine.GetComponent<LineRenderer>().endWidth = 0;
    // }
    // void ShowLine(){
    //     Debug.Log("showing");
    //     reactionChallenge.chainLine.GetComponent<LineRenderer>().startWidth = 1;
    //     reactionChallenge.chainLine.GetComponent<LineRenderer>().endWidth = 1;
    // }
    void OnMouseDown(){
        // reactionChallenge.chainLine.GetComponent<LineRenderer>().positionCount += 1;
        isDragging = true;
    }
    public void MakeHoleViable(GameObject hole){
        reactionChallenge.holes.Add(hole.transform.parent.gameObject);
    }

    // IEnumerator LineHider(){
    //     while(isDragging){
    //         if(reactionChallenge.chainLine.GetComponent<LineRenderer>().positionCount > 0){
    //             if(reactionChallenge.chainLine.GetComponent<LineRenderer>().GetPosition(0) == new Vector3(0,0,0)){
    //                 HideLine();
    //                 yield return new WaitForEndOfFrame();
    //             } else {
    //                 ShowLine();
    //             }
    //         }
    //     }
    // }
    IEnumerator HideSpirit(){
        yield return new WaitForSeconds(5);
        LeanTween.moveLocalY(gameObject, -1, 1f);
        yield return new WaitForSeconds(1f);
        MakeHoleViable(gameObject);
        if(reactionChallenge.chainedSpirits.Contains(gameObject)){
            reactionChallenge.chainedSpirits.Clear();
            reactionChallenge.chainLine.GetComponent<LineRenderer>().positionCount = 0;
            Debug.Log("Big oof");
        }
        Destroy(gameObject);
        reactionChallenge.chainPos = 0;
        Debug.Log("Bad");
        
    }
}
