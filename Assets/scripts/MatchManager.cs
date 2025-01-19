using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using DG.Tweening;


public class MatchManager : MonoBehaviour
{
    public GameObject placementArea; // Yerleþtirme alanýnýn referansý
    public static ParticleSystem matchEffect; // Make this static

    private static GameObject firstObject = null; // Ýlk seçilen obje
    private static GameObject secondObject = null; // Ýkinci seçilen obje

    void Start()
    {
        // Optionally, you can validate if matchEffect is assigned in the inspector
        if (matchEffect == null)
        {
            UnityEngine.Debug.LogWarning("matchEffect is not assigned in the inspector!");
        }
    }

    void Update()
    {
        if (firstObject != null && secondObject != null)
        {
            UnityEngine.Debug.Log("check edilcek 2 si de null deðil");
            CheckMatch();
        }
    }

    public void SetObject(GameObject obj)
    {
        UnityEngine.Debug.Log("SetObject orgin obj is:" + obj);

        if (firstObject == null)
        {
            firstObject = obj;
            UnityEngine.Debug.Log("First object changed" + firstObject);
        }
        else if (secondObject == null)
        {
            UnityEngine.Debug.Log("sec object changed" + secondObject);

            secondObject = obj;
            CheckMatch(); // Artýk CheckMatch'ý direkt olarak çaðýrabilirsiniz
        }
    }
    public void DeleteObject()
    {
        firstObject = null;
        secondObject = null;
    }

    public void CheckMatch()
    {
        UnityEngine.Debug.Log("First Object Checking: " + firstObject);
        UnityEngine.Debug.Log("Second Object Checking: " + secondObject);

        if (firstObject == null || secondObject == null)
        {
            UnityEngine.Debug.LogWarning("One or both objects are null!");
            return;
        }

        ObjectID firstObjectID = firstObject.GetComponent<ObjectID>();
        ObjectID secondObjectID = secondObject.GetComponent<ObjectID>();

        if (firstObjectID == null || secondObjectID == null)
        {
            UnityEngine.Debug.LogWarning("ObjectID component is missing on one or both objects!");
            return;
        }

        if (firstObjectID.id == secondObjectID.id)
        {
            UnityEngine.Debug.Log("Eþleþme baþarýlý!");
            if (GameManager.Instance != null)
            {
                UnityEngine.Debug.Log("obje sayýsý ::: " + GameObject.FindGameObjectsWithTag("spawnedObject").Length);
                GameManager.Instance.AddScore(10);
            }
            else
            {
                UnityEngine.Debug.LogWarning("GameManager instance bulunamadý!");
            }

            // Animasyonla yok olma
            Vector3 centerPosition = (firstObject.transform.position + secondObject.transform.position) / 2;
            PlayMatchEffect(centerPosition);

            // Animasyon baþlat
            AnimateAndDestroy(firstObject);
            AnimateAndDestroy(secondObject);
        }
        else
        {
            UnityEngine.Debug.Log("Eþleþme baþarýsýz, objeler geri dönecek.");
            ReturnToInitialPosition(firstObject);
            ReturnToInitialPosition(secondObject);
        }

        // Reset objects
        firstObject = null;
        secondObject = null;
    }


    private void AnimateAndDestroy(GameObject obj)
    {
        obj.transform.DOScale(Vector3.zero, 0.5f) // 0.5 saniyede küçült
            .SetEase(Ease.InBack) // Geri çekilme efekti ekle
            .OnComplete(() =>
            {
                Destroy(obj); // Animasyon tamamlandýktan sonra yok et
            });
    }

    public void PlayMatchEffect(Vector3 position) // Change from static to instance method
    {
        if (matchEffect != null)
        {
            ParticleSystem effect = Instantiate(matchEffect, position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration); // Efekti yok et
        }
    }


    public static void ReturnToInitialPosition(GameObject obj)
    {
        DragAndDropObject dragScript = obj.GetComponent<DragAndDropObject>();
        if (dragScript != null)
        {
            dragScript.RespawnToCenter();
        }
    }
}