using UnityEngine;
using TMPro; // TextMeshProUGUI kullanmak i�in

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int score = 0;
    public TextMeshProUGUI scoreText; // TextMeshProUGUI referans�

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Skor UI's�n� ba�lat
        UpdateScoreText();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText(); // Skoru UI'da g�ncelle
    }

    public int GetScore()
    {
        return score;
    }

    // Skor metnini g�ncelleyen fonksiyon
    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + score;
        }
        else
        {
            UnityEngine.Debug.LogWarning("Skor TextMeshProUGUI bile�eni atanmad�!");
        }
    }

    // Resetleme fonksiyonu
    public void ResetGame()
    {
        // Skoru s�f�rla
        score = 0;
        UpdateScoreText(); // UI'daki skoru g�ncelle

        // Oyun nesnelerini temizle (�rne�in t�m objeleri sahneden kald�r)
        GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("spawnedObject");
        foreach (GameObject obj in objectsToDestroy)
        {
            Destroy(obj); // Nesneleri sahneden kald�r
        }

        // Gerekirse di�er oyun ba�lang�c� i�lemleri burada yap�labilir
        UnityEngine.Debug.Log("Oyun s�f�rland� ve yeniden ba�lat�ld�!");
    }
}
