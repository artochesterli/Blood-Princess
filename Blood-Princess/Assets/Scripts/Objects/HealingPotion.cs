using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealingPotion : MonoBehaviour
{
    public LayerMask PlayerLayer;
    public GameObject Text;
    public float Proportion;

    // Start is called before the first frame update
    void Start()
    {
        SetText();
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayer();
        if (GetComponent<SpeedManager>().HitGround)
        {
            GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }

    private void DetectPlayer()
    {
        var SpeedManager = GetComponent<SpeedManager>();

        RaycastHit2D Hit = Physics2D.BoxCast(SpeedManager.GetTruePos(), new Vector2(SpeedManager.BodyWidth, SpeedManager.BodyHeight), 0, Vector2.down, 0, PlayerLayer);

        if (Hit.collider != null)
        {
            if (Hit.collider.gameObject.GetComponent<ControlStateManager>().AttachedPotion == null || Hit.collider.gameObject.GetComponent<ControlStateManager>().AttachedPotion == gameObject)
            {
                Text.SetActive(true);
                Hit.collider.gameObject.GetComponent<ControlStateManager>().AttachedPotion = gameObject;

            }
            else
            {
                Text.SetActive(false);
            }
        }
        else
        {
            if (CharacterOpenInfo.Self.GetComponent<ControlStateManager>().AttachedPotion == gameObject)
            {
                CharacterOpenInfo.Self.GetComponent<ControlStateManager>().AttachedPotion = null;
            }
            Text.SetActive(false);
        }
    }

    private void SetText()
    {
        Text.GetComponent<TextMeshProUGUI>().text = "LB: Heal " + Proportion.ToString() + "%";
    }
}
