using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float speed;

    Vector2 normInput = new Vector2();

    Rigidbody2D rb;

    bool hasControl = true;
    Vector3 target;

    public bool HasControl => hasControl;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameController.instance.AssignPlayer(this);
    }

    // Update is called once per frame
    void Update()
    {
        normInput = GetNormInput();
    }

    void FixedUpdate()
    {
        Vector2 velocity = normInput * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + velocity);
    }

    public void SetTarget(Vector3 target)
    {
        hasControl = false;
        this.target = target;
    }


    Vector2 GetNormInput()
    {
        if (hasControl)
        {
            Vector2 input = new Vector2();
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            input.Normalize();
            return input;
        }
        else
        {
            Debug.Log("C");
            Vector2 dir = target - transform.position;
            dir.Normalize();

            var frameDist = dir * Time.fixedDeltaTime * speed;
            frameDist += rb.position;
            if((frameDist - new Vector2(target.x, target.y)).SqrMagnitude() < .1f)
            {
                hasControl = true;
                return GetNormInput();
            }

            return dir;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var door = collision.gameObject.GetComponent<Door>();
        if (door)
        {
            door.OperateDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var doorTrigger = collision.gameObject.GetComponent<DoorTrigger>();
        if (doorTrigger)
        {
            doorTrigger.SignalTrigger();
        }
    }
}
