using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public float MaxSpeed;
    public float GroundAcceleration;
    public float GroundDeceleration;
    public float AirAcceleration;
    public float AirDeceleration;
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
        var CharacterStateManager = GetComponent<CharacterStateManager>();

        if (CharacterStateManager.State != CharacterState.Hit)
        {
            var SpeedManager = GetComponent<SpeedManager>();
            Vector2Int MoveVector = Vector2Int.zero;
            if (InputRight())
            {
                MoveVector += Vector2Int.right;
            }

            if (InputLeft())
            {
                MoveVector += Vector2Int.left;
            }

            if (MoveVector.x > 0)
            {
                transform.eulerAngles = Vector3.one;

                if (SpeedManager.SelfSpeed.x >= 0)
                {
                    if (SpeedManager.HitGround)
                    {
                        SpeedManager.SelfSpeed.x += GroundAcceleration * Time.deltaTime;
                    }
                    else
                    {
                        SpeedManager.SelfSpeed.x += AirAcceleration * Time.deltaTime;
                    }

                    if (SpeedManager.SelfSpeed.x > MaxSpeed)
                    {
                        SpeedManager.SelfSpeed.x = MaxSpeed;
                    }
                }
                else
                {
                    if (SpeedManager.HitGround)
                    {
                        SpeedManager.SelfSpeed.x = GroundAcceleration * Time.deltaTime;
                    }
                    else
                    {
                        SpeedManager.SelfSpeed.x = AirAcceleration * Time.deltaTime;
                    }
                }
            }
            else if (MoveVector.x < 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
                if (SpeedManager.SelfSpeed.x <= 0)
                {
                    if (SpeedManager.HitGround)
                    {
                        SpeedManager.SelfSpeed.x -= GroundAcceleration * Time.deltaTime;
                    }
                    else
                    {
                        SpeedManager.SelfSpeed.x -= AirAcceleration * Time.deltaTime;
                    }

                    if (SpeedManager.SelfSpeed.x < -MaxSpeed)
                    {
                        SpeedManager.SelfSpeed.x = -MaxSpeed;
                    }
                }
                else
                {
                    if (SpeedManager.HitGround)
                    {
                        SpeedManager.SelfSpeed.x = -GroundAcceleration * Time.deltaTime;
                    }
                    else
                    {
                        SpeedManager.SelfSpeed.x = -AirAcceleration * Time.deltaTime;
                    }
                }
            }
            else
            {
                if (SpeedManager.SelfSpeed.x > 0)
                {
                    if (SpeedManager.HitGround)
                    {
                        SpeedManager.SelfSpeed.x -= GroundDeceleration * Time.deltaTime;
                    }
                    else
                    {
                        SpeedManager.SelfSpeed.x -= AirDeceleration * Time.deltaTime;
                    }

                    if (SpeedManager.SelfSpeed.x < 0)
                    {
                        SpeedManager.SelfSpeed.x = 0;
                    }
                }
                else
                {
                    if (SpeedManager.HitGround)
                    {
                        SpeedManager.SelfSpeed.x += GroundDeceleration * Time.deltaTime;
                    }
                    else
                    {
                        SpeedManager.SelfSpeed.x += AirDeceleration * Time.deltaTime;
                    }

                    if (SpeedManager.SelfSpeed.x > 0)
                    {
                        SpeedManager.SelfSpeed.x = 0;
                    }
                }
            }
        }

    }

    private bool InputRight()
    {
        return Input.GetKey(KeyCode.RightArrow);
    }

    private bool InputLeft()
    {
        return Input.GetKey(KeyCode.LeftArrow);
    }
}
