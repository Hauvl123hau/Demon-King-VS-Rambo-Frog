using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    [Header("Boss health settings")]
    public int maxHealth = 3;
    public int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color ogColor;
    private bool isDead = false;
    private bool hasDrunkPotion = false;
    private bool isInvulnerable = false; // Thêm biến này
    public Slider healthBar;
    [Header("Potion Effects")]
    public float scaleMultiplier = 1.2f; // Boss sẽ to lên 1.2 lần
    private Vector3 originalScale;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        ogColor = spriteRenderer.color;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale; // Lưu kích thước gốc
    }

    void Update()
    {
        healthBar.value = currentHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvulnerable) return; // Thêm kiểm tra isInvulnerable
        
        currentHealth -= damage;
        StartCoroutine(Flash());
        
        // Kiểm tra nếu máu <= 50% và chưa uống thuốc
        if (currentHealth <= maxHealth * 0.5f && !hasDrunkPotion)
        {
            hasDrunkPotion = true;
            isInvulnerable = true; // Bật miễn sát thương
            if (anim != null)
            {
                anim.SetTrigger("drinkPotion");
            }
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method này sẽ được gọi từ Animation Event khi animation uống thuốc kết thúc
    public void OnPotionDrinkComplete()
    {
        StartCoroutine(ScaleUpAndEndInvulnerability());
    }

    private IEnumerator ScaleUpAndEndInvulnerability()
    {
        Vector3 targetScale = originalScale * scaleMultiplier;
        float duration = 0.5f; // Thời gian to lên
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
        isInvulnerable = false; // Tắt miễn sát thương sau khi hoàn thành
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        
        if (anim != null)
        {
            anim.SetTrigger("die");
        }
        else
        {
            DieAnimation();
        }
    }
    public void DieAnimation()
    {
        Destroy(gameObject, 0.5f);
    }
    private IEnumerator Flash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = ogColor;
            yield return new WaitForSeconds(0.1f);
        
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = ogColor;
        }
    }
}
