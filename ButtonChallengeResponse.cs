using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonChallengeResponse : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject buttonType;
    public GameObject cover;
    public GameObject holdPulse;
    public GameObject tapBurst;
    public GameObject holdBurst;
    public GameObject dragBurst;
    public GameObject chainBurst;
    public GameObject finishBurst;
    public AudioClip destroyedSound;
    public TMPro.TextMeshProUGUI number;
    public bool isTap;
    public bool isDrag;
    public enum DragDirection{Up, Down, Left, Right};
    public DragDirection dragDirectionNeeded;
    public DragDirection dragDirectionGiven;
    public bool isHold;
    public bool holding;
    public int holdTimeNeeded;
    public float holdTimeGiven;
    public bool isChained;
    public int chainNumber;
    public bool isNothing;
    public int buttonPos;
    private Vector3 startDrag;
    private Vector3 endDrag;
    private bool pulsePlaying;
    void Start(){
        gameManager = GameObject.Find("GameManager");
        // gameObject.GetComponent<Animation>().Play("buttonHover");
    }
    void Update(){
        if(holding){
            holdTimeGiven += Time.deltaTime;
            holdPulse.GetComponent<Animation>().Play("memoryHoldPulse");
            if(!pulsePlaying){
                StartCoroutine(PlayPulseSound());
                pulsePlaying = true;
            }
        }
    }

    public void RemoveButton(){
        gameObject.GetComponent<AudioSource>().PlayOneShot(destroyedSound);
        gameObject.GetComponent<Image>().color = new Color(0,0,0,0);
        gameObject.GetComponent<Button>().interactable = false;
        buttonType.GetComponent<Animation>().Play("buttonImplode");
        cover.GetComponent<Animation>().Play("coverImplode");
        // gameObject.transform.Find("Direction").gameObject.SetActive(false);
        // buttonType.SetActive(false);
        number.gameObject.SetActive(false);
        gameManager.GetComponent<ButtonChallenge>().buttonsToPress -= 1;
        if(gameManager.GetComponent<ButtonChallenge>().buttonsToPress == 0){
            NextLevelTrigger();
        }
    }

    void OnMouseDown(){
        if(gameObject.GetComponent<Button>().interactable == false){
            return;
        }
        if(gameObject.GetComponent<Button>().interactable){
            if(isNothing){
                Debug.Log("Game Over");
                GameOver();
                return;
            }
            else if(isTap){
                if(gameManager.GetComponent<ButtonChallenge>().chainStarted){
                    GameOver();
                }
                // Debug.Log("Destroyed");
                RemoveButton();
                tapBurst.GetComponent<ParticleSystem>().Play();
            }
            else if(isDrag){
                if(gameManager.GetComponent<ButtonChallenge>().chainStarted){
                    GameOver();
                }
                startDrag = Input.mousePosition;
            }
            else if(isHold){
                if(gameManager.GetComponent<ButtonChallenge>().chainStarted){
                    GameOver();
                }
                holdTimeGiven = 0;
                holding = true;
                
            }
            else if(isChained){
                gameManager.GetComponent<ButtonChallenge>().chainStarted = true;
                if(gameManager.GetComponent<ButtonChallenge>().nextChainButtonNeeded == chainNumber){
                    // Debug.Log("Gucci");
                    RemoveButton();
                    chainBurst.GetComponent<ParticleSystem>().Play();
                    gameManager.GetComponent<ButtonChallenge>().nextChainButtonNeeded += 1;
                    if(gameManager.GetComponent<ButtonChallenge>().nextChainButtonNeeded > gameManager.GetComponent<ButtonChallenge>().chainButtons){
                        gameManager.GetComponent<ButtonChallenge>().chainStarted = false;
                    }
                } else {
                    GameOver();
                }  
            }
        } else if(gameObject.GetComponent<Button>().interactable && gameManager.GetComponent<ButtonChallenge>().chainStarted){
            if(!isChained){
                GameOver();
            }
        }

    }
    void OnMouseUp(){
        holding = false;
        if(isDrag && gameObject.GetComponent<Button>().interactable){
            endDrag = Input.mousePosition;
            CalculateDirection();  
        }
        if(isHold && gameObject.GetComponent<Button>().interactable){
            
            // holdPulse.GetComponent<Animation>().Stop("memoryHoldPulse");
            // holdPulse.SetActive(false);
            if(Mathf.Abs(holdTimeGiven - holdTimeNeeded) < .5f || Mathf.Abs(holdTimeNeeded - holdTimeGiven) < .5f){
                RemoveButton();
                holdBurst.GetComponent<ParticleSystem>().Play();
            } else {
                // Debug.Log(Mathf.Abs(holdTimeGiven - holdTimeNeeded));
                GameOver();
            }
        }
    }

    IEnumerator PlayPulseSound(){
        gameObject.GetComponent<AudioSource>().PlayOneShot(gameManager.GetComponent<ButtonChallenge>().pulseSound);
        yield return new WaitForSeconds(1);
        pulsePlaying = false;
    }
    public void CalculateDirection(){
        Vector3 finalVec = (startDrag - endDrag);
        if(Mathf.Abs(finalVec.x) < 10 && Mathf.Abs(finalVec.y) < 10){
            return;
        }
        if(Mathf.Abs(finalVec.x) > Mathf.Abs(finalVec.y)){
            if(finalVec.x > 1){
                // Debug.Log("Left Drag");
                dragDirectionGiven = DragDirection.Left;
            } else {
                // Debug.Log("Right Drag");
                dragDirectionGiven = DragDirection.Right;
            }
        } else {
            if(finalVec.y > 1){
                // Debug.Log("Down Drag");
                dragDirectionGiven = DragDirection.Down;
            } else {
                // Debug.Log("Up Drag");
                dragDirectionGiven = DragDirection.Up;
            }
        }
        GiveDragResult();
    }

    public void GiveDragResult(){
        if(dragDirectionNeeded == dragDirectionGiven){
            // Debug.Log("Gucci");
            RemoveButton();
            dragBurst.GetComponent<ParticleSystem>().Play();
        } else {
            // Debug.Log("Anti-Gucci");
            GameOver();
        }
    }
    public void SetupButton(){
        gameObject.GetComponent<BoxCollider2D>().size = transform.parent.gameObject.GetComponent<GridLayoutGroup>().cellSize;
    }

    public void GameOver(){
        gameManager.GetComponent<ButtonChallenge>().GameOver();
    }
    public void NextLevelTrigger(){
        gameManager.GetComponent<ButtonChallenge>().NextLevelScreen();
    }
}
