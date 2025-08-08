using UnityEngine;

public class OverworldSpawnManager : MonoBehaviour
{
    [SerializeField] private Vector3 startingPosition = Vector3.zero;

    private static bool initialized = false;

    private void Awake()
    {
        if (initialized)
        {
            Destroy(gameObject);
            return;
        }

        initialized = true;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        var player = PlayerOverworldController.Instance;

        if (player == null)
        {
            Debug.LogWarning("PlayerOverworldController not found!");
            return;
        }

        // Only set starting position if no stored position in SceneTracker
        if (!SceneTracker.Instance.HasStoredPosition())
        {
            player.transform.position = startingPosition;
            Debug.Log($"Player set to starting position: {startingPosition}");
        }
        else
        {
            Debug.Log("Player position already stored in SceneTracker; not overriding.");
        }

        player.gameObject.SetActive(true);
    }

    [System.Obsolete("Use SceneTracker.Instance.SetPlayerPosition instead.")]
    public static void SavePlayerPosition(Vector3 position, bool fromBattle = false)
    {
        // Forward to SceneTracker for backward compatibility if needed
        SceneTracker.Instance.SetPlayerPosition(position);
        SceneTracker.returningFromBattle = fromBattle;
    }
}
