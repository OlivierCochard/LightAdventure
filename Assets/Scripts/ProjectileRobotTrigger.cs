using UnityEngine;

public class ProjectileRobotTrigger : MonoBehaviour
{
    public float timeBeforeDestroy;
    public RuntimeAnimatorController runtimeAnimatorController;
    WaveManager waveManager;
    SpriteRenderer sr;
    SfxManager sfxManager;
    bool end;
    
    void PlaySoundColliderProjectile(){
        if (sfxManager != null) sfxManager.AddSoundsSource("colliderProjectile");
    }

    void Start(){
        GameObject sfxManagerObject = GameObject.Find("SfxManager");
        if (sfxManagerObject != null) sfxManager = sfxManagerObject.GetComponent<SfxManager>();
        sr = GetComponent<SpriteRenderer>();
        Invoke("Disappearance", timeBeforeDestroy);
    }

    void OnTriggerEnter2D(Collider2D col){
        if (col.tag == "Monster"){
            col.GetComponent<MonsterTrigger>().Kill();
            waveManager.MonsterKilled();
            col.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;

            Destroy(col.GetComponent<Rigidbody2D>(), .5f);
            foreach (var collider in col.GetComponents<Collider2D>()){
                Destroy(collider, 0.5f);
            }
            Destroy(col.GetComponent<MonsterTrigger>(), .5f);

            col.tag = "Untagged";
        }
    }
    void OnCollisionEnter2D(Collision2D col){
        Disappearance();
    }

    void Disappearance(){
        if (end) return;
        end = true;

        CancelInvoke("Disappearance");
        PlaySoundColliderProjectile();
        GetComponent<Animator>().runtimeAnimatorController  = runtimeAnimatorController;
    }

    public void Delete(){
        Destroy(gameObject);
    }

    public void SetWaveManager(WaveManager waveManager){
        this.waveManager = waveManager;
    }
}
