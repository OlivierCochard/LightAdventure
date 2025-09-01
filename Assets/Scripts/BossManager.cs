using UnityEngine;

public class BossManager : MonoBehaviour
{
    public PlayerManager playerManager;
    public GameObject bossObject;
    public GameObject bossLeftHandObject;
    public GameObject bossRightHandObject;
    public GameObject wall;
    public float bossMovementSpeed;

    public Vector2 attackTime;
    public Vector2 chillTime;
    public Vector2 stopAttackTime;
    public Vector2 prepareAttackTime;

    public GameObject megaBallPrefab;
    public GameObject ballPrefab;
    public GameObject hidePrefab;
    public Transform leftEyeTransform;
    public Transform rightEyeTransform;

    public GameObject vortex;

    public Sprite[] sprites;

    int life;
    GameObject megaBall;
    GameObject[] balls;
    GameObject[] hides;

    Collider2D leftCollider;
    Collider2D rightCollider;

    Rigidbody2D bossRb;
    Animator animator;
    bool prepare;
    int attack;
    bool focus;

    Vector3 leftHandTarget;
    Vector3 rightHandTarget;
    Vector3 leftHandStart;
    Vector3 rightHandStart;

    float handMoveTimer = 0f;
    float handMoveDuration;
    bool handsAreMoving = false;

    void Awake()
    {
    	balls = new GameObject[2];
    	hides = new GameObject[2];

        animator = GetComponent<Animator>();
        bossRb = bossObject.GetComponent<Rigidbody2D>();
        leftCollider = bossLeftHandObject.GetComponent<Collider2D>();
        rightCollider = bossRightHandObject.GetComponent<Collider2D>();
        life = 3;
    }

    public void UseBalls(){
	    for (int i = 0; i < balls.Length; i++){
	        if (balls[i] != null){
	            Destroy(balls[i]);
	            balls[i] = null; // ✅ évite que les anciennes soient considérées existantes
	        }
	    }
	}
    public void UseMegaBall(){
		if (megaBall != null) Destroy(megaBall);
    }
    void CalculDamage(){
    	int res = 0;
    	foreach (GameObject hide in hides){
			if (hide != null) res++;
		}
		if (res < 2) return;

		foreach (GameObject hide in hides){
			if (hide != null) Destroy(hide);
		}
		hides = new GameObject[2];

		Freeze();
    }
    public void ApplyDamage(GameObject obj){
    	print("gotDamage");
    	if (obj.transform == leftEyeTransform){
    		if (hides[0] != null) return;
    		GameObject hide = Instantiate(hidePrefab);
    		hide.transform.SetParent(leftEyeTransform);
    		hide.transform.localPosition = Vector3.zero;
    		hides[0] = hide;
    	}
    	else {
			if (hides[1] != null) return;
    		GameObject hide = Instantiate(hidePrefab);
    		hide.transform.SetParent(rightEyeTransform);
    		hide.transform.localPosition = Vector3.zero;
    		hides[1] = hide;
    	}

    	CalculDamage();
    }
    public void ApplyMegaDamage(){
	    life--;
	    print("life: " + life);
	    bossObject.GetComponent<SpriteRenderer>().sprite = sprites[life];
	    
	    CancelInvoke(); // ✅ Tout clean pour éviter un enchaînement inattendu

	    foreach (GameObject hide in hides){
	        if (hide != null) Destroy(hide);
	    }
	    hides = new GameObject[2];

	    if (life <= 0){
	    	vortex.SetActive(true);
	        return;
	    } 

	    CancelInvoke("Chill");
	    Invoke("Chill", 1f); // ✅ Petit délai pour relancer calmement la boucle
	    SetHandTargets(1f);
	}


    void StopAnimation()
    {
        animator.runtimeAnimatorController = null;
        bossObject.transform.position = new Vector3(0, 1.5f);
        playerManager.transform.position = new Vector3(0, -3.5f);
        wall.transform.position = new Vector3(-8.5f, 0);
        bossRb.simulated = true;
        playerManager.GetComponent<PlayerTrigger>().enabled = true;
        playerManager.GetComponent<PlayerMovement>().enabled = true;
        playerManager.GetComponent<PlayerShoot>().enabled = true;

        PrepareAttack();
    }

    void PrepareAttack()
	{
	    if (life <= 0) return; // ✅ Ne rien faire si boss mort

	    print("PREPARE");

	    leftCollider.gameObject.layer = LayerMask.NameToLayer("Boss");
	    rightCollider.gameObject.layer = LayerMask.NameToLayer("Boss");

	    focus = true;
	    prepare = true;
	    SetHandTargets(prepareAttackTime.x);
	    Invoke("Attack", Random.Range(prepareAttackTime.x, prepareAttackTime.y));
	}


    void Attack()
    {
        print("ATTACK");

        leftCollider.isTrigger = true;
        rightCollider.isTrigger = true;
        leftCollider.tag = "Monster";
        rightCollider.tag = "Monster";

        attack = 1;
        focus = false;
        SetHandTargets(attackTime.x);
        Invoke("StopAttack", Random.Range(attackTime.x, attackTime.y));
    }

    void StopAttack()
    {
        print("STOP");

        leftCollider.isTrigger = false;
        rightCollider.isTrigger = false;
        leftCollider.tag = "Untagged";
        rightCollider.tag = "Untagged";

        GameObject obj = Instantiate(ballPrefab);
        obj.transform.SetParent(leftCollider.transform);
        obj.transform.localPosition = Vector3.up * .5f;
        balls[0] = obj;

        obj = Instantiate(ballPrefab);
        obj.transform.SetParent(rightCollider.transform);
        obj.transform.localPosition = Vector3.up * .5f;
        balls[1] = obj;

        attack = -1;
        prepare = false;
        SetHandTargets(stopAttackTime.x);
        Invoke("Chill", Random.Range(stopAttackTime.x, stopAttackTime.y));
    }

    void Chill()
	{
	    if (life <= 0) return;

	    print("CHILL");

	    leftCollider.gameObject.layer = LayerMask.NameToLayer("Default");
	    rightCollider.gameObject.layer = LayerMask.NameToLayer("Default");
	    leftCollider.isTrigger = true;
	    rightCollider.isTrigger = true;

	    focus = false;
	    attack = 0;
	    SetHandTargets(chillTime.x);
	    Invoke("PrepareAttack", Random.Range(chillTime.x, chillTime.y));
	}

    public void Freeze(){
	    focus = false;
	    attack = 0; // ✅ remets la main en position de repos
	    SetHandTargets(0f);

	    GameObject obj = Instantiate(megaBallPrefab);
	    obj.transform.SetParent(transform);
	    obj.transform.localPosition = Vector3.up * -2f;
	    megaBall = obj;

	    Destroy(obj, 6);
	    CancelInvoke();
	    Invoke("Chill", 6);
	}


    void SetHandTargets(float duration)
    {
        float leftY, rightY;

        if (attack == 1 || attack == -1)
        {
            leftY = -2f;
            rightY = -2f;
        }
        else
        {
            leftY = 0.75f;
            rightY = 0.75f;
        }

        // 🔹 Offsets horizontaux (x) relatifs au boss, mais Y fixé
        Vector3 leftTarget = new Vector3(bossObject.transform.position.x - (prepare ? 1.75f : 3.85f), leftY);
        Vector3 rightTarget = new Vector3(bossObject.transform.position.x + (prepare ? 1.75f : 3.85f), rightY);

        // 🔹 Enregistre la position de départ des mains
        leftHandStart = bossLeftHandObject.transform.position;
        rightHandStart = bossRightHandObject.transform.position;

        // 🔹 Enregistre la cible absolue (avec y contrôlé, x relatif)
        leftHandTarget = leftTarget;
        rightHandTarget = rightTarget;

        handMoveTimer = 0f;
        handMoveDuration = duration;
        handsAreMoving = true;
    }

    void FixedUpdate()
	{
	    if (focus)
	    {
	        float distance = playerManager.transform.position.x - bossObject.transform.position.x;
	        if (Mathf.Abs(distance) < 0.5f)
	        {
	            bossRb.linearVelocity = Vector2.zero;
	            return;
	        }

	        float direction = distance > 0 ? 1 : -1;
	        bossRb.linearVelocity = new Vector2(direction, 0) * bossMovementSpeed;
	    }
	    else
	    {
	        bossRb.linearVelocity = Vector2.zero;  // <--- ARRÊTE le boss quand il ne focus plus
	    }
	}


    void Update()
    {
        if (handsAreMoving)
        {
            handMoveTimer += Time.deltaTime;
            float t = Mathf.Clamp01(handMoveTimer / handMoveDuration);

            bossLeftHandObject.transform.position = Vector3.Lerp(leftHandStart, leftHandTarget, t);
            bossRightHandObject.transform.position = Vector3.Lerp(rightHandStart, rightHandTarget, t);

            if (t >= 1f)
                handsAreMoving = false;
        }
    }
    void LateUpdate()
	{
	    Vector3 pos = bossObject.transform.position;
	    pos.x = Mathf.Clamp(pos.x, -5f, 5f);
	    bossObject.transform.position = pos;
	}
}
