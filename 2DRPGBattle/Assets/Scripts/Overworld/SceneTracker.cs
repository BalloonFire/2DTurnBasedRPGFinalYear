using UnityEngine;
using UnityEngine.UIElements;

public class SceneTracker : Singleton<SceneTracker>
{
    private string previousScene;
    private Vector3 playerPosition;

    public void SetPreviousScene(string sceneName)
    {
        previousScene = sceneName;
    }

    public string GetPreviousScene()
    {
        return previousScene;
    }

    public void SetPlayerPosition(Vector3 pos)
    {
        playerPosition = pos;
    }

    public Vector3 GetPlayerPosition()
    {
        return playerPosition;
    }

    public bool HasStoredPosition()
    {
        return playerPosition != Vector3.zero;
    }
}
