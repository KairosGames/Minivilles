using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class FireworksManager : MonoBehaviour
{
    [SerializeField] GameObject aura;

    [SerializeField] AudioSource sndWhistle;
    [SerializeField] AudioSource sndExplosion;

    float timer;

    Color[] tblColors;

    void Awake()
    {
        timer = Random.Range(5, 21) / 10.0f;

        tblColors = new Color[] { Color.red, Color.blue, Color.yellow, Color.magenta, Color.cyan, Color.white, Color.green };
    }



    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            sndWhistle.Play();
            StartCoroutine(PlayExplosion(200, new Vector2(Random.Range(360,1560), Random.Range(190,920)), tblColors[Random.Range(0, tblColors.Length)]));
            timer = Random.Range(5, 21) / 10.0f;
        }
    }

    IEnumerator PlayExplosion(short pNumberAuraParticles, Vector2 pPos, Color pColor)
    {
        yield return new WaitForSeconds(0.9f);

        sndExplosion.Play();

        for (short i = 0; i < pNumberAuraParticles; i++)
        {
            GameObject lclAura = Instantiate(aura, pPos, new Quaternion(0,0,0,0), transform);
            lclAura.GetComponent<AuraBehaviour>().color = pColor;
        }
    }
}
