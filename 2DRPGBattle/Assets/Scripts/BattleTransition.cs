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

            // Ensure the transition panel is properly initialized
            transitionPanel.gameObject.SetActive(true);
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
        Time.timeScale = 0f;

        transitionPanel.color = GetEnemyColor(CurrentEnemy.enemyType);
        transitionAnimator.SetTrigger("FadeIn");

        yield return new WaitForSecondsRealtime(transitionTime);

        // Destroy player BEFORE scene loads
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Debug.Log("Destroying player before battle scene");
            Destroy(player);
        }

        SceneManager.LoadScene("BattleTest");

        yield return null;

        transitionAnimator.SetTrigger("FadeOut");
        Time.timeScale = 1f;
    }

    private void CleanupOverworldObjects()
    {
        // Double-check and destroy player if it still exists
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Debug.Log("Destroying player before battle scene load.");
            Destroy(player);
        }

        // Clean up UI canvas
        var ui = GameObject.Find("UIOverworldCanvas");
        if (ui != null)
        {
            Destroy(ui);
        }
    }

    IEnumerator TransitionToOverworld()
    {
        // Freeze game
        Time.timeScale = 0f;

        // Play transition animation
        transitionAnimator.SetTrigger("FadeIn");
        yield return new WaitForSecondsRealtime(transitionTime);

        // Load overworld scene
        SceneManager.LoadScene("MapGrass1");

        // FADE OUT AFTER LOADING OVERWORLD - CRITICAL ADDITION
        transitionAnimator.SetTrigger("FadeOut");

        // Wait for fade out to complete
        yield return new WaitForSecondsRealtime(transitionTime);

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