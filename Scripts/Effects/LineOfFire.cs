using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfFire : MonoBehaviour
{
    private Player player;
    private bool playerInLineOfFire;
    [SerializeField] private int damage = 10;

    private void Start()
    {
        Quaternion rotation = Quaternion.identity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>()!=null)
        {
            player = collision.GetComponent<Player>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>()!=null)
        {
            player = null;
            playerInLineOfFire = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            playerInLineOfFire = true;
            player = collision.GetComponent<Player>();
        }
    }

    private void CheckPlayer()
    {
        if (player!=null&&playerInLineOfFire)
        {
            player.GetComponent<PlayerStats>().TakeDamage(damage);
        }
    }

    private void SetSleep()
    {
        gameObject.SetActive(false);
    }

    public void AnimTrigger()
    {
        AudioManager.instance.PlaySFX(13, transform);
    }

}
