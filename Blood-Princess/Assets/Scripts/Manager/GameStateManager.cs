using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class GameStateManager
{
	private FSM<GameStateManager> m_GameState;

	public GameStateManager()
	{
		m_GameState = new FSM<GameStateManager>(this);
		if (SceneManager.GetActiveScene().name.ToLower().Contains("clinic"))
		{
			m_GameState.TransitionTo<HomeState>();
		}
		else
		{
			m_GameState.TransitionTo<RunState>();
		}
		EventManager.instance.AddHandler<PlayerDied>(m_OnPlayerDie);
	}

	public void Update()
	{
		m_GameState.Update();
	}

	public void EnterRunState()
	{
		m_GameState.TransitionTo<RunState>();
	}

	public void EnterHomeState()
	{
		m_GameState.TransitionTo<HomeState>();
	}

	private void m_OnPlayerDie(PlayerDied ev)
	{
		EnterHomeState();
	}

	public void Destroy()
	{
		EventManager.instance.RemoveHandler<PlayerDied>(m_OnPlayerDie);
	}

	private abstract class GameState : FSM<GameStateManager>.State
	{ }

	/// <summary>
	/// Inside Clinic State
	/// </summary>
	private class HomeState : GameState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			if (SceneManager.GetActiveScene().buildIndex != 0)
			{
				SceneManager.LoadScene(0);
			}
		}
	}

	/// <summary>
	/// Doing a Run
	/// </summary>
	private class RunState : GameState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			// Generate Tile
			if (SceneManager.GetActiveScene().buildIndex != 1)
			{
				GameObject.Find("ClinicCanvas").GetComponentInChildren<Image>().DOFade(1f, 1.2f).OnComplete(() =>
				{
					SceneManager.LoadScene(1);
				});
			}
		}
	}
}
