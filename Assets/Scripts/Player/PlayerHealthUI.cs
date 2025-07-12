using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealthUI : MonoBehaviour
{
    public Image heartImage;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    private List<Image> hearts = new List<Image>();
    public void SetMaxHealth(int maxHealth)
    {
        // Clear existing hearts
        foreach (var heart in hearts)
        {
            Destroy(heart.gameObject);
        }
        hearts.Clear();

        // Create new hearts
        for (int i = 0; i < maxHealth; i++)
        {
            Image newHeart = Instantiate(heartImage, transform);
            newHeart.sprite = fullHeartSprite;
            newHeart.color = Color.red;
            hearts.Add(newHeart);
        }
    }

    public void UpdateHeart(int currentHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeartSprite;
                hearts[i].color = Color.red;
            }
            else
            {
                hearts[i].sprite = emptyHeartSprite;
                hearts[i].color = Color.black;
            }
        }
    }
}
