using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float ShakeTime;
    public float StartScale;
    public float EndScale;
    public AnimationCurve Curve;

    public float StayTime;

    private float TimeCount;

    private int Value;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckTime();
    }

    public void ActivateSelf(int damage)
    {
        TimeCount = 0;
        Value += damage;
        GetComponent<TextMeshProUGUI>().text = Value.ToString();
        transform.localScale = Vector3.one * StartScale;
        transform.DOScale(EndScale, ShakeTime).SetEase(Curve);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if(TimeCount >= StayTime)
        {
            Destroy(gameObject);
        }
    }
}
