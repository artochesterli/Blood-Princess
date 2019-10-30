using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalEyeManager : MonoBehaviour
{
    public int CurrentBonus;
    public float EffectTime;
    public GameObject Icon;

    private float TimeCount;
    private float InitScale;
    // Start is called before the first frame update
    void Start()
    {
        InitScale = Icon.transform.localScale.x;
        Icon.GetComponent<SpriteRenderer>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        TimeCount += Time.deltaTime;
        Icon.transform.localScale = Vector3.one * Mathf.Lerp(InitScale, 0, TimeCount / EffectTime);
        if (TimeCount >= EffectTime)
        {
            ResetState();
            Icon.GetComponent<SpriteRenderer>().enabled = false;
            Destroy(this);
        }

    }

    public void ResetState()
    {
        Icon.transform.localScale = Vector3.one * InitScale;
        TimeCount = 0;
    }
}
