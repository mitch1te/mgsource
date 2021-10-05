using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public DataManager dataManager;
    public CameraManager cameraManager;
    public AdManager adManager;
    public GameObject mainCanvas;

    [Header("Screens")]
    public GameObject memoryGamePanel;
    public GameObject mainPagePanel;
    public GameObject newGardenPanel;
    public GameObject newPlayerName;
    public GameObject newGardenName;
    public GameObject windGamePanel;
    public GameObject clickerGamePanel;
    public GameObject reactionGamePanel;
    public GameObject meditationPanel;
    public GameObject tokenSpendConfirmPanel;
    public Text spendTokenQuestion;
    public Button tokenSpendButton;
    public Text tokenSpendText;
    public Text coinsText;
    public Text tokensText;
    public Text activeItemText;
    public Text gardenNameText;
    [Header("Start Screen")]
    public GameObject startScreen;
    public GameObject tapToStart;
    public GameObject bonusActiveText;
    public Text versionNum;
    [Header("Overlay")]
    public GameObject overlayScreen;
    public Text overlayQuote;
    public Text overlayAttribution;
    public Text gardenOverlayQuote;
    public Text gardenOverlayAttribution;
    [Header("Audio")]
    public GameObject soundPlayer;
    public AudioClip tapChime1;
    public AudioClip confirmChime1;
    public AudioClip confirmChime2;
    public AudioClip levelSelectChime1;
    public AudioClip moodCompleteChime1;
    public AudioClip gongHit1;
    [Header("Main Screen")]
    public GameObject gameSelectionPanel;
    public GameObject moodPanel;
    public Text warningText;
    public GameObject warningBar;
    [Header("Gong Stuff")]
    public GameObject gongReadyEnergyBlurb;
    public Text gongReadyEnergyAmount;
    private int gongEnergyEscrow;
    // Bools to store what game to play
    private bool startMemoryGame;
    private bool startWindGame;
    private bool startGongClicker;
    private bool showingLoadingScreen;
    [Header("Bonus Panel")]
    public GameObject bonusPanel;
    public bool showBonus;
    public int bonusNeeded;
    public Text bonusText;
    [Header("Quotes")]
    public List<string> quotes;
    public List<string> attributions;

    void Start(){
        versionNum.text = "version: " + Application.version;
        // ChangeCanvasResolution();
        startScreen.SetActive(true);
        tapToStart.GetComponent<Animation>().Play("startPulse");
        cameraManager.mainCam.gameObject.SetActive(true);
        Application.targetFrameRate = 30;
        // Startup();
        cameraManager.gardenCam.gameObject.SetActive(false);
        cameraManager.gardenHolder.SetActive(false);
    }

    public void Startup(){
        memoryGamePanel.SetActive(false);
        
        try{
            if(dataManager.LoadGame()){   
                // Debug.Log(dataManager.player.playerName);
                newGardenPanel.SetActive(false);
                // soundPlayer.GetComponent<AudioSource>().PlayOneShot(tapChime1);
                CheckForNewTokens();
                startScreen.GetComponent<Image>().raycastTarget = false;
                LoadMainScreen();
            } else {
                newGardenPanel.SetActive(true);
                mainPagePanel.SetActive(false);
                startScreen.SetActive(false);
            }
        } catch (Exception e){
            newGardenPanel.SetActive(true);
            mainPagePanel.SetActive(false);
            startScreen.SetActive(false);
        }


        
        // try{
        //     dataManager.LoadGame();
        //     // Debug.Log(dataManager.player.playerName);
        //     newGardenPanel.SetActive(false);
        //     // soundPlayer.GetComponent<AudioSource>().PlayOneShot(tapChime1);
        //     startScreen.GetComponent<Image>().raycastTarget = false;
        //     LoadMainScreen();
            
        // } catch (Exception e){
        //     Debug.Log("Error: " + e);
        //     newGardenPanel.SetActive(true);
        //     mainPagePanel.SetActive(false);
        //     startScreen.SetActive(false);
        // }
    }

    // IEnumerator I_Startup(){
    //     memoryGamePanel.SetActive(false);
    //     dataManager.LoadGame();
    //     yield return new WaitForSeconds(1);
    //     if(dataManager.player.playerName != ""){   
    //         // Debug.Log(dataManager.player.playerName);
    //         newGardenPanel.SetActive(false);
    //         // soundPlayer.GetComponent<AudioSource>().PlayOneShot(tapChime1);
    //         startScreen.GetComponent<Image>().raycastTarget = false;
    //         LoadMainScreen();
    //     } else {
    //         newGardenPanel.SetActive(true);
    //         mainPagePanel.SetActive(false);
    //         startScreen.SetActive(false);
    //     }
    // }

    public void ShowOverlay(){
        StartCoroutine(I_ShowOverlay());
    }
    IEnumerator I_ShowOverlay(){
        overlayScreen.GetComponent<Image>().raycastTarget = true;
        Animation anims = overlayScreen.GetComponent<Animation>();
        anims.PlayQueued("overlayFadeIn");
        ChangeOverlayQuote();
        yield return StartCoroutine(WaitForAnimation(anims));
        overlayScreen.GetComponent<Image>().raycastTarget = false;
        // anims.Play("overlayFadeOut");
    }

    public void ChangeOverlayQuote(){
        int chosenQuote = UnityEngine.Random.Range(0, quotes.Count);
        overlayQuote.text = quotes[chosenQuote];
        overlayAttribution.text = attributions[chosenQuote];
        gardenOverlayQuote.text = quotes[chosenQuote];
        gardenOverlayAttribution.text = attributions[chosenQuote];
        // Debug.Log("Changing quote");
    }
    public void CreateNewGarden(){
        StartCoroutine(I_CreateNewGarden(newPlayerName.GetComponent<InputField>().text, newGardenName.GetComponent<InputField>().text));
    }

    IEnumerator I_CreateNewGarden(string playerName, string gardenName){
        dataManager.CreateNew(playerName, gardenName);
        dataManager.SaveAll();
        yield return null;
        dataManager.LoadGame();
        LoadMainScreen();
        dataManager.SaveAll();
    }
  
    // LOADING SCREEN STUFF
    void CheckForNewTokens(){
        if(dataManager.player.lastTokenDate == ""){
            dataManager.player.lastTokenDate = System.DateTime.Now.ToString("MM/dd/yyyy");
            dataManager.player.tokens = 10;
            dataManager.SaveAll();
            return;
        }
        if(dataManager.player.lastTokenDate != System.DateTime.Now.ToString("MM/dd/yyyy")){
            if(dataManager.player.tokens <= 99){
                int dayDiff = System.DateTime.Now.DayOfYear - System.DateTime.Parse(dataManager.player.lastTokenDate).DayOfYear;
                Debug.Log(dayDiff);
                for(var i = 0; i < dayDiff; i++){
                    dataManager.player.tokens += 3;
                }
            }
            dataManager.player.lastTokenDate = System.DateTime.Now.ToString("MM/dd/yyyy");
            dataManager.SaveAll();
        }
    }
    public void LoadMainScreen(){
        if(!showingLoadingScreen){
            showingLoadingScreen = true;
            // CheckForNewTokens();
            StartCoroutine(I_LoadMainScreen());
        }
    }

    IEnumerator I_LoadMainScreen(){
        soundPlayer.GetComponent<AudioSource>().PlayOneShot(tapChime1);
        yield return StartCoroutine(I_ShowOverlay());
        moodPanel.GetComponent<MoodHistory>().CheckAllComplete();
        mainPagePanel.SetActive(true);
        startScreen.SetActive(false);
        newGardenPanel.SetActive(false);
        // cameraManager.mainCam.transform.localPosition = new Vector3(0,0,-835);
        gardenNameText.text = dataManager.player.gardenName;
        memoryGamePanel.SetActive(false);
        moodPanel.SetActive(false);
        windGamePanel.SetActive(false);
        clickerGamePanel.SetActive(false);
        reactionGamePanel.SetActive(false);
        meditationPanel.SetActive(false);
        coinsText.text = dataManager.player.coins.ToString();
        tokensText.text = dataManager.player.tokens.ToString();
        activeItemText.text = dataManager.player.currentIsland.activeItems.Count.ToString();
        if(dataManager.player.moodRecord.bonusActive){
            bonusActiveText.SetActive(true);
        } else {
            bonusActiveText.SetActive(false);
        }
        if(showBonus){
            bonusPanel.SetActive(true);
            bonusText.text = bonusNeeded.ToString();
        }
        overlayScreen.GetComponent<Animation>().Play("overlayFadeOut");
        if(dataManager.player.needDisclaimer){
            GetComponent<TutorialManager>().ToggleDisclaimer();
        } else if (!dataManager.player.needDisclaimer && dataManager.player.needMainTutorial && !GetComponent<TutorialManager>().inTutorial){
            // GetComponent<TutorialManager>().LoadMainTutorial();
        }
        showingLoadingScreen = false;
    }

    public  void DismissBonus(){
        dataManager.player.coins += bonusNeeded;
        dataManager.SaveAll();
        bonusPanel.SetActive(false);
        bonusNeeded = 0;
        showBonus = false;
        coinsText.text = dataManager.player.coins.ToString();
    }
    public void GoToGarden(){
        StartCoroutine(I_GoToGarden());
    }

    public void ShowGames(){
        gameSelectionPanel.GetComponent<Animation>().Play("showGames");
        soundPlayer.GetComponent<AudioSource>().PlayOneShot(confirmChime2);
    }

    public void HideGames(){
        gameSelectionPanel.GetComponent<Animation>().Play("hideGames");
        soundPlayer.GetComponent<AudioSource>().PlayOneShot(confirmChime2);
    }
    IEnumerator I_GoToGarden(){
        soundPlayer.GetComponent<AudioSource>().PlayOneShot(confirmChime1);
        yield return StartCoroutine(I_ShowOverlay());
        cameraManager.SwitchCamera();
    }
    public void ShowTokenSpendConfirm(){
        tokenSpendButton.onClick.RemoveAllListeners();
        tokenSpendConfirmPanel.SetActive(true);
        tokenSpendText.text = "Yes!";
        if(dataManager.player.tokens == 0){
            // tokenSpendButton.interactable = false;
            tokenSpendText.text = "Watch an [AD] to gain a crystal";
            tokenSpendButton.onClick.AddListener(WatchAdFromButton);
        } else {
            tokenSpendButton.onClick.AddListener(ConfirmTokenSpend);
        }
    }

    private void WatchAdFromButton(){
        StartCoroutine(I_WatchAdFromButton());
    }
    private IEnumerator I_WatchAdFromButton(){
        adManager.InitiateAd();
        yield return new WaitForSeconds(1f);
        ShowTokenSpendConfirm();
        tokensText.text = dataManager.player.tokens.ToString();
    }
    public void CloseTokenSpend(){
        spendTokenQuestion.text = "Do you want to play [GAME]?";
        tokenSpendConfirmPanel.SetActive(false);
        ResetGameBools();
    }

    private void ResetGameBools(){
        startMemoryGame = false;
        startWindGame = false;
        startGongClicker = false;
    }

    public void ConfirmTokenSpend(){
        
        // bool gameSelected = false;
        if(startMemoryGame){
            // startMemoryGame = false;
            // gameSelected = true;
            StartCoroutine(StartMemoryGame());
        } else if (startWindGame){
            // startWindGame = false;
            // gameSelected = true;
            StartCoroutine(StartWindGame());
        } else if (startGongClicker){
            // startGongClicker = false;
            // gameSelected = true;
            dataManager.player.coins += gongEnergyEscrow;
            dataManager.player.lastGongCheck = GetEpochTime();
            dataManager.SaveAll();
            StartCoroutine(StartClickerGame());
            if(gongReadyEnergyBlurb.activeSelf){
                gongReadyEnergyBlurb.SetActive(false);
            }
        }
        Debug.Log("Spending Tokens");
        dataManager.player.tokens -= 1;
        tokensText.text = dataManager.player.tokens.ToString();
        dataManager.SaveAll();  
        CloseTokenSpend();
    }

    public void InitiateMeditation(){
        StartCoroutine(StartMeditation());
    }
    public void InitiateMemoryGame(){
        spendTokenQuestion.text = spendTokenQuestion.text.Replace("[GAME]", "Elephants Memory");
        startMemoryGame = true;
        ShowTokenSpendConfirm();
    }

    public void InitiateWindGame(){
        startWindGame = true;
        spendTokenQuestion.text = spendTokenQuestion.text.Replace("[GAME]", "Graceful Wind");
        ShowTokenSpendConfirm();
    }

    public void IntitiateClickerGame(){
        spendTokenQuestion.text = spendTokenQuestion.text.Replace("[GAME]", "Gong Ringer");
        startGongClicker = true;
        if(dataManager.player.lastGongCheck != 0){
            gongReadyEnergyBlurb.SetActive(true);
            GongEnergyWaiting();
            gongReadyEnergyAmount.text = GongEnergyWaiting().ToString();
        }
        ShowTokenSpendConfirm();
    }

    int GongEnergyWaiting(){
        gongEnergyEscrow = 0;
        // seconds since last time going into gong level
        int timeBetween = GetEpochTime() - dataManager.player.lastGongCheck;
        
        // int timeBetween = 21600;
        // Debug.Log(timeBetween);
        int secondMult1 = dataManager.player.gongSpiritRateLevel;
        if(secondMult1 < 1){secondMult1 = 1;}
        int secondMult2 = dataManager.player.gongSpiritLevel * 5;
        if(secondMult2 < 1){secondMult2 = 5;}
        float levelMult = 1;
        for(var i = 0; i < dataManager.player.gongMultiplierLevel; i++){
            levelMult += .1f;
        }
        // Debug.Log(levelMult);
        int energyWaiting = (((secondMult1 * secondMult2) * timeBetween) / 1250);
        energyWaiting = (int)Mathf.Round(energyWaiting * levelMult);
        // Debug.Log(energyWaiting);
        gongEnergyEscrow = energyWaiting + 10;
        return gongEnergyEscrow;
    }

    // public void InititateReactionGame(){
    //     mainPagePanel.SetActive(false);
    //     reactionGamePanel.SetActive(true);
    //     StartCoroutine(StartReactionGame());
    // }

    public void InitiateMoodPanel(){
        soundPlayer.GetComponent<AudioSource>().PlayOneShot(confirmChime2);
        moodPanel.SetActive(true);
        moodPanel.GetComponent<MoodHistory>().LoadMoodPanel();
    }

    public void CloseMoodPanel(){
        moodPanel.SetActive(false);
    }

    IEnumerator StartReactionGame(){
        // cameraManager.mainCam.transform.localPosition = new Vector3(0,0,-1340);
        yield return new WaitForSeconds(0.25f);
        gameObject.GetComponent<ReactionChallenge>().Setup();
    }
    IEnumerator StartClickerGame(){
        soundPlayer.GetComponent<AudioSource>().PlayOneShot(levelSelectChime1);
        yield return StartCoroutine(I_ShowOverlay());
        mainPagePanel.SetActive(false);
        clickerGamePanel.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        gameSelectionPanel.GetComponent<Animation>().Play("hideGames");
        gameObject.GetComponent<ClickerGame>().Setup();
        if(dataManager.player.needGongRingerTutorial){
            GetComponent<TutorialManager>().LoadGongTutorial();
        }
        overlayScreen.GetComponent<Animation>().Play("overlayFadeOut");
    }
    IEnumerator StartMeditation(){
        soundPlayer.GetComponent<AudioSource>().PlayOneShot(levelSelectChime1);
        yield return StartCoroutine(I_ShowOverlay());
        mainPagePanel.SetActive(false);
        meditationPanel.SetActive(true);
        gameSelectionPanel.GetComponent<Animation>().Play("hideGames");
        // cameraManager.mainCam.transform.localPosition = new Vector3(0,0,-1340);
        overlayScreen.GetComponent<Animation>().Play("overlayFadeOut");
    }
    IEnumerator StartWindGame(){
        soundPlayer.GetComponent<AudioSource>().PlayOneShot(levelSelectChime1);
        yield return StartCoroutine(I_ShowOverlay());
        // Debug.Log("waiting");
        yield return new WaitForSeconds(2);
        windGamePanel.SetActive(true);
        // Debug.Log("Starting");
        mainPagePanel.SetActive(false);
        windGamePanel.SetActive(true);
        gameObject.GetComponent<WindHoopChallenge>().tapToStart.GetComponent<Animation>().Play("startPulse");
        gameSelectionPanel.GetComponent<Animation>().Play("hideGames");
        // cameraManager.mainCam.transform.localPosition = new Vector3(0,0,-1340);
        gameObject.GetComponent<WindHoopChallenge>().Setup();
        if(dataManager.player.needGracefulWindTutorial){
            GetComponent<TutorialManager>().LoadWindTutorial();
        }
        overlayScreen.GetComponent<Animation>().Play("overlayFadeOut");
    }
    IEnumerator StartMemoryGame(){
        soundPlayer.GetComponent<AudioSource>().PlayOneShot(levelSelectChime1);
        yield return StartCoroutine(I_ShowOverlay());
        mainPagePanel.SetActive(false);
        memoryGamePanel.SetActive(true);
        gameObject.GetComponent<ButtonChallenge>().tapButtons = 1;
        gameSelectionPanel.GetComponent<Animation>().Play("hideGames");
        // cameraManager.mainCam.transform.localPosition = new Vector3(0,0,-1340);
        gameObject.GetComponent<ButtonChallenge>().SetUp();
        if(dataManager.player.needMemoryMatchTutorial){
            GetComponent<TutorialManager>().LoadMemoryTutorial();
        }
        overlayScreen.GetComponent<Animation>().Play("overlayFadeOut");
    }
    private IEnumerator WaitForAnimation ( Animation animation ){
        do { yield return null; } while ( animation.isPlaying );
    }

    public void ShowWarning(string text){
        warningText.GetComponent<Text>().text = text;
        warningText.GetComponent<Animation>().Play("showWarning");
        warningBar.GetComponent<Animation>().Play("popupBarFadeIn");
    }
    void ChangeCanvasResolution(){
        mainCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(Screen.width, Screen.height);
    }
    int GetEpochTime(){
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Local);
        return (int)(System.DateTime.Now - epochStart).TotalSeconds;
    }
    // PLAYER/INVENTORY MANAGEMENT


}
