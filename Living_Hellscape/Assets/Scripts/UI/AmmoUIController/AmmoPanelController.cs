using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoPanelController : MonoBehaviour
{
    public static AmmoPanelController Instance { get; private set; }

    [SerializeField]
    TextMeshProUGUI testCounter;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void UpdateCount(int currentCount)
    {
        testCounter.text = currentCount.ToString();
        TurnOn();
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }
}
