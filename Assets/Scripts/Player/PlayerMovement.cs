using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private float moveInput;
    public bool IsGamePlaying = false;
    private Vector3 beginPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        beginPosition = transform.position;
    }

    void Update()
    {
        if (!IsGamePlaying)
            return;
        moveInput = Input.GetAxis("Horizontal");

        if (rb.velocity != Vector2.zero)
        {
            GameClient.Instance._UdpConnection.SendPlayerPosition(transform.position);
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed * Time.deltaTime, rb.velocity.y);
    }

    public void ResetPosition()
    {
        moveInput = 0;
        transform.position = beginPosition;
    }

    public void ResetMoveInput()
    {
        moveInput = 0;
    }
}