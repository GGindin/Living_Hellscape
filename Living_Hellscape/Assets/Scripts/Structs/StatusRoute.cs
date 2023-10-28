using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusRoute : MonoBehaviour
{
    IStatuser statuser;

    public IStatuser Statuser => statuser;

    private void Awake()
    {
        statuser = GetComponent<IStatuser>();
    }
}
