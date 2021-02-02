using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public static int _apples;
    public static int _record;
    private int _score;

    [Header("Интерфейс")]

    [Tooltip("Префаб UI ножа")]
    [SerializeField]
    private GameObject UI_Knife;
    [Tooltip("Пул ножей")]
    [SerializeField]
    private Transform UI_KnifesPool;
    [Tooltip("Количество яблок")]
    [SerializeField]
    private Text UI_ApplesValue;
    [Tooltip("Текст счета")]
    [SerializeField]
    private Text UI_Score;
    [Tooltip("Текст ошибки")]
    [SerializeField]
    private GameObject FailMessage;
    [Tooltip("Область нажатия")]
    [SerializeField]
    private GameObject HitArea;


    [Header("Окна")]

    [Tooltip("Окно победы")]
    [SerializeField]
    private GameObject VictoryWindow;
    [Tooltip("Окно поражения")]
    [SerializeField]
    private GameObject LoseWindow;
    [Tooltip("Окно паузы")]
    [SerializeField]
    private GameObject PauseWindow;


    [Header("Настройки яблока")]

    [Tooltip("Префаб яблока")]
    [SerializeField]
    private GameObject Apple;
    [Tooltip("Файл настроек яблока")]
    [SerializeField]
    private Apples AppleSettings;

    [Header("Настройки")]

    [Tooltip("Префаб ножа")]
    [SerializeField]
    private GameObject Knife;
    [Tooltip("Спрайты ножей")]
    public Sprite[] KnifeSprites;
    [Tooltip("Количество ножей")]
    [Range(4, 7)]
    public int KnifesCount = 5;
    [Tooltip("Задержка перед появлением окна проигрыша/победы")]
    [SerializeField]
    [Range(2f,4f)]
    private float GameOverDelay = 3;


    public static bool gameOver;

    private bool showingFailMsg;
    private Slice slice;

    private void Start()
    {
        _record = PlayerPrefs.GetInt("Record");
        _score = PlayerPrefs.GetInt("Score");
        _apples = PlayerPrefs.GetInt("Apples");
        UpdateValues();

        int rnd = Random.Range(4, 8);
        KnifesCount = rnd;

        slice = GameObject.FindGameObjectWithTag("Slice").GetComponent<Slice>();
        Time.timeScale = 1;

        rnd = Random.Range(0, 101);
        if(rnd <= AppleSettings.Chance)
            Instantiate(Apple, slice.transform);

        SpawnKnifes();
        CreateKnifesCounter();
        gameOver = false;
    }

    private void SpawnKnifes()
    {
        var spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        int rnd = Random.Range(1, 4);
        for (int i = 0; i <= rnd; i++)
        {
            int random = Random.Range(0, spawnPoints.Length);
            if (spawnPoints[random].transform.childCount == 0)
                Instantiate(Knife, spawnPoints[random].transform);
            else
            {
                while (spawnPoints[random].transform.childCount != 0)
                    random = Random.Range(0, spawnPoints.Length);
                Instantiate(Knife, spawnPoints[random].transform);
            }

            var spawnedKnife = spawnPoints[random].transform.GetChild(0);
            spawnedKnife.GetComponent<Animator>().enabled = false;
            spawnedKnife.GetComponent<SpriteRenderer>().sprite = KnifeSprites[PlayerPrefs.GetInt("EquipedKnife")];
            spawnedKnife.GetComponent<Knife>().knifeIsHitted = true;
            spawnedKnife.GetComponent<Knife>().enabled = false;
            spawnedKnife.transform.localPosition = new Vector3(0, 0, 0);
            spawnedKnife.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void CreateKnifesCounter()
    {
        for (int i = 0; i < KnifesCount; i++)
        {
            Instantiate(UI_Knife, UI_KnifesPool);
            var ui_knife = UI_KnifesPool.GetChild(i);
            var knife = GameObject.FindGameObjectWithTag("Knife");

            ui_knife.transform.localPosition += transform.up * 100 * i;
            knife.GetComponent<SpriteRenderer>().sprite = KnifeSprites[PlayerPrefs.GetInt("EquipedKnife")];
            ui_knife.GetComponent<Image>().sprite = knife.GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void UpdateValues()
    {
            UI_ApplesValue.text = _apples.ToString();
            UI_Score.text = _score.ToString();
    }

    public void SubtractKnife()
    {
        var knife = UI_KnifesPool.GetChild(KnifesCount - 1);
        knife.GetComponent<Image>().color = new Color(0, 0, 0);
        KnifesCount--;
    }

    public IEnumerator GameOver(string result)
    {
        gameOver = true;
        if (result == "Lose")
        {
            PlayerPrefs.SetInt("Score", 0);
            ChangeScore();
            yield return new WaitForSeconds(GameOverDelay/2);

            LoseWindow.SetActive(true);
            LoseWindow.GetComponent<Animator>().enabled = true;
            StartCoroutine(StopAnimation(LoseWindow));
        }
        else
        {
            Vibration.Vibrate();
            slice.Scatter();
            _score++;
            UpdateValues();
            PlayerPrefs.SetInt("Score", _score);
            yield return new WaitForSeconds(GameOverDelay);
            Restart();
        }
    }

    private void ChangeScore()
    {
        if (_record < _score)
        {
            LoseWindow.transform.GetChild(3).gameObject.SetActive(true);
            _record = _score;
            PlayerPrefs.SetInt("Record", _record);
        }
        UI_Score.transform.SetParent(LoseWindow.transform);
        UI_Score.fontSize = 200;
        UI_Score.rectTransform.sizeDelta = new Vector2(850, 235);
        UI_Score.rectTransform.localPosition = new Vector2(0, 130);
    }


    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }

    public void Pause()
    {
        PauseWindow.SetActive(true);
        PauseWindow.GetComponent<Animator>().enabled = true;
        StartCoroutine(StopAnimation(PauseWindow));
        
        HitArea.SetActive(false);
    }
   
    public void Play()
    {
        if (gameOver)
            if (!showingFailMsg)
                StartCoroutine(ShowFailMsg());
            else return;
        else
            Unpause();
    }

    private void Unpause()
    {
        var anim = PauseWindow.GetComponent<Animator>();
        anim.enabled = true;
        anim.SetFloat("speedMultiplier", -1);
        StartCoroutine(StopAnimation(PauseWindow));
        Time.timeScale = 1;
    }

    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator StopAnimation(GameObject window)
    {
        Animator anim = window.GetComponent<Animator>();
        yield return new WaitForSeconds(1f);
        if (anim.GetFloat("speedMultiplier") == -1)
        {
            window.SetActive(false);
            HitArea.SetActive(true);
        }
        else
        {
            anim.enabled = false;
            Time.timeScale = 0; 
        }
    }

    private IEnumerator ShowFailMsg()
    {
        showingFailMsg = true;
        FailMessage.SetActive(true);
        yield return new WaitForSeconds(FailMessage.GetComponent<Animation>().clip.length);
        FailMessage.SetActive(false);
        showingFailMsg = false;
    }
}
