using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public bool inTutorial;
    public DataManager dataManager;
    [Header("Main Pages")]
    public GameObject disclaimerPage;
    public GameObject initialTutorialPanel;
    public List<GameObject> initialTutorialPages;
    [Header("Memory Match")]
    public GameObject memoryMatchPanel;
    public List<GameObject> memoryMatchPages;
    [Header("Graceful Wind")]
    public GameObject gracefulWindPanel;
    public List<GameObject> gracefulWindPages;
    [Header("Gong Ringer")]
    public GameObject gongRingerPanel;
    public List<GameObject> gongRingerPages;
    [Header("Island")]
    public GameObject islandPanel;
    public List<GameObject> islandPages;
    [Header("Wellness")]
    public GameObject wellnessPanel;
    public List<GameObject> wellnessPages;



    public void ToggleDisclaimer(){
        if(!disclaimerPage.activeSelf){
            disclaimerPage.SetActive(true);
        } else {
            disclaimerPage.SetActive(false);
            dataManager.player.needDisclaimer = false;
            dataManager.SaveAll();
            LoadMainTutorial();
        }
    }
    public void LoadMainTutorial(){
        inTutorial = true;
        initialTutorialPanel.SetActive(true);
        initialTutorialPages[0].SetActive(true);
    }
    public void NextMainTutPage(){
        // StartCoroutine(I_NextMainTutPage());
        for(var i = 0; i < initialTutorialPages.Count; i++){
            if(initialTutorialPages[initialTutorialPages.Count-1].activeSelf){
                dataManager.player.needMainTutorial = false;
                initialTutorialPages[initialTutorialPages.Count-1].SetActive(false);
                initialTutorialPanel.SetActive(false);
                dataManager.SaveAll();
                gameObject.GetComponent<MainManager>().GoToGarden();
                break;
            }
            if(initialTutorialPages[i].activeSelf){
                initialTutorialPages[i].SetActive(false);
                initialTutorialPages[i+1].SetActive(true);
                break;
            }
        }
    }

    public void NextWellnessPage(){
        StartCoroutine(I_NextWellnessPage());
    }
    public void LoadMentalWellnessTutorial(){
        StartCoroutine(I_LoadMentalWellness());
    }
    IEnumerator I_LoadMentalWellness(){
        Debug.Log("Start");
        yield return new WaitForSeconds(5);
        Debug.Log("After");
        wellnessPanel.SetActive(true);
        wellnessPages[0].SetActive(true);
    }
    IEnumerator I_NextWellnessPage(){
        for(var i = 0; i < wellnessPages.Count; i ++){
            if(wellnessPages[wellnessPages.Count-1].activeSelf){
                GetComponent<MainManager>().moodPanel.GetComponent<MoodHistory>().tutblocked = false;
                wellnessPanel.SetActive(false);
                dataManager.player.needMainTutorial = false;
                dataManager.SaveAll();
                break;
            }
            yield return null;
            if(wellnessPages[i].activeSelf){
                if(i == 0){
                    GetComponent<MainManager>().InitiateMoodPanel();
                    // yield return new WaitForSeconds(1);
                }
                if(i == 2){
                    GetComponent<MainManager>().moodPanel.GetComponent<MoodHistory>().tutblocked = true;
                    GetComponent<MainManager>().moodPanel.GetComponent<MoodHistory>().buttonClicked = GetComponent<MainManager>().moodPanel.GetComponent<MoodHistory>().meditationButton.gameObject;
                    GetComponent<MainManager>().moodPanel.GetComponent<MoodHistory>().meditationButton.onClick.Invoke();
                }
                if(i == 3){
                    GetComponent<MainManager>().moodPanel.GetComponent<MoodHistory>().buttonClicked = GetComponent<MainManager>().moodPanel.GetComponent<MoodHistory>().affirmationsButton.gameObject;
                    GetComponent<MainManager>().moodPanel.GetComponent<MoodHistory>().affirmationsButton.onClick.Invoke(); 
                }
                if(i == 4){
                    GetComponent<MainManager>().moodPanel.GetComponent<MoodHistory>().buttonClicked = GetComponent<MainManager>().moodPanel.GetComponent<MoodHistory>().gratitudeButton.gameObject;
                    GetComponent<MainManager>().moodPanel.GetComponent<MoodHistory>().gratitudeButton.onClick.Invoke();
                }
                if(i == 6){
                    GetComponent<MainManager>().LoadMainScreen();
                    yield return new WaitForSeconds(3);
                }
                wellnessPages[i].SetActive(false);
                wellnessPages[i + 1].SetActive(true);
                break;
            }
            
        }
    }

    public void LoadMemoryTutorial(){
        memoryMatchPanel.SetActive(true);
        memoryMatchPages[0].SetActive(true);
    }
    public void NextMemoryMatchPage(){
        for(var i = 0; i < memoryMatchPages.Count; i++){
            if(memoryMatchPages[memoryMatchPages.Count-1].activeSelf){
                dataManager.player.needMemoryMatchTutorial = false;
                memoryMatchPages[memoryMatchPages.Count-1].SetActive(false);
                memoryMatchPanel.SetActive(false);
                dataManager.SaveAll();
                break;
            }
            if(memoryMatchPages[i].activeSelf){
                memoryMatchPages[i].SetActive(false);
                memoryMatchPages[i+1].SetActive(true);
                break;
            }
        }
    }

    public void LoadWindTutorial(){
        gracefulWindPanel.SetActive(true);
        gracefulWindPages[0].SetActive(true);
    }
    public void CloseWindTutorial(){
        gracefulWindPages[0].SetActive(false);
        gracefulWindPanel.SetActive(false);
        dataManager.player.needGracefulWindTutorial = false;
        dataManager.SaveAll();
    }

    public void LoadGongTutorial(){
        gongRingerPanel.SetActive(true);
        gongRingerPages[0].SetActive(true);
    }

    public void CloseGongTutorial(){
        gongRingerPages[0].SetActive(false);
        gongRingerPanel.SetActive(false);
        dataManager.player.needGongRingerTutorial = false;
        dataManager.SaveAll();
    }

    public void LoadIslandTutorial(){
        islandPanel.SetActive(true);
        islandPages[0].SetActive(true);
    }

    public void NextIslandPage(){
        for(var i = 0; i < islandPages.Count; i ++){
            if(islandPages[islandPages.Count-1].activeSelf){
                dataManager.player.needIslandTutorial = false;
                islandPages[islandPages.Count-1].SetActive(false);
                islandPanel.SetActive(false);
                dataManager.SaveAll();
                gameObject.GetComponent<MainManager>().cameraManager.gardenManager.otherBlocked = false;
                gameObject.GetComponent<MainManager>().cameraManager.SwitchCamera();
                LoadMentalWellnessTutorial();
                break;
            }
            if(islandPages[i].activeSelf){
                islandPages[i].SetActive(false);
                islandPages[i+1].SetActive(true);
                break;
            }
        }
    }

}
