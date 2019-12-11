using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMerchant : MonoBehaviour
{
    public int Price;

    public GameObject Text;
    public LayerMask PlayerLayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        var SpeedManager = GetComponent<SpeedManager>();

        RaycastHit2D Hit = Physics2D.BoxCast(SpeedManager.GetTruePos(), new Vector2(SpeedManager.BodyWidth, SpeedManager.BodyHeight), 0, Vector2.down, 0, PlayerLayer);

        if (Hit.collider != null && Hit.collider.gameObject.GetComponent<CharacterAction>().EquipedBattleArt != null && Hit.collider.gameObject.GetComponent<CharacterAction>().EquipedBattleArt.Level < 3)
        {
            if (Hit.collider.gameObject.GetComponent<ControlStateManager>().AttachedUpgradeMerchant == null || Hit.collider.gameObject.GetComponent<ControlStateManager>().AttachedUpgradeMerchant == gameObject)
            {
                Text.SetActive(true);
                Hit.collider.gameObject.GetComponent<ControlStateManager>().AttachedUpgradeMerchant = gameObject;
            }
            else
            {
                Text.SetActive(false);
            }
        }
        else
        {
            if (CharacterOpenInfo.Self.GetComponent<ControlStateManager>().AttachedUpgradeMerchant == gameObject)
            {
                CharacterOpenInfo.Self.GetComponent<ControlStateManager>().AttachedUpgradeMerchant = null;
            }

            Text.SetActive(false);
        }
    }
}
