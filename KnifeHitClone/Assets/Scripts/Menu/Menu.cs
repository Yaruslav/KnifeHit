using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("Интерфейс")]

    [Tooltip("Показатель яблок")]
    [SerializeField]
    private Text AppleValue;
    [Tooltip("Показатель счета")]
    [SerializeField]
    private Text Score;
    [Tooltip("Окно магазина")]
    [SerializeField]
    private GameObject Store;

    private void Start()
    {
        var store = Store.transform.GetChild(5).GetChild(1).GetComponent<Store>();
        store.UpdateKnifeSprite();
        store.UpdateEquipedKnifeStats();

        Time.timeScale = 1;
        UpdateValues();
    }

    public void UpdateValues()
    {
        Main._apples = PlayerPrefs.GetInt("Apples");
        Main._record = PlayerPrefs.GetInt("Record");
        AppleValue.text = Main._apples.ToString();
        Score.text = ("Best\n" + Main._record);
    }

    public void Play()
    {
        PlayerPrefs.SetInt("Score", 0);
        StartCoroutine(PlayGame());
    }

    private IEnumerator PlayGame()
    {
        Store.transform.parent.GetChild(4).GetComponent<Animator>().enabled = true;
        Store.transform.parent.GetChild(5).GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);
    }

    public void OpenStore(bool open)
    {
        if (open)
        {
            Store.SetActive(true);
            StartCoroutine(StopAnimation());
        }
        else
        {
            var error = Store.transform.GetChild(6).gameObject;
            var knifes = Store.transform.GetChild(5).GetChild(1);
            for (int i = 0; i < knifes.childCount - 1; i++)
            {
                var stats = knifes.GetChild(i).GetChild(2).gameObject;
                if (stats.activeSelf)
                    stats.SetActive(false);
            }
            if (error.activeSelf)
                error.SetActive(false);
            Store.GetComponent<Animator>().SetFloat("speedMultiplier", -1);
            Store.GetComponent<Animator>().enabled = true;
            StartCoroutine(StopAnimation());
        }
    }

    private IEnumerator StopAnimation()
    {
        Animator anim = Store.GetComponent<Animator>();
        yield return new WaitForSeconds(1f);
        if (anim.GetFloat("speedMultiplier") == -1)
            Store.SetActive(false);
        else
            anim.enabled = false;
    }


}
