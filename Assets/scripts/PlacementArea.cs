using System.Diagnostics;
using UnityEngine;

public class PlacementArea : MonoBehaviour
{
    // Yerleþtirme alanýndaki mevcut nesne
    private GameObject currentObject;

    private void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log("OnTriggerEnter");

        // Eðer yerleþtirme alanýnda bir obje varsa
        if (currentObject != null)
        {
            UnityEngine.Debug.Log("obje var");

            // Yeni gelen obje eþleþiyor mu?
            if (currentObject.tag == other.tag)
            {
                // Eþleþen objeleri yok et
                Destroy(currentObject);
                Destroy(other.gameObject);

                // Puaný artýr (Opsiyonel)
                GameManager.Instance.AddScore(10); // Skoru 10 artýr
            }
            else
            {
                UnityEngine.Debug.Log("eþleþmiyorsa geri fýrlat");
                // Eþleþmiyorsa objeyi geri fýrlat
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(Vector3.back * 500f); // Kuvvetle geri fýrlatma
                }
            }
        }
        else
        {
            // Eðer yerleþtirme alaný boþsa, objeyi yerleþtir
            currentObject = other.gameObject;
            other.transform.position = transform.position;
            other.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // Eðer obje alaný terk ederse, currentObject null yap
        if (other.gameObject == currentObject)
        {
            currentObject = null;
        }
    }
}
