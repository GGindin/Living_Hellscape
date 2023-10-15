using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPanelController : MonoBehaviour
{
    public static HealthPanelController Instance { get; private set; }

    [SerializeField]
    HeartIcon heartIconPrefab;

    HeartIcon[] hearts = new HeartIcon[10];

    private void Awake()
    {
        Instance = this;
        CreateHeartIcons();
    }

    public void UpdatePanel(PlayerStats stats)
    {
        SetMaxHearts(stats.MaxHealth);
        SetCurrentHealth(stats.CurrentHealth);
    }

    void SetMaxHearts(int maxHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            var heart = hearts[i];
            if(i < maxHealth)
            {
                heart.gameObject.SetActive(true);
            }
            else
            {
                heart.gameObject.SetActive(false);
            }
        }
    }

    void SetCurrentHealth(int currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            var heart = hearts[i];
            if (i < currentHealth)
            {
                heart.SetColorState(true);
            }
            else
            {
                heart.SetColorState(false);
            }
        }
    }

    void CreateHeartIcons()
    {
        for(int i = 0; i < hearts.Length; i++)
        {
            hearts[i] = Instantiate(heartIconPrefab, transform);
            hearts[i].gameObject.SetActive(false);
            hearts[i].SetColorState(false);
        }
    }
}
