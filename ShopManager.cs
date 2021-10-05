using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{

// 86400 sec == 1 day;
// TODO: Update, maybe make player variable to so player can upgrade shop cooldowns
// ALSO MAYBE MAKE NUMBER IF ITEMS UPGRADEABLE. START AT 8, have player unlock more
private int shopCountdown = 7200;
private int shopItems = 12;
private int viewingItemPos;
public DataManager dataManager;
public GardenManager gardenManager;
public GameObject invSlot;
public GameObject shopItemHolder;
public GameObject newItemsCountdown;
public GameObject warningText;
public GameObject playerShards;
public GameObject hiderOverlay;



[Header("Viewing Item Panel")]
public GameObject viewingItemPanel;
public Text growingTime;
public Text beauty;
public Text plantName;
public Text bio;
public GameObject plantPreview;
public GameObject purchaseButton;
private bool initialLoad;
private bool viewingItem;

    void Update(){  
        if(dataManager.playerLoaded){
            if(!initialLoad){
                LoadShopInventory();
                initialLoad = true;
            }
            UpdateShopTimer();
        }  
    }

    public void UpdateStuff(){
        playerShards.GetComponent<Text>().text = dataManager.player.coins.ToString();
    }
    public void UpdateShopTimer(){
        if(dataManager.player.lastShopCheck == 0){
            dataManager.player.lastShopCheck = dataManager.GetEpochTime();
            dataManager.SaveAll();
            ChangeShopInventory();
        } else {
            
            if(dataManager.GetEpochTime() - dataManager.player.lastShopCheck > shopCountdown){
                ChangeShopInventory();
                dataManager.player.lastShopCheck = dataManager.GetEpochTime();
                dataManager.SaveAll();
            }
        }
        int timeSinceUpdate = (dataManager.GetEpochTime() - dataManager.player.lastShopCheck);
        string timeUntilUpdate = ConvertEpochToTime(shopCountdown - timeSinceUpdate);
        newItemsCountdown.GetComponent<Text>().text = timeUntilUpdate;
    }

    public void LoadShopInventory(){
        RectTransform shopRect = shopItemHolder.GetComponent<RectTransform>();
        Debug.Log((shopItems/3) * 150);
        shopRect.sizeDelta = new Vector2(shopRect.sizeDelta.x, ((shopItems/3) * 150) + 150);
        foreach(Transform existing in shopItemHolder.transform){
            Destroy(existing.gameObject);
        }
        playerShards.GetComponent<Text>().text = dataManager.player.coins.ToString();
        // Debug.Log("loading initial");
        for(var i = 0; i < dataManager.player.storedShopItems.Count; i++){
            // Debug.Log(dataManager.player.storedShopItems[i]);
            Item chosenItem = new Item();
            var createdItem = Instantiate(invSlot);
            if(dataManager.player.storedShopItems[i] == 0){
                // Debug.Log("hit");
                createdItem.transform.SetParent(shopItemHolder.transform, worldPositionStays: false);
                createdItem.GetComponent<ShopItem>().itemImage.SetActive(false);
                createdItem.GetComponent<ShopItem>().cost = 0;
                createdItem.GetComponent<ShopItem>().costText.GetComponent<Text>().text = createdItem.GetComponent<ShopItem>().cost.ToString();
                createdItem.GetComponent<ShopItem>().soldOutOverlay.SetActive(true);
                createdItem.GetComponent<ShopItem>().shopPosition = i;
                createdItem.GetComponent<Button>().interactable = false; 
                createdItem.GetComponent<ShopItem>().gridItemName.SetActive(false);
                continue;
            }
            foreach(Item itemLookup in dataManager.itemVault.allPlants){
                // Debug.Log("Searching");
                 if(dataManager.player.storedShopItems[i] == itemLookup.itemID){
                    //  Debug.Log("Hit me too");
                    chosenItem = itemLookup;
                    createdItem.transform.SetParent(shopItemHolder.transform, worldPositionStays: false);
                    createdItem.GetComponent<ShopItem>().itemImage.GetComponent<Image>().sprite = chosenItem.GetComponent<ItemDetails>().inventoryImage;
                    createdItem.GetComponent<ShopItem>().cost = chosenItem.GetComponent<ItemDetails>().value;
                    createdItem.GetComponent<ShopItem>().shopPosition = i;
                    createdItem.GetComponent<ShopItem>().gridItemName.GetComponent<Text>().text = chosenItem.GetComponent<Item>().itemName;
                    createdItem.GetComponent<ShopItem>().costText.GetComponent<Text>().text = createdItem.GetComponent<ShopItem>().cost.ToString();
                    createdItem.GetComponent<Button>().onClick.AddListener(delegate {ShowItemInfo(chosenItem, createdItem);});
                    break;
                }
            }

            foreach(Item itemLookup in dataManager.itemVault.allDeco){
                // Debug.Log("Searching");
                 if(dataManager.player.storedShopItems[i] == itemLookup.itemID){
                    //  Debug.Log("Hit me too");
                    chosenItem = itemLookup;
                    createdItem.transform.SetParent(shopItemHolder.transform, worldPositionStays: false);
                    createdItem.GetComponent<ShopItem>().itemImage.GetComponent<Image>().sprite = chosenItem.GetComponent<ItemDetails>().inventoryImage;
                    createdItem.GetComponent<ShopItem>().cost = chosenItem.GetComponent<ItemDetails>().value;
                    createdItem.GetComponent<ShopItem>().shopPosition = i;
                    createdItem.GetComponent<ShopItem>().gridItemName.GetComponent<Text>().text = chosenItem.GetComponent<Item>().itemName;
                    createdItem.GetComponent<ShopItem>().costText.GetComponent<Text>().text = createdItem.GetComponent<ShopItem>().cost.ToString();
                    createdItem.GetComponent<Button>().onClick.AddListener(delegate {ShowItemInfo(chosenItem, createdItem);});
                    break;
                }
            }
        }
    }

    public void ChangeShopInventory(){
        RectTransform shopRect = shopItemHolder.GetComponent<RectTransform>();
        shopRect.sizeDelta = new Vector2(shopRect.sizeDelta.x, ((shopItems/3) * 150) + 150);
        Debug.Log("New Shop");
        dataManager.player.storedShopItems.Clear();
        foreach(Transform existing in shopItemHolder.transform){
            Destroy(existing.gameObject);
        }
        for(var i = 0; i < shopItems; i++){
            int shopPosition = i;
            var createdItem = Instantiate(invSlot);
            Item chosenItem = new Item();
            int plantOrDecor = Random.Range(1, 3);
            if(plantOrDecor == 1){
                // plant
                chosenItem = dataManager.itemVault.allPlants[Random.Range(0, dataManager.itemVault.allPlants.Count)];
            } else {
                // TODO: change to DECO later
                chosenItem = dataManager.itemVault.allDeco[Random.Range(0, dataManager.itemVault.allDeco.Count)];
            }
            createdItem.transform.SetParent(shopItemHolder.transform, worldPositionStays: false);
            createdItem.GetComponent<ShopItem>().itemImage.GetComponent<Image>().sprite = chosenItem.GetComponent<ItemDetails>().inventoryImage;
            createdItem.GetComponent<ShopItem>().cost = chosenItem.GetComponent<ItemDetails>().value;
            createdItem.GetComponent<ShopItem>().costText.GetComponent<Text>().text = createdItem.GetComponent<ShopItem>().cost.ToString();
            createdItem.GetComponent<ShopItem>().shopPosition = i;
            createdItem.GetComponent<ShopItem>().gridItemName.GetComponent<Text>().text = chosenItem.GetComponent<Item>().itemName;
            createdItem.GetComponent<Button>().onClick.AddListener(delegate{ShowItemInfo(chosenItem, createdItem);});
            dataManager.player.storedShopItems.Add(chosenItem.itemID);
        }
    }


    public void ShowItemInfo(Item chosenItem, GameObject createdItem){
        gardenManager.soundPlayer.GetComponent<AudioSource>().PlayOneShot(gardenManager.selectPulse1);
        viewingItemPos = (createdItem.GetComponent<ShopItem>().shopPosition);
        if(!viewingItem){
            // Debug.Log(chosenItem.GetComponent<Item>().uid);
            viewingItemPanel.SetActive(true);
            growingTime.text = ConvertEpochToTime(chosenItem.GetComponent<ItemDetails>().itemGrowingTime).ToString();
            beauty.text = chosenItem.GetComponent<ItemDetails>().beauty.ToString();
            plantName.text = chosenItem.GetComponent<Item>().itemName;
            bio.text = chosenItem.GetComponent<ItemDetails>().bio;
            plantPreview.GetComponent<Image>().sprite = chosenItem.GetComponent<ItemDetails>().itemPreview;
            purchaseButton.GetComponent<Button>().onClick.RemoveAllListeners();
            purchaseButton.GetComponent<Button>().onClick.AddListener(delegate{StartCoroutine(TryToPurchase(chosenItem));});
            viewingItem = true;
            hiderOverlay.SetActive(true);
        }
    }

    public void CloseItemView(){
        viewingItemPanel.SetActive(false);
        viewingItem = false;
        hiderOverlay.SetActive(false);
    }

    IEnumerator TryToPurchase(Item item){
        int purchasePrice = item.GetComponent<ItemDetails>().value;
        Debug.Log("purhcase price: " + purchasePrice);
        int newTotal = dataManager.player.coins - purchasePrice;
        int itemSlot = new int();
        bool foundSlot = false;
        if(newTotal >= 0){
            for(var i = 0; i < dataManager.player.inventorySize; i++){
                if(dataManager.player.inventory[i] == 0){
                    itemSlot = i;
                    foundSlot = true;
                    break;
                }         
            }

            if(foundSlot){
                yield return StartCoroutine(I_ShowPurchase(purchasePrice, itemSlot, item.itemID));
                
            } else {
                ShowWarning("Inventory Full");  
            }     
        } else {
            ShowWarning("Not Enough Energy Shards");
        }
    }
    IEnumerator I_ShowPurchase(int amount, int emptySlot, int itemID){
        gardenManager.soundPlayer.GetComponent<AudioSource>().PlayOneShot(gardenManager.itemPurchased);
        yield return new WaitForEndOfFrame();
        // Debug.Log("shop pos: " + shopPosition);
        dataManager.player.storedShopItems[viewingItemPos] = 0;
        dataManager.player.inventory[emptySlot] = itemID;
        dataManager.player.coins -= amount;
                Debug.Log(dataManager.player.playerName);
        Debug.Log(dataManager.player.currentIsland);
        dataManager.SaveAll(); 
        LoadShopInventory();
        int countdownAmount = amount;
        // Debug.Log("slot: " + emptySlot);
        // Debug.Log("item id: " + itemUID);
        CloseItemView();
        playerShards.GetComponent<Text>().text = dataManager.player.coins.ToString();
        // while(countdownAmount > 0){
        //     countdownAmount -= 2;
        //     playerShards.GetComponent<Text>().text = dataManager.player.coins.ToString();
        //     yield return new WaitForEndOfFrame();
        // }
        // Debug.Log(shopPosition);
  
        
        Debug.Log(dataManager.player.inventory[emptySlot]);
        
    }
    public string ConvertEpochToTime(int timeToConvert){
        var datetime =  System.DateTimeOffset.FromUnixTimeSeconds(timeToConvert);
        return datetime.ToString("HH\\:mm\\:ss");
    }

    public void ShowWarning(string text){
        Debug.Log("Showing Warning: " + text);
        warningText.GetComponent<Text>().text = text;
        warningText.GetComponent<Animation>().Play("showWarning");
    }
}
