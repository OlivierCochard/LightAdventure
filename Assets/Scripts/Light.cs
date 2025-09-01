using UnityEngine;

public class Light : MonoBehaviour
{
    public LightManager lightManager;
    public float lifeDuration;
    public float lifeDurationStart;
    public Animator attachedAnimator;
    bool ignited;
    UnityEngine.Rendering.Universal.Light2D spotLight;
    SfxManager sfxManager;

    void PlaySoundTorchOn(){
        if (sfxManager != null) sfxManager.AddSoundsSource("torchOn");
    }
    void PlaySoundTorchOff(){
        if (sfxManager != null) sfxManager.AddSoundsSource("torchOff");
    }

    void Awake(){
        GameObject sfxManagerObject = GameObject.Find("SfxManager");
        if (sfxManagerObject != null) sfxManager = sfxManagerObject.GetComponent<SfxManager>();

        spotLight = GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();
        if (lifeDurationStart > 0){
            ignited = true;
            return;
        }
    }
    void Start(){
        if (lifeDurationStart > 0){
            Invoke("CancelIgnite", lifeDurationStart);
            GetComponent<SpriteManager>().StartLoading(lifeDurationStart);
        }
    }

    public void Ignite(){
        if (ignited || lightManager.HasMonster()) return;
        PlaySoundTorchOn();

        ignited = true;
        lightManager.RefreshLight();
        GetComponent<SpriteManager>().StartLoading(lifeDuration);
        
        if (attachedAnimator != null) attachedAnimator.SetBool("isOpen", true);

        CancelInvoke("CancelIgnite");
        Invoke("CancelIgnite", lifeDuration);
    }

    public void CancelIgnite(){
        CancelInvoke("CancelIgnite");
        PlaySoundTorchOff();

        ignited = false;
        lightManager.RefreshLight(false);
        if (attachedAnimator != null) attachedAnimator.SetBool("isOpen", false);
    }
    public void CancelIgniteSafeMode(){
        CancelInvoke("CancelIgnite");
        PlaySoundTorchOff();

        ignited = false;
        if (GetComponent<SpriteManager>() != null) GetComponent<SpriteManager>().Stop();
        if (attachedAnimator != null) attachedAnimator.SetBool("isOpen", false);
    }

    public bool IsIgnited(){ return ignited; }
}
