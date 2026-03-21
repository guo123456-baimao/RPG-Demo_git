using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ice And Fire Effect", menuName = "Data/Item effect/Ice And Fire")]
public class IceAndFireEffect :ItemEffect
{
    [SerializeField] private GameObject iceAndFirePrefab;
    [SerializeField] private float xVelocity; 

    public override void ExecuteEffect(Transform _respondPosition)
    {
        Player player = PlayerManager.instance.player;

        bool thirdAttack = player.primaryAttack.comboCounter == 2;

        if (thirdAttack)
        {
            GameObject newIceAndFire = Instantiate(iceAndFirePrefab, _respondPosition.position, player.transform.rotation);
            newIceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity*player.facingDir,0);

            Destroy(newIceAndFire,10);
        }
    }
}
