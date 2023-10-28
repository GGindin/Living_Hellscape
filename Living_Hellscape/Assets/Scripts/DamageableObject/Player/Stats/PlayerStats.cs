using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    [SerializeField]
    int defaultMaxHealth;

    [SerializeField]
    float speed;

    int healthUpgradesCount;

    int currentHealth;

    bool isSetup = false;

    public int MaxHealth => defaultMaxHealth + healthUpgradesCount;

    public int CurrentHealth => currentHealth; 

    public float Speed => speed;

    public void Setup()
    {
        if (!isSetup)
        {
            currentHealth = MaxHealth;
            isSetup = true;
        }
    }

    public void ChangeMaxHealth(int delta)
    {
        healthUpgradesCount += delta;
        HealthPanelController.Instance.UpdatePanel(this);
    }

    public void ChangeCurrentHealth(int delta)
    {
        currentHealth += delta;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
        HealthPanelController.Instance.UpdatePanel(this);
    }
}
