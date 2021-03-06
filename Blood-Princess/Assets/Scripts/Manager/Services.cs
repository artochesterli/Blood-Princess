﻿using System.Collections;
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

    private static StorageManager _storagemanaager;
    public static StorageManager StorageManager
    {
        get
        {
            Debug.Assert(_storagemanaager != null);
            return _storagemanaager;
        }
        set
        {
            _storagemanaager = value;
        }
    }

    private static HomeManager _homemamager;
    public static HomeManager HomeManager
    {
        get
        {
            Debug.Assert(_homemamager != null);
            return _homemamager;
        }
        set
        {
            _homemamager = value;
        }
    }

    private static MapGenerationManager _mapgenerationmanager;
    public static MapGenerationManager MapGenerationManager
    {
        get
        {
            Debug.Assert(_mapgenerationmanager != null);
            return _mapgenerationmanager;
        }
        set
        {
            _mapgenerationmanager = value;
        }
    }

    private static EnemyGenerationManager _enemygenerationmanager;
    public static EnemyGenerationManager EnemyGenerationManager
    {
        get
        {
            Debug.Assert(_enemygenerationmanager != null);
            return _enemygenerationmanager;
        }
        set
        {
            _enemygenerationmanager = value;
        }
    }
}