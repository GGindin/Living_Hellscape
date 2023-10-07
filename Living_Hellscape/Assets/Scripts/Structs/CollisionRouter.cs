using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CollisionRouter : MonoBehaviour
{
    [SerializeField]
    GameObject target;

    public GameObject Target
    {
        get
        {
            Debug.Assert(target != null);
            return target;
        }
    }
}
