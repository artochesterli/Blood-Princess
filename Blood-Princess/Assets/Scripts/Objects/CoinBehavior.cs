using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpeedManager))]
[RequireComponent(typeof(GravityManager))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(ColliderInfo))]
[RequireComponent(typeof(SpriteRenderer))]
//[RequireComponent(typeof(TrailRenderer))]
public class CoinBehavior : MonoBehaviour
{
	public Vector2 XSpeedRange = new Vector2(-6f, -6f);
	public Vector2 YSpeedRange = new Vector2(5f, 15f);
	public float InIdleStateDuration = 0.6f;
	public Vector2 FollowSpeed = new Vector2(0.2f, 0.4f);
	public float RetractDistance = 1f;

	private FSM<CoinBehavior> m_CoinFSM;
	private SpeedManager m_SpeedManager;
	private GravityManager m_GravityManager;
	private Collider2D m_Collider2D;
	//private TrailRenderer m_TrailRenderer;
	private GameObject m_CoinTrail;
	private SpriteRenderer m_SpriteRenderer;

	private void Awake()
	{
		m_CoinFSM = new FSM<CoinBehavior>(this);
		m_SpeedManager = GetComponent<SpeedManager>();
		m_GravityManager = GetComponent<GravityManager>();
		m_Collider2D = GetComponent<Collider2D>();
		//m_TrailRenderer = GetComponent<TrailRenderer>();
		m_CoinTrail = transform.Find("CoinTrail").gameObject;
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		m_CoinFSM.TransitionTo<CoinAppearState>();
	}

	private void Update()
	{
		m_CoinFSM.Update();
	}

	private abstract class CoinState : FSM<CoinBehavior>.State
	{
	}

	private class CoinAppearState : CoinState
	{
		private float m_Timer;
		public override void OnEnter()
		{
			base.OnEnter();
			Context.m_SpeedManager.SelfSpeed = new Vector2(Random.Range(Context.XSpeedRange.x, Context.XSpeedRange.y), Random.Range(Context.YSpeedRange.x, Context.YSpeedRange.y));
			m_Timer = Time.timeSinceLevelLoad + Context.InIdleStateDuration;
			//Context.m_TrailRenderer.enabled = false;
			Context.m_CoinTrail.SetActive(false);
		}

		public override void Update()
		{
			base.Update();
			if (m_Timer < Time.timeSinceLevelLoad)
			{
				TransitionTo<CoinCanFollowState>();
				return;
			}
			if (Context.m_SpeedManager.HitGround)
			{
				Context.m_SpeedManager.SelfSpeed.x *= 0.7f;
				Context.m_SpeedManager.SelfSpeed.y *= -4f;
			}
		}
	}

	private class CoinCanFollowState : CoinState
	{
		GameObject Player;
		public override void OnEnter()
		{
			base.OnEnter();
			Player = GameObject.FindGameObjectWithTag("Player");
		}
		public override void Update()
		{
			base.Update();
			m_PlayerEnterRange();
			if (Context.m_SpeedManager.HitGround)
			{
				Context.m_SpeedManager.SelfSpeed.x = 0f; 
			}
		}

		private void m_PlayerEnterRange()
		{
			if (Vector2.Distance(Player.transform.position, Context.transform.position) < Context.RetractDistance)
			{
				TransitionTo<CoinFollowState>();
				return;
			}
		}
	}

	private class CoinFollowState : CoinState
	{
		private GameObject Player;
		private float FollowSpeed;
		private bool isStop;
		public override void OnEnter()
		{
			base.OnEnter();
			Context.m_GravityManager.Gravity = 0f;
			Player = GameObject.FindGameObjectWithTag("Player");
			FollowSpeed = Random.Range(Context.FollowSpeed.x, Context.FollowSpeed.y);
			//Context.m_TrailRenderer.enabled = true;
			Context.m_CoinTrail.SetActive(true);
		}

		public override void Update()
		{
			float playerDistance = Vector2.Distance(Player.transform.position, Context.transform.position);
			base.Update();
			if (!isStop)
			{
				Context.transform.position = Vector3.Lerp(Context.transform.position, Player.transform.position, FollowSpeed);
				FollowSpeed *= 1.1f;
			}


			if (playerDistance < 0.2f)
			{
				Destroy(Context.gameObject, 0.3f);
				if (!isStop)
				{
					GameObject getCoinParticles = GameObject.Instantiate(
						Resources.Load("Prefabs/Particles/GetCoinParticle") as GameObject, Context.transform.position,
						Quaternion.identity);
					getCoinParticles.transform.parent = Context.transform;
				}
				
				Context.m_SpriteRenderer.enabled = false;
				Context.m_SpeedManager.SelfSpeed = Vector2.zero;
				//Context.transform.position = Player.transform.position;
				isStop = true;
				return;
			}
			
			

		}
	}
}
