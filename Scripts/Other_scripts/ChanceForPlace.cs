using System.Collections;
using UnityEngine;

public class ChanceForPlace : MonoBehaviour
{
    [SerializeField] private ChanceForPlace targetPlace;
    [SerializeField] private KeyCode openKey = KeyCode.K;
    private Player currentPlayer;
    

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>()!=null)
        {
           currentPlayer = collision.GetComponent<Player>();    
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            currentPlayer = null;
        }
    }

    private void Update()
    {
        if (currentPlayer != null && Input.GetKeyDown(openKey))
        {
            StartCoroutine(OpenCFP(targetPlace, currentPlayer));
        }
    }


    private IEnumerator OpenCFP(ChanceForPlace TargetPlace, Player player)
    {
        anim.SetBool("open", true);
        yield return new WaitForSeconds(0.35f);
        player.SetChanceForPlace(TargetPlace);
        anim.SetBool("open",false);

    }



}
