using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public PlayerTrigger playerTrigger;
    public List<Light> currentLight;
    public List<MonsterTrigger> currentMonster;
    public UnityEngine.Rendering.Universal.Light2D globalLight;
    public float maxIntensity;
    public float minIntensity;
    public Color endColor;

    public bool monsterActivedByLight;
    public List<Animator> attachedAnimatorList;

    SfxManager sfxManager;
    bool ignited;
    bool hasMonster;
    bool end;

    void Start(){
        GameObject sfxManagerObject = GameObject.Find("SfxManager");
        if (sfxManagerObject != null) sfxManager = sfxManagerObject.GetComponent<SfxManager>();
        RefreshLight(false);
    }
    public void RefreshLight(bool safeMode = false){
        if (monsterActivedByLight){
            int lightAmount = 0;
            foreach (Light light in currentLight){
                if (light.IsIgnited()) lightAmount++;
            }

            if (lightAmount == currentLight.Count){
                foreach (MonsterTrigger monster in currentMonster){
                    monster.Ignite();
                }

                foreach (Animator animator in attachedAnimatorList){
                    if (animator != null) animator.SetBool("isOpen", true);
                }
            }
        }

        hasMonster = false;
        foreach (MonsterTrigger monster in currentMonster){
            if (monster.IsIgnited()){
                ignited = true;
                hasMonster = true;

                foreach (Light light in currentLight){
                    light.CancelIgniteSafeMode();
                }

                SoftKill();
                return;
            }
            else globalLight.color = Color.white;
        }
        if (sfxManager != null) sfxManager.StopMusicSource();

        foreach (Light light in currentLight){
            if (light.IsIgnited()){
                globalLight.intensity = maxIntensity;
                ignited = true;
                return;
            }
        }

        ignited = false;
        globalLight.intensity = minIntensity;
        if (safeMode == false) playerTrigger.CheckUnderLight();
    }

    public void Kill(bool enemyToo = true){
        end = true;
        globalLight.intensity = minIntensity;
        foreach (Light light in currentLight){
            light.GetComponent<SpriteManager>().Normal();
            foreach (Transform lightChildren in light.transform){
                Destroy(lightChildren.gameObject);
            }
        }

        if (enemyToo == false) return;
        foreach (MonsterTrigger monster in currentMonster){
            if (monster == null) continue;

            foreach (Transform monsterChildren in monster.transform){
                Destroy(monsterChildren.gameObject);
            } 
        }
    }
    void SoftKill(){
        foreach (Light light in currentLight){
            if (light != null && light.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>() != null) light.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>().enabled = false;
        }
        foreach (MonsterTrigger monster in currentMonster){
            if (monster == null || monster.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>() == null) continue;
            monster.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>().enabled = false;
        }
        SpecialEffect();
    }
    public void SpecialEffect(){
        sfxManager.PlayMusicSource();
        globalLight.intensity = 10;
        globalLight.color = endColor;
    }
    public void RemoveEffect(){
        sfxManager.StopMusicSource();
        globalLight.intensity = maxIntensity;
        globalLight.color = Color.white;
    }


    public bool IsEnd(){ return end; }
    public bool IsIgnited(){ return ignited; }
    public bool HasMonster(){ return hasMonster; }
}
