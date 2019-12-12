using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialPickup : MonoBehaviour
{

    private void Update()
    {
        if (CheckPlayerInside() && Utility.InputPickUp())
        {

            Destroy(gameObject);
        }
    }
    private bool CheckPlayerInside()
    {
        var Speedmanager = GetComponent<SpeedManager>();
        RaycastHit2D[] hits = Physics2D.BoxCastAll(Speedmanager.GetTruePos(), new Vector2(Speedmanager.BodyWidth, Speedmanager.BodyHeight), 0f, Vector2.zero);
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
                return true;
        }
        return false;
    }
}
