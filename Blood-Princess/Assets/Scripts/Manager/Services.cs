using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Services
{
	private static AudioManager _audiomanager;
	public static AudioManager AudioManager
	{
		get
		{
			Debug.Assert(_audiomanager != null);
			return _audiomanager;
		}
		set
		{
			_audiomanager = value;
		}
	}

	private static GameFeelManager _gameFeelManager;
	public static GameFeelManager GameFeelManager
	{
		get
		{
			Debug.Assert(_gameFeelManager != null);
			return _gameFeelManager;
		}
		set
		{
			_gameFeelManager = value;
		}
	}

	private static VFXManager _visualEffectManager;
	public static VFXManager VisualEffectManager
	{
		get
		{
			Debug.Assert(_visualEffectManager != null);
			return _visualEffectManager;
		}
		set
		{
			_visualEffectManager = value;
		}
	}

	private static Loot.LootManager _lootManager;
	public static Loot.LootManager LootManager
	{
		get
		{
			Debug.Assert(_lootManager != null);
			return _lootManager;
		}

		set
		{
			_lootManager = value;
		}
	}

	private static Loot.CoinManager _coinManager;
	public static Loot.CoinManager CoinManager
	{
		get
		{
			Debug.Assert(_coinManager != null);
			return _coinManager;
		}

		set
		{
			_coinManager = value;
		}
	}

	private static GameStateManager _gamestatemanager;
	public static GameStateManager GameStateManager
	{
		get
		{
			Debug.Assert(_gamestatemanager != null);
			return _gamestatemanager;
		}
		set
		{
			_gamestatemanager = value;
		}
	}
}