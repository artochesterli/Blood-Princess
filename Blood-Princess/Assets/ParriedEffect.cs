using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParriedEffect : MonoBehaviour
{
    private float TimeCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TimeCount += Time.deltaTime;
        transform.localScale = Vector3.one * (1 + 0.2f * TimeCount / 0.1f) * 2;
        if(TimeCount >= 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
