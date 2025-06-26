using UnityEngine;
using System.Collections; // Thêm dòng này

public class GunController : MonoBehaviour
{
    public Transform Gun;
    public Animator gunAnimator;
    Vector2 direction;
    public GameObject bullet;
    public float bulletSpeed = 10f;
    public Transform shootPoint;
    public float shootRate = 0.5f;
    float nextShootTime = 0f;
    public int currentClip, maxClip = 7, currentAmmo, maxAmmo = 30;
    public float reloadDelay = 1.5f; // Thời gian thay đạn
    public GameObject[] ammo_ui;

    bool isReloading = false;

    void Start()
    {
        // Khởi tạo UI đạn
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
            GameObject bulletIns = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
            bulletIns.GetComponent<Rigidbody2D>().AddForce(bulletIns.transform.right * bulletSpeed);
            gunAnimator.SetTrigger("shoot");
            Destroy(bulletIns, 2f);
            currentClip--;
            UpdateAmmoUI(); // Cập nhật UI sau khi bắn

            // Tự động thay đạn khi hết băng
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
        // Kiểm tra nếu ammo_ui đã được gán
        if (ammo_ui == null || ammo_ui.Length == 0)
            return;

        // Cập nhật hiển thị các viên đạn trong băng
        for (int i = 0; i < ammo_ui.Length; i++)
        {
            if (ammo_ui[i] != null)
            {
                // Hiển thị viên đạn nếu vị trí này có đạn, ẩn nếu không
                ammo_ui[i].SetActive(i < currentClip);
            }
        }
    }
}
