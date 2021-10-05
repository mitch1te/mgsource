using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilestoneManager : MonoBehaviour
{
    public MeditationMilestones meditationMilestones;
}


public class MeditationMilestones : MonoBehaviour {
    public int totalTime;
    public int longestTime;
    public int dayStreak;

    public List<bool> totalTimeUnlocks;
    public int totalTimePrestige;
    public List<bool> longestTimeUnlocks;
    public int longestTimePrestige;
    public List<bool> dayStreakUnlocks;
    public int dayStreakPrestige;


    public void CheckLongestTime(int timeToCheck){
        if(timeToCheck > longestTime){
            longestTime = timeToCheck;
        }
        // total time in seconds required for next unlock
        List<int> longestTimeList = new List<int>(){60, 120, 240, 300, 500};
        int posToCheck = 0;
        for(var i = 0; i < longestTimeUnlocks.Count; i ++){
            if(longestTimeUnlocks[i] == false){
                posToCheck = i;
                break;
            }
        }
        if(timeToCheck >= longestTimeList[posToCheck]){
            if(posToCheck == longestTimeList.Count-1 && longestTimePrestige < 3){
                longestTimePrestige += 1;
                for(var i = 0; i < longestTimeUnlocks.Count; i ++){
                    longestTimeUnlocks[i] = false;
                    Debug.Log("prestiging");
                    // display presitege popup
                    // award stuff
                    gameObject.GetComponent<DataManager>().SaveAll();
                    return;
                }
            }
            longestTimeUnlocks[posToCheck] = true;
            Debug.Log("unlocking achievement");
            // show achievement popup
            // award stuff
            gameObject.GetComponent<DataManager>().SaveAll();
        }
    }
}

