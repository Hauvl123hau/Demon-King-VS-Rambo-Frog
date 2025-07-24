using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 7; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GunController gunController = collision.gameObject.GetComponentInChildren<GunController>();
        if (gunController != null)
        {
            gunController.AddAmmo(ammoAmount); 
            Destroy(gameObject); 
        }
    }
}
