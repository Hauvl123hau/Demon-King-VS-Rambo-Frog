using UnityEngine;
using UnityEngine.SceneManagement;
public class NextLV : MonoBehaviour
{
    public string nextLV2;    public void loadNextLevel()
    {
        SceneManager.LoadScene(nextLV2);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            loadNextLevel();
        }
    }

}
