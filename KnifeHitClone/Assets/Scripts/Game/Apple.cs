using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    private bool falling = false;
    private float speed;

    private Slice slice;


    private void Start()
    {
        slice = GameObject.FindGameObjectWithTag("Slice").GetComponent<Slice>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Knife"))
        {
            speed = ((slice.RotationSpeed > 0) ? slice.RotationSpeed : -slice.RotationSpeed) * 3;
            transform.SetParent(null);
            falling = true;
            GetComponent<Collider2D>().enabled = false;
            Main._apples++;
            PlayerPrefs.SetInt("Apples", Main._apples);
            Camera.main.GetComponent<Main>().UpdateValues();
        }
    }

    private void Update()
    {
        if (falling)
            Fall();
    }

    private void Fall()
    {
        speed *= 1.02f;
        var firstHalf = transform.GetChild(0);
        var secondHalf = transform.GetChild(1);
        firstHalf.transform.position += (Vector3.down + (Vector3.right * 0.25f)) * speed * Time.deltaTime;
        firstHalf.transform.RotateAroundLocal(Vector3.forward, (speed * 1.5f) * Time.deltaTime);
        secondHalf.transform.position += (Vector3.down + (Vector3.left * 0.25f)) * speed * Time.deltaTime;
        secondHalf.transform.RotateAroundLocal(Vector3.back, (speed * 1.5f) * Time.deltaTime);
    }
}
