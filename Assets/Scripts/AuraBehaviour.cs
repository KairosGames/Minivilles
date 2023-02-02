using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AuraBehaviour : MonoBehaviour
{
    internal Image aura;
    internal Vector2 speed;
    internal Vector2 weight;
    internal Vector2 friction;
    internal Vector2 power;

    internal Color color;
    float alpha;
    float dispersion;
    
    bool toDisable;
    float lifeTimer;
    float timeToLive;

    float lclScale;

    void Awake()
    {
        aura = GetComponent<Image>();

        var pwr = Random.Range(75, 376);
        var angle = Random.Range(0, 361);
        var vx = (float)Mathf.Cos(angle * Mathf.Deg2Rad);
        var vy = (float)Mathf.Sin(angle * Mathf.Deg2Rad);
        var frct = (vx > 0) ? 0.75f : -0.75f;

        speed = new Vector2(vx, vy);
        weight = new Vector2(0, -0.5f);
        friction = new Vector2(frct, 0);
        power = new Vector2(pwr, pwr);

        lclScale = Random.Range(100, 201) / 100.0f;
        aura.transform.localScale = new Vector2(lclScale, lclScale);

        alpha = 1.0f;
        dispersion = 70.0f / Random.Range(45, 91);

        toDisable = false;
        lifeTimer = 0;
        timeToLive = Random.Range(75, 151) / 100;

        aura.color = color;
    }

    

    void Update()
    {
        lifeTimer += Time.deltaTime;

        alpha -= dispersion*Time.deltaTime;
        color.a = alpha;
        aura.color = color;


        if (lifeTimer >= timeToLive || alpha <= 0) { toDisable = true; }

        speed -= (weight + friction)*Time.deltaTime;

        Vector2 movement = speed * power * Time.deltaTime;
        aura.transform.position -= (Vector3)movement;

        if (toDisable) { gameObject.SetActive(false); }
    }
}
