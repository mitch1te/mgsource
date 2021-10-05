using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraManager : MonoBehaviour
{
    public GameObject gardenOverlay;
    public MainManager mainManager;
    public Camera mainCam;
    public Camera gardenCam;
    public GameObject gardenHolder;
    public GardenManager gardenManager;
    private Vector3 originalPos;
    private Quaternion originalRot;
 
    [Header("Camera Controls")]
    public GameObject gardenCamHolder;
    public bool gardenActive;
    public Camera cam;
    float MouseZoomSpeed = 15.0f;
    float TouchZoomSpeed = 0.1f;
    float ZoomMinBound = 1.6f;
    float ZoomMaxBound = 50f;
    public float speed = 400f;
    private bool dragging;
    private Vector3 initPos;
    private float rotX;
    private float rotY;
    private float dir;
    private Vector3 savedPos;
    private bool posSaved;
    private bool canMove;
    private Vector3 measureFrom;
    public Vector3 savedCamRot;
    public Vector3 savedCamPos;
    public GameObject readout;
    public string readText;


    // stuff
    private bool needNewCenter;
    private bool needReset;
    public bool zooming;
    public bool rotating;
    public bool moving;
    private Vector2 startVec;
    public Single angleOffest;
    private float rotGestureWidth;
    private float rotAngleMinimum = 5;
    private Vector3 rotateAroundThis;
    public float moveSpeed;
    public Volume v;
    private DepthOfField dof;
    public int touchCount;

    void Start(){
        originalPos = gardenCam.transform.position;
        originalRot = gardenCam.transform.rotation;
        cam = gardenCam;
        dir = -1;
        v.profile.TryGet<DepthOfField>(out dof);

        try{
            Input.gyro.enabled = true;
        } catch (Exception e) {
            Debug.Log(e);
        }
        // cam.transform.LookAt(gardenManager.currentIsland.transform);
    }

    void Update(){
        touchCount = Input.touchCount;
        if(gardenActive){
            // keyboard controls
            if(Input.GetKey(KeyCode.LeftShift)){
                ZoomIn();
            }
            if(Input.GetKey(KeyCode.LeftControl)){
                ZoomOut();
            }

   
            
            if(Input.gyro.enabled){
                SetView();
            }
            

            // touch controls
            if(Input.touchCount == 1 && !zooming && !rotating && !gardenManager.planting &&!gardenManager.windowOpen){
                
                // RaycastHit hit;
                // if(Physics.Raycast(cam.transform.position, -Vector3.up, out hit)){
                    if(gardenCamHolder.transform.position.y <= -475){
                        moveSpeed = 1;
                    } else if(gardenCamHolder.transform.position.y > -475 && gardenCamHolder.transform.position.y < -415){
                        moveSpeed = 4;
                    } else if (gardenCamHolder.transform.position.y > -415 && gardenCamHolder.transform.position.y <= 0){
                        moveSpeed = 6;
                    }
                // }
                //     Debug.Log("Hit dist: " + hit.distance);
                //     if(hit.distance < 30){
                //         moveSpeed = 1;
                //     } else if(hit.distance >= 30 && hit.distance < 400){
                //         moveSpeed = 4;
                //     } else {
                //         moveSpeed = 6;
                //     }
                // } else {
                //     moveSpeed = 12;
                // }
                Touch t = Input.GetTouch(0);
                // Debug.Log(t.deltaPosition);
                if(t.deltaPosition.x > 1 && !gardenManager.adjustingItem){
                    moving = true;
                    MoveCamRight();
                }
                if(t.deltaPosition.x < -1 && !gardenManager.adjustingItem){
                    moving = true;
                    MoveCamLeft();
                }
                if (t.deltaPosition.y > 1 && !gardenManager.adjustingItem){
                    moving = true;
                    MoveCamForward();
                } 
                if(t.deltaPosition.y < -1 && !gardenManager.adjustingItem){
                    moving = true;
                    MoveCamBack();
                }
              
            }


            if(Input.touchCount > 1){
                // Debug.Log(angleOffest);
                Touch tZero = Input.GetTouch(0);
                Touch tOne = Input.GetTouch(1);

                Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
                Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

                float oldTouchDistance = Vector2.Distance (tZeroPrevious, tOnePrevious);
                float currentTouchDistance = Vector2.Distance (tZero.position, tOne.position);

                float difference = currentTouchDistance - oldTouchDistance;
                
                if(!rotating){
                    startVec = tOne.position - tZero.position;
                    rotating = startVec.sqrMagnitude > rotGestureWidth * rotGestureWidth;
                } else {
                    var currVector = tOne.position - tZero.position;
                    angleOffest = Vector2.Angle(startVec, currVector);
                    var LR = Vector3.Cross(startVec, currVector);
    
                    if(angleOffest > rotAngleMinimum){
                        rotating = true;
         
                        if(LR.z > 0){
                            // Debug.Log("counterclockwise");
                            gardenCamHolder.transform.RotateAround(rotateAroundThis, Vector3.up, 75 * Time.deltaTime);
                        } else if (LR.z < 00){
                            // Debug.Log("Clockwise rot");
                            gardenCamHolder.transform.RotateAround(rotateAroundThis, Vector3.up, -75 * Time.deltaTime);
                        }
                    }
                }

                if(!needNewCenter){
                    needNewCenter = true;
                    var centerPos = (tZero.position + tOne.position) /2;
                    RaycastHit hit;
                    Ray ray = cam.ScreenPointToRay(centerPos);
                    if(Physics.Raycast(ray, out hit, 2000f)){
                        rotateAroundThis = hit.point;
                    }
                }
                Vector3 direction = cam.transform.position - measureFrom;
                // Debug.Log(difference);
                if(difference > 8){
                    if(!cam.GetComponent<CameraCollider>().colliding){
                        zooming = true;
                        ZoomIn();
                    }  
                } else if(difference < -8){
                    zooming = true;
                    ZoomOut();
                } 

            } else {
                if(Input.touchCount < 2){
                    if(moving || rotating || zooming){
                        StartCoroutine(ResetBools());
                    }        
                }
            }
        // --------------END GARDEN ACTIVE--------
        }
      
    }

    IEnumerator ResetBools(){
        // Debug.Log("resetting");
        yield return new WaitForSeconds(.1f);
        needNewCenter = false;
        rotating = false;
        zooming = false;
        moving = false;
    }
    public void SetView(){
        
    }
    public void ZoomIn(){

        if(Vector3.Distance(gardenManager.currentIsland.transform.position, cam.transform.position) > 20){
            gardenCamHolder.transform.Translate(Vector3.forward * speed);
            gardenCamHolder.transform.Translate(-Vector3.up * speed);
        }
        AdjustDOF();
     }

    public void ZoomOut(){

        if(Vector3.Distance(gardenManager.currentIsland.transform.position, cam.transform.position) < 1000){
            gardenCamHolder.transform.Translate(-Vector3.forward * speed);
            gardenCamHolder.transform.Translate(Vector3.up * speed);
            
        }
        AdjustDOF();
    }

    void AdjustDOF(){
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, -Vector3.up, out hit)){
            // Debug.Log(hit.distance);
            if(hit.distance < 50){
                // moveSpeed = 1;
                dof.gaussianStart.value = LeanTween.linear((float)dof.gaussianStart.value, 50, 2);
                dof.gaussianEnd.value = LeanTween.linear((float) dof.gaussianEnd.value, 500, 2);
            } else if(hit.distance >= 51 && hit.distance < 220){
                // moveSpeed = 3;
                dof.gaussianStart.value = LeanTween.linear((float)dof.gaussianStart.value, 300, 2);
                dof.gaussianEnd.value = LeanTween.linear((float) dof.gaussianEnd.value, 750, 2);
            } else {
                // moveSpeed = 6;
                dof.gaussianStart.value = LeanTween.linear((float)dof.gaussianStart.value, 1000, 2);
                dof.gaussianEnd.value = LeanTween.linear((float) dof.gaussianEnd.value, 3000, 2);
            }
        } else {
            // moveSpeed = 12;
            dof.gaussianStart.value = LeanTween.linear((float)dof.gaussianStart.value, 1000, 2);
            dof.gaussianEnd.value = LeanTween.linear((float) dof.gaussianEnd.value, 3000, 2);
        }
    }
    void MoveCamForward(){
        // gardenCamHolder.transform.position = Vector3.Lerp(gardenCamHolder.transform.position, gardenCamHolder.transform.position + -Vector3.forward, 10000);
        gardenCamHolder.transform.Translate(-Vector3.forward * moveSpeed);
    }
    void MoveCamBack(){
        // gardenCamHolder.transform.position = Vector3.Lerp(gardenCamHolder.transform.position, gardenCamHolder.transform.position + Vector3.forward, 10000);
        gardenCamHolder.transform.Translate(Vector3.forward * moveSpeed);
    }
    void MoveCamLeft(){
        // gardenCamHolder.transform.position = Vector3.Lerp(gardenCamHolder.transform.position, gardenCamHolder.transform.position + Vector3.right, 10000);
        gardenCamHolder.transform.Translate(Vector3.right * moveSpeed);
    }
    void MoveCamRight(){
        // gardenCamHolder.transform.position = Vector3.Lerp(gardenCamHolder.transform.position, gardenCamHolder.transform.position + -Vector3.right, 10000);
        gardenCamHolder.transform.Translate(-Vector3.right * moveSpeed);
    }
    public void ResetObjectCam(){
        LeanTween.move(cam.gameObject, savedCamPos, 1f);
        LeanTween.rotate(cam.gameObject, savedCamRot, 1f);
    }
    public void SwitchCamera(){
        StartCoroutine(I_SwitchCamera());
    }

    IEnumerator I_SwitchCamera(){
        if(mainCam.gameObject.activeSelf){
            gardenOverlay.SetActive(true);
            Animation anims = gardenOverlay.GetComponent<Animation>();
            yield return anims.Play("overlayFadeOut");
            mainManager.mainPagePanel.SetActive(false);
            gardenHolder.SetActive(true);
            gardenCam.gameObject.SetActive(true);
            mainCam.gameObject.SetActive(false);
            gardenManager.LoadAllExisting();
            StartCoroutine(StartFade(mainManager.GetComponent<AudioSource>(), 2f, 0));
            // StartCoroutine(StartFade(gardenManager.wind, 2f, .4f));
            cam = gardenCam;
            gardenActive = true;
            gardenManager.backToMainButton.GetComponent<Button>().interactable = true;
            if(mainManager.dataManager.player.needIslandTutorial){
                gardenManager.backToMainButton.SetActive(false);
                gardenManager.otherBlocked = true;
                yield return new WaitForSeconds(1.5f);
                gardenCamHolder.GetComponent<Animation>().Play("introCamMove1");
                yield return new WaitForSeconds(30);               
                mainManager.GetComponent<TutorialManager>().LoadIslandTutorial();
                
            } else {
                gardenManager.backToMainButton.SetActive(true);
            }
        } else {
            gardenActive = false;
            mainManager.soundPlayer.GetComponent<AudioSource>().PlayOneShot(mainManager.confirmChime1);
            gardenOverlay.SetActive(true);
            Animation anims = gardenOverlay.GetComponent<Animation>();
            anims.PlayQueued("overlayFinalFade");
            yield return StartCoroutine(WaitForAnimation(anims));
            mainCam.gameObject.SetActive(true);
            gardenCam.gameObject.SetActive(false);
            StartCoroutine(StartFade(mainManager.GetComponent<AudioSource>(), 2f, .08f));
            gardenHolder.SetActive(false);
            mainManager.LoadMainScreen();
        }
    }


    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    public void WholeGardenView(){
        gardenManager.zoomOutButton.SetActive(false);
        gardenManager.backToMainButton.SetActive(true);
        LeanTween.move(gardenCam.gameObject, originalPos, 1f);
        LeanTween.rotate(gardenCam.gameObject, originalRot.eulerAngles, 1f);
        // gardenManager.selectedPot = null;
        Destroy(gardenManager.plantToPlant);
        // Destroy(gardenManager.potToPlace);
        gardenManager.planting = false;
    }

    private IEnumerator WaitForAnimation ( Animation animation ){
        do { yield return null; } while ( animation.isPlaying );
    }



}
