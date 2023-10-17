using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageRoute : MonoBehaviour
{
    IDamager damager;

    public IDamager Damager => damager;

    private void Awake()
    {
        damager = GetComponent<IDamager>();
    }
}
