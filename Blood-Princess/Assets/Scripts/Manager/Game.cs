using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loot;

public class Game : MonoBehaviour
{
    public AudioScriptableObject AudioData;
    public VFXScriptableObject VFXData;
    public GameFeelScriptableObject GameFeelData;
    public LootScriptableObject LootData;
    public TextAsset Database;
    public static Game instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        Services.AudioManager = new AudioManager(AudioData);
        Services.GameFeelManager = new GameFeelManager(GameFeelData);
        Services.VisualEffectManager = new VFXManager(VFXData);
        Services.GameStateManager = new GameStateManager();
        Services.LootManager = new LootManager(Database, LootData);
        Services.CoinManager = new CoinManager(LootData);
        Services.StorageManager = new StorageManager();
        Services.HomeManager = new HomeManager();
    }

    // Update is called once per frame
    void Update()
    {
        Services.GameStateManager.Update();
    }

    private void OnDestroy()
    {
        if (Game.instance != this) return;
        Services.AudioManager.Destroy();
        Services.AudioManager = null;

        Services.GameFeelManager.Destroy();
        Services.GameFeelManager = null;

        Services.VisualEffectManager.Destroy();
        Services.VisualEffectManager = null;

        Services.GameStateManager.Destroy();
        Services.GameStateManager = null;

        Services.LootManager.Destroy();
        Services.LootManager = null;

        Services.CoinManager.Destroy();
        Services.CoinManager = null;

        Services.StorageManager.Destroy();
        Services.StorageManager = null;

        Services.HomeManager.Destroy();
        Services.HomeManager = null;
    }
}