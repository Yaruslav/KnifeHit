using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Knife", fileName = "New Knife")]
public class Knifes : ScriptableObject
{
    [Header("��������� ����")]


    [Tooltip("������ ����")]
    [SerializeField] private Sprite sprite;
    public Sprite Sprite
    {
        get { return sprite; }
        protected set { }
    }

    [Tooltip("���� ����")]
    [SerializeField] private int price;
    public int Price
    {
        get { return price; }
        protected set { }
    }

    [Tooltip("������ �� ���?")]
    [SerializeField] private bool purchased;
    public bool Purchased
    {
        get { return purchased; }
        set { purchased = value; }
    }

    [Tooltip("���������� �� ���?")]
    [SerializeField] private bool equiped;
    public bool Equiped
    {
        get { return equiped; }
        set { equiped = value; }
    }


    [Header("�������������� ����")]


    [Tooltip("�������� ����� ����")]
    [SerializeField] private float attackSpeed;
    public float AttackSpeed
    {
        get { return attackSpeed; }
        protected set { }
    }

    [Tooltip("����� ���������� ����")]
    [SerializeField] private float cooldown;
    public float Cooldown
    {
        get { return cooldown; }
        protected set { }
    }
}
