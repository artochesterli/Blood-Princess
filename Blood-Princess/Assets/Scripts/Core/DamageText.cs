using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public float DisappearTime;
    public Vector2 TravelVector;

    private float TimeCount;
    private Vector2 OriPos;
    // Start is called before the first frame update
    void Start()
    {
        OriPos = GetComponent<RectTransform>().localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Travel();
    }

    private void Travel()
    {
        TimeCount += Time.deltaTime;
        GetComponent<RectTransform>().localPosition = Vector2.Lerp(OriPos, OriPos + TravelVector,TimeCount/DisappearTime);
        if (TimeCount >= DisappearTime)
        {
            Destroy(gameObject);
        }
    }
}
