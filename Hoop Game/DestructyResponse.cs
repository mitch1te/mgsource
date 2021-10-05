using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructyResponse : MonoBehaviour
{
    public GameObject gameManager;
    public bool instaLoss;
    private float rotateDir;
    void Awake(){
        gameManager = GameObject.Find("GameManager");
        Physics.IgnoreLayerCollision(6,6);
        List<float> dirs = new List<float>(){.5f, -.5f};
        rotateDir = dirs[Random.Range(0, dirs.Count)];
    }
    void Update(){
        if(!instaLoss){
            gameObject.transform.Rotate(new Vector3(0,rotateDir,0));
        }
        // destroy if too far
        if(gameObject.transform.localPosition.x < -850 || gameObject.transform.localPosition.x > 850){
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision col){
        // if(instaLoss){
            gameManager.GetComponent<WindHoopChallenge>().GameOver();
        //     return;
        // }
        // Debug.Log(col.gameObject.name);
        // foreach(Transform obj in gameManager.GetComponent<WindHoopChallenge>().spawnedHolder.transform){
        //     Destroy(obj.gameObject);
        // }
        // gameManager.GetComponent<WindHoopChallenge>().lives -= 1;
        // gameManager.GetComponent<WindHoopChallenge>().balance = 50;
        // gameManager.GetComponent<WindHoopChallenge>().coinsEarned =
        // Mathf.RoundToInt(gameManager.GetComponent<WindHoopChallenge>().coinsEarned /2);
        // Destroy(gameObject);
    }
}
