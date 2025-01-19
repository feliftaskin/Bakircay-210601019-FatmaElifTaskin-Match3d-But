using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using TMPro; // TextMeshPro için gerekli kütüphane

public class SkillButtonManager : MonoBehaviour
{
    public Button skill1Button; // Skill 1 düğmesi
    public Button skill2Button; // Skill 2 düğmesi
    public Button skill3Button; // Skill 3 düğmesi
    public Button skill4Button; // Skill 4 düğmesi (Yeni skill)
    public float cooldownTime = 5f; // Her düğme için cooldown süresi
    private int score = 0; // Puan
    public TMP_Text scoreText; // Skoru göstermek için TMP_Text objesi

    void Start()
    {
        // Düğme tıklama işlemlerini bağla
        skill1Button.onClick.AddListener(() => OnSkillButtonPressed(skill1Button, "Skill 1 kullanıldı!"));
        skill2Button.onClick.AddListener(() => OnSkillButtonPressed(skill2Button, "Skill 2 kullanıldı!"));
        skill3Button.onClick.AddListener(() => OnSkillButtonPressed(skill3Button, "Skill 3 kullanıldı!"));
        skill4Button.onClick.AddListener(() => OnSkillButtonPressed(skill4Button, "Skill 4 kullanıldı!"));
    }

    void OnSkillButtonPressed(Button button, string message)
    {
        // Eğer düğme devre dışıysa tıklanmasına izin verme
        if (!button.interactable)
            return;

        // Beceri aktif edildiğinde mesaj yazdır
        UnityEngine.Debug.Log(message);

        // Skill 1: Objeleri sabitle ve tıklanamaz, sürüklenemez yap (5 saniye boyunca)
        if (button == skill1Button)
        {
            StartCoroutine(FixAllObjectsForDuration(5f));
        }

        // Skill 2: Nesneleri dondur
        if (button == skill2Button)
        {
            FreezeAllObjects();
        }

        // Skill 3: İki nesneyi vurgula
        if (button == skill3Button)
        {
            HighlightTwoObjects();
        }

        // Skill 4: Nesneleri eşleştir
        if (button == skill4Button)
        {
            MatchObjects();
        }

        // Düğmeyi devre dışı bırak
        button.interactable = false;

        // Cooldown başlat
        StartCoroutine(CooldownCoroutine(button));

        // Düğme animasyonu ekle
        AnimateButton(button);
    }

    // Skill 4: Aynı türdeki nesneleri eşleştir
   void MatchObjects()
{
    GameObject[] spawnedObjects = GameObject.FindGameObjectsWithTag("spawnedObject");
    Dictionary<string, List<GameObject>> groupedObjects = new Dictionary<string, List<GameObject>>();

    // Nesneleri etiketlerine göre grupla
    foreach (GameObject obj in spawnedObjects)
    {
        string tag = obj.tag; // Nesnenin etiketiyle grupla
        if (!groupedObjects.ContainsKey(tag))
        {
            groupedObjects[tag] = new List<GameObject>();
        }
        groupedObjects[tag].Add(obj);
    }

    // Aynı türde iki nesne grubu bul
    foreach (var group in groupedObjects)
    {
        if (group.Value.Count >= 2) // Aynı türden en az iki nesne varsa
        {
            // İlk iki nesneyi al ve eşleştir
            GameObject firstObject = group.Value[0];
            GameObject secondObject = group.Value[1];

            // Nesnelerin kaybolmalarını sağla
            firstObject.SetActive(false);
            secondObject.SetActive(false);

            // Skoru arttır
            GameManager.Instance.AddScore(10); // Skoru 10 artır

            // Skorun güncellenmesi
            scoreText.text = "SCORE: " + GameManager.Instance.GetScore(); // Skoru doğru şekilde yansıt

            // Nesneler kaybolduktan sonra kısa bir animasyonla kaybolmalarını sağlayabiliriz
            firstObject.transform.DOPunchScale(Vector3.one * 1.5f, 0.3f);
            secondObject.transform.DOPunchScale(Vector3.one * 1.5f, 0.3f);
        }
    }
}

    // Skill 1: Nesneleri sabitle ve sadece 5 saniye boyunca tıklanamaz hale getir
    IEnumerator FixAllObjectsForDuration(float duration)
    {
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("spawnedObject");

        foreach (GameObject obj in allObjects)
        {
            // Nesnenin Rigidbody bileşenini al ve hareket etmelerini engelle
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Nesnenin hareket etmemesi için kinematik hale getir
            }

            // Nesneyi sürüklenemez yap
            Collider col = obj.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false; // Nesnenin collider'ını devre dışı bırak
            }
        }

        // 5 saniye bekle
        yield return new WaitForSeconds(duration);

        // 5 saniye sonra nesneleri tekrar eski haline getir
        foreach (GameObject obj in allObjects)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Nesnenin hareket etmesine izin ver
            }

            Collider col = obj.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true; // Nesnenin collider'ını yeniden etkinleştir
            }
        }
    }

    void FreezeAllObjects()
    {
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("spawnedObject");

        foreach (GameObject obj in allObjects)
        {
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        StartCoroutine(UnfreezeObjectsAfterDelay(5f));
    }

    IEnumerator UnfreezeObjectsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("spawnedObject");

        foreach (GameObject obj in allObjects)
        {
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }

        Debug.Log("Nesneler yeniden aktif hale getirildi.");
    }

    void HighlightTwoObjects()
    {
        GameObject[] spawnedObjects = GameObject.FindGameObjectsWithTag("spawnedObject");

        if (spawnedObjects.Length >= 2)
        {
            GameObject firstObject = spawnedObjects[0];
            GameObject secondObject = spawnedObjects[1];

            // Vurgulama işlemi, objelerin boyutunu geçici olarak artırır
            firstObject.transform.DOPunchScale(Vector3.one * 1.5f, 0.5f, 5, 0.3f);
            secondObject.transform.DOPunchScale(Vector3.one * 1.5f, 0.5f, 5, 0.3f);
        }
    }

    void AnimateButton(Button button)
    {
        button.GetComponent<Image>().DOFade(0.5f, 0.2f)
            .OnComplete(() => button.GetComponent<Image>().DOFade(1f, 0.2f));
    }

    IEnumerator CooldownCoroutine(Button button)
    {
        yield return new WaitForSeconds(cooldownTime);
        button.interactable = true;
    }
}
