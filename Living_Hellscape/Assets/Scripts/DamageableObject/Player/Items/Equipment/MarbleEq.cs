using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleEq : Equipment
{
    [SerializeField]
    Projectile marbleProjectilePrefab;

    [SerializeField]
    float fireRate;

    float coolDown = 0f;

    Vector2 direction;

    public override void EndAction() { }

    public override void SetDirection(Vector2 direction)
    {
        this.direction = direction;
    }

    public override void TriggerAction()
    {
        if(coolDown <= 0f)
        {
            var marble = Instantiate(marbleProjectilePrefab, transform.position, Quaternion.identity);
            marble.Direction = direction;
            coolDown = fireRate;
        }
    }

    private void Update()
    {
        if(coolDown > 0)
        {
            coolDown -= Time.deltaTime;
        }
    }
}
