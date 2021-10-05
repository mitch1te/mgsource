using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandListItem : MonoBehaviour
{
    
    public Item item;
    public int uid;
    public Text itemName;
    public Text value;
    public Text beauty;
    public Text growTime;
    
    public Button harvestButton;
    public Button destroyButton;

    public GameObject selectionClicker;

}
