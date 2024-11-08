using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    CapsuleCollider2D capCollider;
    GameManager gameManager;

    public int playerHealth = 3;

    public float jumpForce = 5f;
    private int jumpCount = 0;
    private bool isGrounded;
    private bool isSliding;
    private float groundCheckDistance = 1f; //나중에 더 좋은 조작감으로 수정
    private float obstacleCheckDistance = 1.5f;
    private float v;
    private Vector2 originalColliderSize;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        capCollider = GetComponent<CapsuleCollider2D>();
        gameManager = FindObjectOfType<GameManager>();
        originalColliderSize = capCollider.size;
    }

    void Update()
    {
        v = Input.GetAxisRaw("Vertical");
        bool jDown = Input.GetKeyDown(KeyCode.UpArrow);
        bool jUp = Input.GetKeyUp(KeyCode.UpArrow);
        bool sDown = Input.GetKeyDown(KeyCode.DownArrow);
        bool sUp = Input.GetKeyUp(KeyCode.DownArrow);

        // Jump
        if (jDown && jumpCount < 2)
        {
            rigid.velocity = Vector2.zero;
            rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);
            jumpCount++;
        }

        //Sliding
        if (sDown && !sUp && !isSliding) StartSliding();
        else if(!sDown || !isGrounded || sUp) StopSliding();
    }
    private void StartSliding()
    {
        isSliding = true;
        capCollider.size = new Vector2(capCollider.size.x, capCollider.size.y / 2);
        Debug.Log("슬라이딩 여부: " + isSliding);
        Debug.Log("\n" + originalColliderSize);
        Debug.Log("\n" + capCollider.size);
    }

    private void StopSliding()
    {
        isSliding = false;
        capCollider.size = originalColliderSize;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Debug.Log("충돌 발생!");
            gameManager.DecreasePlayerHealth();
        }
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(rigid.position, Vector2.down * groundCheckDistance, new Color(0, 1, 0));
        Debug.DrawRay(rigid.position, Vector2.up * groundCheckDistance, new Color(0, 1, 0));
        Debug.DrawRay(rigid.position, Vector2.right * obstacleCheckDistance, new Color(0, 1, 0));

        //check whether player is on ground
        RaycastHit2D hitDown = Physics2D.Raycast(rigid.position, Vector2.down, groundCheckDistance, LayerMask.GetMask("Ground"));
        if (hitDown.collider != null)
        {
            isGrounded = hitDown.collider.CompareTag("Ground");
            if (isGrounded)
                jumpCount = 0;
            else
                isGrounded = false;
        }

    }
}
