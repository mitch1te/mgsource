using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilestoneButton : MonoBehaviour
{
    public GameObject milestone;
    public GameObject milestonePanel;


    public void LoadMilestone(){
        var ms = Instantiate(milestone);
        ms.transform.SetParent(milestonePanel.transform, worldPositionStays: false);
    }

}
