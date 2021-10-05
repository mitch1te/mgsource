using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChallenge : MonoBehaviour
{
[Header("Player Information")]
public int coinsEarned;
public Text coinsEarnedText;
[Header("Button Stuff")]
public GameObject buttonHolder;
public GameObject button;
public GameObject background;
public GameObject readyButton;
[Header("Screens")]
public GameObject gameOverScreen;
public GameObject nextLevelScreen;
public Text levelText;
public Text leaveWithEnergyText;
[Header("Level Information")]
public int level;
public int strikes;
public int buttonsToPress;
public int numberOfButtons;
public int tapButtons;
public int dragButtons;
public int holdButtons;
public int chainButtons;
public bool chainStarted;
public int nextChainButtonNeeded;
public List<int> usedPos;

[Header("Art Stuff")]
public Sprite tapButtonArt;
public Sprite holdButtonArt;
public Sprite chainButtonArt;
public Sprite upArrow;
public Sprite downArrow;
public Sprite leftArrow;
public Sprite rightArrow;
private string red = "#B72C14";
private string purple = "#8C4A86";
private string yellow = "#F9CB46";
private Vector3 coinsEarnedLoc;
[Header("Audio")]
public AudioClip levelCompleteChime;
public AudioClip gameOverChime;
public AudioClip wrongAnwerChime;
public AudioClip pulseSound;
[Header("Other")]
public GameObject strikeSection;
public Text gameOverResults;
public GameObject startRound;
public GameObject strikeResetScreen;
// todo: balance and progression 
void Start(){
        coinsEarnedLoc = coinsEarnedText.gameObject.transform.localPosition;
        level = 1;
    // SetUp();
}

    public void SetUp(){
        readyButton.SetActive(true);
        readyButton.GetComponent<Button>().interactable = true;
        readyButton.transform.Find("Text").GetComponent<Text>().text = "ready";
        readyButton.GetComponent<Animation>().Play("readyButtonFadeIn");
        readyButton.GetComponent<Animation>().PlayQueued("readyButtonPulse");
        coinsEarnedText.text = coinsEarned.ToString();
        levelText.text = level.ToString();
        buttonHolder.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0,0,0);
        nextChainButtonNeeded = 1;
        chainStarted = false;
        ResetLevel();
        CreateButtons();
        AssignTapButtons();
        AssignDragButtons();
        AssignHoldButtons();
        AssignChainButtons();
        AssignBlanks();
        buttonsToPress = (tapButtons + dragButtons + holdButtons + chainButtons);
        
    }
    public void CreateButtons(){
        usedPos.Clear();
        for(var i = 0; i < numberOfButtons; i++){
            GameObject newButton = Instantiate(button);
            newButton.transform.SetParent(buttonHolder.transform, worldPositionStays: false);
            newButton.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
            newButton.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
            newButton.GetComponent<ButtonChallengeResponse>().buttonPos = i+1;
            newButton.GetComponent<ButtonChallengeResponse>().SetupButton();
            newButton.name = "Button" + (i+1).ToString();
        }
        foreach(Transform button in buttonHolder.transform){
            button.gameObject.GetComponent<Button>().interactable = false;
        }
        
    }


    public void AssignTapButtons(){
        int tapButtonsNeeded = tapButtons;
        List<int> buttonsToChange = new List<int>();
        while(tapButtonsNeeded > 0){
            int randomButton = Random.Range(1, numberOfButtons+1);
            if(!buttonsToChange.Contains(randomButton) && !usedPos.Contains(randomButton)){
                buttonsToChange.Add(randomButton);
                usedPos.Add(randomButton);
                tapButtonsNeeded -= 1;
            }
        }
        foreach(Transform button in buttonHolder.transform){
            if(buttonsToChange.Contains(button.gameObject.GetComponent<ButtonChallengeResponse>().buttonPos)){
                button.gameObject.GetComponent<ButtonChallengeResponse>().buttonType.GetComponent<Image>().sprite = tapButtonArt;
                button.gameObject.GetComponent<ButtonChallengeResponse>().isTap = true;
                // button.Find("Direction").gameObject.SetActive(false);
            }
        }
    }

    public void AssignDragButtons(){
        int dragButtonsNeeded = dragButtons;
        List<int> buttonsToChange = new List<int>();
        while(dragButtonsNeeded > 0){
            int randomButton = Random.Range(1, numberOfButtons+1);
            if(!buttonsToChange.Contains(randomButton) && !usedPos.Contains(randomButton)){
                buttonsToChange.Add(randomButton);
                usedPos.Add(randomButton);
                dragButtonsNeeded -= 1;
            }
        }
        foreach(Transform button in buttonHolder.transform){
            if(buttonsToChange.Contains(button.gameObject.GetComponent<ButtonChallengeResponse>().buttonPos)){
                int randDirection = Random.Range(0,4);
                // button.gameObject.GetComponent<Image>().color = Color.yellow;
                button.gameObject.GetComponent<ButtonChallengeResponse>().isDrag = true;
                if(randDirection == 0){
                    button.gameObject.GetComponent<ButtonChallengeResponse>().dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Up;
                    button.gameObject.GetComponent<ButtonChallengeResponse>().buttonType.GetComponent<Image>().sprite = upArrow;
                } else if (randDirection == 1){
                    button.gameObject.GetComponent<ButtonChallengeResponse>().dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Down;
                    button.gameObject.GetComponent<ButtonChallengeResponse>().buttonType.GetComponent<Image>().sprite = downArrow;
                } else if (randDirection == 2){
                    button.gameObject.GetComponent<ButtonChallengeResponse>().dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Left;
                    button.gameObject.GetComponent<ButtonChallengeResponse>().buttonType.GetComponent<Image>().sprite = leftArrow;
                } else {
                    button.gameObject.GetComponent<ButtonChallengeResponse>().dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Right;
                    button.gameObject.GetComponent<ButtonChallengeResponse>().buttonType.GetComponent<Image>().sprite = rightArrow;
                }
            }
        }
    }
    public void AssignHoldButtons(){
        int holdButtonsNeeded = holdButtons;
        List<int> buttonsToChange = new List<int>();
        while(holdButtonsNeeded > 0){
            int randomButton = Random.Range(1, numberOfButtons+1);
            if(!buttonsToChange.Contains(randomButton) && !usedPos.Contains(randomButton)){
                buttonsToChange.Add(randomButton);
                usedPos.Add(randomButton);
                holdButtonsNeeded -= 1;
            }
        }
        foreach(Transform button in buttonHolder.transform){
            if(buttonsToChange.Contains(button.gameObject.GetComponent<ButtonChallengeResponse>().buttonPos)){
                // button.Find("Direction").gameObject.SetActive(false);
                // button.gameObject.GetComponent<Image>().color = Color.green;
                button.gameObject.GetComponent<ButtonChallengeResponse>().isHold = true; 
                int holdAmount = (int)Random.Range(1,4);      
                button.gameObject.GetComponent<ButtonChallengeResponse>().buttonType.GetComponent<Image>().sprite = holdButtonArt;
                button.gameObject.GetComponent<ButtonChallengeResponse>().number.gameObject.SetActive(true);
                button.gameObject.GetComponent<ButtonChallengeResponse>().number.text = holdAmount.ToString();
                button.gameObject.GetComponent<ButtonChallengeResponse>().holdTimeNeeded = holdAmount;
            }
        }
    }
    public void AssignChainButtons(){
        int chainButtonsNeeded = chainButtons;
        List<int> buttonsToChange = new List<int>();
        while(chainButtonsNeeded > 0){
            int randomButton = Random.Range(1, numberOfButtons+1);
            if(!buttonsToChange.Contains(randomButton) && !usedPos.Contains(randomButton)){
                buttonsToChange.Add(randomButton);
                usedPos.Add(randomButton);
                chainButtonsNeeded -= 1;
            }
        }
        foreach(Transform button in buttonHolder.transform){
            if(buttonsToChange.Contains(button.gameObject.GetComponent<ButtonChallengeResponse>().buttonPos)){
                Debug.Log(nextChainButtonNeeded);
                // button.Find("Direction").gameObject.SetActive(false);
                button.gameObject.GetComponent<ButtonChallengeResponse>().buttonType.GetComponent<Image>().sprite = chainButtonArt;
                // button.gameObject.GetComponent<Image>().color = Color.blue;
                button.gameObject.GetComponent<ButtonChallengeResponse>().isChained = true;   
                button.gameObject.GetComponent<ButtonChallengeResponse>().chainNumber = nextChainButtonNeeded;
                button.gameObject.GetComponent<ButtonChallengeResponse>().number.text = nextChainButtonNeeded.ToString();
                button.gameObject.GetComponent<ButtonChallengeResponse>().number.gameObject.SetActive(true);
                nextChainButtonNeeded += 1;
            }
        }
        nextChainButtonNeeded = 1;
    }
    public void AssignBlanks(){
        foreach(Transform button in buttonHolder.transform){
            if(!usedPos.Contains(button.gameObject.GetComponent<ButtonChallengeResponse>().buttonPos)){
                button.gameObject.GetComponent<ButtonChallengeResponse>().isNothing = true;
                button.gameObject.GetComponent<ButtonChallengeResponse>().buttonType.GetComponent<Image>().color = new Color(0,0,0,0);
            }
        }
    }

    public void ReadyLevel(){
        Debug.Log("Clicked");
        readyButton.GetComponent<Button>().interactable = false;
        StartCoroutine(StartLevelAnimations());
    }

    IEnumerator StartLevelAnimations(){
        // readyButton.transform.Find("Text").GetComponent<Text>().text = "start";
        yield return new WaitForSeconds(.25f);
        readyButton.GetComponent<Animation>().Play("readyButtonFadeOut");

        // Do fadeout before rotation if after level 30
        if(level >= 75){
            Debug.Log("Fading before");
            foreach(Transform button in buttonHolder.transform){
                button.gameObject.GetComponent<ButtonChallengeResponse>().buttonType.GetComponent<Animation>().Play("buttonTypeFadeOut");
            }
        }
        yield return new WaitForSeconds(1);
        if(level >= 50){
            yield return StartCoroutine(DoRotations());
        }

        // if before level 30, do fadeout after rotation
        if(level < 75){
            Debug.Log("Fading after");
            foreach(Transform button in buttonHolder.transform){
                button.gameObject.GetComponent<ButtonChallengeResponse>().buttonType.GetComponent<Animation>().Play("buttonTypeFadeOut");
            }
        }
        yield return new WaitForSeconds(1f);
        readyButton.SetActive(false);
        // startRound.GetComponent<Animation>().Play("startRound");
        // yield return new WaitForSeconds(2.5f);
        // make buttons interactable
        foreach(Transform button in buttonHolder.transform){
            button.gameObject.GetComponent<Button>().interactable = true;
        }
    }

    IEnumerator DoRotations(){
        yield return new WaitForSeconds(1);
        List<int> numOfRotations = new List<int>();
        if(level >= 50){
            numOfRotations.Add(1);
        }  
        if (level >= 60){
            numOfRotations.Add(2);
        }
        if(level >= 70){
            numOfRotations.Add(3);
        }
        int chosenRotations = numOfRotations[Random.Range(0, numOfRotations.Count)];
        // Debug.Log("Rotating " + chosenRotations + " times");
        for(var i = 0; i < chosenRotations; i++){
            yield return StartCoroutine(RotateButtons());
            yield return new WaitForSeconds(0.5f);
        } 
    }
    public void ResetLevel(){
        foreach(Transform button in buttonHolder.transform){
            button.gameObject.GetComponent<ButtonChallengeResponse>().buttonPos = 0;
            Destroy(button.gameObject);
        }
        // readyButton.GetComponent<Animation>().Play("readyButtonFadeIn");
    }
    
    public void RemoveHalf(){
        StartCoroutine(I_RemoveHalf());
    }

    IEnumerator I_RemoveHalf(){
        yield return new WaitForSeconds(.5f);
        List<GameObject> buttonsAvailable = new List<GameObject>();
        foreach(Transform button in buttonHolder.transform){
            if(button.GetComponent<ButtonChallengeResponse>().isNothing){
                buttonsAvailable.Add(button.gameObject);
            }
        }
        List<GameObject> buttonsToBurst = new List<GameObject>();
        for(var i =0 ; i < buttonsAvailable.Count/2; i++){
            int chosenButton = Random.Range(0, buttonsAvailable.Count);
            buttonsToBurst.Add(buttonsAvailable[chosenButton]);
        }
        foreach(GameObject button in buttonsToBurst){
            button.gameObject.GetComponent<Animation>().Play("buttonImplode");
        }
        yield return new WaitForSeconds(.25f);
        gameObject.GetComponent<MainManager>().soundPlayer.GetComponent<AudioSource>().PlayOneShot(levelCompleteChime);
        foreach(GameObject button in buttonsToBurst){
            button.GetComponent<ButtonChallengeResponse>().isNothing = false;
            button.GetComponent<ButtonChallengeResponse>().finishBurst.GetComponent<ParticleSystem>().Play();
            button.GetComponent<Button>().interactable = false;
        }
        
    }


    public void GameOver(){
        if(strikes == 0){
            strikes += 1;
            gameObject.GetComponent<MainManager>().soundPlayer.GetComponent<AudioSource>().PlayOneShot(wrongAnwerChime);
            strikeSection.GetComponent<Animation>().Play("strike1");
            RemoveHalf();
            StartCoroutine(TapTimeout());
        } else if(strikes == 1){
            strikes += 1;
            gameObject.GetComponent<MainManager>().soundPlayer.GetComponent<AudioSource>().PlayOneShot(wrongAnwerChime);
            strikeSection.GetComponent<Animation>().Play("strike2");
            RemoveHalf();
            StartCoroutine(TapTimeout());
        } else if (strikes == 2){
            strikeSection.GetComponent<Animation>().Play("strike3");
            StartCoroutine(ProcessGameOver());
        } 
    
    }

    public void LeaveGame(){
        strikes = 0;
        coinsEarned = 0;
        tapButtons = 0;
        holdButtons = 0;
        dragButtons = 0;
        chainButtons = 0;
        level = 1;
        strikeSection.GetComponent<Animation>().Play("resetStrikes");
        // gameObject.GetComponent<MainManager>().LoadMainScreen();
        gameOverScreen.GetComponent<Animation>().Play("hideGameOverScreen");
        nextLevelScreen.GetComponent<Animation>().Play("hideNextLevelScreen");
    }
    IEnumerator ProcessGameOver(){
        gameOverResults.text = "you made it to level " + level.ToString();
        foreach(Transform button in buttonHolder.transform){
            button.GetComponent<Button>().interactable = false;
        }
        gameObject.GetComponent<MainManager>().soundPlayer.GetComponent<AudioSource>().PlayOneShot(gameOverChime);
        yield return null;
        gameOverScreen.GetComponent<Animation>().Play("showGameOverScreen");
    }

    public void CashOut(){
        // nextLevelScreen.GetComponent<Animation>().Play("hideNextLevelScreen");
        DataManager data = GetComponent<MainManager>().dataManager;
        int coinTotal = data.player.coins + coinsEarned;
        data.player.coins = coinTotal;
        // Debug.Log(coinTotal);
        if(gameObject.GetComponent<MainManager>().dataManager.player.moodRecord.bonusActive){
            GetComponent<MainManager>().showBonus = true;
            GetComponent<MainManager>().bonusNeeded = (int)coinsEarned/2;
        }
        LeaveGame();
        data.SaveAll();
        coinsEarned = 0;
        GetComponent<MainManager>().LoadMainScreen();
    }
    public void NextLevelScreen(){
        StartCoroutine(I_NextLevelScreen());
    }
    IEnumerator I_NextLevelScreen(){
        yield return new WaitForSeconds(.5f);
        foreach(Transform button in buttonHolder.transform){
            button.gameObject.GetComponent<Animation>().Play("buttonImplode");
        }
        gameObject.GetComponent<MainManager>().soundPlayer.GetComponent<AudioSource>().PlayOneShot(levelCompleteChime);
        yield return new WaitForSeconds(.25f);
        foreach(Transform button in buttonHolder.transform){
            if(button.GetComponent<ButtonChallengeResponse>().isNothing){
                button.GetComponent<ButtonChallengeResponse>().finishBurst.GetComponent<ParticleSystem>().Play();
                button.GetComponent<Button>().interactable = false;
            } 
        }
        yield return new WaitForSeconds(.25f);
        GiveCoins();
        leaveWithEnergyText.text = coinsEarned.ToString();
        buttonHolder.GetComponent<Image>().raycastTarget = false;
        // background.SetActive(false);
        nextLevelScreen.GetComponent<Animation>().Play("showNextLevelScreen");
        coinsEarnedText.text = coinsEarned.ToString();
    }
    public void GiveCoins(){
        int totalCoins = 0;
        totalCoins += (tapButtons * 3);
        totalCoins += (dragButtons * 15);
        totalCoins += (holdButtons * 7);
        totalCoins += (chainButtons * 5);
        coinsEarned += totalCoins;
    }
    public void NextLevel(){
        StartCoroutine(I_NextLevel());       
    }
    IEnumerator I_NextLevel(){
        yield return new WaitForSeconds(.25f);
        nextLevelScreen.GetComponent<Animation>().Play("hideNextLevelScreen");
        ProgressLevel();
        SetUp();
    }
    public void AdjustDragButtons(int rotation){
        foreach(Transform button in buttonHolder.transform){
            ButtonChallengeResponse cr = button.gameObject.GetComponent<ButtonChallengeResponse>();

            if(cr.isDrag && rotation == -90){
                if(cr.dragDirectionNeeded == ButtonChallengeResponse.DragDirection.Up){
                    cr.dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Right;
                    // return;
                } else if(cr.dragDirectionNeeded == ButtonChallengeResponse.DragDirection.Right){
                    cr.dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Down;
                    // return;
                } else if(cr.dragDirectionNeeded == ButtonChallengeResponse.DragDirection.Down){
                    cr.dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Left;
                    // return;                
                } else if(cr.dragDirectionNeeded == ButtonChallengeResponse.DragDirection.Left){
                    cr.dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Up;
                    // return;
                }
            }

            if(cr.isDrag && rotation == 90){
                if(cr.dragDirectionNeeded == ButtonChallengeResponse.DragDirection.Up){
                    cr.dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Left;
                    // return;
                } else if(cr.dragDirectionNeeded == ButtonChallengeResponse.DragDirection.Left){
                    cr.dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Down;
                    // return;
                } else if(cr.dragDirectionNeeded == ButtonChallengeResponse.DragDirection.Down){
                    cr.dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Right;
                    // return;
                } else if(cr.dragDirectionNeeded == ButtonChallengeResponse.DragDirection.Right){
                    cr.dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Up;
                    // return;
                }
            }

            if(cr.isDrag & rotation == -180){
                if(cr.dragDirectionNeeded == ButtonChallengeResponse.DragDirection.Up){
                    cr.dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Down;
                    // return;
                } else if(cr.dragDirectionNeeded == ButtonChallengeResponse.DragDirection.Right){
                    cr.dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Left;
                    // return;
                } else if(cr.dragDirectionNeeded == ButtonChallengeResponse.DragDirection.Down){
                    cr.dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Up;
                    // return;
                } else if(cr.dragDirectionNeeded == ButtonChallengeResponse.DragDirection.Left){
                    cr.dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Right;
                    // return;
                }
            }
            if(cr.isDrag & rotation == 180){
                if(cr.dragDirectionNeeded == ButtonChallengeResponse.DragDirection.Up){
                    cr.dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Down;
                    // return;
                } else if(cr.dragDirectionNeeded == ButtonChallengeResponse.DragDirection.Right){
                    cr.dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Left;
                    // return;
                } else if(cr.dragDirectionNeeded == ButtonChallengeResponse.DragDirection.Down){
                    cr.dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Up;
                    // return;
                } else if(cr.dragDirectionNeeded == ButtonChallengeResponse.DragDirection.Left){
                    cr.dragDirectionNeeded = ButtonChallengeResponse.DragDirection.Right;
                    // return;
                }
            }
        }
    }
    IEnumerator RotateButtons(){
        List<int> rotation = new List<int>(){90, -90, 180, -180};
        int chosenRotation = rotation[Random.Range(0, rotation.Count)];
        int rotationSpeed = 0;
        Debug.Log("chosenRotation:" + chosenRotation);
        if(Mathf.Abs(chosenRotation)> 90){
            rotationSpeed = 2;
        } else {
            rotationSpeed = 1;
        }
        var euler = buttonHolder.transform.eulerAngles;
        AdjustDragButtons(chosenRotation);
        yield return LeanTween.rotate(buttonHolder, new Vector3(0,0, euler.z + chosenRotation), rotationSpeed);
        yield return new WaitForSeconds(rotationSpeed);
    }

    IEnumerator TapTimeout(){
        foreach(Transform button in buttonHolder.transform){
            button.GetComponent<Button>().interactable = false;   
        }
        yield return new WaitForSeconds(.5f);
        foreach(Transform button in buttonHolder.transform){
            button.GetComponent<Button>().interactable = true;   
        }
    }
    private void ResetStrikes(){
        strikeResetScreen.SetActive(true);
    }
    public void CloseStrikeResetScreen(){
        strikeResetScreen.SetActive(false);
        strikeSection.GetComponent<Animation>().Play("resetStrikes");
        strikes = 0;
    }
    private IEnumerator WaitForAnimation ( Animation animation )
    {
        do
        {
            yield return null;
        } while ( animation.isPlaying );
    }

    private void ProgressLevel(){
        if(level % 10 == 0){
            Debug.Log("Strikes Restting");
            ResetStrikes();
        }
        // ramp up to 6 concurent buttons, level off until level barrier at 10
        // every 10 levels, add new button type
        bool buttonAdded = false;
        // if(level == 1){
        //     level = 90;
        // } else {
        level += 1;

        // }
        if(level < 6){
            tapButtons += 1;
            buttonAdded = true;
        }

        if(level == 10){
            tapButtons = 1;
            holdButtons += 1;
        }

        if(level > 11 && level < 21){
            if(holdButtons <= 3 && holdButtons < tapButtons-2 && !buttonAdded){
                holdButtons += 1;
                buttonAdded = true;
            }
            if(tapButtons <= 3 && !buttonAdded){
                tapButtons += 1;
                buttonAdded = true;
            }
        }

        
        if(level == 20){
            tapButtons = 1;
            holdButtons = 1;
            chainButtons = 2;
        }


        // level ramp starts to get hard here
        if(level > 21 && level < 30){ 
            if(tapButtons < 4){
                tapButtons += 1;
            }
            if(tapButtons == 4 && holdButtons < 2 && !buttonAdded){
                holdButtons += 1;
                buttonAdded = true;
            }
        }

        if(level == 30){
            chainButtons = 4;
            holdButtons = 1;
            tapButtons = 2;
        }

        if(level == 40){
            chainButtons = 2;
            tapButtons = 1;
            holdButtons = 1;
            dragButtons = 1;
        }

        if(level > 40 && level < 55){
            if(tapButtons < 2 && !buttonAdded){
                tapButtons += 1;
                buttonAdded = true;
            }
            if(holdButtons < 2 && !buttonAdded){
                holdButtons += 1;
                buttonAdded = true;
            }
            if(dragButtons < 2 && !buttonAdded){
                dragButtons += 1;
                buttonAdded = true;
            }
        }

        if(level > 55 && level <= 65){
            chainButtons = 4;
            tapButtons = 2;
            holdButtons = 1;
            dragButtons = 2;
        }

        if(level == 65){
            chainButtons = 5;
            tapButtons = 0;
            holdButtons = 0;
            dragButtons = 0;
        }

        if(level > 65 && level <= 72){
            chainButtons += 1;
            // tapButtons += 1;
        }

        // if(level == 70){
        //     chainButtons = 0;
        //     tapButtons = 0;
        //     dragButtons = 3;
        // }

        // if(level > 71 && level < 75){
        //     dragButtons += 1;
        // }

        if(level == 75){
            tapButtons = 6;
            chainButtons = 6;
        }

        if(level == 80){
            chainButtons = 0;
            tapButtons = 5;
            holdButtons = 5;
        }

        if(level == 90){
            chainButtons = 3;
            dragButtons = 2;
            tapButtons = 7;
            holdButtons = 2;
        }

        if(level == 95){
            chainButtons = 4;
            dragButtons = 4;
            tapButtons = 4;
            holdButtons = 4;
        }
    }
}
