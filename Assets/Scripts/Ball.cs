using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    [SerializeField] private MeshRenderer ballMesh;
    [SerializeField] private TrailRenderer trailRenderer;

    private Rigidbody rb;
    private bool smash, invincible;
    private float currentTime;
    private int currentBrokenStacks, totalStack;

    public GameObject invincibleObj;
    public GameObject fireEffect, winEffect, splashEffect;
    public Image invincibleFill;

    public enum BallState{
        Prepare,
        Playing,
        Died,
        Finish
    }

    [HideInInspector] public BallState ballState = BallState.Prepare;
    public AudioClip bounceOffClip, deadClip, winClip, destroyClip, iDestroyClip;


    private void Awake() {
        rb = GetComponent<Rigidbody>();
        currentBrokenStacks = 0;
    }

    private void Start() {
        totalStack = FindObjectsOfType<StackController>().Length;
        trailRenderer.material.color = ballMesh.material.color;
    }

    private void Update() {
        switch (ballState) {
            case BallState.Prepare:
                // if(Input.GetMouseButtonDown(0)){
                //     ballState = BallState.Playing;
                // }
                    
                break;
            case BallState.Playing:
                if(Input.GetMouseButtonDown(0)) {
                    smash = true;
                }
                if(Input.GetMouseButtonUp(0)) {
                    smash = false;
                }

                if(invincible) {
                    currentTime -= Time.deltaTime * .35f;
                    if(!fireEffect.activeInHierarchy) {
                        fireEffect.SetActive(true);
                    }
                } else{
                    if(fireEffect.activeInHierarchy) {
                        fireEffect.SetActive(false);    
                    }

                    if(smash) {
                        currentTime += Time.deltaTime * .8f;
                    } else{
                        currentTime -= Time.deltaTime * .5f;
                    }
                }

                if(currentTime >= 0.3f || invincibleFill.color == Color.red) {
                    invincibleObj.SetActive(true);
                } else {
                    invincibleObj.SetActive(false);
                }

                if(currentTime >= 1) {
                    currentTime = 1;
                    invincible = true;
                    invincibleFill.color = Color.red;
                } else if(currentTime <= 0){
                    currentTime = 0;
                    invincible = false;
                    invincibleFill.color = Color.white;
                }

                if(invincibleObj.activeInHierarchy) {
                    invincibleFill.fillAmount = currentTime / 1;
                }
                break;

            case BallState.Died:
                break;

            case BallState.Finish:
                if(Input.GetMouseButtonDown(0)) {
                    FindObjectOfType<LeverSpawner>().NextLevel();
                }
                break;
            
        }

        trailRenderer.material.color = ballMesh.material.color;

    }

    private void FixedUpdate() {
        if(ballState == BallState.Playing) {
            if(Input.GetMouseButton(0)) {
                smash = true;
                rb.velocity = new Vector3(0, -100 * Time.fixedDeltaTime * 7, 0);    
            }
        }

        if(rb.velocity.y > 5) {
            rb.velocity = new Vector3(rb.velocity.x, 5, rb.velocity.z);    
        }
    }

    public void IncreaseBrokenStacks () {
        currentBrokenStacks++;

        if(!invincible) {
            ScoreManager.Instance.AddScore(1);
            SoundManager.Instance.PlaySoundFx(destroyClip, 0.5f);
        } else {
            ScoreManager.Instance.AddScore(2);
            SoundManager.Instance.PlaySoundFx(iDestroyClip, 0.5f);
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(!smash) {
            rb.velocity = new Vector3(0, 50 * Time.deltaTime * 5, 0);

            if(other.gameObject.tag != "Finish") {
                GameObject splash = Instantiate(splashEffect);
                splash.transform.SetParent(other.transform);
                splash.transform.localEulerAngles = new Vector3(90, 
                    Random.Range(0, 359), 0);
                float randomScale = Random.Range(0.18f, 0.25f);
                splash.transform.localScale = new Vector3(randomScale, 
                    randomScale, 1);
                splash.transform.position = new Vector3(transform.position.x, 
                    transform.position.y - 0.22f, transform.position.z);
                splash.GetComponent<SpriteRenderer>().color = 
                    transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
            }

            SoundManager.Instance.PlaySoundFx(bounceOffClip, 0.5f);
        } else{
            if(invincible) {
                if(other.gameObject.tag == "enemy" 
                || other.gameObject.tag == "plane") 
                {
                    other.transform.parent.GetComponent<StackController>().ShatterAllPart();
                }
            } else {
                if(other.gameObject.tag == "enemy") 
                {
                    other.transform.parent.GetComponent<StackController>().ShatterAllPart();
                } 
                else if (other.gameObject.tag == "plane")
                {
                    rb.isKinematic = true;
                    transform.GetChild(0).gameObject.SetActive(false);
                    SetBallStateDied();
                    SoundManager.Instance.PlaySoundFx(deadClip, 0.5f);
                }              
            }

        }

        FindObjectOfType<GameUI>().LevelSliderFill(currentBrokenStacks / (float)totalStack);

        if(other.gameObject.tag == "Finish" 
        && ballState == BallState.Playing) 
        {
            ballState = BallState.Finish;
            SoundManager.Instance.PlaySoundFx(winClip, 0.7f);
            GameObject win = Instantiate(winEffect);
            win.transform.SetParent(Camera.main.transform);
            win.transform.localPosition = Vector3.up * 1.5f;
            win.transform.eulerAngles = Vector3.zero;
        }
    }

    private void OnCollisionStay(Collision other) {
        if(!smash || other.gameObject.tag == "Finish") {
            rb.velocity = new Vector3(0, 50 * Time.deltaTime * 5, 0);
        }
    }

    public bool IsGamePrepare(){
        return ballState == BallState.Prepare;
    }

    public bool IsGameDied(){
        return ballState == BallState.Died;
    }

    public bool IsGameFinish(){
        return ballState == BallState.Finish;
    }

    public void SetBallStatePlaying(){
        ballState = BallState.Playing;
    }

    public void SetBallStateDied(){
        ballState = BallState.Died;
    }
}
