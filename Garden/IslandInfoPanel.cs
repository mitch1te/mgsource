using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandInfoPanel : MonoBehaviour
{
    public DataManager dataManager;
    public GardenManager gardenManager;
    public GameObject islandName;
    public GameObject listHolder;
    public GameObject listItem;
    public Color offsetColor;
    public Text totalBeautyText;
    public Text totalPlantsText;
    public Text totalDecorationsText;
    private ItemDetails details;

    private int totalBeauty;
    private int totalPlants;
    private int totalDecorations;

    public void LoadList(){
        totalBeauty = 0;
        totalPlants = 0;
        totalDecorations = 0;
        gardenManager.windowOpen = true;
        foreach(Transform t in listHolder.transform){
            Destroy(t.gameObject);
        }
        RectTransform listRect = listHolder.GetComponent<RectTransform>();
        listRect.sizeDelta = new Vector2(listRect.sizeDelta.x, (dataManager.player.currentIsland.activeItems.Count * 40) + 40);
        int offset = 1;
        foreach(Item item in dataManager.player.currentIsland.activeItems){
            bool itemFound = false;
            var createdItem = Instantiate(listItem);
            if(offset == 1){
                offset = 0;
            } else {
                createdItem.GetComponent<Image>().color = offsetColor;
                offset = 1;
            }
            IslandListItem listDetails = createdItem.GetComponent<IslandListItem>();
            createdItem.transform.SetParent(listHolder.transform, worldPositionStays: false);
            listDetails.item = item;
            listDetails.itemName.text = item.itemName;
            listDetails.uid = item.uid;
            details = new ItemDetails();
            // Debug.Log(item.GetComponent<ItemDetails>().fullyGrown);


            // TODO: PULL FROM ACTIVE OBJECTS ON ISLAND TO GET STATUSES 
            foreach(Item itemSearch in dataManager.itemVault.allPlants){
                if(item.itemName == itemSearch.itemName){
                    details = itemSearch.GetComponent<ItemDetails>();
                    totalBeauty += details.beauty;
                    totalPlants += 1;
                    itemFound = true;
                    break;
                }
            }

            if(!itemFound){
                foreach(Item itemSearch in dataManager.itemVault.allDeco){
                    if(item.itemName == itemSearch.itemName){
                        details = itemSearch.GetComponent<ItemDetails>();
                        itemFound = true;
                        totalBeauty += details.beauty;
                        totalDecorations += 1;
                        listDetails.harvestButton.gameObject.SetActive(false);
                        break;
                    }
                }
            }

            listDetails.value.text = details.value.ToString();
            listDetails.beauty.text = details.beauty.ToString();
            listDetails.selectionClicker.GetComponent<Button>().onClick.AddListener(delegate{ ZoomToObject(item);});


            if(details.growable){
                listDetails.destroyButton.gameObject.SetActive(false);
                if(GetEpochTime() - item.placedDatetime < details.itemGrowingTime){
                    int timeLeft = (details.itemGrowingTime) - (GetEpochTime() - item.placedDatetime);
                    listDetails.growTime.text = ConvertIntIntoTime(timeLeft);
                    listDetails.harvestButton.interactable = false;
                } else {
                    listDetails.growTime.text = "Fully grown!";
                    listDetails.harvestButton.interactable = true;
                }
            } else {
                listDetails.growTime.text = "";
                listDetails.harvestButton.gameObject.SetActive(false);
            }
            totalBeautyText.text = totalBeauty.ToString();
            totalPlantsText.text = totalPlants.ToString();
            totalDecorationsText.text = totalDecorations.ToString();
            listDetails.destroyButton.onClick.AddListener(delegate {gardenManager.DestroyObject(item.uid);});
            listDetails.harvestButton.onClick.AddListener(delegate {gardenManager.HarvestItem(item.uid);});
        }
    }

    public void ZoomToObject(Item item){

        foreach(Transform thing in gardenManager.plantedItemHolder.transform){
            var itemDeets = thing.gameObject.GetComponent<Item>();
            if(itemDeets.uid == item.uid){
                var pos = thing.transform.position;
                Debug.Log(pos);
                Vector3 setRot = new Vector3(0,-23,0);
                Vector3 setPos = new Vector3(pos.x + 17, -490, pos.z - 39);
                gardenManager.cameraManager.gardenCamHolder.transform.rotation = Quaternion.Euler(setRot);
                gardenManager.cameraManager.gardenCamHolder.transform.localPosition = setPos;
                gardenManager.ToggleIslandInfoPanel();
                gardenManager.objectToAdjust = thing.gameObject;
                gardenManager.LoadObjectDetails();
            }
        }
    }
    
    public int GetEpochTime(){
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Local);
        return (int)(System.DateTime.Now - epochStart).TotalSeconds;
    }

        public string ConvertEpochToTime(int timeToConvert){
        var datetime =  System.DateTimeOffset.FromUnixTimeSeconds(timeToConvert);
        return datetime.ToString("HH\\:mm\\:ss");
    }

        public string ConvertIntIntoTime(int timeToConvert){
        var time = System.TimeSpan.FromSeconds(timeToConvert);
        return time.ToString();
    }

}
