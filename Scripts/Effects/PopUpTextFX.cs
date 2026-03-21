using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpTextFX : MonoBehaviour
{
    private TextMeshPro mytext;

    [SerializeField] private float speed;
    [SerializeField] private float desapearanceSpeed;
    [SerializeField] private float colorDesapearanceSpeed;

    [SerializeField] private float lifeTime;

    private float textTimer;



    // Start is called before the first frame update
    void Start()
    {
        mytext = GetComponent<TextMeshPro>();
        textTimer=lifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + 1), speed* Time.deltaTime);
        textTimer-= Time.deltaTime;

        if(textTimer<0)
        {
            float alpha=mytext.alpha-colorDesapearanceSpeed*Time.deltaTime;
            mytext.color=new Color(mytext.color.r,mytext.color.g,mytext.color.b,alpha);

            if (mytext.color.a < 50)
                speed = desapearanceSpeed;  

            if(mytext.color.a <= 0)
                Destroy(gameObject);
        }
       
    }
}
