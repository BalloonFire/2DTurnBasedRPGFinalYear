using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldReturnPosition : MonoBehaviour
{
    public GameObject playerPrefab; // Assign this in Inspector 

    private void Start()
    {
        float x = PlayerPrefs.GetFloat("PlayerX", 0f);
        float y = PlayerPrefs.GetFloat("PlayerY", 0f);
        Vector3 returnPosition = new Vector3(x, y, 0f);

        GameObject player = GameObject.FindWithTag("Player");

        if (player == null && playerPrefab != null)
        {
            player = Instantiate(playerPrefab, returnPosition, Quaternion.identity);
        }
        else if (player != null)
        {
            player.transform.position = returnPosition;
            player.SetActive(true);
        }

        // Enable UI
        GameObject overworldUI = GameObject.Find("UIOverworldCanvas");
        if (overworldUI) overworldUI.SetActive(true);
    }
}
