    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageFX : MonoBehaviour
{
    private float colorLooseRate;
    [SerializeField] private SpriteRenderer sr;

    public void SetupAfterImage(float _loosingSpeed,Sprite _spriteImage)
    {
        colorLooseRate = _loosingSpeed;

        sr.sprite = _spriteImage;

    }

    private void Update()
    {
        float alpha=sr.color.a-Time.deltaTime*colorLooseRate;

        sr.color=new Color(sr.color.r,sr.color.g,sr.color.b,alpha);

        if (sr.color.a < 0)
            Destroy(gameObject);


    }



}
