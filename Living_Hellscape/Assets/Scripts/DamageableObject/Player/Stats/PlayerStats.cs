using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    [SerializeField]
    int maxHealth;

    [SerializeField]
    float speed;

    int currentHealth;

    bool isSetup = false;

    public int MaxHealth => maxHealth;

    public int CurrentHealth => currentHealth; 

    public float Speed => speed;

    public void Setup()
    {
        if (!isSetup)
        {
            currentHealth = maxHealth;
            isSetup = true;
        }
    }

    public void ChangeMaxHealth(int delta)
    {
        maxHealth += delta;
        HealthPanelController.Instance.UpdatePanel(this);
    }

    public void ChangeCurrentHealth(int delta)
    {
        currentHealth += delta;
        HealthPanelController.Instance.UpdatePanel(this);
    }
}
