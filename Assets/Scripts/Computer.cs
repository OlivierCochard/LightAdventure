using UnityEngine;

public class Computer : MonoBehaviour
{  
    public Sprite activatedSprite;
    public Sprite triggerSprite;
    public Sprite normalSprite;

    public GameObject robotObject;
    public LightManager lightManager;

    public WaveManager waveManager;

    bool activated;
    SpriteRenderer sr;
    SfxManager sfxManager;

    void PlaySoundComputer(){
        if (sfxManager != null) sfxManager.AddSoundsSource("computer");
    }

    void Awake(){
        GameObject sfxManagerObject = GameObject.Find("SfxManager");
        if (sfxManagerObject != null) sfxManager = sfxManagerObject.GetComponent<SfxManager>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void Activate(){
        if (activated) return;
        activated = true;

        PlaySoundComputer();

        robotObject.SetActive(true);
        sr.sprite = activatedSprite;

        Invoke("ActivateLater", 2f);
    }
    void ActivateLater(){
        lightManager.SpecialEffect();
        waveManager.enabled = true;
    }

    public void Trigger(bool trigger){
        if (activated) return;

        if (trigger){
            sr.sprite = triggerSprite;
            return;
        }
        sr.sprite = normalSprite;
        return;
    }
}
