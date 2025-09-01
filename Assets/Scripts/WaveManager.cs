using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public RobotManager robotManager;
    public PlayerManager playerManager;
    public LightManager lightManager;
    public Vector2[] spawnPositions;
    public GameObject monsterPrefab;
    public int[] amountMonsterPerWave;
    public float spawnDelay;
    public float waveDelay;
    int waveIndex = -1;
    int monsterAmount;
    int monsterKilled;

    public void OnEnable(){
        NextWave();
    }

    void NextWave(){
        waveIndex++;
        if (waveIndex >= amountMonsterPerWave.Length){
            robotManager.Unactive();
            return;
        }

        lightManager.SpecialEffect();
        monsterAmount = 0;
        monsterKilled = 0;
        MonsterSpawn();
    }

    void MonsterSpawn(){
        if (monsterAmount >= amountMonsterPerWave[waveIndex]) return;

        GameObject obj = Instantiate(monsterPrefab);
        obj.SetActive(true);
        int rdmIndex = Random.Range(0, spawnPositions.Length);
        obj.transform.position = spawnPositions[rdmIndex];
        monsterAmount++;

        MonsterTrigger monsterTrigger = obj.GetComponent<MonsterTrigger>();
        monsterTrigger.Ignite();
        monsterTrigger.vortexTransform = lightManager.transform;
        monsterTrigger.playerManager = playerManager;
        
        lightManager.currentMonster.Add(monsterTrigger);
        Invoke("MonsterSpawn", spawnDelay);
    }

    public void MonsterKilled(){
        monsterKilled++;
        if (monsterKilled < amountMonsterPerWave[waveIndex]) return;

        lightManager.RemoveEffect();
        Invoke("NextWave", waveDelay);
    }
}
