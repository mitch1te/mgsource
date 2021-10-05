using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
    public DataManager dataManager;
    public MainManager mainManager;
    public int androidID;
    public int iosID;

    void Start(){
        Advertisement.Initialize(androidID.ToString(), false);
    }
    public void InitiateAd(){
        PlayFullScreenAd();
        Debug.Log("Started ad");
        AdFinished();
    }
    public void AdFinished(){
        Debug.Log("Ad finished, Adding crystal");
        dataManager.player.tokens += 1;
        dataManager.SaveAll();
        mainManager.tokensText.text = dataManager.player.tokens.ToString();
    }

    void PlayFullScreenAd(){
        if(Advertisement.IsReady()){
            Advertisement.Show("Interstitial_Android");
            Debug.Log("Showing ad");
        } else {
            Debug.Log("Ad not ready");
        }
    }
}
