using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSoundBGM : MonoBehaviour
{
    [SerializeField] private int BGMIndex;
    private bool inArea = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            AudioManager.instance.PlayBGM(BGMIndex);
            inArea = true;
        }
    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            AudioManager.instance.PlayBGM(0);
            inArea = false;
        }
    }
}
