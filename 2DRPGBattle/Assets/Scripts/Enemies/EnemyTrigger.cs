using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyTrigger : MonoBehaviour
{
    [Header("Enemy Settings")]
    public string enemyID = "slime_01"; // Set this in the Inspector

    [Header("Scene Settings")]
    public string battleScene = "BattleTest";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Store info for returning
            PlayerPrefs.SetString("BattleEnemyID", enemyID);
            PlayerPrefs.SetString("ReturnScene", SceneManager.GetActiveScene().name);
            PlayerPrefs.SetFloat("PlayerX", other.transform.position.x);
            PlayerPrefs.SetFloat("PlayerY", other.transform.position.y);

            // Load the battle scene
            SceneManager.LoadScene(battleScene);
        }
    }
}
