using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class EntityFX : MonoBehaviour
{
    protected Player player;
    protected SpriteRenderer sr;

    [Header("Pop Up Text")]
    [SerializeField] private GameObject popUpTextPrefab;


    [Header("Flash FX")]
    [SerializeField] private Material hitMat;
    [SerializeField] private float flashDuration;
    private Material originalMat;

    [Header("Ailment colors")]
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] shockColor;


    [Header("Ailment partical")]
    [SerializeField] private ParticleSystem igniteFX;
    [SerializeField] private ParticleSystem chillFX;
    [SerializeField] private ParticleSystem shockFX;

    [Header("Hit FX")]
    [SerializeField] private GameObject hitFX;
    [SerializeField] private GameObject criticalHitFX;



    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        
        originalMat = sr.material;
        player = PlayerManager.instance.player;
    }


    public virtual void Update()
    {
        
    }


    public void CreatePopUpText(string _text)
    {
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(3,5);
        Vector3 positionOffset=new Vector3(randomX,randomY,0);

        GameObject newText=Instantiate(popUpTextPrefab,transform.position+positionOffset,Quaternion.identity);

        newText.GetComponent<TextMeshPro>().text = _text;
    }



    public void MakeTransprent(bool _transprent)
    {
        if (_transprent)
        {
            sr.color = Color.clear;
        }
        else
        {
            sr.color = Color.white;
        }
    }

    private IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color currentColor = sr.color;

        sr.color = Color.white;
        yield return new WaitForSeconds(flashDuration);

        sr.color = currentColor;
        sr.material = originalMat;
    }

    private void RedColorBlink()
    {
        if (sr.color != Color.white)
        {
            sr.color = Color.white;
        }
        else
        {
            sr.color = Color.red;
        }
    }
    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;

        igniteFX.Stop();
        chillFX.Stop();
        shockFX.Stop();
    }

    public void IgniteFxFor(float _seconds)
    {
        igniteFX.Play();

        InvokeRepeating("IgniteColorFx", 0f, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }
    public void ChillFxFor(float _seconds)
    {
        chillFX.Play();

        InvokeRepeating("ChillColorFx", 0f, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }


    public void ShockFxFor(float _seconds)
    {
        shockFX.Play();

        InvokeRepeating("ShockColorFx", 0f, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    private void IgniteColorFx()
    {
        if (sr.color != igniteColor[0])
        {
            sr.color = igniteColor[0];
        }
        else
        {
            sr.color = igniteColor[1];
        }
    }
    private void ChillColorFx()
    {
        if (sr.color != chillColor[0])
        {
            sr.color = chillColor[0];
        }
        else
        {
            sr.color = chillColor[1];
        }
    }

    private void ShockColorFx()
    {
        if (sr.color != shockColor[0])
        {
            sr.color = shockColor[0];
        }
        else
        {
            sr.color = shockColor[1];
        }
    }

    public void CreateHitFX(Transform _target,bool _critical)
    {
        float xPosition = Random.Range(-0.5f, 0.5f);
        float yPosition = Random.Range(-0.5f, 0.5f);
        float zRotation = Random.Range(-90,90);

        Vector3 hitFxRotation=new Vector3(0,0,zRotation);

        GameObject hitprefab = hitFX;

        if (_critical)
        {
            hitprefab=criticalHitFX;

            float yRotayion = 0;
            zRotation = Random.Range(-45, 45);

            if (GetComponent<Entity>().facingDir == -1)
                yRotayion = 180;

            hitFxRotation=new Vector3(0,yRotayion,zRotation);   
        }

        GameObject newHitFX = Instantiate(hitprefab, _target.position+new Vector3(xPosition,yPosition), Quaternion.identity);

        newHitFX.transform.Rotate(hitFxRotation);

        Destroy(newHitFX,.5f);
        
    }




}
