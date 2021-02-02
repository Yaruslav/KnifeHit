using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    [Header("Настройки магазина")]

    [Tooltip("Шаблон ножа")]
    [SerializeField]
    private GameObject TemplateKnife;
    [Tooltip("Настройки ножей")]
    [SerializeField]
    private Knifes[] KnifeSettings;
    [Tooltip("Шаблон последнего ножа")]
    [SerializeField]
    private GameObject SoonKnife;

    private int equipedKnife;
    private Vector2 previousPosition;
    private bool showingError;

    private void Start()
    {
        equipedKnife = PlayerPrefs.GetInt("EquipedKnife");
        previousPosition = GameObject.Find("StartPosition").transform.localPosition;
        LoadStore();
    }

    public void UpdateEquipedKnifeStats()
    {
        equipedKnife = PlayerPrefs.GetInt("EquipedKnife");
        PlayerPrefs.SetFloat("Cooldown", KnifeSettings[equipedKnife].Cooldown);
        PlayerPrefs.SetFloat("AttackSpeed", KnifeSettings[equipedKnife].AttackSpeed);
    }

    public void UpdateKnifeSprite()
    {
        var knife = transform.parent.parent.parent.GetChild(5).GetChild(0);
        knife.GetComponent<Image>().sprite = KnifeSettings[PlayerPrefs.GetInt("EquipedKnife")].Sprite;
    }
    private void LoadStore()
    {
        for (int i = 0; i < KnifeSettings.Length; i++)
        {
            Instantiate(TemplateKnife, transform);

            var knife = transform.GetChild(i);
            var stats = knife.GetChild(2);
            var price = knife.GetChild(3);
            var equipButton = knife.GetChild(4).GetChild(0);
            var purchaseButton = knife.GetChild(4).GetChild(1);
            var statsButton = knife.GetChild(4).GetChild(2);
            var nameOfKnife = knife.GetChild(5);

            if (i == 0)
                knife.localPosition = previousPosition;
            else
                knife.localPosition = new Vector2(previousPosition.x + 185, previousPosition.y);

            knife.GetChild(1).GetComponent<Image>().sprite = KnifeSettings[i].Sprite;

            if (PlayerPrefs.GetInt(i + "") == 1)
                KnifeSettings[i].Purchased = true;

            if (KnifeSettings[i].Purchased)
            {
                price.gameObject.SetActive(false);
                equipButton.gameObject.SetActive(true);
                purchaseButton.gameObject.SetActive(false);

                if (i == equipedKnife)
                    KnifeSettings[i].Equiped = true;

                if (KnifeSettings[i].Equiped)
                {
                    equipButton.GetComponent<Button>().interactable = false;
                    equipButton.GetChild(0).GetComponent<Text>().text = "Equiped";
                }
                else
                {
                    equipButton.GetChild(0).GetComponent<Text>().text = "Equip";
                    equipButton.GetComponent<Button>().interactable = true;
                }
            }
            else
            {
                price.gameObject.SetActive(true);
                price.GetChild(3).GetComponent<Text>().text = KnifeSettings[i].Price.ToString();
                equipButton.gameObject.SetActive(false);
                purchaseButton.gameObject.SetActive(true);
                var purchaseID = i;
                purchaseButton.GetComponent<Button>().onClick.AddListener(() => Purchase(purchaseID));
            }
            var ID = i;
            equipButton.GetComponent<Button>().onClick.AddListener(() => Equip(ID));
            statsButton.GetComponent<Button>().onClick.AddListener(() => ShowStats(ID));

            stats.GetChild(2).GetComponent<Text>().text = "AttackSpeed\n" + KnifeSettings[i].AttackSpeed;
            stats.GetChild(3).GetComponent<Text>().text = "Cooldown\n" + KnifeSettings[i].Cooldown + " sec";
            nameOfKnife.GetComponent<Text>().text = KnifeSettings[i].ToString();

            previousPosition = knife.localPosition;
        }
        Instantiate(SoonKnife, transform);
        transform.GetChild(KnifeSettings.Length).localPosition = new Vector2(previousPosition.x + 185, previousPosition.y);
    }


    public void Equip(int knifeIndex)
    {
        equipedKnife = PlayerPrefs.GetInt("EquipedKnife");
        var equipedKnifeEquipButton = transform.GetChild(equipedKnife).GetChild(4).GetChild(0);
        var currentKnifeEquipButton = transform.GetChild(knifeIndex).GetChild(4).GetChild(0);

        KnifeSettings[equipedKnife].Equiped = false;
        equipedKnifeEquipButton.GetComponent<Button>().interactable = true;
        equipedKnifeEquipButton.GetChild(0).GetComponent<Text>().text = "Equip";

        KnifeSettings[knifeIndex].Equiped = true;
        currentKnifeEquipButton.GetComponent<Button>().interactable = false;
        currentKnifeEquipButton.GetChild(0).GetComponent<Text>().text = "Equiped";

        PlayerPrefs.SetInt("EquipedKnife", knifeIndex);
        UpdateEquipedKnifeStats();
        UpdateKnifeSprite();
    }

    public void Purchase(int knifeIndex)
    {
        if (Main._apples >= KnifeSettings[knifeIndex].Price)
        {
            var knifeButtons = transform.GetChild(knifeIndex).GetChild(4);

            KnifeSettings[knifeIndex].Purchased = true;
            transform.GetChild(knifeIndex).GetChild(3).gameObject.SetActive(false);
            knifeButtons.GetChild(0).gameObject.SetActive(true);
            knifeButtons.GetChild(1).gameObject.SetActive(false);

            Main._apples -= KnifeSettings[knifeIndex].Price;
            PlayerPrefs.SetInt("Apples", Main._apples);
            Camera.main.GetComponent<Menu>().UpdateValues();

            PlayerPrefs.SetInt(knifeIndex + "", 1);
        }
        else
            if (!showingError)
                StartCoroutine(ShowError());
    }   
    public void NextKnife()
    {
        var buttons = transform.parent.parent.GetChild(4);

        transform.localPosition = new Vector2(transform.localPosition.x - 185, transform.localPosition.y);
        buttons.GetChild(0).GetComponent<Button>().interactable = true;

        if (transform.localPosition.x == -(transform.childCount - 3) * 185)
            buttons.GetChild(1).GetComponent<Button>().interactable = false;
    }
    public void PreviousKnife()
    {
        var buttons = transform.parent.parent.GetChild(4);

        transform.localPosition = new Vector2(transform.localPosition.x + 185, transform.localPosition.y);
        buttons.GetChild(1).GetComponent<Button>().interactable = true;

        if (transform.localPosition == new Vector3(0, transform.localPosition.y))
            buttons.GetChild(0).GetComponent<Button>().interactable = false;
    }

    public void ShowStats(int knifeIndex)
    {
        var stats = transform.GetChild(knifeIndex).GetChild(2).gameObject;
        if (!stats.activeSelf)
            stats.SetActive(true);    
        else
            stats.SetActive(false);
            
    }

    private IEnumerator ShowError()
    {
        showingError = true;
        var ErrorText = transform.parent.parent.GetChild(6).gameObject;
        ErrorText.SetActive(true);
        yield return new WaitForSeconds(ErrorText.GetComponent<Animation>().clip.length * 2);
        ErrorText.SetActive(false);
        showingError = false;
    }
}
