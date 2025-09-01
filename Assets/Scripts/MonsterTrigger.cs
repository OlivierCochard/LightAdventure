using UnityEngine;

public class MonsterTrigger : MonoBehaviour
{   
    public PlayerManager playerManager;
    public LightManager lightManager;
    public Transform vortexTransform;
    public Sprite ignitedSprite;
    public Sprite deathSprite;
    public Animator attachedAnimator;
    public float forceJump;
    public float delayBetweenJump;
    Rigidbody2D rb;
    SpriteRenderer sr;

    bool ignited;
    bool canJump;

    public void Ignite(){
        if (ignited) return;
        ignited = true;
        if (lightManager != null) lightManager.RefreshLight();
        Invoke("CanJumpTrue", delayBetweenJump);
        
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        sr.sprite = ignitedSprite;
        rb.bodyType = RigidbodyType2D.Dynamic;

        if (attachedAnimator != null) attachedAnimator.SetBool("isOpen", true);
    }

    public bool IsIgnited(){ return ignited; }

    void FixedUpdate(){
        if (ignited == false || canJump == false) return;

        WantJump();
    }

    int GetSide(){
        if (vortexTransform.position.x < transform.position.x) return -1;
        if (vortexTransform.position.x > transform.position.x) return +1;
        return 0;
    }
    void WantJump(){
        int side = GetSide();
        if (side == 0) return;
        sr.flipX = side == -1;

        canJump = false;
        Invoke("CanJumpTrue", delayBetweenJump);
        rb.AddForce(new Vector2(side, 1).normalized * forceJump, ForceMode2D.Impulse);
    }
    void CanJumpTrue(){
        canJump = true;
    }

    void OnTriggerEnter2D(Collider2D col){
        if (col.tag == "Vortex"){
            Invoke("CancelIgnite", .25f);
        }
    }
    void OnCollisionEnter2D(Collision2D col){
        if (col.transform.tag == "Player"){
            playerManager.Kill();
        }
    }

    public void Kill(){
        ignited = false;
        sr.sprite = deathSprite;
    }

    void CancelIgnite(){
        if (attachedAnimator != null) attachedAnimator.SetBool("isOpen", false);

        Disappearance();
    }
    void Disappearance(){
        CancelInvoke("Disappearance");
        GetComponent<Animator>().enabled = true;
    }

    public void Delete(){
        ignited = false;
        lightManager.RefreshLight(true);
        playerManager.Lose(gameObject);
        Destroy(gameObject, 0.5f);
    }

    /*
    void ResetVelocity(){
        rb.velocity = 
    }
    */
}
