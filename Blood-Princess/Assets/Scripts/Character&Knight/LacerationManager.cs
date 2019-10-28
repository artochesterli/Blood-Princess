using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LacerationManager : MonoBehaviour
{
    public float StateTime;
    public GameObject LacerationMark;

    private float TimeCount;
    private float InitScale;
    // Start is called before the first frame update
    void Start()
    {
        LacerationMark = transform.Find("LacerationMark").gameObject;
        LacerationMark.GetComponent<SpriteRenderer>().enabled = true;
        InitScale = LacerationMark.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        TimeCount += Time.deltaTime;
        LacerationMark.transform.localScale = Vector3.one * Mathf.Lerp(InitScale, 0, TimeCount / StateTime);
        if(TimeCount >= StateTime)
        {
            LacerationMark.transform.localScale = Vector3.one * InitScale;
            LacerationMark.GetComponent<SpriteRenderer>().enabled = false;
            Destroy(this);
        }
    }

    public void ResetState()
    {
        TimeCount = 0;
    }
}
