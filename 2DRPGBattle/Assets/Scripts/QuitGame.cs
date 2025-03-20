using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // Start is called before the first frame update
    public void Quit()
    {
        // Quit the application
        Application.Quit();

        // Log a message to the console (for testing in the Editor)
        Debug.Log("Quit button clicked. Application would quit now.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
