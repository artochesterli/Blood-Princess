using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectInfo
{
    public GameObject Effect;
    public bool right;

    public HitEffectInfo(GameObject effect, bool r)
    {
        Effect = effect;
        right = r;
    }
}

public class HitEffectManager : MonoBehaviour
{
    public List<HitEffectInfo> AllInfo;

    // Start is called before the first frame update
    void Start()
    {
        AllInfo = new List<HitEffectInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        Clean();
        EffectFollow();
    }

    private void EffectFollow()
    {
        for(int i = 0; i < AllInfo.Count; i++)
        {
            Vector3 Pos = AllInfo[i].Effect.transform.position;
            if (AllInfo[i].right)
            {
                Pos.x = GetComponent<SpeedManager>().GetTruePos().x + GetComponent<SpeedManager>().BodyWidth / 2;
                
            }
            else
            {
                Pos.x = GetComponent<SpeedManager>().GetTruePos().x - GetComponent<SpeedManager>().BodyWidth / 2;
            }
            AllInfo[i].Effect.transform.position = Pos;
        }
    }

    private void Clean()
    {
        for(int i = 0; i < AllInfo.Count; i++)
        {
            if(AllInfo[i].Effect == null)
            {
                AllInfo.Remove(AllInfo[i]);
                i--;
            }
        }
    }
}
