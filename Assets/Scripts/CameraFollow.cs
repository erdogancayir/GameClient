using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;         // Oyuncunun Transform componenti
    public float smoothing = 0.5f;   // Kamera hareketinin ne kadar pürüzsüz olacağını belirler
    public Vector3 offset;           // Kameranın oyuncu ile arasındaki mesafe farkı

    void LateUpdate()
    {
        // Oyuncunun mevcut pozisyonunu al ve offset'i uygula
        Vector3 targetPosition = player.position + offset;

        // Lerp fonksiyonu ile mevcut kamera pozisyonunu ve hedef pozisyon arasında pürüzsüz bir geçiş yap
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);

        // Kameranın pozisyonunu güncelle
        transform.position = smoothedPosition;
    }
}
