using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionChallenge : MonoBehaviour
{
    public List<GameObject> holes;
    public List<GameObject> chainedSpirits;
    public GameObject ground;
    public int chainLength;
    public GameObject chainLine;
    public GameObject badSpirit;
    public bool gameStarted;
    public int chainPos;
    

    //TODO: Add bad things so you dont click. Add chain bonuses. Think of more
    void Start(){
        // Setup();
    }
    public void Setup(){
        gameStarted = true;
        StartCoroutine(SpiritSpawner());
    }
    IEnumerator SpiritSpawner(){
        while(gameStarted){
            yield return new WaitForSeconds(Random.Range(1, 3));
            if(holes.Count > 0 && gameStarted){          
                SpawnSpirit();
            } 
        }        
    }
    public void SpawnSpirit(){
        GameObject chosenHole = holes[Random.Range(0, holes.Count)];
        holes.Remove(chosenHole);
        var spawnedSpirit = Instantiate(badSpirit);
        spawnedSpirit.transform.SetParent(chosenHole.transform, worldPositionStays: false);
        spawnedSpirit.transform.localPosition = new Vector3(0, -1, 0);
    }

    public void EndGame(){
        gameStarted = false;
        StartCoroutine(ProcessEndGame());
    }
    IEnumerator ProcessEndGame(){
        foreach(Transform hole in ground.transform){
            foreach(Transform child in hole){
                Destroy(child.gameObject);
            }
        }
        chainLine.GetComponent<LineRenderer>().positionCount = 0;
        chainLength = 0;
        chainedSpirits.Clear();
        yield return new WaitForSeconds(.25f);
        gameObject.GetComponent<MainManager>().LoadMainScreen();
    }
}
