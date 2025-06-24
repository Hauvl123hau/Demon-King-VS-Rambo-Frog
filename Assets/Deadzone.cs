using UnityEngine;
using UnityEngine.SceneManagement;

public class Deadzone : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Load level 1 when player falls into the hole
            SceneManager.LoadScene("Lever-1");
            Debug.Log("Player fell into hole. Reloading Lever 1.");
        }
    }
}

  