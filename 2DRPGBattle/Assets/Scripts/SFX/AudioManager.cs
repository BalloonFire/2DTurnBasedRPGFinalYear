using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Menu Sounds")]
    public AudioClip menuMusic;
    public AudioClip menuSelection;

    [Header("Overworld Music")]
    public AudioClip grass1;
    public AudioClip grass2;
    public AudioClip dungeon;

    [Header("Battle Music and SFX")]
    public AudioClip battleMusic;
    public AudioClip bossMusic;
    public AudioClip battleStart;
    public AudioClip battleSwing;
    public AudioClip battleSelect;
    public AudioClip battleConfirm;
    public AudioClip battleDeny;
    public AudioClip battleHurt;
    public AudioClip battleDead;
    public AudioClip battleVictory;
    public AudioClip battleLose;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // subscribe to scene change
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Ensure AudioSources are assigned
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // clean unsubscribe
        }
    }

    // Automatically play music when scene changes
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string currentScene = scene.name;

        if (currentScene == "MenuScenes")
            PlayMenuMusic();
        else if (currentScene == "BattleTest")
            PlayBattleMusic();
        else if (currentScene == "BattleBoss")
            PlayBossMusic();
        else if (currentScene == "Map grass 1")
            PlayBGMusic1();
        else if (currentScene == "Map grass 2")
            PlayBGMusic2();
        else if (currentScene == "Dungeon 1")
            PlayBGMusic3();
    }

    // Music Controls
    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    public void PlayBattleMusic()
    {
        PlayMusic(battleMusic);
    }

    public void PlayBossMusic()
    {
        PlayMusic(bossMusic);
    }

    public void PlayBGMusic1()
    {
        PlayMusic(grass1);
    }

    public void PlayBGMusic2()
    {
        PlayMusic(grass2);
    }

    public void PlayBGMusic3()
    {
        PlayMusic(dungeon);
    }

    public void PlayVictoryMusic()
    {
        musicSource.loop = false;
        PlayMusic(battleVictory);
    }

    public void PlayLoseMusic()
    {
        musicSource.loop = false;
        PlayMusic(battleLose);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip && musicSource.isPlaying) return; // avoid restart
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // SFX Controls
    public void PlayMenuSelection() => sfxSource.PlayOneShot(menuSelection);
    public void PlayBattleStart() => sfxSource.PlayOneShot(battleStart);
    public void PlayBattleSwing() => sfxSource.PlayOneShot(battleSwing);
    public void PlayBattleSelect() => sfxSource.PlayOneShot(battleSelect);
    public void PlayBattleConfirm() => sfxSource.PlayOneShot(battleConfirm);
    public void PlayBattleDeny() => sfxSource.PlayOneShot(battleDeny);
    public void PlayBattleHurt() => sfxSource.PlayOneShot(battleHurt);
    public void PlayBattleDead() => sfxSource.PlayOneShot(battleDead);
}