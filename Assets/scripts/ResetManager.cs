using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için gerekli

public class ResetManager : MonoBehaviour
{
    // Reset butonuna baðlanacak fonksiyon
    public void ResetGame()
    {
        // Skoru sýfýrla
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(-GameManager.Instance.GetScore()); // Skoru sýfýrla
        }

        // Tüm objeleri temizle
        GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("spawnedObject");
        foreach (GameObject obj in objectsToDestroy)
        {
            Destroy(obj);
        }

        // Aktif sahneyi yeniden yükle (oyunu baþtan baþlat)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        UnityEngine.Debug.Log("Oyun sýfýrlandý ve yeniden baþlatýldý!");
    }
}
