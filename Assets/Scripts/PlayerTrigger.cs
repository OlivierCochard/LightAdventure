using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public LevelManager levelManager;
    public LightManager lightManager;
    public PlayerManager playerManager;

    [Header("Contrôles")]
    public KeyCode[] useKeys;

    GameObject vortexObject;
    GameObject lightObject;
    Computer computer;
    RobotManager robotManager;

    bool underLight;
    bool died;
    SfxManager sfxManager;

    void PlaySoundWin(){
        if (sfxManager != null) sfxManager.AddSoundsSource("win");
    }
    void Awake(){
        GameObject sfxManagerObject = GameObject.Find("SfxManager");
        if (sfxManagerObject != null) sfxManager = sfxManagerObject.GetComponent<SfxManager>();
    }

    public void CheckUnderLight(){
        if (underLight == false && lightManager.IsIgnited() == false && died == false){
            playerManager.Kill();
            died = true;
        }
    }

    void OnTriggerEnter2D(Collider2D col){
        if (lightManager.IsEnd()) return;

        if (col.tag == "Monster"){
            Debug.Log("Touched by: " + col.name);
            playerManager.Kill();
        }
        

        if (col.tag == "LightArea"){
            underLight = true;
        }

        if (col.tag == "Vortex"){
            vortexObject = col.gameObject;
            if (vortexObject == null || vortexObject.Equals(null)) return;
            if (vortexObject.TryGetComponent<SpriteManager>(out var spriteManager)) {
                spriteManager.DisplayOutlineSprite();
            }
        }
        if (col.tag == "Light"){
            lightObject = col.gameObject;
            if (lightObject == null || lightObject.Equals(null)) return;
            if (lightObject.TryGetComponent<SpriteManager>(out var spriteManager)) {
                spriteManager.DisplayOutlineSprite();
            }
        }
        if (col.tag == "Computer"){
            computer = col.GetComponent<Computer>();
            computer.Trigger(true);
        }
        if (col.tag == "Robot"){
            robotManager = col.GetComponent<RobotManager>();
            robotManager.Trigger(true);
        }
    }
    void OnTriggerExit2D(Collider2D col){
        if (lightManager.IsEnd()) return;

        if (col.CompareTag("LightArea")){
            Vector2 position = (Vector2)transform.position + new Vector2(0f, -0.0575f);
            Vector2 size = new Vector2(0.625f, 0.75f);
            float angle = 0f;

            ContactFilter2D filter = new ContactFilter2D();
            filter.useTriggers = true;
            filter.SetLayerMask(LayerMask.GetMask("Default"));
            filter.useLayerMask = true;

            Collider2D[] results = new Collider2D[10];
            int count = Physics2D.OverlapCapsule(position, size, CapsuleDirection2D.Vertical, angle, filter, results);
            bool foundLight = false;
            for (int i = 0; i < count; i++){
                if (results[i].CompareTag("LightArea")){
                    foundLight = true;
                    break;
                }
            }

            if (!foundLight){
                underLight = false;
                CheckUnderLight();
            }
        }
        if (col.tag == "Vortex"){
            col.gameObject.GetComponent<SpriteManager>().HideOutlineSprite();
            vortexObject = null;
        }
        if (col.tag == "Light"){
            col.gameObject.GetComponent<SpriteManager>().HideOutlineSprite();
            lightObject = null;
        }
        if (col.tag == "Computer"){
            if (computer != null) computer.Trigger(false);
            computer = null;
        }
        if (col.tag == "Robot"){
            if (robotManager != null) robotManager.Trigger(false);
            robotManager = null;
        }
    }

    void Update(){
        bool wantUse = GetInputUse();
        if (wantUse == false) return;

        if (robotManager != null){
            robotManager.Activate();
        }

        if (computer != null){
            computer.Activate();
        }

        if (vortexObject != null){
            PlaySoundWin();
            Invoke("NextLevel", 1f);
        }

        if (lightObject != null){
            Light light = lightObject.GetComponent<Light>();
            if (light != null){
                light.Ignite();
            }
        }
    }
    bool GetInputUse(){
        for (int i = 0; i < useKeys.Length; i++) {
            if (Input.GetKey(useKeys[i])) return true;
        }
        return false;
    }

    bool GetUnderLight(){ return underLight; }

    void NextLevel(){
        levelManager.NextLevel();
    }
}
