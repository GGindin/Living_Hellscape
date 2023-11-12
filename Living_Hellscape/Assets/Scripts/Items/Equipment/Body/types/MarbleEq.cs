using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleEq : BodyEquipment
{
    [SerializeField]
    Projectile marbleProjectilePrefab;

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
        if(coolDown <= 0f && PlayerManager.Instance.Active.Inventory.MarbleAmmo > 0)
        {
            if (gameObject.activeInHierarchy)
            {
                PlayerManager.Instance.Active.StartCoroutine(PlayerManager.Instance.Active.StopControlForTime(.25f));
            }
            var marble = Instantiate(marbleProjectilePrefab, transform.position, Quaternion.identity);
            marble.SetDamage(new Stun(stun));
            marble.transform.SetParent(RoomController.Instance.ActiveRoom.DynamicObjectsHolder, true);
            marble.Direction = direction;
            PlayerManager.Instance.Active.Inventory.UseAmmo(1);
            coolDown = fireRate;
        }
    }

    public override void OnFirstAddToInventory()
    {
        if (!GameStateController.Instance.HasSlingShot)
        {
            GameStateController.Instance.HasSlingShot = true;
            AmmoPanelController.Instance.UpdateCount(0);
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
