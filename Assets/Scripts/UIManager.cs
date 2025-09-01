using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    void Awake(){
        //PlayerPrefs.SetInt("Level", 0);
        if (isInGameMenu == false){
            ActiveMainUI();
        }
        else {
            UnactiveAllInGame();
            isActivated = false;
        }
    }

    public LevelManager levelManager;

    public GameObject MenuUI;
    public GameObject MainUI;
    public GameObject PlayUI;
    public GameObject CreditsUI;
    public GameObject SettingsUI;
    public GameObject ControlsPlayUI;
    public GameObject[] LevelUI;

    public Slider MusicSliderUI;
    public Slider SoundsSliderUI;

    public bool isInGameMenu;
    bool isActivated;

    public bool IsActivated(){ return isActivated; }
    
    public void UnactiveAllInGame(){
        isActivated = false;
        UnactiveAll();
        UnactiveMenu();
        Time.timeScale = 1f;
    }
    public void ActiveAllInGame(){
        isActivated = true;
        ActiveMainUI();
        ActiveMenu();
        Time.timeScale = 0f;
    }

    void UnactiveAll(){
        if (MainUI != null) MainUI.SetActive(false);
        if (PlayUI != null) PlayUI.SetActive(false);
        if (CreditsUI != null) CreditsUI.SetActive(false);
        if (SettingsUI != null) SettingsUI.SetActive(false);
        if (ControlsPlayUI != null) ControlsPlayUI.SetActive(false);
    }
    void UnactiveMenu(){
        if (MenuUI != null) MenuUI.SetActive(false);
    }
    void ActiveMenu(){
        if (MenuUI != null) MenuUI.SetActive(true);
    }

    public void ActiveMainUI(){
        UnactiveAll();
        MainUI.SetActive(true);
    }
    public void ActivePlayUI(){
        UnactiveAll();
        PlayUI.SetActive(true);
        for (int i = 0; i < LevelUI.Length; i++){
            LevelUI[i].GetComponent<Button>().interactable = (i <= PlayerPrefs.GetInt("Level", 0));
        }
    }
    public void ActiveCreditsUI(){
        UnactiveAll();
        CreditsUI.SetActive(true);
    }
    public void ActiveSettingsUI(){
        UnactiveAll();
        SettingsUI.SetActive(true);

        MusicSliderUI.value = PlayerPrefs.GetFloat("Music", 0.5f);
        SoundsSliderUI.value = PlayerPrefs.GetFloat("Sounds", 0.5f);
    }
    public void ActiveControlsPlayUI(){
        UnactiveAll();
        ControlsPlayUI.SetActive(true);
    }

    public void RefreshMusic(){
        PlayerPrefs.SetFloat("Music", MusicSliderUI.value);
        PlayerPrefs.Save();

        GameObject sfxManagerObject = GameObject.Find("SfxManager");
        if (sfxManagerObject != null) sfxManagerObject.GetComponent<SfxManager>().RefreshVolume();
    }
    public void RefreshSounds(){
        PlayerPrefs.SetFloat("Sounds", SoundsSliderUI.value);
        PlayerPrefs.Save();

        GameObject sfxManagerObject = GameObject.Find("SfxManager");
        if (sfxManagerObject != null) sfxManagerObject.GetComponent<SfxManager>().RefreshVolume();
    }

    public void Quit(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void LoadMainMenu(){
        levelManager.MainMenu();
    }

    public void LoadLevel_0(){
        levelManager.StartLevel(0);
    }
    public void LoadLevel_1(){
        levelManager.StartLevel(1);
    }
    public void LoadLevel_2(){
        levelManager.StartLevel(2);
    }
    public void LoadLevel_3(){
        levelManager.StartLevel(3);
    }
    public void LoadLevel_4(){
        levelManager.StartLevel(4);
    }
    public void LoadLevel_5(){
        levelManager.StartLevel(5);
    }
    public void LoadLevel_6(){
        levelManager.StartLevel(6);
    }
    public void LoadLevel_7(){
        levelManager.StartLevel(7);
    }
    public void LoadLevel_8(){
        levelManager.StartLevel(8);
    }
}
