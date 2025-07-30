using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldReturnPosition : MonoBehaviour
{
    void Start()
    {
        GameObject player = GameObject.Find("Player");

        if (player != null)
        {
            if (PlayerPrefs.HasKey("PlayerX") && PlayerPrefs.HasKey("PlayerY"))
            {
                float x = PlayerPrefs.GetFloat("PlayerX");
                float y = PlayerPrefs.GetFloat("PlayerY");
                player.transform.position = new Vector3(x, y, player.transform.position.z);
            }

            player.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No player found to return to.");
        }

        GameObject overworldUI = GameObject.Find("UIOverworldCanvas");
        if (overworldUI) overworldUI.SetActive(true);
    }
}
