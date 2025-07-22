using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BulletUI : MonoBehaviour
{
    public Image bulletImage;
    public Sprite fullBulletSprite;
    public Sprite emptyBulletSprite;

    private List<Image> bullets = new List<Image>();
    public void SetMaxBullets(int maxBullets)
    {
        // Clear existing bullets
        foreach (var bullet in bullets)
        {
            Destroy(bullet.gameObject);
        }
        bullets.Clear();

        // Create new bullets
        for (int i = 0; i < maxBullets; i++)
        {
            Image newBullet = Instantiate(bulletImage, transform);
            newBullet.sprite = fullBulletSprite;
            newBullet.color = Color.white;
            bullets.Add(newBullet);
        }
    }

    public void UpdateBullets(int currentBullets)
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            // Tính toán từ bên phải: viên đạn sẽ biến mất từ trái qua phải
            int rightIndex = bullets.Count - 1 - i;
            if (rightIndex < currentBullets)
            {
                bullets[i].sprite = fullBulletSprite;
                bullets[i].color = Color.white;
            }
            else
            {
                bullets[i].sprite = emptyBulletSprite;
                bullets[i].color = Color.black;
            }
        }
    }
}
