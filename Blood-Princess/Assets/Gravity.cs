using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float NormalGravity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var SpeedManager = GetComponent<SpeedManager>();
        var CharacterStateManager = GetComponent<CharacterStateManager>();
        if (!SpeedManager.HitGround && CharacterStateManager.GravityState==CharacterGravityState.Normal)
        {
            SpeedManager.ForcedSpeed.y -= NormalGravity * Time.deltaTime * 10;
        }
    }
}
