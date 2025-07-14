using Enemy.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleTransition : MonoBehaviour
{
    [Header("Settings")]
    public float transitionTime = 1f;

    [Header("References")]
    public Image transitionPanel;
    public Animator transitionAnimator;

    private static BattleTransition _instance;
    private static EnemySO _currentEnemy;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void StartBattle(EnemySO enemy)
    {
        _currentEnemy = enemy;
        _instance.StartCoroutine(_instance.TransitionToBattle());
    }

    IEnumerator TransitionToBattle()
    {
        // Freeze gameplay
        Time.timeScale = 0f;

        // Setup transition visuals using enemy type color
        transitionPanel.color = GetEnemyTypeColor(_currentEnemy.enemyType);

        // Play transition animation
        transitionAnimator.SetTrigger("StartBattle");
        yield return new WaitForSecondsRealtime(transitionTime);

        // Load battle scene
        SceneManager.LoadScene("BattleScene");
    }

    private Color GetEnemyTypeColor(EnemySO.EnemyType type)
    {
        switch (type)
        {
            case EnemySO.EnemyType.Melee: return new Color(0.8f, 0.2f, 0.2f); // Red
            case EnemySO.EnemyType.Ranged: return new Color(0.2f, 0.2f, 0.8f); // Blue
            case EnemySO.EnemyType.Boss: return new Color(0.8f, 0.1f, 0.8f); // Purple
            default: return Color.black;
        }
    }

    public static void SetupBattleScene(SpriteRenderer enemySprite, Animator enemyAnimator)
    {
        if (_currentEnemy == null) return;

        // Apply enemy data to battle scene
        enemySprite.sprite = _currentEnemy.enemySprite;

        // Setup animator if available
        if (enemyAnimator != null && _currentEnemy.animatorController != null)
        {
            enemyAnimator.runtimeAnimatorController = _currentEnemy.animatorController;
        }

        // Resume gameplay
        Time.timeScale = 1f;
    }

    public static EnemySO GetCurrentEnemy()
    {
        return _currentEnemy;
    }
}
