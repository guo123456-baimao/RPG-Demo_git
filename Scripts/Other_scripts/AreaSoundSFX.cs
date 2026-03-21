using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSoundSFX : MonoBehaviour
{
    [SerializeField] private int SFXIndex;
    private bool inArea=false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>()!=null)
        {
            AudioManager.instance.PlaySFX(SFXIndex, null);
            inArea = true;
        }
    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            AudioManager.instance.StopSFXWithTime(SFXIndex);
            inArea = false;
        }
    }


}
