using Enemy.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleTransition : MonoBehaviour
{
    // Singleton instance for easy access
    public static BattleTransition Instance { get; private set; }

    // Current enemy being battled
    public static EnemySO CurrentEnemy { get; private set; }

    // Inspector settings
    public float transitionTime = 1f;
    public Image transitionPanel;
    public Animator transitionAnimator;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this when player hits an enemy
    public void StartBattle(EnemySO enemy)
    {
        CurrentEnemy = enemy;
        StartCoroutine(TransitionToBattle());
    }

    // Call this when battle ends
    public void ReturnToOverworld()
    {
        StartCoroutine(TransitionToOverworld());
    }

    IEnumerator TransitionToBattle()
    {
        // Freeze game
        Time.timeScale = 0f;

        // Set transition color based on enemy type
        transitionPanel.color = GetEnemyColor(CurrentEnemy.enemyType);

        // Play transition animation
        transitionAnimator.SetTrigger("FadeIn");
        yield return new WaitForSecondsRealtime(transitionTime);

        // Load battle scene
        SceneManager.LoadScene("BattleScene");
    }

    IEnumerator TransitionToOverworld()
    {
        // Freeze game
        Time.timeScale = 0f;

        // Play transition animation
        transitionAnimator.SetTrigger("FadeIn");
        yield return new WaitForSecondsRealtime(transitionTime);

        // Load overworld scene
        SceneManager.LoadScene("OverworldScene");

        // Fade out
        transitionAnimator.SetTrigger("FadeOut");

        // Unfreeze game
        Time.timeScale = 1f;
    }

    private Color GetEnemyColor(EnemySO.EnemyType type)
    {
        // Return color based on enemy type
        return type switch
        {
            EnemySO.EnemyType.Melee => new Color(0.8f, 0.2f, 0.2f), // Red
            EnemySO.EnemyType.Ranged => new Color(0.2f, 0.2f, 0.8f), // Blue
            EnemySO.EnemyType.Boss => new Color(0.8f, 0.1f, 0.8f),   // Purple
            _ => Color.black
        };
    }
}
