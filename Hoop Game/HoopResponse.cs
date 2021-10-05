using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopResponse : MonoBehaviour
{
    public GameObject gameManager;
    public List<int> ignoreLayers;
    public GameObject model;
    public GameObject burst;
    public GameObject perfectBurst;
    private float rotateDir;
    private bool burstDone;
    public bool collided;
    void Awake(){
        gameManager = GameObject.Find("GameManager");
        List<float> dirs = new List<float>(){.5f, -.5f};
        rotateDir = dirs[Random.Range(0, dirs.Count)];
        Physics.IgnoreLayerCollision(6,6);
    }
    void Update(){
        gameObject.transform.Rotate(new Vector3(0,0,rotateDir));
        // destroy if too far
        if(gameObject.transform.localPosition.x < -800 || gameObject.transform.localPosition.x > 800){
            Destroy(gameObject);
            // gameManager.GetComponent<WindHoopChallenge>().ChangeAusp(-6);
        }
    }

    // void OnTriggerExit(Collider col){
    //     if(!ignoreLayers.Contains(col.gameObject.layer)){
    //         StartCoroutine(I_CloudDestroyed());
    //     }  
    // }

    public void CloudDestroyed(){
        StartCoroutine(I_CloudDestroyed());
    }
    IEnumerator I_CloudDestroyed(){
        if(collided && !burstDone){
            // gameManager.GetComponent<WindHoopChallenge>().ChangeAusp(1);
            gameManager.GetComponent<WindHoopChallenge>().normalHoops += 1;
            // gameManager.GetComponent<WindHoopChallenge>().coinsEarned += 5;
        } else {
            model.GetComponent<Renderer>().material.SetColor("_EMColor", new Color(1, .823f, 0, 1));
            // gameManager.GetComponent<WindHoopChallenge>().ChangeAusp(3);
            gameManager.GetComponent<WindHoopChallenge>().perfectHoops += 1;
            // gameManager.GetComponent<WindHoopChallenge>().coinsEarned += 10;
        }
        yield return new WaitForSeconds(.1f);
        gameObject.GetComponent<MeshCollider>().enabled = false;
        model.GetComponent<MeshRenderer>().enabled = false;
        if(!burstDone){
            if(!collided){
                perfectBurst.GetComponent<ParticleSystem>().Play();
            }
            burst.GetComponent<ParticleSystem>().Play();
            burstDone = true;
        }
        yield return new WaitForSeconds(2);
        gameManager.GetComponent<WindHoopChallenge>().destructyChance += 10f;
        Destroy(gameObject);  
    }
}
