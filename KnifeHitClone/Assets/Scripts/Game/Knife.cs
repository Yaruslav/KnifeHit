using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
   
    [Header("Настройки ножа")]

    [Tooltip("Префаб ножа")]
    [SerializeField]
    private GameObject KnifePrefab;

    [HideInInspector]
    public bool knifeIsHitted;

    [HideInInspector]
    public bool moving;

    private float attackSpeed = 25;
    private float cooldown = 0.01f;
    private bool miss;
    private bool clickInput;
    private bool canHit;
    private Vector3 startPosition;
    private Vector3 reboundVector;
    private float reboundMoveSpeed;
    private float reboundRotationSpeed;

    private Slice slice;
    private Main main;

    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);

        main = Camera.main.GetComponent<Main>();
        slice = GameObject.FindGameObjectWithTag("Slice").GetComponent<Slice>();
        attackSpeed = PlayerPrefs.GetFloat("AttackSpeed");
        cooldown = PlayerPrefs.GetFloat("Cooldown");

        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Animator>().enabled = true;
        GetComponent<Animator>().speed = 1 / cooldown;

        StartCoroutine(StopAnimation());

        startPosition = transform.position;
        knifeIsHitted = false;
    }


    private void Update()
    {
        clickInput = Input.GetMouseButtonDown(0);
        if (clickInput && canHit)
            GetMousePosiition();
        if (moving)
            Move();
    }

    private IEnumerator StopAnimation()
    {
        yield return new WaitForSeconds(cooldown);
        GetComponent<Animator>().enabled = false;
        canHit = true;
    }

    private void GetMousePosiition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            if (hit.transform.name == "HitArea")
                moving = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vibration.Vibrate();
        if (collision.gameObject.CompareTag("Knife"))
        {
            SetRebound();
            StartCoroutine(main.GameOver("Lose"));
            miss = true;
            knifeIsHitted = true;
        }
        else
        if (collision.gameObject.CompareTag("Slice"))
        {
            if (!knifeIsHitted)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                StartCoroutine(HitSlice(collision.gameObject));
                GetComponent<Collider2D>().enabled = false;
                GetComponent<SpriteRenderer>().enabled = false;
                knifeIsHitted = true;
                moving = false;
                main.SubtractKnife();
                if (main.KnifesCount > 0)
                    StartCoroutine(CreateNewKnife());
                else
                    StartCoroutine(main.GameOver("Win"));
                CreateEmptyKnife(collision.transform);
            }
        }
    }


    private void SetRebound()
    {
        reboundVector = ((slice.RotationSpeed > 0) ? Vector2.right : -Vector2.right) / (5 / slice.RotationSpeed) - Vector2.up / (30 / attackSpeed);
        reboundMoveSpeed = attackSpeed * (slice.RotationSpeed * .65f) * .85f;
        reboundRotationSpeed = (slice.RotationSpeed * attackSpeed) * .64f;
    }

    private void Move()
    {
        if (!miss)
            transform.position += Vector3.up * attackSpeed * Time.deltaTime;
        else
        {
            reboundMoveSpeed *= .995f;
            reboundRotationSpeed *= .99f;
            transform.position += reboundVector * reboundMoveSpeed * Time.deltaTime;
            transform.RotateAroundLocal(Vector3.back, reboundRotationSpeed * Time.deltaTime);
        }
    }



    private IEnumerator HitSlice(GameObject obj)
    {
        Transform slice = obj.transform;
        Color clr = obj.GetComponent<SpriteRenderer>().color;

        slice.position = new Vector2(slice.position.x, slice.position.y + 0.08f);
        obj.GetComponent<SpriteRenderer>().color = new Color(1, .82f, .78f, 1);
        yield return new WaitForSeconds(0.05f);
        obj.GetComponent<SpriteRenderer>().color = clr;
        slice.position = new Vector2(slice.position.x, slice.position.y - 0.08f);
    }

    private IEnumerator CreateNewKnife()
    {
        yield return new WaitForSeconds(cooldown);
        Quaternion rotation = new Quaternion();
        Instantiate(KnifePrefab, startPosition, rotation);
        transform.GetChild(0).gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void CreateEmptyKnife(Transform slice)
    {
        Instantiate(KnifePrefab,transform.position,transform.rotation, slice);
        var hittedKnife = slice.GetChild(slice.childCount - 1);
        Destroy(hittedKnife.GetChild(0).gameObject);
        hittedKnife.GetComponent<Knife>().enabled = false;
        hittedKnife.GetComponent<Collider2D>().enabled = true;
        hittedKnife.GetComponent<SpriteRenderer>().enabled = true;
        hittedKnife.transform.localScale = new Vector3(transform.localScale.x * 1.428f, transform.localScale.y * 1.415f, transform.localScale.z);
    }
}
