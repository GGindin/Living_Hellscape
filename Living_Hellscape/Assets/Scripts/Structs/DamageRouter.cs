using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageRouter : MonoBehaviour
{
    [SerializeField]
    DamageRoute target;

    public DamageRoute Target
    {
        get
        {
            Debug.Assert(target != null);
            return target;
        }
    }
}
