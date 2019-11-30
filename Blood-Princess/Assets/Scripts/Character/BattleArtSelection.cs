using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleArtSelection : MonoBehaviour, ISkillSelection
{
    public BattleArtType Type;
    public BattleArt Ability;
    public bool Selected { get; set; }

    public Color UnselectedColor;
    public Color SelectedColor;

    // Start is called before the first frame update
    void Start()
    {
        switch (Type)
        {
            case BattleArtType.PowerSlash:
                Ability = new PowerSlash();
                break;
            case BattleArtType.CrossSlash:
                Ability = new CrossSlash();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAppearance()
    {
        if (Selected)
        {
            GetComponent<Text>().color = SelectedColor;
        }
        else
        {
            GetComponent<Text>().color = UnselectedColor;
        }
    }
}
