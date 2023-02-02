using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip[] clips;

    private bool move = false;

    public int finalSide;
    public Sprite[] diceSides;

    private SpriteRenderer rend;
    public GameObject diceEnd;
    public  GameObject animator;

    [SerializeField] Vector3 startPosition;

    Vector3 rollSpeed;
    Vector3 startDiceScale;
    Vector3 endDiceScale;

   private void Awake()
    {
        rollSpeed = new Vector3(5.0f, 0, 0);
        startDiceScale = new Vector3(2.0f, 2.0f, 1.0f);
        endDiceScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    private void Start()
    {
        rend = diceEnd.GetComponent<SpriteRenderer>();

        animator.SetActive(false);
        animator.GetComponent<Animator>().speed =  0.7f; //Change la vitesse de l'anim

        diceEnd.SetActive(false);
        animator.SetActive(false);
    }

    public int Roll() //fonction appelé dans Game
    {
        transform.position = startPosition;

        StartCoroutine("RollTheDice");

        int randomDiceSide;
        randomDiceSide = Random.Range(0, 6);
        rend.sprite = diceSides[randomDiceSide];

        finalSide = randomDiceSide + 1;

        return finalSide;
    }

    public IEnumerator RollTheDice()
    {
        transform.localScale = startDiceScale;
        LeanTween.scale(gameObject, endDiceScale, 1.1f).setEase(LeanTweenType.easeOutBounce); // permet de faire un effet de rebond lors du lancé du dé
        source.PlayOneShot(clips[Random.Range(0, clips.Length)]);

        move = true;

        animator.SetActive(true);
        diceEnd.SetActive(false);

        yield return new WaitForSeconds(1.2f);

        animator.SetActive(false);
        diceEnd.SetActive(true);
        move = false;
    }

    public void HideDice()
    {
        diceEnd.SetActive(false);
    }

    public void Update() 
    {
        if (move)
            transform.position += rollSpeed * Time.deltaTime; // fait avancer le dé 
    }
}


