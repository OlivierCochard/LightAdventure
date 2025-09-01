using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 8f;
    public float acceleration = 20f;
    public float deceleration = 30f;

    [Header("Jump")]
    public float jumpForce = 12f;
    public LayerMask floorLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Contrôles")]
    public KeyCode[] jumpKeys;
    public KeyCode[] leftKeys;
    public KeyCode[] rightKeys;

    private Animator animator;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private float direction;
    private bool wantJump;
    private bool isGrounded;
    private SfxManager sfxManager;


    private bool canWalkSound;

    void PlaySoundFootSteps(){
        if (sfxManager != null) sfxManager.AddSoundsSource("footsteps");
    }
    void PlaySoundJump(){
        if (sfxManager != null) sfxManager.AddSoundsSource("jump");
    }
    void PlaySoundDrop(){
        if (sfxManager != null) sfxManager.AddSoundsSource("drop");
    }

    void Awake(){
        GameObject sfxManagerObject = GameObject.Find("SfxManager");
        if (sfxManagerObject != null) sfxManager = sfxManagerObject.GetComponent<SfxManager>();
        animator = GetComponentInChildren<Animator>();
        canWalkSound = true;
    }
    void Start(){
        sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update(){
        direction = GetInputDirection();
        wantJump = GetInputJump();

        if (wantJump && IsGrounded()){
            PlaySoundJump();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void CanWalkSound(){
        canWalkSound = true;
    }
    void FixedUpdate(){
        if (direction != 0 && canWalkSound && IsGrounded()){
            canWalkSound = false;
            PlaySoundFootSteps();
            Invoke("CanWalkSound", 0.5f);
        }

        float targetSpeed = direction * maxSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;

        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        float movement = accelRate * speedDiff * Time.fixedDeltaTime;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x + movement, rb.linearVelocity.y);
        if (rb.linearVelocity.x > 0){
            sr.flipX = false;
        }
        if (rb.linearVelocity.x < 0){
            sr.flipX = true;
        }
    }

    float GetInputDirection(){
        float direction = 0f;
        int totalKeys = leftKeys.Length;

        for (int i = 0; i < totalKeys; i++) {
            if (Input.GetKey(leftKeys[i])) direction --;
            if (Input.GetKey(rightKeys[i])) direction ++;
        }
        if (direction < 0) direction = -1;
        if (direction > 0) direction = +1;

        animator.SetBool("isMoving", direction != 0);
        return direction;
    }
    bool GetInputJump(){
        for (int i = 0; i < jumpKeys.Length; i++) {
            if (Input.GetKeyDown(jumpKeys[i])) return true;
        }
        return false;
    }

    bool IsGrounded(){
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, floorLayer);
        return isGrounded;
    }

    void OnDrawGizmosSelected(){
        if (groundCheck != null){
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
