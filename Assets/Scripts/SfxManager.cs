using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public AudioSource soundsSource;
    public AudioSource musicSource;

    public AudioClip colliderProjectile;//
    public AudioClip pressurePlate;//
    public AudioClip footSteps;//

    public AudioClip torchClick;//
    public AudioClip torchOff;//
    public AudioClip torchOn;//

    public AudioClip computer;
    public AudioClip elevator;

    public AudioClip ropeCut;//
    public AudioClip shoot;//
    public AudioClip death;//
    public AudioClip jump;//
    public AudioClip door;//
    public AudioClip drop;//
    public AudioClip lose;//
    public AudioClip win;//

    void Awake(){
        RefreshVolume();
    }
    public void RefreshVolume(){
        musicSource.volume = PlayerPrefs.GetFloat("Music", 0.5f);
        soundsSource.volume = PlayerPrefs.GetFloat("Sounds", 0.5f);
    }

    public void PlayMusicSource(){
        musicSource.Play();
    }

    public void StopMusicSource(){
        musicSource.Stop();
    }

    public void AddSoundsSource(string nom){
        AudioClip clipToPlay = null;

        switch (nom.ToLower()){
            case "colliderprojectile":
                clipToPlay = colliderProjectile;
                break;
            case "pressureplate":
                clipToPlay = pressurePlate;
                break;
            case "footsteps":
                clipToPlay = footSteps;
                break;

            case "torchclick":
                clipToPlay = torchClick;
                break;
            case "torchoff":
                clipToPlay = torchOff;
                break;
            case "torchon":
                clipToPlay = torchOn;
                break;

            case "elevator":
                clipToPlay = elevator;
                break;
            case "computer":
                clipToPlay = computer;
                break;

            case "ropecut":
                clipToPlay = ropeCut;
                break;
            case "shoot":
                clipToPlay = shoot;
                break;
            case "death":
                clipToPlay = death;
                break;
            case "jump":
                clipToPlay = jump;
                break;
            case "door":
                clipToPlay = door;
                break;
            case "drop":
                clipToPlay = drop;
                break;
            case "lose":
                clipToPlay = lose;
                break;
            case "win":
                clipToPlay = win;
                break;

            default:
                Debug.LogWarning("SFX introuvable : " + nom);
                return;
        }

        if (clipToPlay != null){
            soundsSource.PlayOneShot(clipToPlay);
        }
    }
}
