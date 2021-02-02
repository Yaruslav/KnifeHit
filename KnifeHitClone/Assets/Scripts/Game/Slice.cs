using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slice : MonoBehaviour
{
    [Header("Настройки бревна")]

    [Tooltip("Спрайты бревна")]
    [SerializeField]
    private Sprite[] SliceSprites;
    [Tooltip("Префабы сломанных бревен")]
    [SerializeField]
    private GameObject[] BrokenSlices;

    [HideInInspector]
    public float RotationSpeed = 2;

    private int indexSlice;
    private float scatterSpeed;
    private bool scattering;
    private bool changingRotation;

    private void Start()
    {
        indexSlice = Random.Range(0, SliceSprites.Length);
        GetComponent<SpriteRenderer>().sprite = SliceSprites[indexSlice];
    }

    private void Update()
    {
        if (scattering)
            ScatterSlice();
        else
            RollSlice();

        if(!changingRotation)
            StartCoroutine(ChangeSliceRotation());
    }

    private IEnumerator ChangeSliceRotation()
    {
        while (!Main.gameOver)
        {
            changingRotation = true;
            float time = Random.Range(1f, 3f);
            yield return new WaitForSeconds(time);
            float speed = Random.Range(-5f, 5f);
            RotationSpeed = speed;
        }
    }

    private void RollSlice()
    {
        if (Main.gameOver)
            RotationSpeed *= 0.995f;
        transform.RotateAroundLocal(Vector3.forward, RotationSpeed * Time.deltaTime);
    }


    public void Scatter()
    {
        Instantiate(BrokenSlices[indexSlice]);
        scatterSpeed = ((RotationSpeed > 0) ? RotationSpeed : -RotationSpeed) * 1.5f;
        scattering = true;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void ScatterSlice()
    {
        if (scatterSpeed > 0)
        {
            scatterSpeed *= .98f;
            RotationSpeed *= .99f;
        }
        else
        {
            scatterSpeed *= 1.015f;
            RotationSpeed *= 1.005f;
        }


        if (scatterSpeed > 0)
        {
            MovePiece(0, Vector2.up + (Vector2.left * .5f));
            MovePiece(1, Vector2.up + (Vector2.right * .5f));
            MovePiece(2, Vector2.up);
            if (scatterSpeed < 0.75f)
                scatterSpeed *= -1;
        }
        else
            for (int i = 0; i < 3; i++)
                MovePiece(i, Vector2.up);
        MoveKnife();
    }

    private void MovePiece(int index, Vector3 vector)
    {
        var brokenSlice = GameObject.FindGameObjectWithTag("BrokenSlice").transform.GetChild(index).transform;
        brokenSlice.position += vector * scatterSpeed * Time.deltaTime;
        brokenSlice.RotateAroundLocal(Vector3.forward, RotationSpeed * Time.deltaTime);
    }
    private void MoveKnife()
    {  
        var knifes = GameObject.FindGameObjectsWithTag("Knife");

        for (int i = 0; i < knifes.Length; i++)
        {
            var newRotation = Quaternion.LookRotation(Vector3.back - knifes[i].transform.position);
            knifes[i].GetComponent<Collider2D>().enabled = false;
            knifes[i].transform.position += Vector3.up * (scatterSpeed * 1.5f) * Time.deltaTime;
            knifes[i].transform.rotation = Quaternion.Slerp(knifes[i].transform.rotation, newRotation, 2 * Time.deltaTime);
        }
    }
}
