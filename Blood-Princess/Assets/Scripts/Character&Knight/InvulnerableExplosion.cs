using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvulnerableExplosion : MonoBehaviour
{
    public GameObject Source;
    public float StartScale;
    public float EndScale;
    public float Duration;
    public int Damage;

    public LayerMask EnemyLayer;

    private float TimeCount;
    private List<GameObject> HitEnemies;


    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.one * StartScale;
        HitEnemies = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        TimeCount += Time.deltaTime;
        if(TimeCount >= Duration)
        {
            Destroy(gameObject);
        }
        else
        {
            float Scale = Mathf.Lerp(StartScale, EndScale, TimeCount / Duration);

            transform.localScale = Vector3.one * Scale;

            Color color = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(color.r, color.g, color.b, 1), new Color(color.r, color.g, color.b, 0), TimeCount / Duration);

            Collider2D[] AllHits = Physics2D.OverlapCircleAll(transform.position, Scale / 2, EnemyLayer);

            for(int i = 0; i < AllHits.Length; i++)
            {
                if (!HitEnemies.Contains(AllHits[i].gameObject))
                {
                    HitEnemies.Add(AllHits[i].gameObject);
                    CharacterAttackInfo ExplosionAttack = new CharacterAttackInfo(Source, CharacterAttackType.Explosion, AllHits[i].gameObject.GetComponent<SpeedManager>().GetTruePos().x > transform.position.x, Damage, Vector2.zero, Vector2.zero);
                    AllHits[i].GetComponent<IHittable>().OnHit(ExplosionAttack);
                }
            }
        }
    }
}
