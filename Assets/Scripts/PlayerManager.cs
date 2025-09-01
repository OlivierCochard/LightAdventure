using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public UIManager uiManager;
    public Sprite loseSprite;
    public Sprite workingSprite;
    public Sprite normalSprite;
    public LevelManager levelManager;
    public GameObject headDeathPrefab;
    public GameObject bodyDeathPrefab;
    public LightManager lightManager;
    bool dead;
    SfxManager sfxManager;

    void PlaySoundDeath(){
        if (sfxManager != null) sfxManager.AddSoundsSource("death");
    }
    void PlaySoundLose(){
        if (sfxManager != null) sfxManager.AddSoundsSource("lose");
    }

    void Awake(){
        GameObject sfxManagerObject = GameObject.Find("SfxManager");
        if (sfxManagerObject != null) sfxManager = sfxManagerObject.GetComponent<SfxManager>();
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (uiManager.IsActivated()){
                uiManager.UnactiveAllInGame();
            }
            else {
                uiManager.ActiveAllInGame();
            }
        }
    }

    public void HasStarted(){
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        Animator animator = GetComponentInChildren<Animator>();
        Rigidbody2D rb = GetComponentInChildren<Rigidbody2D>();
        
        animator.SetBool("hasStarted", true);
        animator.Update(0f);
        rb.simulated = true;
        Vector3 pos = sr.transform.position;
        sr.transform.localPosition = Vector3.zero;
        transform.position = pos;

        GetComponent<PlayerMovement>().enabled = true;
    }

    public void Kill(){
        print("kill");
        PlaySoundDeath();
        dead = true;
        lightManager.Kill();

        LineRenderer lr = GetComponent<LineRenderer>();
        if (lr != null) lr.enabled = false;
        
        Destroy(GetComponent<PlayerMovement>());
        Destroy(GetComponent<PlayerTrigger>());
        Destroy(GetComponent<Rigidbody2D>());
        foreach (Transform child in transform){
            Destroy(child.gameObject);
        }

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders){
            if (collider != null){
                Destroy(collider);
            }
        }

        GameObject obj = Instantiate(headDeathPrefab);
        obj.transform.position = transform.position;
        obj.transform.SetParent(transform);
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(0f, 1f);
        obj.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(x, y).normalized * 5f;

        obj = Instantiate(bodyDeathPrefab);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        Invoke("ResetLevel", 2f);
    }

    public void Lose(GameObject go){
        if (dead) return;
        PlaySoundLose();
        lightManager.Kill(false);

        GetComponent<LineRenderer>().enabled = false;
        Destroy(GetComponent<PlayerMovement>());
        Destroy(GetComponent<PlayerShoot>());
        Destroy(GetComponent<Animator>());
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        Transform obj = go.transform.GetChild(0).transform;
        obj.SetParent(transform);
        obj.position = transform.position;
        obj.GetComponent<Collider2D>().enabled = true;
        obj.GetComponent<UnityEngine.Rendering.Universal.Light2D>().enabled = true;

        GetComponentInChildren<SpriteRenderer>().sprite = loseSprite;
        Invoke("ResetLevel", 2f);
    }

    public void UseRobot(){
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerShoot>().enabled = false;
        GetComponentInChildren<SpriteRenderer>().sprite = workingSprite;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        transform.position = new Vector3(0, transform.position.y);
    }
    public void CancelRobot(){
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<PlayerShoot>().enabled = true;
        GetComponentInChildren<SpriteRenderer>().sprite = normalSprite;
    }

    void ResetLevel(){
        levelManager.ResetLevel();
    }
}
