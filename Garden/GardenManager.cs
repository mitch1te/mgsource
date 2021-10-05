using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GardenManager : MonoBehaviour
{
    [Header("Managers")]
    public DataManager dataManager;
    public CameraManager cameraManager;
    public GameObject soundPlayer;
    [Header("Shop")]
    private Vector3 shopScrollPos;
    public GameObject shopPanel;
    public GameObject shopButton;
    // public GameObject shopText;
    [Header("Island List")]
    public GameObject islandInfoButton;
    // public GameObject islandInfoText;
    public GameObject islandInfoPanel;
    public Text islandInfoName;
    [Header("Inventory Stuff")]
    public GameObject inventoryButton;
    // public GameObject inventoryText;

    public GameObject inventoryObject;
    public GameObject inventoryHolder;
    public GameObject invSlot;
    public GameObject zoomOutButton;
    public GameObject backToMainButton;
    public GameObject bottomBar;
    [Header("Colors and Stuff")]
    public Color inactiveColor;
    public Color activeColor;
    [Header("Selected Item")]
    public Item loadedItem;
    public int loadedItemSlot;
    // public GameObject selectedPot;
    public GameObject selectedItemScreen;
    public GameObject itemImage;
    public Text itemName;
    public Text growingTime;
    public Text bio;
    public Text inventoryBeautyAmount;
    public GameObject plantToPlant;
    public GameObject plantButton;
    // public GameObject potToPlace;
    // public GameObject placePotButton;
    public GameObject placingHolder;
    [Header("Adjustment Stuff")]
    public GameObject highlighter;
    public GameObject distanceMeasurer;
    public GameObject objectDetailsPanel;
    public GameObject objectMovementPanel;
    public Text objectName;
    public Text bioblock;
    public Text placedDatetime;
    public Text harvestDateTime;
    public GameObject harvestButton;
    public GameObject adjustmentPanel;
    public GameObject previousObject;
    public GameObject shopClickArea;
    public GameObject inventoryClickArea;
    public GameObject islandStatsClickArea;

    [Header("Garden Layout")]
    public GameObject currentIsland;
    public GameObject plantedItemHolder;

    [Header ("Statuses")]
    public bool windowOpen;
    public bool movingCam;
    public bool viewingItem;
    public bool planting;
    // public bool placingPot;
    public bool inventoryOpen;
    public bool bottomBarOpen;
    public bool movementButtonHeld;
    public GameObject warningText;
    public GameObject warningBar;

    [Header("Sounds")]
    public AudioSource wind;

    // Moving Statuses
    private bool movingObjectForward;
    private bool movingObjectBackward;
    private bool movingObjectLeft;
    private bool movingObjectRight;
    private bool rotatingObjectCW;
    private bool rotatingObjectCCW;
    public bool movingAnObject;
    [Header("other")]
    public GameObject invDraggingItem;
    
    public GameObject objectToAdjust;
    public List<GameObject> clickedObjects;
    public bool adjustingItem;
    public GameObject multiList;
    public Text listOfSelectedObjects;
    public bool otherBlocked;
    [Header("Sounds")]
    public AudioClip selectPulse1;
    public AudioClip itemPurchased;

    void Update(){

        // keep selected item grow time updating 
        if(objectToAdjust != null){
            UpdateGrowTime();
        } else if (objectToAdjust == null && highlighter.transform.position != new Vector3(0,0,0)){
            highlighter.transform.position = new Vector3(0,0,0);
        }

        if(Input.GetMouseButtonUp(0) && !planting && !cameraManager.moving && !cameraManager.zooming && !cameraManager.rotating && !otherBlocked && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()){
            // RaycastHit hit;
            // Ray ray = cameraManager.gardenCam.ScreenPointToRay(Input.mousePosition);

            // if(objectToAdjust != null && !objectToAdjust.GetComponent<Item>().moving){
            //     objectToAdjust.GetComponent<ItemDetails>().selected = false;
            // }
            // int layerMask = 1 << LayerMask.NameToLayer("Island") | 1 << LayerMask.NameToLayer("Soil");
            // if(Physics.Raycast(ray, out hit, 20000f, ~layerMask) && !planting && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && !cameraManager.rotating && !cameraManager.zooming){
            //     GameObject clicked = hit.transform.gameObject;
            //     Debug.Log(clicked.name);
            //     if(clicked.layer == LayerMask.NameToLayer("Items")){
            //         objectToAdjust = clicked;
            //         LoadObjectDetails();
            //         return;
            //     }
            // }
            int layerMask = 1 << LayerMask.NameToLayer("Items");
            Ray ray = cameraManager.gardenCam.ScreenPointToRay(Input.mousePosition); 
            RaycastHit[] hits = Physics.RaycastAll(ray, 10000f, layerMask);
            List<GameObject> tmpObjects = new List<GameObject>();
            for(int i = 0; i < hits.Length; i ++){
                // if(!clickedObjects.Contains(objectToAdjust)){
                //     clickedObjects.Clear();
                // }
                if(hits.Length == 1 && clickedObjects.Count == 0){
                    Debug.Log("Found 1");
                    objectToAdjust = hits[0].transform.gameObject;
                    clickedObjects.Add(objectToAdjust);
                    LoadObjectDetails();
                    break;
                }
                // if(hits.Length != clickedObjects.Count){
                //     clickedObjects.Clear();
                //     objectToAdjust = null;
                // }
                if(objectToAdjust != null && clickedObjects.Contains(objectToAdjust) && hits.Length != 0){
                    int idx = clickedObjects.FindIndex(x => x.Equals(objectToAdjust));
                    if(idx == clickedObjects.Count-1){
                        objectToAdjust = clickedObjects[0];
                        Debug.Log("cycling next");
                        LoadObjectDetails();
                        break;
                    }
                    objectToAdjust = clickedObjects[idx+1];
                    Debug.Log("cycling to first");
                    LoadObjectDetails();
                    break;
                }

                if(hits[i].transform.gameObject.layer == LayerMask.NameToLayer("Items")){
                    clickedObjects.Add(hits[i].transform.gameObject);
                }

            }
            
            if(clickedObjects.Count > 1){
                listOfSelectedObjects.text = "";
                multiList.SetActive(true);
                foreach(var thing in clickedObjects){
                    listOfSelectedObjects.text = listOfSelectedObjects.text + thing.gameObject.GetComponent<Item>().itemName + "\n";
                }
            } else {
                multiList.SetActive(false);
            }

            if(hits.Length == 0 && objectToAdjust != null){
                // CloseDetailsPanel();
                clickedObjects.Clear();
            }
            if(objectToAdjust == null && clickedObjects.Count > 0){
                objectToAdjust = clickedObjects[0];
                LoadObjectDetails();
            }
            
            // if(objectToAdjust != null){
            //     LoadObjectDetails();
            // }
        }

        if(planting){
            RaycastHit hit;
            Ray ray = cameraManager.gardenCam.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << LayerMask.NameToLayer("Items") | 1 << LayerMask.NameToLayer("Island");
            if(Physics.Raycast(ray, out hit, 3000f,  ~layerMask)){
                // var turn = (Quaternion.FromToRotation(-hit.transform.up, hit.normal));
                Vector3 objPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                plantToPlant.transform.position = objPoint;
                // plantToPlant.transform.rotation = turn;
            }
            if(Input.GetMouseButtonUp(0)){
                try {
                    if(!plantToPlant.GetComponent<Item>().colliding && hit.transform.gameObject.layer == LayerMask.NameToLayer("Soil")){
                        PlacingReleased(true);
                    }  else {
                        PlacingReleased(false);
                    }         
                } catch {
                    PlacingReleased(false);
                }
              
            }  
        }

        // Resetting movement and other bools
        if(Input.GetMouseButtonUp(0)){
            movingObjectForward = false;
            movingObjectBackward = false;
            movingObjectLeft = false;
            movingObjectRight = false;
            rotatingObjectCCW = false;
            rotatingObjectCW = false;
        }


        if(objectToAdjust != null){
            if(movingObjectForward){
                adjustingItem = true;
                Vector3 origPos = objectToAdjust.transform.position;
                var forward = cameraManager.gardenCam.transform.forward;
                Vector3 moveVector = new Vector3(forward.x, 0, forward.z);
                StartCoroutine(CheckMovement(origPos, moveVector, true));
                // cameraManager.gardenCam.transform.position += (.1f * moveVector);
            } else if (movingObjectBackward){
                adjustingItem = true;
                Vector3 origPos = objectToAdjust.transform.position;
                var backward = cameraManager.gardenCam.transform.forward;
                Vector3 moveVector = new Vector3(backward.x, 0, backward.z);
                StartCoroutine(CheckMovement(origPos, moveVector, false));
                // cameraManager.gardenCam.transform.position += (.1f * -moveVector);
            } else if (movingObjectLeft){
                adjustingItem = true;
                Vector3 origPos = objectToAdjust.transform.position;
                var left = cameraManager.gardenCam.transform.right;
                Vector3 moveVector = new Vector3(left.x, 0, left.z);
                StartCoroutine(CheckMovement(origPos, moveVector, false));
                // cameraManager.gardenCam.transform.position += (.1f * -moveVector);
            } else if (movingObjectRight){
                adjustingItem = true;
                Vector3 origPos = objectToAdjust.transform.position;
                var right = cameraManager.gardenCam.transform.right;
                Vector3 moveVector = new Vector3(right.x, 0, right.z);
                StartCoroutine(CheckMovement(origPos, moveVector, true));
                // cameraManager.gardenCam.transform.position += (.1f * moveVector);
            } else if (rotatingObjectCW){
                adjustingItem = true;
                objectToAdjust.transform.Rotate(Vector3.up * 1);
            } else if (rotatingObjectCCW){
                adjustingItem = true;
                objectToAdjust.transform.Rotate(Vector3.up * -1);
            } else {
                adjustingItem = false;
            }
        }

    }
    public void AddTestPlant(){
        for(var i = 0; i < dataManager.player.inventorySize; i++){
            if(dataManager.player.inventory[i] == 0){
                dataManager.player.inventory[i] = 11;
                dataManager.SaveAll();
                break;
            }
        }
    }

    public void ShowPlantDetails(Item item, int invSlot){
        soundPlayer.GetComponent<AudioSource>().PlayOneShot(selectPulse1);
        // placePotButton.SetActive(false);
        plantButton.SetActive(true);
        loadedItemSlot = invSlot;
        loadedItem = item;
        itemImage.GetComponent<Image>().sprite = item.GetComponent<ItemDetails>().itemPreview;
        itemName.text = item.itemName;
        growingTime.text = System.TimeSpan.FromSeconds(item.GetComponent<ItemDetails>().itemGrowingTime).ToString();
        inventoryBeautyAmount.text = item.GetComponent<ItemDetails>().beauty.ToString();
        bio.text = item.GetComponent<ItemDetails>().bio;
        if(!viewingItem){
            Animation anim = selectedItemScreen.GetComponent<Animation>();
            anim.PlayQueued("revealSelected");
        }
        viewingItem = true;
    }

    public void ToggleInventory(){
        soundPlayer.GetComponent<AudioSource>().PlayOneShot(selectPulse1);
        if(shopPanel.activeSelf){
            ToggleShopPanel();
        }
        if(islandInfoPanel.activeSelf){
            ToggleIslandInfoPanel();
        }
        if(!inventoryObject.activeSelf){
            inventoryObject.SetActive(true);
            ReloadInventory();
            inventoryObject.GetComponent<Animation>().Play("revealInventory");
            CloseDetailsPanel();
            inventoryButton.GetComponent<Image>().color = activeColor;
            // inventoryText.GetComponent<Text>().color = activeColor;
        } else {
            if(viewingItem){
                HideSelected();
            }
            StartCoroutine(I_HideInv());
            inventoryButton.GetComponent<Image>().color = inactiveColor;
            // inventoryText.GetComponent<Text>().color = inactiveColor;
        }
    }

    public void ToggleIslandInfoPanel(){
        soundPlayer.GetComponent<AudioSource>().PlayOneShot(selectPulse1);
        if(inventoryObject.activeSelf){
            ToggleInventory();
        }
        if(shopPanel.activeSelf){
            ToggleShopPanel();
        }
        if(!islandInfoPanel.activeSelf){
            islandInfoPanel.GetComponent<IslandInfoPanel>().LoadList();
            windowOpen = true;
            islandInfoName.text = dataManager.player.gardenName;
            islandInfoPanel.SetActive(true);
            islandInfoButton.GetComponent<Image>().color = activeColor;
            // islandInfoText.GetComponent<Text>().color = activeColor;
        } else {
            windowOpen = false;
            islandInfoPanel.SetActive(false);
            islandInfoButton.GetComponent<Image>().color = inactiveColor;
            // islandInfoText.GetComponent<Text>().color = inactiveColor;
        }
    }
    public void ToggleShopPanel(){
        soundPlayer.GetComponent<AudioSource>().PlayOneShot(selectPulse1);
        if(inventoryObject.activeSelf){
            ToggleInventory();
        }
        if(islandInfoPanel.activeSelf){
            ToggleIslandInfoPanel();
        }
        if(!shopPanel.activeSelf){
            shopScrollPos = shopPanel.GetComponent<ShopManager>().shopItemHolder.GetComponent<RectTransform>().position;
            windowOpen = true;
            shopPanel.SetActive(true);
            shopButton.GetComponent<Image>().color = activeColor;
            shopPanel.GetComponent<ShopManager>().UpdateStuff();
            // shopText.GetComponent<Text>().color = activeColor;
        } else {
            shopPanel.GetComponent<ShopManager>().shopItemHolder.GetComponent<RectTransform>().position = shopScrollPos;
            windowOpen = false;
            shopPanel.SetActive(false);
            shopButton.GetComponent<Image>().color = inactiveColor;
            // shopText.GetComponent<Text>().color = inactiveColor;
        }
    }

    public void CloseDetailsPanel(){
        // cameraManager.ResetObjectCam();
        objectToAdjust = null;
        movingAnObject = false;
        clickedObjects.Clear();
        // cameraManager.cam.transform.LookAt(currentIsland.transform);
        objectDetailsPanel.SetActive(false);
        objectMovementPanel.SetActive(false);
    }

    public void ReloadInventory(){
        foreach(Transform item in inventoryHolder.transform){
            Destroy(item.gameObject);
        }
        for(var i = 0; i < dataManager.player.inventorySize; i++){
            GameObject slot = Instantiate(invSlot);
            slot.transform.SetParent(inventoryHolder.transform, worldPositionStays: false);
            slot.transform.GetChild(0).GetComponent<InvSlotController>().slotNum = i;
            if(dataManager.player.inventory[i] != 0){
                int slotNum = i;
                foreach(Item item in dataManager.itemVault.allDeco){
                    if(dataManager.player.inventory[i] == item.itemID){
                        slot.transform.GetChild(0).GetComponent<Image>().sprite = item.GetComponent<ItemDetails>().inventoryImage;
                        slot.transform.GetChild(0).GetComponent<InvSlotController>().itemID = item.itemID;
                        slot.GetComponent<Button>().onClick.AddListener(delegate {ShowPlantDetails(item, slotNum);});
                        break;
                    }
                }
                foreach(Item item in dataManager.itemVault.allPlants){
                    if(dataManager.player.inventory[i] == item.itemID){
                        slot.transform.GetChild(0).GetComponent<Image>().sprite = item.GetComponent<ItemDetails>().inventoryImage;
                        slot.transform.GetChild(0).GetComponent<InvSlotController>().itemID = item.itemID;
                        slot.GetComponent<Button>().onClick.AddListener(delegate {ShowPlantDetails(item, slotNum);});
                        break;
                    }
                }
            } else {
                slot.transform.GetChild(0).GetComponent<Image>().enabled = false;
            }
        }
    }

    public void TryToPlace(){
        placingHolder.SetActive(false);
        planting = true;
    }

    public void ReadyPlant(){
        ToggleInventory();
        plantToPlant = Instantiate(loadedItem.gameObject);
        plantToPlant.GetComponent<Item>().moving = true;
        placingHolder.GetComponent<Image>().sprite = loadedItem.GetComponent<ItemDetails>().itemPreview;
        placingHolder.SetActive(true);
    }

    public void PlacingReleased(bool acceptedPos){
       if(loadedItem.GetComponent<ItemDetails>().itemType == ItemDetails.ItemType.Plant || loadedItem.GetComponent<ItemDetails>().itemType == ItemDetails.ItemType.Decoration){
            if(acceptedPos == false){
                planting = false;
                plantToPlant.transform.position = new Vector3(2000,2000,2000);
                placingHolder.SetActive(true);
            } else {
                PlantThePlant();
                placingHolder.GetComponent<Image>().sprite = null;
                placingHolder.SetActive(false);
            }
       } 
    }

    public void LoadObjectDetails(){
        movingAnObject = true;
        bioblock.text = objectToAdjust.GetComponent<ItemDetails>().bio;
        // cameraManager.cam.transform.LookAt(objectToAdjust.transform);
        objectDetailsPanel.SetActive(true);
        objectName.text = objectToAdjust.GetComponent<Item>().itemName;
        placedDatetime.text = ConvertEpochToDate(objectToAdjust.GetComponent<Item>().placedDatetime);
        highlighter.transform.position = new Vector3(objectToAdjust.transform.position.x, objectToAdjust.transform.position.y + 1, objectToAdjust.transform.position.z);
        // Debug.Log(objectToAdjust.GetComponent<Renderer>().bounds.size);
        highlighter.GetComponent<Light>().range = objectToAdjust.GetComponent<Renderer>().bounds.size.x;
        highlighter.GetComponent<Light>().intensity = objectToAdjust.GetComponent<Renderer>().bounds.size.x * 2;
        ShowHarvestButton();

    }

    public void ShowHarvestButton(){
        harvestButton.SetActive(true);
        harvestButton.GetComponent<Button>().onClick.RemoveAllListeners();
        Item details = objectToAdjust.GetComponent<Item>();
        if(objectToAdjust.GetComponent<ItemDetails>().itemType == ItemDetails.ItemType.Plant){
            if(GetEpochTime() - details.placedDatetime < objectToAdjust.GetComponent<ItemDetails>().itemGrowingTime){
                harvestButton.GetComponent<Button>().interactable = false;
                harvestButton.transform.Find("Text").GetComponent<Text>().text = "Still Growing";
            } else {
                harvestButton.transform.Find("Text").GetComponent<Text>().text = "Harvest";
                harvestButton.GetComponent<Button>().interactable = true;
                harvestButton.GetComponent<Button>().onClick.AddListener(delegate{HarvestItem(details.uid);});
            }
        } else {
            harvestButton.SetActive(false);
        }
    }

    public void UpdateGrowTime(){
        Item details = objectToAdjust.GetComponent<Item>();
        if(objectToAdjust.GetComponent<ItemDetails>().itemType != ItemDetails.ItemType.Plant){
            harvestDateTime.text = "Does not grow";
            return;
        }
        if(GetEpochTime() - details.placedDatetime < objectToAdjust.GetComponent<ItemDetails>().itemGrowingTime){
                int timeLeft = (objectToAdjust.GetComponent<ItemDetails>().itemGrowingTime) - (GetEpochTime() - details.placedDatetime);
                harvestDateTime.text = ConvertIntIntoTime(timeLeft);
        } else {
            ShowHarvestButton();
            harvestDateTime.text = "Fully grown!";
        }
    }
    public void AdjustObject(){
        shopClickArea.GetComponent<Button>().interactable = false;
        shopButton.GetComponent<Button>().interactable = false;
        inventoryClickArea.GetComponent<Button>().interactable = false;
        inventoryButton.GetComponent<Button>().interactable = false;
        islandStatsClickArea.GetComponent<Button>().interactable = false;
        islandInfoButton.GetComponent<Button>().interactable = false;
        objectMovementPanel.SetActive(true);
        objectToAdjust.GetComponent<Item>().movingValid = true;
    }

    public void MoveObjectForward(){  
        movingObjectForward = true;
    }

    public void MoveObjectBackward(){
        movingObjectBackward = true;
    }

    public void MoveObjectLeft(){
        movingObjectLeft = true;
    }

    public void MoveObjectRight(){
        movingObjectRight = true;
    }
    public void RotateObjectCW(){
        rotatingObjectCW = true;
    }

    public void RotateObjectCCW(){
        rotatingObjectCCW = true;
    }

    IEnumerator CheckMovement(Vector3 origPos, Vector3 moveVec, bool posMovement){
        float origDistance = 0;
        float nextDistance = 0;
        Vector3 shootVec = new Vector3(objectToAdjust.transform.position.x, distanceMeasurer.transform.position.y, objectToAdjust.transform.position.z);
        RaycastHit hit;
        if(Physics.Raycast(shootVec, Vector3.down, out hit, 1000, 1 << LayerMask.NameToLayer("Soil"))){
            origDistance = hit.distance;
        }
        if(objectToAdjust.GetComponent<Item>().movingValid){
            if(posMovement){
                objectToAdjust.transform.position += (.1f * moveVec);
            } else {
                objectToAdjust.transform.position += (.1f * -moveVec);
            }
            shootVec = new Vector3(objectToAdjust.transform.position.x, distanceMeasurer.transform.position.y, objectToAdjust.transform.position.z);
            if(Physics.Raycast(shootVec, Vector3.down, out hit, 1000, 1 << LayerMask.NameToLayer("Soil"))){
                nextDistance = hit.distance;
            }
            // var turn = (Quaternion.FromToRotation(-hit.transform.up, hit.normal));
            float yDistDiff = (nextDistance / origDistance);
            Vector3 finalPos = new Vector3(objectToAdjust.transform.position.x, (objectToAdjust.transform.position.y * yDistDiff), objectToAdjust.transform.position.z);
            if(float.IsNaN(finalPos.x) || float.IsNaN(finalPos.y) || float.IsNaN(finalPos.z)){
                objectToAdjust.GetComponent<Item>().movingValid = false;
                movingObjectBackward = false;
                movingObjectForward = false;
                movingObjectLeft = false;
                movingObjectRight = false;
                yield break;
            }
            if(objectToAdjust.GetComponent<Item>().movingValid && finalPos.y < -1 && Mathf.Abs(finalPos.y) != Mathf.Infinity){
                objectToAdjust.transform.position = finalPos;
                if(posMovement){
                    cameraManager.gardenCam.transform.parent.transform.position += (.1f * moveVec);
                } else {
                    cameraManager.gardenCam.transform.parent.transform.position += (.1f * -moveVec);
                }
            }     
        }
        yield return null;
        if(!objectToAdjust.GetComponent<Item>().movingValid){
            if(posMovement){
                objectToAdjust.transform.position = (origPos += (1f * -moveVec)); 
                // objectToAdjust.transform.position += (1f * -moveVec);    
            } else {
                objectToAdjust.transform.position = (origPos += (1f  *moveVec));
                // objectToAdjust.transform.position += (1f * moveVec);
            }
            ShowWarning("Can't Move " + objectToAdjust.GetComponent<Item>().itemName + " There");
            // yield return new WaitForSeconds(.1f);
            objectToAdjust.GetComponent<Item>().movingValid = true;
        }
        highlighter.transform.position = new Vector3(objectToAdjust.transform.position.x, objectToAdjust.transform.position.y + 2, objectToAdjust.transform.position.z);
        yield return new WaitForEndOfFrame();
    }

    public void ConfirmNewPosition(){
        Debug.Log(dataManager.player.playerName);
        Debug.Log(dataManager.player.currentIsland);
        var activeItems = dataManager.player.currentIsland.activeItems;
        for(var i = 0; i < activeItems.Count; i++){
            if(activeItems[i].uid == objectToAdjust.GetComponent<Item>().uid){
                activeItems[i].itemLocation = objectToAdjust.transform.localPosition;
                activeItems[i].itemRotation = objectToAdjust.transform.localRotation.eulerAngles;
                break; 
            }
        }
        dataManager.SaveAll();
        objectMovementPanel.SetActive(false);
        objectToAdjust.GetComponent<Item>().moving = false;
        movingAnObject = false;
        shopClickArea.GetComponent<Button>().interactable = true;
        shopButton.GetComponent<Button>().interactable = true;
        inventoryClickArea.GetComponent<Button>().interactable = true;
        inventoryButton.GetComponent<Button>().interactable = true;
        islandStatsClickArea.GetComponent<Button>().interactable = true;
        islandInfoButton.GetComponent<Button>().interactable = true;
    }

    public void PlantThePlant(){
        plantToPlant.transform.SetParent(plantedItemHolder.transform);
        dataManager.player.currentIsland.activeItems.Add(plantToPlant.GetComponent<Item>());
        Item itemRef = dataManager.player.currentIsland.activeItems[dataManager.player.currentIsland.activeItems.Count-1];
        itemRef.itemLocation = plantToPlant.transform.localPosition;
        itemRef.itemRotation = plantToPlant.transform.localRotation.eulerAngles;
        itemRef.placedDatetime = GetEpochTime();
        List<int> activeUIDs = new List<int>();
        for(var i = 0; i < dataManager.player.currentIsland.activeItems.Count; i++){
            activeUIDs.Add(dataManager.player.currentIsland.activeItems[i].uid);
        }
        for(var i = 1; i < dataManager.player.currentIsland.activeItems.Count; i++){
            if(!activeUIDs.Contains(i)){
                itemRef.uid = i;
            }
        }
        itemRef.moving = false;
        plantToPlant.name = itemRef.itemName + dataManager.player.currentIsland.activeItems.Count;
        itemRef.GetComponent<ItemDetails>().planted = true;
        planting = false;
        objectToAdjust = plantToPlant;
        plantToPlant = null;
        dataManager.player.inventory[loadedItemSlot] = 0;
        loadedItemSlot = 0;
        dataManager.SaveAll();
        LoadObjectDetails();
    }

    public void LoadAllExisting(){
        StartCoroutine(I_LoadAllExisting());
    }


    IEnumerator I_LoadAllExisting(){
        // if(dataManager.player.currentIsland.activeItems.Count == 0){
        //     yield break;
        // }
        GameObject lastObject = new GameObject();
        foreach(Transform pot in plantedItemHolder.transform){
            Destroy(pot.gameObject);
        }
        foreach(Item item in dataManager.player.currentIsland.activeItems){
            foreach(Item itemSearch in dataManager.itemVault.allDeco){
                if(item.itemName == itemSearch.itemName){
                     GameObject createdItem = Instantiate(itemSearch.gameObject);
                    createdItem.transform.SetParent(plantedItemHolder.transform);  
                    createdItem.transform.localPosition = item.itemLocation;
                    createdItem.transform.localRotation = Quaternion.Euler(item.itemRotation);
                    createdItem.name = createdItem.GetComponent<Item>().itemName + item.uid;
                    createdItem.GetComponent<Item>().uid = item.uid;
                    createdItem.GetComponent<Item>().placedDatetime = item.placedDatetime;
                    break;
                }
            } 
        }

        foreach(Item item in dataManager.player.currentIsland.activeItems){
            foreach(Item itemSearch in dataManager.itemVault.allPlants){
                if(item.itemName == itemSearch.itemName){
                    GameObject createdItem = Instantiate(itemSearch.gameObject);
                    createdItem.transform.SetParent(plantedItemHolder.transform);  
                    createdItem.transform.localPosition = item.itemLocation;
                    createdItem.transform.localRotation = Quaternion.Euler(item.itemRotation);
                    createdItem.name = createdItem.GetComponent<Item>().itemName + item.uid;
                    // Debug.Log(createdItem.name);
                    createdItem.GetComponent<Item>().uid = item.uid;
                    createdItem.GetComponent<ItemDetails>().planted = true;
                    createdItem.GetComponent<Item>().placedDatetime = item.placedDatetime;
                    break;
                }
            } 
        }
        yield return new WaitForSeconds(.1f);
    }


    public void DestroyObject(int uid){
        for(var i = 0; i < dataManager.player.currentIsland.activeItems.Count; i ++){
            if(uid == dataManager.player.currentIsland.activeItems[i].uid){
                dataManager.player.currentIsland.activeItems.RemoveAt(i);
                dataManager.SaveAll();
            }
        }
        islandInfoPanel.GetComponent<IslandInfoPanel>().LoadList();
        LoadAllExisting();
    }

    public void HarvestItem(int uid){
        Debug.Log("hit");
        Item foundItem = null;
        for(var i = 0; i < dataManager.player.currentIsland.activeItems.Count; i++){
            if(uid == dataManager.player.currentIsland.activeItems[i].uid){
                foreach(Item item in dataManager.itemVault.allPlants){
                    if(dataManager.player.currentIsland.activeItems[i].itemID == item.itemID){
                        foundItem = item;
                        Debug.Log(foundItem.itemName);
                        break;
                    }
                }
                Debug.Log(dataManager.player.coins);
                dataManager.player.currentIsland.activeItems.RemoveAt(i);
                dataManager.player.coins += (int)(foundItem.GetComponent<ItemDetails>().value * 1.5f);
                Debug.Log(dataManager.player.coins);
                dataManager.SaveAll();
            }
        }
        CloseDetailsPanel();
        islandInfoPanel.GetComponent<IslandInfoPanel>().LoadList();
        LoadAllExisting();
    }
    public void HideSelected(){
        viewingItem = false;
        StartCoroutine(I_HideSelected());
    }

    public void ToggleBottomBar(){
        if(!bottomBarOpen){
            bottomBar.GetComponent<Animation>().Play("showBottomBar");
            bottomBarOpen = true;
        } else {
            bottomBar.GetComponent<Animation>().Play("hideBottomBar");
            bottomBarOpen = false;
        }
    }
    IEnumerator I_HideSelected(){
        Animation hideAnim = selectedItemScreen.GetComponent<Animation>();
        hideAnim.PlayQueued("hideSelected");
        yield return StartCoroutine(WaitForAnimation(hideAnim));
    }

    IEnumerator I_HideInv(){
        Animation hideAnim = inventoryObject.GetComponent<Animation>();
        hideAnim.PlayQueued("hideInventory");
        yield return StartCoroutine(WaitForAnimation(hideAnim));
        foreach(Transform item in inventoryHolder.transform){
            Destroy(item.gameObject);
        }
        inventoryObject.SetActive(false);
    }

    public void BackToMainMenu(){
        backToMainButton.GetComponent<Button>().interactable = false;
        cameraManager.SwitchCamera();
    }
    public void ShowWarning(string text){
        warningText.GetComponent<Text>().text = text;
        warningText.GetComponent<Animation>().Play("showWarning");
        warningBar.GetComponent<Animation>().Play("popupBarFadeIn");
    }
    public IEnumerator ZoomToObject(GameObject clickedObj){
        if(clickedObj.GetComponent<Item>() != null && !movingCam){
            movingCam = true;
            zoomOutButton.SetActive(true);
            backToMainButton.SetActive(false);
            Camera cam = cameraManager.gardenCam;
            cam.transform.SetParent(clickedObj.transform);
            LeanTween.moveLocal(cam.gameObject, new Vector3(0, 2.25f, -2.5f), 1);
            LeanTween.rotateLocal(cam.gameObject, new Vector3(40,0,0), 1);
            // selectedPot = clickedObj;
            yield return new WaitForSeconds(1);
            cam.transform.SetParent(transform.root);
            movingCam = false;
        }
    }
    private IEnumerator WaitForAnimation ( Animation animation ){
        do { yield return null; } while ( animation.isPlaying );
    }

    public int GetEpochTime(){
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Local);
        return (int)(System.DateTime.Now - epochStart).TotalSeconds;
    }
    public string ConvertEpochToDate(int timeToConvert){
        var datetime =  System.DateTimeOffset.FromUnixTimeSeconds(timeToConvert);
        Debug.Log(datetime.ToString("MM/dd/yyyy"));
        return datetime.ToString("MM/dd/yyyy");
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
