using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatuser
{
    public StatusEffect GetStatus(DamageableObject recievingObject);
}
