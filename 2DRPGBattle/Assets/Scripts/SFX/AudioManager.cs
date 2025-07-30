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

    private void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

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

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Ensure AudioSources are assigned
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.loop = true;
    }

    // Music Controls
    public void PlayMenuMusic()
    {
        musicSource.clip = menuMusic;
        musicSource.Play();
    }

    public void PlayBattleMusic()
    {
        musicSource.clip = battleMusic;
        musicSource.Play();
    }

    public void PlayBossMusic()
    {
        musicSource.clip = bossMusic;
        musicSource.Play();
    }

    public void PlayBGMusic1()
    {
        musicSource.clip = grass1;
        musicSource.Play();
    }

    public void PlayBGMusic2()
    {
        musicSource.clip = grass2;
        musicSource.Play();
    }

    public void PlayBGMusic3()
    {
        musicSource.clip = dungeon;
        musicSource.Play();
    }

    public void PlayVictoryMusic()
    {
        musicSource.loop = false;
        musicSource.clip = battleVictory;
        musicSource.Play();
    }

    public void PlayLoseMusic()
    {
        musicSource.loop = false;
        musicSource.clip = battleLose;
        musicSource.Play();
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