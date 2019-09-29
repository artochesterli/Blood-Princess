using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteData : MonoBehaviour
{
    public List<Sprite> IdleSeries;
    public List<Sprite> LightAnticipationSeries;
    public List<Sprite> LightRecoverySeries;
    public List<Sprite> HeavyAnticipationSeries;
    public List<Sprite> HeavyRecoverySeries;
    public List<Sprite> HitSeries;

    public Vector2 IdleOffset;
    public Vector2 IdleSize;
    public Vector2 LightAnticipationOffset;
    public Vector2 LightAnticipationSize;
    public Vector2 LightRecoveryOffset;
    public Vector2 LightRecoverySize;
    public Vector2 HeavyAnticipationOffset;
    public Vector2 HeavyAnticipationSize;
    public Vector2 HeavyRecoveryOffset;
    public Vector2 HeavyRecoverySize;
    public Vector2 HitOffset;
    public Vector2 HitSize;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
