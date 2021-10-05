using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClickerGame : MonoBehaviour
{
    public int nextUpgradeCost;
    public List<Button> upgradeButtons;
    public GameObject clickerButton;
    public Text coinsText;
    public int perClickIncrease;
    public int clickCooldown;
    private DataManager dataManager;

    [Header("Upgrade Screen Stuff")]
    public GameObject clickAmountReadout;
    public GameObject plusSign;
    public Text upgradeCost;
    public GameObject upgradePanel;
    public List<Image> gongPowerPips;
    public GameObject gongPowerMax;
    public List<Image> gongRatePips;
    public GameObject gongRateMax;
    public List<Image> spiritPowerPips;
    public GameObject spiritPowerMax;
    public List<Image> spiritRatePips;
    public GameObject spiritRateMax;
    public List<Image> environmentLevelPips;
    public GameObject environmentMax;
    public Image currentWeatherBG;
    public Image currentWeatherOverlay;
    public Sprite filledPip;
    public List<Sprite> weatherBackgrounds;
    public List<GameObject> rainbows;
    public List<Sprite> weatherOverlays;
    public List<GameObject> activeSpirits;
    [Header("Upgrade Confirmation")]
    public GameObject upgradeConfirmScreen;
    public Text upgradeFinalCost;
    public Image upgradeFinalIcon;
    public Text upgradeFinalName;
    public Text blurb;
    public GameObject confirmUpgradeButton;
    public Sprite gongPowerIcon;
    public Sprite gongRateIcon;
    public Sprite spiritIcon;
    public Sprite spiritRateIcon;
    public Sprite serenityIcon;

    [Header("Gong Art")]
    public Sprite gongBronze;
    public Sprite gongSilver;
    public Sprite gongGold;
    public Sprite gongFrameNew;
    public GameObject gongFrameMoss;
    public Image gongDisc;
    public Image gongFrame;
    public GameObject gongDeco;
    public GameObject gongMoss;
    public GameObject gongRust;
    public Image gongRope;
    public Sprite gongRopeNew;
    public GameObject gongMallet1;

    [Header("Upgrade Animation")]
    public GameObject upgradeAnimationScreen;
    public List<GameObject> bursts;
    [Header("Other Animated Assets")]

    private bool loaded;
    private bool playingAnimations;


    void Update(){
        if(loaded && !playingAnimations){
            playingAnimations = true;
            StartCoroutine(PlaySpiritAnimations());
        }
    }

    public void Setup(){
        dataManager = gameObject.GetComponent<MainManager>().dataManager;
        coinsText.text = (dataManager.player.coins.ToString());
        LoadPowerLevels();
        SetScene();
        loaded = true;
    }
    public void ClickedButton(){
        gameObject.GetComponent<MainManager>().soundPlayer.GetComponent<AudioSource>().PlayOneShot(gameObject.GetComponent<MainManager>().gongHit1);
        dataManager.player.coins += perClickIncrease;
        clickAmountReadout.GetComponent<TextMeshProUGUI>().text = perClickIncrease.ToString();
        clickAmountReadout.GetComponent<Animation>().Play("clickReadout");
        Debug.Log(perClickIncrease);
        gameObject.GetComponent<MainManager>().cameraManager.mainCam.GetComponent<Animation>().Play("cameraShake");
        coinsText.text = (dataManager.player.coins.ToString());
        clickerButton.GetComponent<Button>().interactable = false;
        ChangeUpgradeCost();
        StartCoroutine(WaitForCooldown());
    }

    IEnumerator WaitForCooldown(){
        yield return new WaitForSeconds(clickCooldown);
        clickerButton.GetComponent<Button>().interactable = true;
    }

    void SetScene(){
        int gongPower = dataManager.player.gongClickPowerLevel;
        int gongRate = dataManager.player.gongClickRateLevel;
        if(dataManager.player.gongMultiplierLevel <= 7){
            currentWeatherBG.sprite = weatherBackgrounds[dataManager.player.gongMultiplierLevel];
        } else {
            currentWeatherBG.sprite = weatherBackgrounds[weatherBackgrounds.Count-1];
        }

        if(dataManager.player.gongMultiplierLevel <= weatherOverlays.Count-1){
            currentWeatherOverlay.sprite = weatherOverlays[dataManager.player.gongMultiplierLevel];
        } else if (dataManager.player.gongMultiplierLevel > weatherOverlays.Count-1){
            currentWeatherOverlay.gameObject.SetActive(false);
        }

        if(dataManager.player.gongMultiplierLevel == 9){
            rainbows[0].SetActive(true);
        }
        if(dataManager.player.gongMultiplierLevel == 10){
            rainbows[0].SetActive(false);
            rainbows[1].SetActive(true);
        }
        for(var i = 1; i < dataManager.player.gongSpiritLevel+1; i++){
            activeSpirits[i-1].SetActive(true);
        }

        if(gongPower >= 1){
            gongRust.SetActive(false);
        }
        if (gongPower >= 2){
            gongDisc.sprite = gongBronze;
        }
        if (gongPower >= 3){
            gongDisc.sprite = gongSilver;
        }
        if(gongPower >= 4){
            gongDisc.sprite = gongGold;
        }
        if(gongPower >= 5){
            gongDeco.SetActive(true);
        }

        if(gongRate >= 1){
            gongFrameMoss.SetActive(false);
        }
        if(gongRate >= 2){
            gongMoss.SetActive(false);
        }
        if(gongRate >= 3){
            gongFrame.sprite = gongFrameNew;
        }
        if(gongRate >= 4){
            gongRope.sprite = gongRopeNew;  
        }
        if(gongRate >= 5){
            gongMallet1.SetActive(true);
        }
        
    }

#region Upgrade Functions
   public void UpgradeGongPower(){
        SetupConfirmationScreen();
        ChangeUpgradeConfirmation(gongPowerIcon, "gong power");
        blurb.text = "each time you ring the gong you will summon more energy";
        confirmUpgradeButton.GetComponent<Button>().onClick.AddListener(ConfirmGongPowerUpgrade);
    }
    void ConfirmGongPowerUpgrade(){
        dataManager.player.gongClickPowerLevel += 1; 
        dataManager.player.coins -= nextUpgradeCost;
        dataManager.SaveAll();
        CloseUpgradeScreens(true);
    }

    public void UpgradeGongRate(){
        SetupConfirmationScreen();
        ChangeUpgradeConfirmation(gongRateIcon, "ring rate");
        blurb.text = "ring the gong faster without the risk of it breaking";
        confirmUpgradeButton.GetComponent<Button>().onClick.AddListener(ConfirmGongRateUpgrade);
    }

    void ConfirmGongRateUpgrade(){
        dataManager.player.gongClickRateLevel += 1;
        dataManager.player.coins -= nextUpgradeCost;
        dataManager.SaveAll();
        CloseUpgradeScreens(true);
    }

   public void UpgradeSpiritLevel(){
        SetupConfirmationScreen();
        ChangeUpgradeConfirmation(spiritIcon, "spirits");
        blurb.text = "summon another spirit to sound the gong in your absence";
        confirmUpgradeButton.GetComponent<Button>().onClick.AddListener(ConfirmSpiritLevelUpgrade);
    }

    void ConfirmSpiritLevelUpgrade(){
        dataManager.player.gongSpiritLevel +=1 ;
        dataManager.player.coins -= nextUpgradeCost;
        dataManager.SaveAll();
        CloseUpgradeScreens(true);
    }

    public void UpgradeSpiritRate(){
        SetupConfirmationScreen();
        ChangeUpgradeConfirmation(spiritRateIcon, "spirit power");
        blurb.text = "embue your spirits with more energy so the ring the gong more often";
        confirmUpgradeButton.GetComponent<Button>().onClick.AddListener(ConfirmSpiritRateUpgrade);
    }

    void ConfirmSpiritRateUpgrade(){
        dataManager.player.gongSpiritRateLevel += 1;
        dataManager.player.coins -= nextUpgradeCost;
        dataManager.SaveAll();
        CloseUpgradeScreens(true);
    }

    public void UpgradeSerenity(){
        SetupConfirmationScreen();
        ChangeUpgradeConfirmation(serenityIcon, "serenity");
        blurb.text = "remove some negative energy around you to improve your overall energy generation";
        confirmUpgradeButton.GetComponent<Button>().onClick.AddListener(ConfirmUpgradeSerenity);
    }

    void ConfirmUpgradeSerenity(){
        dataManager.player.gongMultiplierLevel += 1;
        dataManager.player.coins -= nextUpgradeCost;
        dataManager.SaveAll();
        CloseUpgradeScreens(true);
    }

#endregion
    void ChangeUpgradeConfirmation(Sprite icon, string upgradeName){
        upgradeFinalIcon.sprite = icon;
        upgradeFinalName.text = upgradeName;
    }

    void SetupConfirmationScreen(){
        upgradeConfirmScreen.SetActive(true);
        confirmUpgradeButton.GetComponent<Button>().onClick.RemoveAllListeners();
        upgradeFinalCost.text = nextUpgradeCost.ToString();
    }
    public void CloseUpgradeScreens(bool showAnim){
        if(showAnim){
            StartCoroutine(UpgradeAnimation());
        }
    }

    public void CloseUpgradeWithoutAnim(){
        upgradeConfirmScreen.SetActive(false);
    }

    IEnumerator PlaySpiritAnimations(){
        while(playingAnimations){
            var timeBetween = 6 - dataManager.player.gongSpiritRateLevel;
            foreach(var spirit in activeSpirits){
                if(spirit.activeSelf){
                    spirit.GetComponent<Animation>().Play("spiritWiggle"); 
                    dataManager.player.coins += (1);
                    coinsText.text = dataManager.player.coins.ToString();
                }  
            }
            yield return new WaitForSeconds(timeBetween);
            dataManager.SaveAll();
        }
    }
    IEnumerator UpgradeAnimation(){
        upgradeAnimationScreen.GetComponent<Animation>().Play("fadein");
        yield return new WaitForSeconds(1);
        foreach(var burst in bursts){
            burst.GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(.75f);
        }
        yield return new WaitForSeconds(.5f);
        upgradeAnimationScreen.GetComponent<Animation>().Play("fadeout");
        upgradeConfirmScreen.SetActive(false);
        upgradePanel.SetActive(false);
        Setup();
    }
    void ChangeUpgradeCost(){
        var dm = dataManager.player;
        int currentNumOfUpgrades = (dm.gongClickRateLevel + dm.gongClickPowerLevel + dm.gongSpiritLevel + dm.gongSpiritRateLevel + dm.gongMultiplierLevel);
        if(currentNumOfUpgrades > 1){
            nextUpgradeCost = (int)Mathf.Pow(currentNumOfUpgrades * 25, 2);
        } else {
            nextUpgradeCost = 300;
        } 
        upgradeCost.text = nextUpgradeCost.ToString();
        if(dataManager.player.coins >= nextUpgradeCost){
            plusSign.gameObject.SetActive(true);
            foreach(var button in upgradeButtons){
                button.interactable = true;
            }
        } else {
            plusSign.gameObject.SetActive(false);
            foreach(var button in upgradeButtons){
                button.interactable = false;
            }
        }
    }
    void LoadPowerLevels(){
        ChangeUpgradeCost();
        #region Power Levels Stuff
        // click power
        for(var i = 1; i < gongPowerPips.Count+1; i++){
            if(dataManager.player.gongClickPowerLevel >= i){
                gongPowerPips[i-1].GetComponent<Image>().sprite = filledPip;
            }
            if(dataManager.player.gongClickPowerLevel == 5){
                gongPowerMax.SetActive(true);
            }
        }

        // click rate
        for(var i = 1; i < gongRatePips.Count+1; i++){
            if(dataManager.player.gongClickRateLevel >= i){
                gongRatePips[i-1].GetComponent<Image>().sprite = filledPip;
            }
            if(dataManager.player.gongClickRateLevel == 5){
                gongRateMax.SetActive(true);
            }
        }

        // spirit power
        for(var i = 1; i < spiritPowerPips.Count+1; i++){
            if(dataManager.player.gongSpiritLevel >= i){
                spiritPowerPips[i-1].GetComponent<Image>().sprite = filledPip;
            }
            if(dataManager.player.gongSpiritLevel == 5){
                spiritPowerMax.SetActive(true);
            }
        }

        // spirit rate
        for(var i = 1; i < spiritRatePips.Count+1; i++){
            if(dataManager.player.gongSpiritRateLevel >= i){
                spiritRatePips[i-1].GetComponent<Image>().sprite = filledPip;
            }
            if(dataManager.player.gongSpiritRateLevel == 5){
                spiritRateMax.SetActive(true);
            }
        }

        // enviornment
        for(var i = 1; i < environmentLevelPips.Count+1; i++){
            if(dataManager.player.gongMultiplierLevel >= i){
                environmentLevelPips[i-1].GetComponent<Image>().sprite = filledPip;
            }
            if(dataManager.player.gongMultiplierLevel == 10){
               environmentMax.SetActive(true);
            }
        }

        #endregion
        ChangeClickValues();

    }

    void ChangeClickValues(){
        perClickIncrease = 1 + (10 * dataManager.player.gongClickPowerLevel);
        perClickIncrease += perClickIncrease * (dataManager.player.gongMultiplierLevel*10) / 100;
        clickCooldown = (10 - (dataManager.player.gongClickRateLevel *2));
        // Debug.Log(clickCooldown);
    }
    public void ToggleUpgradePanel(){
        
        if(!upgradePanel.activeSelf){
            upgradePanel.SetActive(true);
        } else {
            upgradePanel.SetActive(false);
        }
    }



    int GetEpochTime(){
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Local);
        return (int)(System.DateTime.Now - epochStart).TotalSeconds;
    }



}
