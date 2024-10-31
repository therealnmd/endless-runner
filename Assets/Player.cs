using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private bool isDead;

    public bool runBegun;
    public bool isGrounded;
    public bool wallDetected;

    

    private bool canDoubleJump;

    [Header("Move info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedMulitiplier;
    private float defaultSpeed;
    [SerializeField] private float milestoneIncreaser;
    private float defaultMilestoneIncreaser;
    private float speedMilestone;

    [Header("Jump info")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;

    //[Header("Slide info")]
    //[SerializeField] private float slideSpeed;
    //[SerializeField] private float slideTimer;
    //[SerializeField] private float slideCoolDown;
    //private float slideCoolDownCount;
    //private float slideTimeCount;
    //private bool isSliding;
    


    [Header("Collision info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsDown;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize;
    [HideInInspector] public bool ledgeDetected;


    [Header("Knockback info")]
    [SerializeField] private Vector2 knockbackDir;
    private bool isKnocked;
    private bool canBeKnocked = true;

    //[Header("Ledge info")]
    //[SerializeField] private Vector2 offset1;
    //[SerializeField] private Vector2 offset2;
    //private Vector2 climbBegunPosition;
    //private Vector2 climbOverPosition;
    //private bool canGrabLedge = true;
    //private bool canClimb;
    // Start is called before the first frame update

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        speedMilestone = milestoneIncreaser;
        defaultSpeed = moveSpeed;
        defaultMilestoneIncreaser = milestoneIncreaser;

        
        //ở Start có thể dùng để assign default value, assign components
        //Hàm Start sẽ chỉ chạy 1 lần đầu tiên khi gọi và chỉ duy nhất 1 lần đó.
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollision();
        AnimatorControllers();

        if (Input.GetKeyDown(KeyCode.K))
            KnockBack();

        if (Input.GetKeyDown(KeyCode.O))
            StartCoroutine(Die());

        if (isDead)
        {
            return;
        }

        if (isKnocked)
            return;

        if (runBegun == true)
            CheckMove();

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        //slideTimeCount = slideTimeCount - Time.deltaTime;
        //slideCoolDownCount = slideCoolDownCount - Time.deltaTime;

        //CheckForSlide();
        SpeedController();
        CheckInput();
    }

    private IEnumerator Die()
    {
        isDead = true;
        canBeKnocked = false;
        rb.velocity = knockbackDir;
        anim.SetBool("isDead", true);

        yield return new WaitForSeconds(.5f);
        rb.velocity = new Vector2(0, 0);
        
    }

    //private void CheckForSlide()
    //{
    //    if (slideTimeCount < 0)
    //        isSliding = false;

    //}

    private IEnumerator Invincibility()
    {
        Color originalColor = sr.color;
        Color knockbackColor = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a / 5);

        canBeKnocked = false;
        sr.color = knockbackColor;
        yield return new WaitForSeconds(.1f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.1f);

        sr.color = knockbackColor;
        yield return new WaitForSeconds(.15f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.25f);

        sr.color = knockbackColor;
        yield return new WaitForSeconds(.35f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.45f);

        sr.color = knockbackColor;
        yield return new WaitForSeconds(.5f);

        canBeKnocked = true;
        sr.color = originalColor;
    }

    private void KnockBack()
    {
        if (canBeKnocked == false)
        {
            return;
        }
        StartCoroutine(Invincibility());
        isKnocked = true;
        rb.velocity = knockbackDir;
    }

    private void CancelKnockback()
    {
        isKnocked = false;
    }

    private void SpeedController()
    {
        if (moveSpeed == maxSpeed)
        {
            return;
        }

        if (transform.position.x > speedMilestone)
        {
            speedMilestone += milestoneIncreaser;

            moveSpeed *= speedMulitiplier;
            milestoneIncreaser *= speedMulitiplier;

            if (moveSpeed > maxSpeed)
            { 
                moveSpeed = maxSpeed;
            }
        }
    }

    private void SpeedReset()
    {
        moveSpeed = defaultSpeed;
        milestoneIncreaser = defaultMilestoneIncreaser;
    }

    //private void CheckForLedge()
    //{
    //    if (ledgeDetected && canGrabLedge)
    //    {
    //        canGrabLedge = false;

    //        Vector2 ledgePosition = GetComponentInChildren<LedgeDetection>().transform.position;
    //        climbBegunPosition = ledgePosition + offset1;
    //        climbOverPosition = ledgePosition + offset2;

    //        canClimb = true;
    //    }

    //    if (canClimb)
    //    {
    //        transform.position = climbBegunPosition;
    //    }
    //}
    private void CheckMove()
    {
        //if (isSliding)
        //{
        //    rb.velocity = new Vector2(slideSpeed, rb.velocity.y);
        //}
        //else
        
        if (wallDetected==true)
        {
            SpeedReset();
            return;
        }

        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        
        //rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    private void AnimatorControllers()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("canDoubleJump", canDoubleJump);
        //anim.SetBool("canClimb", canClimb);
        anim.SetBool("isKnocked", isKnocked);
        
        //anim.SetBool("isSliding", isSliding);
    }

    private void CheckCollision()
    {
        //kiểm tra xem player có đang chạm ground không?
        //Physics2D.Raycast: Hàm Raycast trong Physics2D gửi ra một tia (ray) từ một vị trí cụ thể,
        //theo một hướng nhất định, và với một khoảng cách nhất định.
        //Nếu tia này va chạm với một bề mặt hoặc đối tượng trong khoảng cách đã định,
        //hàm sẽ trả về true, nếu không, sẽ trả về false
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsDown);
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallCheckSize, 0, Vector2.zero, 0, whatIsDown);
        
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.D))
            runBegun = true;

        if (Input.GetKeyDown(KeyCode.Space)) //gọi phương thức bằng keycode.phím
            JumpButton();

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    SlideButton();
        //}
    }

    //private void SlideButton()
    //{
    //    if (rb.velocity.x != 0 && slideCoolDownCount < 0)
    //    {
    //        isSliding = true;
    //        slideTimeCount = slideTimer;
    //        slideCoolDownCount = slideCoolDown;
    //    }
    //}

    private void JumpButton()
    {
        if (isGrounded)
        {
            //kiểm tra nếu đang chạm đất thì có thể thực hiện nhảy đôi            
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else if (canDoubleJump)
        {            
            //sau khi nhảy lần 1 thì candoublejump từ true -> false và cho phép nhảy thêm 1 lần nữa
            //tạo lần nhảy thứ 2 (double jump) và khi này candoublejump đang là false, k thể thực hiện
            //nhảy đôi nữa. Cần chạm đất thì thực hiện lại
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
            canDoubleJump = false;            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }
}
