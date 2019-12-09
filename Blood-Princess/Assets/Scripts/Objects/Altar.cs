using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Altar : MonoBehaviour
{
    public LayerMask PlayerLayer;
    public GameObject Text;

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

        if (Hit.collider != null)
        {
            if (Hit.collider.gameObject.GetComponent<ControlStateManager>().AttachedAltar == null || Hit.collider.gameObject.GetComponent<ControlStateManager>().AttachedAltar == gameObject)
            {
                Hit.collider.gameObject.GetComponent<ControlStateManager>().AttachedAltar = gameObject;
                Text.SetActive(true);
            }
            else
            {
                Text.SetActive(false);
            }
        }
        else
        {
            Text.SetActive(false);
        }
    }
}
