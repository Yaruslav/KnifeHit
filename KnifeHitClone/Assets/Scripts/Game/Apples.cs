using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Apple", fileName = "New Apple")]
public class Apples : ScriptableObject
{
    [Tooltip("Шанс появления яблока")]
    [SerializeField] private float chance;
    public float Chance
    {
        get { return chance; }
        protected set { }
    }
}
