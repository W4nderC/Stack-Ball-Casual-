using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private Text scoreText;
    public int score;

    private void Awake() {
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        MakeSingleton();
    }

    private void Start() {
        AddScore(0);
        // scoreText.text = score.ToString();
    }

    private void Update() {
        if(scoreText == null) {
            scoreText = GameObject.Find ("ScoreText").GetComponent<Text>();
            scoreText.text = score.ToString();
        }
    }

    private void MakeSingleton () {
        if(Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void AddScore(int amount) {
        score += amount;
        if(score > PlayerPrefs.GetInt("HighScore", 0)) {
            PlayerPrefs.SetInt("HighScore", score);
        }
        scoreText.text = score.ToString();
    }

    public void ResetScore () {
        score = 0;
    }
}
