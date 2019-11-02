using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalEyeManager : MonoBehaviour
{
    public CharacterAttackType Type;
    public int Bonus;
    public int BattleArtBonusNumber;

    public GameObject Icon;

    // Start is called before the first frame update
    void Start()
    {
        Icon.GetComponent<SpriteRenderer>().enabled = true;
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DestroySelf()
    {
        Icon.GetComponent<SpriteRenderer>().enabled = false;
        Destroy(this);
    }

}
