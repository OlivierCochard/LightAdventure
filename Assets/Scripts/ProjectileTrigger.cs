using UnityEngine;

public class ProjectileTrigger : MonoBehaviour
{
    public Sprite ignitedSprite;
    public float timeBeforeDestroy;
    public RuntimeAnimatorController runtimeAnimatorController ;
    SpriteRenderer sr;
    SfxManager sfxManager;
    bool megaActivated;
    bool activated;
    bool end;
    bool used;

    void PlaySoundRopeCut(){
        if (sfxManager != null) sfxManager.AddSoundsSource("ropeCut");
    }
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
        if (used) return;
        if (col.tag == "Head"){
            if (megaActivated){
                used = true;
                GameObject.Find("BOSS_ROOM").GetComponent<BossManager>().ApplyMegaDamage();
                Destroy(gameObject);
            }
        }
        if (col.tag == "MegaBall"){
            megaActivated = true;
            sr.sprite = ignitedSprite;
            GameObject.Find("BOSS_ROOM").GetComponent<BossManager>().UseMegaBall();
        }
        if (col.tag == "Ball"){
            if (activated) return;

            activated = true;
            sr.sprite = ignitedSprite;

            GameObject.Find("BOSS_ROOM").GetComponent<BossManager>().UseBalls();  // ❗ Avant le Destroy
            Destroy(col.gameObject);
        }
        if (col.tag == "Eye"){
            if (activated){
                GameObject.Find("BOSS_ROOM").GetComponent<BossManager>().ApplyDamage(col.gameObject);
                Destroy(gameObject);
            }
        }



        if (col.tag == "Player" || col.tag == "LightArea") return;
        if (col.tag == "Light"){
            if (activated){
                if (col.GetComponent<Light>().IsIgnited()) return;
                col.GetComponent<Light>().Ignite();
            }
            else {
                if (!col.GetComponent<Light>().IsIgnited()) return;
                activated = true;
                sr.sprite = ignitedSprite;
            }
            return;
        }
        if (col.tag == "Monster"){
            if (activated){
                if (col.GetComponent<MonsterTrigger>().IsIgnited()) return;
                col.GetComponent<MonsterTrigger>().Ignite();
            }
            return;
        }
        if (col.tag == "Rope"){
            if (activated){
                if (col.GetComponent<SpringJoint2D>().enabled){
                    col.GetComponent<SpringJoint2D>().enabled = false;
                    PlaySoundRopeCut();
                }
            }
            return;
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
        GetComponent<Animator>().runtimeAnimatorController = runtimeAnimatorController;
    }

    public void Delete(){
        Destroy(gameObject);
    }
}
