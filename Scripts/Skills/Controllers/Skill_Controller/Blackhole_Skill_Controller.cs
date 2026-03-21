using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotkeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;
    private float blackholeTimer;

    private bool canGrow=true;
    private bool canShrink;
    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;
    private bool playerCanDisapear=true;

    private int amountOfAttacks = 4;
    private float cloneAttackCooldown=.3f;
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotKey=new List<GameObject>();

    public bool playerCanExitState {  get; private set; }

    public void SetupBlackhole(float _maxSize,float _growSpeed,float _shrinkSpeend,int _amountOfAttack,float _cloneAttackCooldown,float _blackholeDuration)
    {
        maxSize=_maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeend;
        amountOfAttacks= _amountOfAttack;
        cloneAttackCooldown = _cloneAttackCooldown;
        blackholeTimer = _blackholeDuration;

        if (SkillManager.instance.clone.crystalInsteadOfClone)
        {
            playerCanDisapear= false;
        }


    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        if (blackholeTimer<0)
        {
            blackholeTimer = Mathf.Infinity;
            if (targets.Count>0)
            {
                ReleaseCloneAttack();
            }
            else
            {
                FinishBlackHoleAbility();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }
        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);
            if (transform.localScale.x < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void ReleaseCloneAttack()
    {
        if (targets.Count<=0)
        {
            return;
        }

        DestroyHotKeys();
        cloneAttackReleased = true;
        canCreateHotKeys = false;
        if (playerCanDisapear)
        {
            playerCanDisapear = false;
            PlayerManager.instance.player.fx.MakeTransprent(true);
        }
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased && amountOfAttacks>0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            float xOffset;
            if (Random.Range(0, 100) > 50)
            {
                xOffset = 1;
            }
            else
            {
                xOffset = -1;
            }

            int randomIndex = Random.Range(0, targets.Count);

            if (SkillManager.instance.clone.crystalInsteadOfClone)
            {
                SkillManager.instance.crystal.CreateCrystal();
                SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
            }
            else
            {
                SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));
            }
            amountOfAttacks--;

            if (amountOfAttacks <= 0)
            {
                Invoke("FinishBlackHoleAbility",0.7f);
            }
        }

      
    }

    private void FinishBlackHoleAbility()
    {
        DestroyHotKeys();
        playerCanExitState = true;
        canShrink = true;
        cloneAttackReleased = false;
        
    }

    private void DestroyHotKeys()
    {
        if (createdHotKey.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < createdHotKey.Count; i++)
        {
            Destroy(createdHotKey[i]);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>()!=null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreateHotKey(collision);

        }
    }

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count<=0)
        {
            Debug.Log("Not enough HotKey");
            return;
        }
        if (!canCreateHotKeys)
        {
            return;
        }
        GameObject newHotKey = Instantiate(hotkeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createdHotKey.Add(newHotKey);

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        Blackhole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_HotKey_Controller>();
        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>()!=null)
        {
            collision.GetComponent<Enemy>().FreezeTime(false);
        }
    }




    public void AddEnemyToList(Transform _enemytransform)=>targets.Add(_enemytransform);
}
