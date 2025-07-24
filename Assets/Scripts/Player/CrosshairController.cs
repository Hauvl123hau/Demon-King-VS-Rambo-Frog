using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;
    }

    private void Start()
    {
        Cursor.visible = false;
    }
}