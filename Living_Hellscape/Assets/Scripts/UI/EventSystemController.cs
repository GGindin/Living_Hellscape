using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystemController : MonoBehaviour
{
    private static EventSystemController Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        Instance = this;
    }
}
