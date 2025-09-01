using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int currentLevelIndex;
    public bool isLastLevel;

    public void StartLevel(int levelIndex){
        SceneManager.LoadScene(levelIndex + 1);
    }
    public void MainMenu(){
        SceneManager.LoadScene("menu");
    }

    public void ResetLevel(){
        StartLevel(currentLevelIndex);
    }
    public void NextLevel(){
        if (isLastLevel){
            MainMenu();
            return;
        }

        int nextLevelIndex = currentLevelIndex + 1;
        if (PlayerPrefs.GetInt("Level", 0) < nextLevelIndex) PlayerPrefs.SetInt("Level", nextLevelIndex);
        PlayerPrefs.Save();

        StartLevel(nextLevelIndex);
    } 
}
