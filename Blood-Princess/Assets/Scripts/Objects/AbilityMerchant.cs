using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMerchant : MonoBehaviour
{
    public int GenerateNumber;
    public float GeneratePosIntervalX;

    public GameObject AbilityObject;

    // Start is called before the first frame update
    void Start()
    {
        GenerateAbilityObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateAbilityObjects()
    {
        float StartX = GetComponent<SpeedManager>().GetTruePos().x - GeneratePosIntervalX * (GenerateNumber / 2);

        float X = StartX;
        float Y = GetComponent<SpeedManager>().GetTruePos().y - GetComponent<SpeedManager>().BodyHeight / 2;

        for(int i = 0; i < GenerateNumber; i++)
        {
            GameObject Obj = GameObject.Instantiate(AbilityObject);
            Obj.GetComponent<SpeedManager>().SetInitInfo();
            Obj.GetComponent<SpeedManager>().MoveToPoint(new Vector2(X, Y+Obj.GetComponent<SpeedManager>().BodyHeight/2));

            X += GeneratePosIntervalX;
        }


    }
}
