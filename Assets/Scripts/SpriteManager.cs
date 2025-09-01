using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public SpriteRenderer sr_main;
    public SpriteRenderer sr_outline;
    public Sprite outlineSprite;
    public Sprite normalSprite;
    public Sprite endSprite;
    public Sprite[] loadingSprite;
    int loadingIndex;
    float duration;
    SfxManager sfxManager;

    void PlaySoundTorchClick(){
        if (sfxManager != null) sfxManager.AddSoundsSource("torchClick");
    }

    void Awake(){
        GameObject sfxManagerObject = GameObject.Find("SfxManager");
        if (sfxManagerObject != null) sfxManager = sfxManagerObject.GetComponent<SfxManager>();
    }

    public void DisplayOutlineSprite(){
        sr_outline.sprite = outlineSprite;
    }
    public void HideOutlineSprite(){
        sr_outline.sprite = null;
    }

    public void StartLoading(float duration){
        CancelInvoke("LoadNextSprite");
        this.duration = duration/(loadingSprite.Length);
        loadingIndex = 0;
        sr_main.sprite = loadingSprite[loadingIndex];
        Invoke("LoadNextSprite", this.duration);
    }
    void LoadNextSprite(){
        loadingIndex++;
        if (loadingIndex < loadingSprite.Length){
            PlaySoundTorchClick();
            sr_main.sprite = loadingSprite[loadingIndex];
            Invoke("LoadNextSprite", this.duration);
            return;
        }
        sr_main.sprite = normalSprite;
    }
    public void Stop(){
        CancelInvoke("LoadNextSprite");
        if (sr_main != null) sr_main.sprite = endSprite;
        if (sr_outline != null) sr_outline.enabled = false;
    }
    public void Normal(){
        sr_main.sprite = normalSprite;
    }
}
