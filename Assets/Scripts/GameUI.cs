using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject homeUI, inGameUI, 
    allBtn, finishUI, gameOverUI;

    private bool btn;

    [Header("Pre Game")]
    [SerializeField] private Button soundBtn;
    [SerializeField] private Sprite soundOnS, soundOffS;

    [Header("In Game")]
    public Image levelSlider;
    public Image currentLevelImg;
    public Image nextLevelImg;
    public Text currentLevelText, nextLevelText;  

    [Header("Finish")]
    [SerializeField] private Text finishLevelText;

    [Header("GameOver")]
    [SerializeField] private Text gameOverScoreText;
    [SerializeField] private Text gameOverBestScoreText;

    // private Material ballMat;
    private LeverSpawner leverSpawner;
    private Ball ball;

    private void Awake() {
        // ballMat = FindObjectOfType<Ball>().transform.GetChild(0).GetComponent<MeshRenderer>().material;
        leverSpawner = FindObjectOfType<LeverSpawner>();
        ball = FindObjectOfType<Ball>();

        levelSlider.transform.parent.GetComponent<Image>().color = leverSpawner.plateMat.color + Color.gray;
        // levelSlider.color = ballMat.color;
        // currentLevelImg.color = ballMat.color;
        // nextLevelImg.color = ballMat.color;
        
        levelSlider.color = leverSpawner.plateMat.color;
        currentLevelImg.color = leverSpawner.plateMat.color;
        nextLevelImg.color = leverSpawner.plateMat.color;

        soundBtn.onClick.AddListener(() => {
            SoundManager.Instance.SoundOnOff();
        });
    }

    private void Start() {
        currentLevelText.text = FindObjectOfType<LeverSpawner>().level.ToString();
        nextLevelText.text = FindObjectOfType<LeverSpawner>().level + 1 + "";
    }

    private void Update() {
        if(ball.IsGamePrepare()) {
            if(SoundManager.Instance.sound 
            && soundBtn.GetComponent<Image>().sprite != soundOnS) 
            {
                soundBtn.GetComponent<Image>().sprite = soundOnS;
            } 
            else if(!SoundManager.Instance.sound 
            && soundBtn.GetComponent<Image>().sprite != soundOffS) 
            {
                soundBtn.GetComponent<Image>().sprite = soundOffS;
            }
        }

        if(Input.GetMouseButtonDown(0)
        && !IgnoreUI()
        && ball.IsGamePrepare()) 
        {
            ball.SetBallStatePlaying();
            homeUI.SetActive(false);
            inGameUI.SetActive(true);
            finishUI.SetActive(false);
            gameOverUI.SetActive(false);
        }

        if(ball.IsGameFinish()) {
            homeUI.SetActive(false);
            inGameUI.SetActive(false);
            finishUI.SetActive(true);
            gameOverUI.SetActive(false);

            finishLevelText.text = "Level " + FindObjectOfType<LeverSpawner>().level;
        }

        if(ball.IsGameDied()) {
            homeUI.SetActive(false);
            inGameUI.SetActive(false);
            finishUI.SetActive(false);
            gameOverUI.SetActive(true);

            gameOverScoreText.text = ScoreManager.Instance.score.ToString();
            gameOverBestScoreText.text = PlayerPrefs.GetInt("HighScore").ToString();

            if(Input.GetMouseButtonDown(0)) {
                ScoreManager.Instance.ResetScore();
                SceneManager.LoadScene(0);
            }
        }
    }

    private bool IgnoreUI () {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultsList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);    

        for (int i = 0; i < raycastResultsList.Count; i++){
            if(raycastResultsList[i].gameObject.GetComponent<Ignore>() != null) {
                raycastResultsList.RemoveAt(i);
                i--;
            }
        }

        return raycastResultsList.Count > 0;
    }

    public void LevelSliderFill (float fillAmount) {
        levelSlider.fillAmount = fillAmount;
    }

    public void Setting(){
        btn = !btn;
        allBtn.SetActive(btn);
    }
}
