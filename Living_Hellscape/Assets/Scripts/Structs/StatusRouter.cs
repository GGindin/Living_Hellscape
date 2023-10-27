using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatusRouter : MonoBehaviour
{
    [SerializeField]
    StatusRoute target;

    public StatusRoute Target
    {
        get
        {
            Debug.Assert(target != null);
            return target;
        }
    }
}
