using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;

public class AudioVol : MonoBehaviour {
    public AudioMixer mixer;
    public Image sfxButton;
    public Image musicButton;
    public Sprite sfxOn;
    public Sprite sfxOff;
    public Sprite musicOn;
    public Sprite musicOff;

    public bool SFXOn { get; private set; }
    public bool MusicOn { get; private set; }

    void Start () {
        LoadVolumePrefs();
        UpdateVolsAndSprites();
    }

    public void LoadVolumePrefs () {
        if (PlayerPrefs.HasKey("sfx")) {
            SetSFX(PlayerPrefs.GetInt("sfx") == 1);
        } else {
            SetSFX(true);
        }

        if (PlayerPrefs.HasKey("music")) {
            SetMusic(PlayerPrefs.GetInt("music") == 1);
        } else {
            SetMusic(true);
        }
    }

    public void SetSFX (bool on) {
        SFXOn = on;
        PlayerPrefs.SetInt("sfx", on ? 1 : 0);
        UpdateVolsAndSprites();
    }

    public void ToggleSFX () {
        SetSFX(!SFXOn);
    }

    public void SetMusic (bool on) {
        MusicOn = on;
        PlayerPrefs.SetInt("music", on ? 1 : 0);
        UpdateVolsAndSprites();
    }

    public void ToggleMusic () {
        SetMusic(!MusicOn);
    }

    void UpdateVolsAndSprites () {
        mixer.SetFloat("SFX", SFXOn ? 0f : -100f);
        mixer.SetFloat("Music", MusicOn ? 0f : -100f);
        mixer.SetFloat("Master", (SFXOn || MusicOn) ? 0f : -100f);

        sfxButton.sprite = SFXOn ? sfxOn : sfxOff;
        musicButton.sprite = MusicOn ? musicOn : musicOff;
    }
}
