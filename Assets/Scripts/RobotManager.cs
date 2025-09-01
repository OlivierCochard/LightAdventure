using UnityEngine;

public class RobotManager : MonoBehaviour
{
    public RobotShoot robotShoot;
    public PlayerManager playerManager;
    public Sprite triggerSprite;
    public Sprite normalSprite;
    public Sprite sleepSprite;
    public Sprite activatedSprite;
    public SpriteRenderer sr;
    bool activated;
    bool ready;
    SfxManager sfxManager;

    void OnEnable(){
        sr.sprite = normalSprite;
        GameObject sfxManagerObject = GameObject.Find("SfxManager");
        if (sfxManagerObject != null) sfxManager = sfxManagerObject.GetComponent<SfxManager>();
    }

    void PlaySoundElevator(){
        if (sfxManager != null) sfxManager.AddSoundsSource("elevator");
    }

    public void Activate(){
        if (ready == false || activated) return;
        activated = true;

        PlaySoundElevator();

        sr.sprite = activatedSprite;
        playerManager.UseRobot();
        robotShoot.enabled = true;
    }
    public void Unactive(){
        PlaySoundElevator();
        
        sr.sprite = sleepSprite;
        playerManager.CancelRobot();
        robotShoot.GetLineRenderer().enabled = false;
        robotShoot.enabled = false;
        GetComponent<Animator>().SetTrigger("end");
    }
    public void SetReady(){
        ready = true;
        GetComponent<Collider2D>().enabled = true;
    }

    public void Trigger(bool trigger){
        if (ready == false || activated) return;

        if (trigger){
            sr.sprite = triggerSprite;
            return;
        }
        sr.sprite = normalSprite;
        return;
    }
}
