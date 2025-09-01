using UnityEngine;

public class PressurePlateTrigger : MonoBehaviour
{
    public Sprite normalSprite;
    public Sprite activatedSprite;
    public Animator attachedAnimator;
    public bool cantBack;
    SpriteRenderer sr;

    bool activated;
    bool hasPlayer;
    bool hasMonster;
    int crateAmount;
    SfxManager sfxManager;

    void PlaySoundPressurePlate(){
        if (sfxManager != null) sfxManager.AddSoundsSource("pressurePlate");
    }
    void PlaySoundDoor(){
        if (sfxManager != null) sfxManager.AddSoundsSource("door");
    }

    void Awake(){
        GameObject sfxManagerObject = GameObject.Find("SfxManager");
        if (sfxManagerObject != null) sfxManager = sfxManagerObject.GetComponent<SfxManager>();
        sr = GetComponent<SpriteRenderer>();
    }
    void OnTriggerEnter2D(Collider2D col){
        if (col.tag == "Monster"){
            hasMonster = true;
        }
        if (col.tag == "Player"){
            hasPlayer = true;
        }
        if (col.tag == "Crate"){
            crateAmount ++;
        }
        CheckActivated();
    }
    void OnTriggerExit2D(Collider2D col){
        if (col.tag == "Monster"){
            hasMonster = false;
        }
        if (col.tag == "Player"){
            hasPlayer = false;
        }
        if (col.tag == "Crate"){
            crateAmount --;
        }
        CheckActivated();
    }

    void CheckActivated(){
        bool tmp = (hasPlayer || hasMonster || crateAmount > 0);
        if (tmp == activated) return;
        activated = tmp;

        PlaySoundPressurePlate();
        if (activated){
            sr.sprite = activatedSprite;
        }
        else {
            sr.sprite = normalSprite;
        }
        
        if (attachedAnimator != null){
            PlaySoundDoor();
            if (cantBack && activated == false) return;
            
            attachedAnimator.SetBool("isOpen", activated);
        }
    }
}
