using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class WindHoopChallenge : MonoBehaviour
{

    [Header("Tracking")]
    public int level;
    public int coinsEarned;
    public int chain;
    // public float auspLevel;
    public int normalHoops;
    public int perfectHoops;
    public int specialHoops;
    public int discs;
    [Header("Objects")]
    public GameObject tapToStart;
    public GameObject orb;
    public GameObject hoop;
    public GameObject doubleDisc;
    public List<GameObject> obstalces;
    public GameObject destructy;
    public GameObject bgCloudsEmitter;
    // public GameObject auspMeter;
    public GameObject spawnedHolder;
    [Header("UI Stuff")]
    public GameObject levelIndicator;
    public GameObject levelUpText;
    public GameObject meterFill;
    [Header("Statuses")]
    public bool gameActive;
    public bool holding;
    public float heldTime;
    private Vector3 lastDirection;
    public Vector3 orbStartPos;
    public GameObject gameOverScreen;
    [Header("Spawning")]
    public float spawnTime;
    public float spawnCountUp;
    public float obstacleSpawnTime;
    public float obstacleCountUp;
    public float destructyChance;
    [Header("Game Over Screen")]
    public Text normalHoopAmount;
    public Text normalHoopTotal;
    public Text perfectHoopAmount;
    public Text perfectHoopTotal;
    public Text specialHoopAmount;
    public Text specialHoopTotal;
    public Text discAmount;
    public Text discTotal;
    public Text totalEnergy;
    [Header("Other")]
    public Volume mainCamVolume;
    private Bloom camBloom;
    private Vignette camVin;
    private bool levelingUp;
    private bool firstRun;

    void Start(){
        // orbStartPos = orb.transform.localPosition;      
        mainCamVolume.profile.TryGet<Bloom>(out camBloom);
        mainCamVolume.profile.TryGet<Vignette>(out camVin);
        // cameraManager = gameObject.GetComponent<MainManager>().cameraManager;
        // tapToStart.GetComponent<Animation>().Play("startPulse");
    }

    void Update(){
        if(gameActive){   
            // if(auspLevel <= 0 && gameActive){
            //     GameOver();
            // }
            // if(auspMeter.GetComponent<Slider>().value >= auspMeter.GetComponent<Slider>().maxValue && level < 5 && !levelingUp){
            //     NextLevel();
            // }
            spawnCountUp += Time.deltaTime;
            obstacleCountUp += Time.deltaTime;
            // Mathf.Clamp(auspLevel, -10, 100);
            // auspMeter.GetComponent<Slider>().value = auspLevel;
            if(holding){
                heldTime += (Time.deltaTime);
            }
            if(spawnCountUp > spawnTime){
                // SpawnDisc();
                SpawnHoop(hoop);
                SpawnDestructy();
                spawnCountUp = 0;
            }
            if(obstacleCountUp > obstacleSpawnTime){
                obstacleCountUp = 0;
                SpawnObstacle();
            }
            if(Input.GetMouseButtonDown(0)){
                holding = true;
            }
            if(Input.GetMouseButtonUp(0)){
                holding = false;
                DoImpulse();
                heldTime = 0;
            }
        }
    }

    public void Setup(){
        tapToStart.SetActive(true);
        tapToStart.GetComponent<Text>().color = new Color(1,1,1,.8f);
        // Debug.Log(orbStartPos);
        orb.gameObject.SetActive(true);
        orb.GetComponent<Rigidbody>().isKinematic = true;
        tapToStart.SetActive(true);
        spawnTime = 4;
        // camBloom.intensity.value = 5;
        obstacleSpawnTime = 20;
        orb.transform.localPosition = new Vector3(0,0,-110);
        // auspMeter.GetComponent<Slider>().value = 50;
    }


    public void StartGame(){
        StartCoroutine(I_StartGame());
        // camBloom.intensity.value = Mathf.Lerp(0f, 5f, 2f);
    }

    IEnumerator I_StartGame(){
        orb.GetComponent<Rigidbody>().isKinematic = false;
        level = 1;
        discs = 0;
        normalHoops = 0;
        perfectHoops = 0;
        specialHoops = 0;
        // auspLevel = 50;
        tapToStart.GetComponent<Animation>().Play("startFadeOut");
        yield return new WaitForSeconds(2);
        tapToStart.SetActive(false);
        gameActive = true;
        orb.GetComponent<OrbResponse>().ReadyUp();
        bgCloudsEmitter.GetComponent<ParticleSystem>().Play();
        // ChangeColor();
    }


    public void SpawnHoop(GameObject spawnedItem){
        StartCoroutine(I_SpawnHoop(spawnedItem));
        // int vertPos = Random.Range(-420, 550);
        // List<int> leftOrRight = new List<int>(){-800, 800};
        // int side = leftOrRight[Random.Range(0, leftOrRight.Count)];
        // // -112 on Z axis for ball to be centered
        // Vector3 hoopSpawnLoc = new Vector3(side, vertPos, -112);
        // GameObject newHoop = Instantiate(spawnedItem, hoopSpawnLoc, Quaternion.identity);
        // newHoop.transform.SetParent(spawnedHolder.transform, worldPositionStays: false);
        // if(side == -800){
        //     LeanTween.move(newHoop, new Vector3(900, newHoop.transform.position.y, newHoop.transform.position.z), 100);
        // } else {
        //     LeanTween.move(newHoop, new Vector3(-900, newHoop.transform.position.y, newHoop.transform.position.z), 100);
        // }
        
        // float destructySpawn = (Random.Range(0,1001) + destructyChance);
        // Debug.Log("Destructy Chance: " + destructySpawn);
        // if(destructySpawn >= 800){
        //     SpawnHoop(destructy);
        // }
        // float discChance = (Random.Range(0, 1001) + (auspLevel * 10));
        // Debug.Log("disc chance: " + discChance);
        // if(discChance < 500){
        //     SpawnDisc();
        // }
    }

    IEnumerator I_SpawnHoop(GameObject spawnedItem){
        int vertPos = Random.Range(-420, 550);
        List<int> leftOrRight = new List<int>(){-800, 800};
        int side = leftOrRight[Random.Range(0, leftOrRight.Count)];
        // -112 on Z axis for ball to be centered
        Vector3 hoopSpawnLoc = new Vector3(side, vertPos, -112);
        GameObject newHoop = Instantiate(spawnedItem, hoopSpawnLoc, Quaternion.identity);
        newHoop.transform.SetParent(spawnedHolder.transform, worldPositionStays: false);
        if(side == -800){
            LeanTween.move(newHoop, new Vector3(900, newHoop.transform.position.y, newHoop.transform.position.z), 100);
        } else {
            LeanTween.move(newHoop, new Vector3(-900, newHoop.transform.position.y, newHoop.transform.position.z), 100);
        }
        
        // float destructySpawn = (Random.Range(0,1001) + destructyChance);
        // Debug.Log("Destructy Chance: " + destructySpawn);
        // if(destructySpawn >= 800 && !firstRun){
        //     firstRun = true;
        //     int amountToSpawn = Random.Range(1,4);
        //     for(var i = 0; i < amountToSpawn; i++){
        //         SpawnHoop(destructy);
        //     }       
        // }
        firstRun = false;
        yield return null;
    }

    public void SpawnDestructy(){
        float destructySpawn = (Random.Range(0,1001) + destructyChance);
        Debug.Log("Destructy Chance: " + destructySpawn);
        if(destructySpawn < 800){
            return;
        }
        int amountToSpawn = Random.Range(1,4);
        for(var i = 0; i < amountToSpawn; i++){
            int vertPos = Random.Range(-420, 550);
            List<int> leftOrRight = new List<int>(){-800, 800};
            int side = leftOrRight[Random.Range(0, leftOrRight.Count)];
            // -112 on Z axis for ball to be centered
            Vector3 spawnLoc = new Vector3(side, vertPos, -112);
            GameObject newHoop = Instantiate(destructy, spawnLoc, destructy.transform.rotation);
            newHoop.transform.SetParent(spawnedHolder.transform, worldPositionStays: false);
            if(side == -800){
                LeanTween.move(newHoop, new Vector3(900, newHoop.transform.position.y, newHoop.transform.position.z), 100);
            } else {
                LeanTween.move(newHoop, new Vector3(-900, newHoop.transform.position.y, newHoop.transform.position.z), 100);
            }
        }
    }
    public void SpawnDisc(){
        Debug.Log("spawning disc");
        int horzPos = Random.Range(-350, 350);
        Vector3 discSpawnLoc = new Vector3(horzPos, 700, 0);
        GameObject newDisc = Instantiate(doubleDisc, discSpawnLoc, Quaternion.identity);
        newDisc.transform.SetParent(spawnedHolder.transform, worldPositionStays: false);
        LeanTween.move(newDisc, new Vector3(newDisc.transform.position.x, -1100, newDisc.transform.position.z), 200);
    }

    public void SpawnObstacle(){
        Debug.Log("spawning obstacle");
        Vector3 obstacleSpawnLoc = new Vector3(0, 0, 0);
        GameObject obsToSpawn = obstalces[Random.Range(0, obstalces.Count-1)];
        Debug.Log(obsToSpawn.name);
        if(obsToSpawn.name.Contains("_L")){
            obstacleSpawnLoc = new Vector3(-250, 2000, -92);
        } else if (obsToSpawn.name.Contains("_R")){
            obstacleSpawnLoc = new Vector3 (250, 2000, -92);
        }
        GameObject newObstacle = Instantiate(obsToSpawn, obstacleSpawnLoc, obsToSpawn.transform.rotation);
        newObstacle.transform.SetParent(spawnedHolder.transform, worldPositionStays: false);
        LeanTween.move(newObstacle, new Vector3(newObstacle.transform.position.x, -2200, newObstacle.transform.position.z), 600);
    }

    public void DetermineClickDirection(){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << LayerMask.NameToLayer("Colliders");
            if(Physics.Raycast(ray, out hit, 1000f, ~layerMask)){
                Vector3 clickPos = (hit.point);
                // Debug.Log("Click Pos" + clickPos);
                // Debug.Log("Orb Pos:" + orb.transform.position);
                Vector3 direction = (orb.transform.position - clickPos);
                lastDirection = direction;
            }
    }
    public void DoImpulse(){
        float impulseAmount = Mathf.Clamp((heldTime * 225), 50, 600);
        // Debug.Log("impulse: " + impulseAmount);
        // Debug.Log("hold time: " + heldTime);
        DetermineClickDirection();
        orb.GetComponent<Rigidbody>().AddForce(lastDirection * Mathf.Clamp(impulseAmount, 20, 100), ForceMode.Acceleration);
    }

    // public void NextLevel(){
    //     StartCoroutine(I_NextLevel());
    //     levelingUp = true;
    // }

    // IEnumerator I_NextLevel(){
    //     levelUpText.SetActive(true);
    //     level += 1;
    //     ChangeColor();
    //     levelIndicator.GetComponent<Text>().text= level.ToString();
    //     levelUpText.GetComponent<Animation>().Play("displayLevelUp");
    //     yield return new WaitForSeconds(1f);
    //     levelUpText.SetActive(false);
    //     auspMeter.GetComponent<Slider>().maxValue = level * 100;
    //     auspLevel = (level * 100)/2;
    //     levelingUp = false;
    // }

    // private void ChangeColor(){
    //     if(level == 1){
    //         meterFill.GetComponent<Image>().color = new Color(.757f, .871f, .878f);
    //     } else if(level == 2){
    //         meterFill.GetComponent<Image>().color = new Color(.478f, .784f, .455f);
    //     } else if (level == 3){
    //         meterFill.GetComponent<Image>().color = new Color(.141f, .212f, .945f);
    //     } else if (level == 4){
    //         meterFill.GetComponent<Image>().color = new Color(.678f, .043f, .812f);
    //     } else if (level == 5){
    //         meterFill.GetComponent<Image>().color = new Color(.89f, .31f, 0);
    //     }
    // }
    // public void ChangeAusp(float changeNeeded){
    //     StartCoroutine(I_ChangeAusp(changeNeeded));
    // }
    // IEnumerator I_ChangeAusp(float changeNeeded){
    //     float reqAusp = auspLevel + (changeNeeded);
    //     // if(reqAusp >= (auspMeter.GetComponent<Slider>().maxValue * .5f)){
    //     //     camBloom.intensity.value = Mathf.Lerp(auspLevel/10, reqAusp/10, 1);
    //     // } else {
    //     //     camBloom.intensity.value = Mathf.Lerp(auspLevel/10, 0, 1f);
    //     // }
        
    //     if(changeNeeded < 0){
    //         changeNeeded = (changeNeeded * level);
    //         while(auspLevel > reqAusp){
    //             if(auspLevel <= (auspMeter.GetComponent<Slider>().maxValue * .4f)){
    //                 camVin.intensity.value += .002f;
    //                 camVin.intensity.value = Mathf.Clamp(camVin.intensity.value, 0, .35f);
    //             }
    //             auspLevel = auspLevel -= (.1f * level);
    //             yield return new WaitForEndOfFrame();
    //         }
    //     } else {
    //         while(auspLevel < reqAusp){
    //             if(auspLevel >= (auspMeter.GetComponent<Slider>().maxValue * .4f)){
    //                 camVin.intensity.value -= .002f;
    //                 camVin.intensity.value = Mathf.Clamp(camVin.intensity.value, 0, .35f);
    //             }
    //             auspLevel = auspLevel += (.1f * level);
    //             yield return new WaitForEndOfFrame();
    //         }
    //     }
    // }

    public void ReturnToMainScreen(){
        orb.transform.position = new Vector3(0,0,-110);
        int normalScore = normalHoops * 3;
        int perfectScore = perfectHoops * 5;
        int specialHoopScore = specialHoops * 50;
        int discScore = discs * 5;
        int totalAll = (normalScore + perfectScore + specialHoopScore + discScore);
        gameObject.GetComponent<MainManager>().dataManager.player.coins += totalAll;
        gameObject.GetComponent<MainManager>().dataManager.SaveAll();
        if(gameObject.GetComponent<MainManager>().dataManager.player.moodRecord.bonusActive){
            GetComponent<MainManager>().showBonus = true;
            GetComponent<MainManager>().bonusNeeded = (int)totalAll/2;
        }
        gameObject.GetComponent<MainManager>().LoadMainScreen();
        gameOverScreen.GetComponent<Animation>().Play("GO_reset");
    }

    public void GameOver(){
        orb.GetComponent<Rigidbody>().useGravity = false;
        camBloom.intensity.value = 0;
        camVin.intensity.value = 0;
        gameActive = false;
        orb.gameObject.SetActive(false);
        foreach(Transform item in spawnedHolder.transform){
            Destroy(item.gameObject);
        }
        StartCoroutine(I_GameOver());
    }

    IEnumerator I_GameOver(){
        int normalScore = normalHoops * 3;
        int perfectScore = perfectHoops * 5;
        int specialHoopScore = specialHoops * 50;
        int discScore = discs * 5;
        
        int totalAll = (normalScore + perfectScore + specialHoopScore + discScore);
        int counter = 0;
        gameOverScreen.GetComponent<Animation>().Play("GO_showScreen");
        yield return new WaitForSeconds(2);

        gameOverScreen.GetComponent<Animation>().Play("GO_showNormal");
        yield return new WaitForSeconds(.75f);
        normalHoopAmount.text = normalHoops.ToString();
        yield return new WaitForSeconds(.25f);
        while(counter != normalScore){
            counter += 1;
            normalHoopTotal.text = counter.ToString();
            yield return new WaitForEndOfFrame();
        }
        counter = 0;

        gameOverScreen.GetComponent<Animation>().Play("GO_showPerfect");
        yield return new WaitForSeconds(.75f);
        perfectHoopAmount.text = perfectHoops.ToString();
        yield return new WaitForSeconds(.25f);
        while(counter != perfectScore){
            counter += 1;
            perfectHoopTotal.text = counter.ToString();
            yield return new WaitForEndOfFrame();
        }
        counter = 0;

        gameOverScreen.GetComponent<Animation>().Play("GO_showSpecial");
        yield return new WaitForSeconds(.75f);
        specialHoopAmount.text = specialHoops.ToString();
        yield return new WaitForSeconds(.25f);
        while(counter != specialHoopScore){
            counter += 1;
            specialHoopTotal.text = counter.ToString();
            yield return new WaitForEndOfFrame();
        }
        counter = 0;

        gameOverScreen.GetComponent<Animation>().Play("GO_showDisc");
        yield return new WaitForSeconds(.75f);
        discAmount.text = discs.ToString();
        yield return new WaitForSeconds(.25f);
        while(counter != discScore){
            counter += 1;
            discAmount.text = counter.ToString();
            yield return new WaitForEndOfFrame();
        }
        counter = 0;

        gameOverScreen.GetComponent<Animation>().Play("GO_showFinal");
        yield return new WaitForSeconds(1.25f);
        while(counter != totalAll){
            counter += 1;
            totalEnergy.text = counter.ToString();
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);
        gameOverScreen.GetComponent<Animation>().Play("GO_showMenuButton");
    }
}
