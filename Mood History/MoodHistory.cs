using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoodHistory : MonoBehaviour
{
    public MainManager mainManager;
    // 8 hrs in seconds == 28800
    private int timeBetweenRecords = 21600;
    [Header("Recording Stuff")]
    public GameObject moodCheckmark;
    public GameObject meditationCheckMark;
    public GameObject affCheckmark;
    public GameObject thankfulCheckMark;
    public DataManager dataManager;
    public List<Image> moodImages;
    public Text streakReadout;
    public GameObject moodselector;
    // public GameObject recordMoodButton;
    public Text recordMoodText;
    private bool needRecordCountdown;
    private bool recordCountdownStarted;
    public Text nextMoodRecordCountdown;
    public Text startOfDayMood;
    public Text endOfDayMood;
    public GameObject confirmMoodPopUp;
    public Text confirmedMoodAmount;
    public Image confirmationImagePreview;
    private int moodToRecord;
    [Header("History Stuff")]
    public GameObject blankBuffer;
    public Text header;
    public GameObject dateSection;
    public GameObject monthHistoryPanel;
    public GameObject monthHistoryContainer;
    public System.DateTime viewingDate;
    public List<Sprite> moodIcons;
    private GameObject lastSection;
    [Header("Selection Stuff")]
    public List<GameObject> activityButtons;
    public List<GameObject> activitySections;
    [Header("Affirmations")]
    public Text affirmationAmountDisplay;
    public GameObject startAffirmationButton;
    public GameObject affirmationPopupWindow;
    public GameObject affirmationEditSection;
    public GameObject editAffirmationsPanel;
    public GameObject addAffirmationButton;
    public List<GameObject> allAffirmations;
    // affirmation readout
    public GameObject affirmationReadoutPanel;
    public Text affirmationInstructions;
    public Text affirmationReadout;
    public GameObject spokenConfirmationButton;
    bool readyForNextAff = false;
    [Header ("Meditation")]
    public Text meditationStatusText;
    [Header ("Gratitude")]
    public GameObject gratitudeHeader;
    public GameObject gratitudeInput;
    public GameObject gratitudeSubmit;
    public Text gratitudeText;
    [Header("Buttons")]
    public Button meditationButton;
    public Button moodButton;
    public Button affirmationsButton;
    public Button gratitudeButton;
    public GameObject buttonClicked;
    public bool tutblocked;
    public GameObject shardReadout;

    void Update(){
      if(needRecordCountdown == true && recordCountdownStarted == false){
          StartCoroutine(NextRecordCountdown());
          recordCountdownStarted = true;
      }
    }
    public void LoadMoodPanel(){
        // do checks to see if completed everything
        // CheckEnoughAffs();
        // CheckMeditationStatus();
        // CheckGratitudeStatus();
        SetupMoodRecording();
        CheckAllComplete();
        ShowHistoryPanel();
    }

    #region Mood stuff
    public void SetupMoodRecording(){
        // recordMoodButton.SetActive(true);
        moodselector.SetActive(false);
        var moodStatus = dataManager.player.moodRecord.moodLevel;
        int[] currentRecord = new int[2];
        
        if(!moodStatus.ContainsKey(System.DateTime.Now.ToString("MM/dd/yyyy"))){
            moodStatus.Add(System.DateTime.Now.ToString("MM/dd/yyyy").ToString(), new int[2]);
            moodStatus[System.DateTime.Now.ToString("MM/dd/yyyy")].SetValue(0, 0);
            moodStatus[System.DateTime.Now.ToString("MM/dd/yyyy")].SetValue(0, 1);
            currentRecord = moodStatus[System.DateTime.Now.ToString("MM/dd/yyyy")];
        } 

        currentRecord = moodStatus[System.DateTime.Now.ToString("MM/dd/yyyy")];

        if(currentRecord[0] != 0 && currentRecord[1] != 0){
            recordMoodText.text = "mood recorded for day. \n Good Job!";
            nextMoodRecordCountdown.gameObject.SetActive(false);
            moodCheckmark.SetActive(true);
            dataManager.player.moodRecord.dailyMoodComplete = true;
            dataManager.SaveAll();
            // recordMoodButton.GetComponent<Button>().interactable = false;
            return;
        }

        // set int here for seconds between mood records 8 hrs?
        if(GetEpochTime() - dataManager.player.moodRecord.lastMoodInput < timeBetweenRecords){
            // recordMoodButton.GetComponent<Button>().interactable = false;
            recordMoodText.text = "too soon to record other record";
            needRecordCountdown = true;
            nextMoodRecordCountdown.gameObject.SetActive(true);
            return;
        } else {
            needRecordCountdown = false;
            recordCountdownStarted = false;
            nextMoodRecordCountdown.gameObject.SetActive(false);
            dataManager.player.moodRecord.dailyMoodComplete = false;
            dataManager.SaveAll();
            // recordMoodButton.GetComponent<Button>().interactable = true;
        }

        if(currentRecord[0] == 0){
            // recordMoodText.text = "input first record";
            moodselector.SetActive(true);
            nextMoodRecordCountdown.gameObject.SetActive(false);
        }  else {
            // recordMoodText.text = "input second record";
            moodselector.SetActive(true);
            nextMoodRecordCountdown.gameObject.SetActive(false);
        }
    }

    IEnumerator NextRecordCountdown(){
        int timeLeft = timeBetweenRecords;
        timeLeft -= (GetEpochTime() - dataManager.player.moodRecord.lastMoodInput);
        while(needRecordCountdown == true){  
            nextMoodRecordCountdown.text =  System.DateTimeOffset.FromUnixTimeSeconds(timeLeft).ToString("HH:mm:ss");
            yield return new WaitForSeconds(1);
            timeLeft -= 1;
            if(timeLeft == -1){
                SetupMoodRecording();
                break;
            }
        }
    }
    #region  Mood Levels
    public void Record1(){
        moodToRecord = 1;
        ShowMoodConfirmation();
    }

    public void Record2(){
        moodToRecord = 2;
        ShowMoodConfirmation();
    }
    
    public void Record3(){
        moodToRecord = 3;
        ShowMoodConfirmation();
    }
    
    public void Record4(){
        moodToRecord = 4;
        ShowMoodConfirmation();
    }
    
    public void Record5(){
        moodToRecord = 5;
        ShowMoodConfirmation();
    }
    
    public void Record6(){
        moodToRecord = 6;
        ShowMoodConfirmation();
    }
    

    #endregion

    public void ShowMoodConfirmation(){
        confirmMoodPopUp.SetActive(true);
        confirmedMoodAmount.text = moodToRecord.ToString();
        confirmationImagePreview.sprite = moodIcons[moodToRecord-1];
    }

    public void CloseMoodConfirmation(){
        confirmMoodPopUp.SetActive(false);
    }
    public void ConfirmedMood(){
        RecordMood(moodToRecord);
        confirmMoodPopUp.SetActive(false);

    }

    public void RecordMood(int moodLevel){
        var moodStatus = dataManager.player.moodRecord.moodLevel;
        int[] currentRecord = moodStatus[System.DateTime.Now.ToString("MM/dd/yyyy")];
        if(currentRecord[0] == 0){
            currentRecord[0] = moodLevel;
        } else {
            currentRecord[1] = moodLevel;
        }
        dataManager.player.moodRecord.lastMoodInput = GetEpochTime();

        Vector3 readoutPos = new Vector3(moodButton.transform.position.x, -200, moodButton.transform.position.z);
        shardReadout.transform.position = readoutPos;
        shardReadout.GetComponent<TextMeshProUGUI>().text = "+ 100";
        shardReadout.GetComponent<Animation>().Play("addShards");
        dataManager.player.coins += 100;
        mainManager.soundPlayer.GetComponent<AudioSource>().PlayOneShot(mainManager.moodCompleteChime1);
        dataManager.SaveAll();
        LoadMoodPanel();
    }

    public void ChangeActivitySelection(){
        // setup stuff if needed

        
        CheckEnoughAffs();


        if(!tutblocked){
            buttonClicked = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        }
        LeanTween.moveLocal(buttonClicked, new Vector3(buttonClicked.transform.localPosition.x, -175, buttonClicked.transform.localPosition.z), .25f);
        int buttonPos = 0;
        for(var i = 0; i < activityButtons.Count; i++){
            if(activityButtons[i] == buttonClicked){
                buttonPos = i;
            } else {
                if(activityButtons[i].transform.position.y != -200){
                    LeanTween.moveLocal(activityButtons[i], new Vector3(activityButtons[i].transform.localPosition.x, -200, activityButtons[i].transform.localPosition.z), .25f);
                }
            }
        }
        for(var i = 0; i < activitySections.Count; i++){
            if(i != buttonPos){
                activitySections[i].gameObject.SetActive(false);
            } else {
                activitySections[i].gameObject.SetActive(true);
            }
        }
    }
    #endregion 

    #region History Stuff

    public void ShowHistoryPanel(){
        monthHistoryPanel.SetActive(true);
        viewingDate = System.DateTime.Now;
        LoadHistory();
    }
    public void HideHistoryPanel(){
        monthHistoryPanel.SetActive(false);
    }

    public void NextMonth(){
        viewingDate = viewingDate.AddMonths(1);
        LoadHistory();
    }

    public void PreviousMonth(){
        viewingDate = viewingDate.AddMonths(-1);
        LoadHistory();
    }

    public void LoadHistory(){
        var moodStatus = dataManager.player.moodRecord.moodLevel;
        var completeStatus = dataManager.player.moodRecord.dailyFullyComplete;
        foreach(Transform item in monthHistoryContainer.transform){
            Destroy(item.gameObject);
        }
        
        var currentMonth = viewingDate.Month;
        header.text = viewingDate.ToString("MMMM yyyy");
        int daysInMonth = System.DateTime.DaysInMonth(viewingDate.Year, viewingDate.Month);

        var firstDayOfMonth = new System.DateTime(viewingDate.Year, viewingDate.Month, 1);
        GenerateBlanks(firstDayOfMonth.DayOfWeek);

        for(var i = 1; i < daysInMonth+1; i++){
            var dayString = viewingDate.Month.ToString("00")+"/"+i.ToString("00")+"/"+viewingDate.Year.ToString();
            var day = System.DateTime.Parse(dayString);
            var newDateReadout = Instantiate(dateSection);
            newDateReadout.transform.SetParent(monthHistoryContainer.transform, worldPositionStays: false);
            DateSectionInfo info = newDateReadout.GetComponent<DateSectionInfo>();

            if(dayString == System.DateTime.Now.Date.ToString("MM/dd/yyyy")){
                info.todayHighlight.SetActive(true);
            }
            info.date.GetComponent<TMPro.TextMeshProUGUI>().text = System.DateTime.Parse(day.Date.ToString()).ToString("dd");

            foreach(var item in moodStatus){
                if(item.Key == dayString){

                    if(item.Value[0] != 0){
                        info.firstDailyImage.gameObject.SetActive(true);
                        info.firstDailyImage.GetComponent<Image>().sprite = moodIcons[item.Value[0]-1];
                    }
                   
                    if(item.Value[1] != 0){
                        info.secondDailyImage.gameObject.SetActive(true);
                        info.secondDailyImage.GetComponent<Image>().sprite = moodIcons[item.Value[1]-1];
                    } 
                }
            }

            foreach(var item in completeStatus){
                if(item.Key == dayString){
                    info.dayRainbowBorder.SetActive(true);
                }
            }
        }
    }

    private void GenerateBlanks(System.DayOfWeek dayOfWeek){
        int neededBlanks = 0;
        if(dayOfWeek == System.DayOfWeek.Monday){
            neededBlanks = 1;
        } else if (dayOfWeek == System.DayOfWeek.Tuesday){
            neededBlanks = 2;
        } else if (dayOfWeek == System.DayOfWeek.Wednesday){
            neededBlanks = 3;
        } else if (dayOfWeek == System.DayOfWeek.Thursday){
            neededBlanks = 4;
        } else if (dayOfWeek == System.DayOfWeek.Friday){
            neededBlanks = 5;
        } else if (dayOfWeek == System.DayOfWeek.Saturday){
            neededBlanks = 6;
        }
        for(var i = 0; i < neededBlanks; i++){
            var blank = Instantiate(blankBuffer);
            blank.transform.SetParent(monthHistoryContainer.transform, worldPositionStays: false);
        }
    }
    #endregion
   
    #region affirmation stuff

    // affirmation stuff
    // editing
    public void CloseEditAffirmationScreen(){
        editAffirmationsPanel.SetActive(false);
        affirmationPopupWindow.SetActive(false);
        CheckEnoughAffs();
    }

    public void CheckEnoughAffs(){
        if(dataManager.player.moodRecord.lastAffCompleted != System.DateTime.Now.ToString("MM/dd/yyyy")){
            startAffirmationButton.GetComponent<Button>().interactable = true;
            if(dataManager.player.moodRecord.dailyAffCompleted == true){
                dataManager.player.moodRecord.dailyAffCompleted = false;
            }
        } else {
            affirmationAmountDisplay.text = "affirmations completed for day!";
            startAffirmationButton.GetComponent<Button>().interactable = false;
            affCheckmark.SetActive(true);
            return;
        }
      
        affirmationAmountDisplay.text = dataManager.player.moodRecord.affirmations.Count + "/4 affirmations set";
        if(dataManager.player.moodRecord.affirmations.Count < 4){
            startAffirmationButton.GetComponent<Button>().interactable = false;
        } else {
            startAffirmationButton.GetComponent<Button>().interactable = true;
        }
    }
    public void ShowEditAffirmationScreen(){
        allAffirmations.Clear();
        foreach(Transform child in editAffirmationsPanel.transform){
            if(child.gameObject != addAffirmationButton){
                Destroy(child.gameObject);
            }
        }
        affirmationPopupWindow.SetActive(true);
        editAffirmationsPanel.SetActive(true);
        foreach(var affirmation in dataManager.player.moodRecord.affirmations){
            var affSection = Instantiate(affirmationEditSection, editAffirmationsPanel.transform, worldPositionStays: false);
            var input = affSection.GetComponent<AffirmationInfo>().affirmationText.GetComponent<InputField>();
            input.text = affirmation;
            affSection.GetComponent<AffirmationInfo>().deleteButton.GetComponent<Button>().onClick.AddListener(delegate {DeleteAffirmation(input.text);});
            allAffirmations.Add(affSection);
        }
        addAffirmationButton.transform.SetAsLastSibling();
        Debug.Log(allAffirmations.Count);
        if(allAffirmations.Count >= 4){
            addAffirmationButton.GetComponent<Button>().interactable = false;
        } else {
            addAffirmationButton.GetComponent<Button>().interactable = true;
        }
    }

    public void AddAffirmation(){
        var affSection = Instantiate(affirmationEditSection, editAffirmationsPanel.transform, worldPositionStays: false);
        allAffirmations.Add(affSection);
        addAffirmationButton.transform.SetAsLastSibling();
        if(allAffirmations.Count >= 4){
            addAffirmationButton.GetComponent<Button>().interactable = false;
        } else {
            addAffirmationButton.GetComponent<Button>().interactable = true;
        }
    }

    public void DeleteAffirmation(string affirmationToDelete){
        foreach(var affirmation in allAffirmations){
            if(affirmation.GetComponent<AffirmationInfo>().affirmationText.GetComponent<InputField>().text == affirmationToDelete){
                Destroy(affirmation.gameObject);
                break;
            }
        }
        for(var i = 0; i < dataManager.player.moodRecord.affirmations.Count; i++){
            if(affirmationToDelete == dataManager.player.moodRecord.affirmations[i]){
                dataManager.player.moodRecord.affirmations.RemoveAt(i);
                break;
            }
        }
        dataManager.SaveAll();
        ShowEditAffirmationScreen();
    }
    public void SaveAffirmations(){
        for(var i = 0; i < allAffirmations.Count; i++){
            var affText = allAffirmations[i].GetComponent<AffirmationInfo>().affirmationText.GetComponent<InputField>().text;
            if(affText == ""){
                DeleteAffirmation(affText);
                continue;
            }
            if(i >= dataManager.player.moodRecord.affirmations.Count){
                dataManager.player.moodRecord.affirmations.Add("");
            }
            dataManager.player.moodRecord.affirmations[i] = affText;
        }
        dataManager.SaveAll();
        mainManager.ShowWarning("All Affirmations Saved");
        ShowEditAffirmationScreen();
    }

    // affirmation readout
    public void InitiateReadout(){
        StartCoroutine(ReadoutSteps());
    }
    public void ReadyForNextChange(){
        readyForNextAff = true;
    }
    IEnumerator ReadoutSteps(){
        affirmationReadoutPanel.SetActive(true);
        affirmationReadoutPanel.GetComponent<Animation>().Play("fadeinpanel");
        yield return new WaitForSeconds(2);
        affirmationInstructions.GetComponent<Animation>().Play("instructionfadein");
        yield return new WaitForSeconds(2);
        foreach(var affirmation in dataManager.player.moodRecord.affirmations){
            readyForNextAff = false;
            yield return new WaitForSeconds(.5f);
            affirmationReadout.text = "";
            for(var i = 0; i < affirmation.Length; i++){
                affirmationReadout.text += affirmation[i];
                yield return new WaitForSeconds(.1f);
            }
            yield return new WaitForSeconds(1);
            spokenConfirmationButton.GetComponent<Animation>().Play("buttonfadein");
            while(!readyForNextAff){
                yield return new WaitForSeconds(.5f);
            }
            spokenConfirmationButton.GetComponent<Animation>().Play("buttonfadeout");
            yield return new WaitForSeconds(2);
        }
        affirmationReadout.text = "";
        dataManager.player.moodRecord.lastAffCompleted = System.DateTime.Now.ToString("MM/dd/yyyy");
        dataManager.player.moodRecord.dailyAffCompleted = true;
  
        affirmationInstructions.text = "Good job! \n Affirmations Completed!";
        yield return new WaitForSeconds(2);
        affirmationInstructions.GetComponent<Animation>().Play("instructionfadeout");
        yield return new WaitForSeconds(2);
        affirmationReadoutPanel.GetComponent<Animation>().Play("fadeoutpanel");
        yield return new WaitForSeconds(2);
        affirmationReadoutPanel.SetActive(false);
        
        Vector3 readoutPos = new Vector3(affirmationsButton.transform.position.x, -200, affirmationsButton.transform.position.z);
        shardReadout.transform.position = readoutPos;
        shardReadout.GetComponent<TextMeshProUGUI>().text = "+ 100";
        shardReadout.GetComponent<Animation>().Play("addShards");
        dataManager.player.coins += 100;
        mainManager.soundPlayer.GetComponent<AudioSource>().PlayOneShot(mainManager.moodCompleteChime1);
        dataManager.SaveAll();
        CheckEnoughAffs();
    }
#endregion

    #region meditaiton stuff

    public void CheckMeditationStatus(){
        if(dataManager.player.moodRecord.lastDateMeditated != System.DateTime.Now.ToString("MM/dd/yyyy")){
            // Debug.Log("hit1");
            dataManager.player.moodRecord.daysLongestMeditation = 0;
            dataManager.player.moodRecord.meditationComplete = false;
            if(dataManager.player.moodRecord.lastDateMeditated == "" || dataManager.player.moodRecord.lastDateMeditated == null || dataManager.player.moodRecord.lastDateMeditated != System.DateTime.Now.ToString("MM/dd/yyyy")){
                meditationStatusText.text = "Meditation Not Started";
                // Debug.Log("hit2");
            }
        } else {
            // Debug.Log("hit3");
            if(dataManager.player.moodRecord.daysLongestMeditation < 600 && dataManager.player.moodRecord.daysLongestMeditation > 0){
                // Debug.Log("hit4");
                string timeReadout = System.TimeSpan.FromSeconds(dataManager.player.moodRecord.daysLongestMeditation).ToString(@"mm\:ss");
                meditationStatusText.text = timeReadout + " completed. \n Try for 10 minutes!";
                return;
            }
            if(dataManager.player.moodRecord.daysLongestMeditation >= 600){
                meditationStatusText.text = "Completed!";

                Vector3 readoutPos = new Vector3(meditationButton.transform.position.x, -200, meditationButton.transform.position.z);
                shardReadout.transform.position = readoutPos;
                shardReadout.GetComponent<TextMeshProUGUI>().text = "+ 1000";
                shardReadout.GetComponent<Animation>().Play("addShards");
                dataManager.player.coins += 1000;
                mainManager.soundPlayer.GetComponent<AudioSource>().PlayOneShot(mainManager.moodCompleteChime1);

                meditationCheckMark.SetActive(true);
            }
        }
        dataManager.SaveAll();
    }

    #endregion

    #region gratitude stuff

    public void CheckGratitudeStatus(){
        if(dataManager.player.moodRecord.lastGratitudeComplete != System.DateTime.Now.ToString("MM/dd/yyyy")){
            dataManager.player.moodRecord.gratitudeComplete = false;
            gratitudeHeader.GetComponent<Text>().text = "What are you thankful for today?";
            gratitudeText.gameObject.SetActive(false);
            gratitudeInput.SetActive(true);
            gratitudeSubmit.SetActive(true);
            thankfulCheckMark.SetActive(false);
        } else {
            dataManager.player.moodRecord.gratitudeComplete = true;
        }
        if(dataManager.player.moodRecord.gratitudeComplete){
            gratitudeText.gameObject.SetActive(true);
            gratitudeHeader.GetComponent<Text>().text = "You're thankful for . . .";
            gratitudeInput.SetActive(false);
            gratitudeSubmit.SetActive(false);
            thankfulCheckMark.SetActive(true);
            foreach(var item in dataManager.player.moodRecord.gratitudeItem){
                if(item.Key == System.DateTime.Now.ToString("MM/dd/yyyy")){
                    gratitudeText.text = item.Value;
                }
            }
        }
        dataManager.SaveAll();
    }

    public void SubmitGratitude(){
        if(!dataManager.player.moodRecord.gratitudeItem.ContainsKey("MM/dd/yyyy")){
            dataManager.player.moodRecord.lastGratitudeComplete = System.DateTime.Now.ToString("MM/dd/yyyy");
            dataManager.player.moodRecord.gratitudeItem.Add(System.DateTime.Now.ToString("MM/dd/yyyy"), gratitudeInput.GetComponent<InputField>().text);

            Vector3 readoutPos = new Vector3(gratitudeButton.transform.position.x, -200, gratitudeButton.transform.position.z);
            shardReadout.transform.position = readoutPos;
            shardReadout.GetComponent<TextMeshProUGUI>().text = "+ 100";
            shardReadout.GetComponent<Animation>().Play("addShards");
            mainManager.soundPlayer.GetComponent<AudioSource>().PlayOneShot(mainManager.moodCompleteChime1);
            dataManager.player.coins += 100;
        }
        CheckGratitudeStatus();

    }


    #endregion
   
   public void CheckAllComplete(){
        CheckEnoughAffs();
        CheckMeditationStatus();
        CheckGratitudeStatus();
       var mr = dataManager.player.moodRecord;
       if(mr.gratitudeComplete == true &&
        mr.dailyMoodComplete == true &&
        mr.dailyAffCompleted == true &&
        mr.meditationComplete == true &&
        !mr.dailyFullyComplete.ContainsKey(System.DateTime.Now.ToString("MM/dd/yyyy"))){
            mr.dailyFullyComplete.Add(System.DateTime.Now.ToString("MM/dd/yyyy"), true);
            mr.bonusActive = true; 
            Debug.Log("Finished");
        }
        if(mr.dailyFullyComplete.ContainsKey(System.DateTime.Now.ToString("MM/dd/yyyy"))){
            mr.bonusActive = true;
        }
         else {
            mr.bonusActive = false;
        }
        dataManager.SaveAll();
   }
   public void GenerationTest(){
       var moodStatus = dataManager.player.moodRecord.moodLevel;
       var start = new System.DateTime(2021, 01, 01);
       for(var i = 0; i < 5; i++){
           for(var j = 1; j < 13; j++){
                for(var k = 1; k < System.DateTime.DaysInMonth(start.Year, j); k++){
                    if(!moodStatus.ContainsKey(start.Date.ToString("MM/dd/yyyy"))){
                        moodStatus.Add(start.Date.ToString("MM/dd/yyyy"), new int[2]);
                        moodStatus[start.Date.ToString("MM/dd/yyyy")].SetValue(Random.Range(0,7), 0);
                        moodStatus[start.Date.ToString("MM/dd/yyyy")].SetValue(Random.Range(0,7), 1);
                        start = start.AddDays(1);
                    }
                }
           }

        dataManager.SaveAll();
       }
       LoadMoodPanel();
   }
    
    public int GetEpochTime(){
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Local);
        return (int)(System.DateTime.Now - epochStart).TotalSeconds;
    }

}
