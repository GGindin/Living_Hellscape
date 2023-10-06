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
        MoveByUserInput();
    }

    public void SetTarget(Vector3 target)
    {
        StartCoroutine(TransitionRoom(target));
    }

    void MoveByUserInput()
    {
        if (!hasControl) return;

        Vector2 velocity = normInput * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + velocity);
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

        return Vector2.zero;
    }

    IEnumerator TransitionRoom(Vector3 target)
    {
        hasControl = false;
        Vector3 startPos = rb.position;

        float duration = 2f;
        float currentTime = 0f;

        while(currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float t = Mathf.InverseLerp(0f, duration, currentTime);
            rb.position = Vector3.Lerp(startPos, target, t);
            yield return null;
        }

        rb.position = target;
        hasControl = true;
        GameController.instance.EndRoomTransition();
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
