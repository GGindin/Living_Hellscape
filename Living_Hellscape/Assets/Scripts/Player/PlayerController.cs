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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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


    Vector2 GetNormInput()
    {
        Vector2 input = new Vector2();
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input.Normalize();
        return input;
    }
}
