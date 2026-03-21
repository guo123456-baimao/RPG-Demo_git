using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FadeScreen : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void FadeOut() => anim.SetTrigger("fadeOut");                  //变黑
    public void FadeIn() => anim.SetTrigger("fadeIn");                    //变亮，每次进入场景自动调用
}
