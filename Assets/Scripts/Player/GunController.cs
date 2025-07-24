using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GunController : MonoBehaviour
{
    AudioSource shootSound;
    public Transform Gun;
    public Animator gunAnimator;
    Vector2 direction;
    public GameObject bullet;
    public float bulletSpeed = 10f;
    public Transform shootPoint;
    public float shootRate = 0.5f;
    float nextShootTime = 0f;
    public int currentClip, maxClip = 7, currentAmmo, maxAmmo = 30;
    public float reloadDelay = 1.5f; 
    public BulletUI bulletUI; 
    public Text ammoText;

    bool isReloading = false;

    void Start()
    {
        shootSound = GetComponent<AudioSource>();
        currentClip = maxClip; // Bắt đầu với băng đạn đầy
        bulletUI.SetMaxBullets(maxClip);
        UpdateAmmoUI();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePosition - (Vector2)Gun.position;
        FaceMouse();
    }

    void FaceMouse()
    {
        Gun.transform.right = direction;
    }

    public void Shoot()
    {
        if (currentClip > 0)
        {
            shootSound.Play(); // Phát âm thanh khi bắn
            GameObject bulletIns = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
            bulletIns.GetComponent<Rigidbody2D>().AddForce(bulletIns.transform.right * bulletSpeed);
            gunAnimator.SetTrigger("shoot");
            Destroy(bulletIns, 2f);
            currentClip--;
            UpdateAmmoUI();

            if (currentClip == 0 && currentAmmo > 0 && !isReloading)
            {
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        gunAnimator.SetTrigger("reload");
        yield return new WaitForSeconds(reloadDelay);
        FinishReload();
    }

    public void Reload()
    {
        if (!isReloading && currentClip < maxClip && currentAmmo > 0)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    public void AddAmmo(int ammoAmount)
    {
        currentAmmo += ammoAmount;
        if (currentAmmo > maxAmmo)
        {
            currentAmmo = maxAmmo; // Ensure we don't exceed max ammo
        }

        // Nếu hết băng và có đạn dự trữ, tự động thay đạn
        if (currentClip == 0 && currentAmmo > 0 && !isReloading)
        {
            StartCoroutine(ReloadCoroutine());
        }
        UpdateAmmoUI(); // Cập nhật UI sau khi thêm đạn
    }
    public void FinishReload()
    {
        int reloadAmount = maxClip - currentClip;
        reloadAmount = (currentAmmo >= reloadAmount) ? reloadAmount : currentAmmo;
        currentClip += reloadAmount;
        currentAmmo -= reloadAmount;
        isReloading = false;
        UpdateAmmoUI(); // Cập nhật UI sau khi thay đạn
    }

    // Phương thức cập nhật UI hiển thị đạn
    void UpdateAmmoUI()
    {
        // Cập nhật UI hiển thị số đạn trong băng
        if (bulletUI != null)
        {
            bulletUI.UpdateBullets(currentClip);
        }
        
        // Cập nhật text hiển thị đạn dự trữ
        if (ammoText != null)
        {
            ammoText.text = "/" + currentAmmo.ToString();
        }
    }
}
