using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWind : GhostEquipment
{
    [SerializeField]
    WindProjectile windProjectilePrefab;

    [SerializeField]
    Scare scare;

    [SerializeField]
    Stun stun;

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
        if (coolDown <= 0f)
        {
            PlayerManager.Instance.Active.StartCoroutine(PlayerManager.Instance.Active.StopControlForTime(.25f));
            var wind = Instantiate(windProjectilePrefab, transform.position, Quaternion.LookRotation(direction, Vector3.back));
            wind.SetStun(new Stun(stun));
            wind.SetScare(new Scare(scare));
            wind.transform.SetParent(RoomController.Instance.ActiveRoom.DynamicObjectsHolder, true);
            wind.Direction = direction;
            coolDown = fireRate;
        }
    }

    private void Update()
    {
        if (coolDown > 0)
        {
            coolDown -= Time.deltaTime;
        }
    }
}
