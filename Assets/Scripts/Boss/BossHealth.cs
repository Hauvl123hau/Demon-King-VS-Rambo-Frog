using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    public Animator anim;
    public SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    [Header("Boss health settings")]
    public int maxHealth;
    public int currentHealth;
    private Color ogColor;
    private bool isDead = false;
    public Slider healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        ogColor = spriteRenderer.color;
        // Auto-find Animator if not assigned
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        healthBar.value = currentHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartCoroutine(Flash());
        if (currentHealth <= 0)
        {
            Die();
        }
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
