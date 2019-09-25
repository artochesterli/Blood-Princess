using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterJump : MonoBehaviour
{
    public float JumpSpeed;
    public float JumpHoldingTime;

    private float HoldingTimeCount;
    public bool Jumping;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        var SpeedManager = GetComponent<SpeedManager>();
        var CharacterStateManager = GetComponent<CharacterStateManager>();

        if (SpeedManager.HitGround)
        {
            if (InputDown())
            {
                SpeedManager.SelfSpeed.y = JumpSpeed;
                CharacterStateManager.GravityState = CharacterGravityState.Ignore;
                Jumping = true;
                HoldingTimeCount = 0;
            }
        }

        if (Jumping)
        {
            HoldingTimeCount += Time.deltaTime;
            if (InputRelease() || HoldingTimeCount >= JumpHoldingTime)
            {
                CharacterStateManager.GravityState = CharacterGravityState.Normal;
                Jumping = false;
            }
        }

    }

    private bool InputDown()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private bool InputRelease()
    {
        return Input.GetKeyUp(KeyCode.Space);
    }

    
}
