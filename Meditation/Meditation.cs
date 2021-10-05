using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Meditation : MonoBehaviour
{
    public DataManager dataManager;
    public MainManager mainManager;
    public bool held;
    public GameObject darknessOverlay;
    public GameObject finishedPopUp;
    public GameObject backdrop;
    public Text meditationCount;
    public Text sessionLongest;
    public int prevMeditation;
    public int meditationTime;
    public int fingers;
    private bool meditating;
    private bool doneCountdown;
    public float doneCount;
    private LTDescr fadetween;
    private UnityEngine.Coroutine med;
    void Start(){
        finishedPopUp.SetActive(false);
        // backdrop.SetActive(false);
    }
    void Update(){
        if(!held){
            meditating = false;
            ResetOverlayColor();
        }
        if(held && !meditating){
            doneCount = 0;
            meditating = true;
            doneCountdown = false;
            med = StartCoroutine(CountMeditation());
        }     
        if(doneCountdown && doneCount < 3){
            doneCount += 1 * Time.deltaTime;
        }
        if(doneCount >= 3 && !held){
            doneCount = 0;
            doneCountdown = false;
            // backdrop.SetActive(true);
            finishedPopUp.SetActive(true);
        }
    }

    public void ButtonHeld(){
        held = true;
        fingers += 1;
    }
    public void ButtonReleased(){
        fingers -= 1;
        if(fingers == 0){
            held = false;
            StopCoroutine(med);
            doneCountdown = true;
        }
    }


    IEnumerator CountMeditation(){
        Color lastColor = darknessOverlay.GetComponent<Image>().color;
        lastColor.a = 0;
        LeanTween.alpha(darknessOverlay.GetComponent<Image>().rectTransform, 1, 5);
        if(!held){
            meditating = false;
            yield break;
        }
        while(meditating){
            yield return new WaitForSeconds(1);
            if(held){
                meditationTime += 1;
                meditationCount.text = ConvertIntToTime(meditationTime);    
            }
        }
    }

    public void ContinueMeditation(){
        if(meditationTime > prevMeditation){
            sessionLongest.text = ConvertIntToTime(meditationTime);
        }
        prevMeditation = meditationTime;
        meditationTime = 0;
        finishedPopUp.SetActive(false);
    }

    public void SaveAndExit(){
        dataManager.player.moodRecord.lastDateMeditated = System.DateTimeOffset.Now.ToString("MM/dd/yyyy");
        if(meditationTime > dataManager.player.moodRecord.longestMeditation){
            dataManager.player.moodRecord.longestMeditation = meditationTime;
        }
        if(meditationTime > dataManager.player.moodRecord.daysLongestMeditation){
            dataManager.player.moodRecord.daysLongestMeditation = meditationTime;
        }
        if(meditationTime >= 600){
            dataManager.player.moodRecord.meditationComplete = true;
        }
        dataManager.SaveAll();
        finishedPopUp.SetActive(false);
        mainManager.GetComponent<MainManager>().InitiateMoodPanel();
        meditationTime = 0;
        prevMeditation =  0;
        gameObject.SetActive(false);
    }
    private void ResetOverlayColor(){
        LeanTween.cancel(darknessOverlay.GetComponent<Image>().rectTransform);
        LeanTween.alpha(darknessOverlay.GetComponent<Image>().rectTransform, 0, 1);
    }
    public string ConvertIntToTime(int timeToConvert){
        var time = System.TimeSpan.FromSeconds(timeToConvert);
        return time.ToString(@"hh\:mm\:ss");
    }

    public string ConvertEpochToDate(int timeToConvert){
        var datetime =  System.DateTimeOffset.FromUnixTimeSeconds(timeToConvert);
        Debug.Log(datetime.ToString("MM/dd/yyyy"));
        return datetime.ToString("MM/dd/yyyy");
    }
}
