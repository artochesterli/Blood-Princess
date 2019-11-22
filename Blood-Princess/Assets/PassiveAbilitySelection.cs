using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveAbilitySelection : MonoBehaviour
{
    public PassiveAbilityType Type;
    public PassiveAbility Ability;
    public bool Selected { get; set; }

    public Color UnselectedColor;
    public Color SelectedColor;
    // Start is called before the first frame update
    void Start()
    {
        switch (Type)
        {

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
