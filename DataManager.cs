using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BayatGames.SaveGameFree;

public class DataManager : MonoBehaviour
{
    public PlayerData player;
    public ItemVault itemVault;
    public bool playerLoaded;
    public Island initialIsland;
    public void CreateNew(string player, string garden){
        PlayerData newPlayer = new PlayerData();
        newPlayer.playerName = player;
        newPlayer.gardenName = garden;
        newPlayer.coins = 500;
        newPlayer.tokens = 3;
        newPlayer.lastLogin = GetEpochTime();
        newPlayer.lastTokenDate = "";
        newPlayer.inventorySize = 40;
        newPlayer.lastShopCheck = 0;
        newPlayer.storedShopItems = new List<int>();
        newPlayer.inventory = new List<int>();
        for(var i = 0; i < newPlayer.inventorySize; i ++){
            newPlayer.inventory.Add(0);
        }
        newPlayer.currentIsland = initialIsland;
        newPlayer.islands = new List<Island>();

        newPlayer.moodRecord = new MoodRecord();
        newPlayer.moodRecord.moodLevel = new Dictionary<string, int[]>();
        newPlayer.moodRecord.dailyFullyComplete = new Dictionary<string, bool>();
        newPlayer.moodRecord.gratitudeItem = new Dictionary<string, string>();
        newPlayer.moodRecord.affirmations = new List<string>();
        newPlayer.moodRecord.lastAffCompleted = "";
        newPlayer.moodRecord.dailyAffCompleted = false;
        newPlayer.moodRecord.dailyMoodComplete = false;
        newPlayer.moodRecord.meditationComplete = false;
        newPlayer.moodRecord.gratitudeComplete = false;
        newPlayer.moodRecord.lastDateMeditated = "";
        newPlayer.moodRecord.lastGratitudeComplete = "";
        newPlayer.moodRecord.longestMeditation = 0;
        newPlayer.moodRecord.daysLongestMeditation = 0;
        newPlayer.moodRecord.bonusActive = false;

        // newPlayer.moodRecord = new Dictionary<string, int[]>();
        // newPlayer.lastMoodInput = 0;
        // newPlayer.moodRecordStreak = 0;
        
        newPlayer.gongClickPowerLevel = 0;
        newPlayer.gongClickRateLevel = 0;
        newPlayer.gongSpiritLevel = 0;
        newPlayer.gongSpiritRateLevel = 0;
        newPlayer.gongMultiplierLevel = 0;
        newPlayer.lastGongCheck = 0;
        
        newPlayer.meditationMilestones = new MeditationMilestones();
        SetupMeditationMilestones(newPlayer);

        //tutorials
        newPlayer.needDisclaimer = true;
        newPlayer.needMainTutorial = true;
        newPlayer.needMemoryMatchTutorial = true;
        newPlayer.needGracefulWindTutorial = true;
        newPlayer.needGongRingerTutorial = true;
        newPlayer.needIslandTutorial = true;

        SaveGame.Save<PlayerData>("playerData", newPlayer);
        this.player = newPlayer;
        SaveAll();
    }

    public void SaveAll(){
        SaveGame.Save<PlayerData>("playerData", player);
    }

    public bool LoadGame(){
        
        if(!SaveGame.Exists("playerData")){
            return false;
        } else {
            player = SaveGame.Load<PlayerData>("playerData");
            playerLoaded = true;
            return true;
        }
        // if(player.playerName != null){
        //     playerLoaded = true;
        //     return true;
        // } else {
        //     return false;
        // }
    }

    public void CleanInventory(){
        for(var i = 0; i < player.inventorySize; i++){
            if(player.inventory[i] <= 10){
                player.inventory[i] = 0;
            }
        }
        SaveAll();
    }

    public int GetEpochTime(){
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Local);
        return (int)(System.DateTime.Now - epochStart).TotalSeconds;
    }


    public void SetupMeditationMilestones(PlayerData newPlayer){
        newPlayer.meditationMilestones.totalTime = 0;
        newPlayer.meditationMilestones.longestTime = 0;
        newPlayer.meditationMilestones.dayStreak = 0;
        newPlayer.meditationMilestones.totalTimeUnlocks = new List<bool>(){false, false, false, false, false};
        newPlayer.meditationMilestones.totalTimePrestige = 0;
        newPlayer.meditationMilestones.longestTimeUnlocks = new List<bool>(){false, false, false, false, false};
        newPlayer.meditationMilestones.longestTimePrestige = 0;
        newPlayer.meditationMilestones.dayStreakUnlocks = new List<bool>(){false, false, false, false, false};
        newPlayer.meditationMilestones.dayStreakPrestige = 0;
    }



    static void OnUnload(){
        SaveBeforeQuit();
    }
    static void SaveBeforeQuit(){
        DataManager dm = GameObject.Find("DataManager").GetComponent<DataManager>();
        if(dm.playerLoaded){
            dm.SaveAll();
            Debug.Log("Saving via force quit");
        } else {
            Debug.Log("No Player found. Leaving without saving");
        }
    }
    [RuntimeInitializeOnLoadMethod]
    static void RunOnStart()
    {
        Application.quitting += OnUnload;
        // EditorApplication.quitting += OnUnload;
    }


}

public class PlayerData : MonoBehaviour
{
    public string playerName;
    public string gardenName;
    // coins are currency to buy things
    public int coins;
    // tokens are used to play mini games
    public int tokens;
    public int lastLogin;
    public string lastTokenDate;
    public bool dailyTokensClaimed;
    public int inventorySize;
    public int lastShopCheck;
    public List<int> inventory;
    public List<int> storedShopItems;
    public Island currentIsland;
    public List<Island> islands;

    public MoodRecord moodRecord;
    // Game and Other STuff
    // public int lastDateMeditated;
    // public int longestMeditation;

    // gong clicker stuff
    public int gongClickPowerLevel;
    public int gongClickRateLevel;
    public int gongSpiritLevel;
    public int gongSpiritRateLevel;
    public int gongMultiplierLevel;
    public int lastGongCheck;
    public MeditationMilestones meditationMilestones;
    
    // tutorials
    public bool needDisclaimer;
    public bool needMainTutorial;
    public bool needMemoryMatchTutorial;
    public bool needGracefulWindTutorial;
    public bool needGongRingerTutorial;
    public bool needIslandTutorial;
    
}

public class MoodRecord {
    public bool bonusActive = true;
    public Dictionary<string, bool> dailyFullyComplete;
    public Dictionary<string, int[]> moodLevel;
    public int lastMoodInput;
    public bool dailyMoodComplete;
    public int moodLevelStreak;
    public bool meditationComplete;
    public bool gratitudeComplete;
    public string lastGratitudeComplete;

    // affirmation and days left
    public bool dailyAffCompleted;
    public string lastAffCompleted;
    public List<string> affirmations;
    public Dictionary<string, string> gratitudeItem;
    public string lastDateMeditated;
    public int daysLongestMeditation;
    public int longestMeditation;

}

