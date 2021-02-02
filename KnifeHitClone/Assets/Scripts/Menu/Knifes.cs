using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Knife", fileName = "New Knife")]
public class Knifes : ScriptableObject
{
    [Header("Настройки ножа")]


    [Tooltip("Спрайт ножа")]
    [SerializeField] private Sprite sprite;
    public Sprite Sprite
    {
        get { return sprite; }
        protected set { }
    }

    [Tooltip("Цена ножа")]
    [SerializeField] private int price;
    public int Price
    {
        get { return price; }
        protected set { }
    }

    [Tooltip("Куплен ли нож?")]
    [SerializeField] private bool purchased;
    public bool Purchased
    {
        get { return purchased; }
        set { purchased = value; }
    }

    [Tooltip("Экипирован ли нож?")]
    [SerializeField] private bool equiped;
    public bool Equiped
    {
        get { return equiped; }
        set { equiped = value; }
    }


    [Header("Характеристики ножа")]


    [Tooltip("Скорость атаки ножа")]
    [SerializeField] private float attackSpeed;
    public float AttackSpeed
    {
        get { return attackSpeed; }
        protected set { }
    }

    [Tooltip("Время доставания ножа")]
    [SerializeField] private float cooldown;
    public float Cooldown
    {
        get { return cooldown; }
        protected set { }
    }
}
