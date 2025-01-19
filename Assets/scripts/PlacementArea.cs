using System.Diagnostics;
using UnityEngine;

public class PlacementArea : MonoBehaviour
{
    // Yerle�tirme alan�ndaki mevcut nesne
    private GameObject currentObject;

    private void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log("OnTriggerEnter");

        // E�er yerle�tirme alan�nda bir obje varsa
        if (currentObject != null)
        {
            UnityEngine.Debug.Log("obje var");

            // Yeni gelen obje e�le�iyor mu?
            if (currentObject.tag == other.tag)
            {
                // E�le�en objeleri yok et
                Destroy(currentObject);
                Destroy(other.gameObject);

                // Puan� art�r (Opsiyonel)
                GameManager.Instance.AddScore(10); // Skoru 10 art�r
            }
            else
            {
                UnityEngine.Debug.Log("e�le�miyorsa geri f�rlat");
                // E�le�miyorsa objeyi geri f�rlat
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(Vector3.back * 500f); // Kuvvetle geri f�rlatma
                }
            }
        }
        else
        {
            // E�er yerle�tirme alan� bo�sa, objeyi yerle�tir
            currentObject = other.gameObject;
            other.transform.position = transform.position;
            other.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // E�er obje alan� terk ederse, currentObject null yap
        if (other.gameObject == currentObject)
        {
            currentObject = null;
        }
    }
}
