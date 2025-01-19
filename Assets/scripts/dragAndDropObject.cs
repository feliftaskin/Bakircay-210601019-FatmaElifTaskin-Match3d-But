using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class DragAndDropObject : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Rigidbody rb;

    public float liftHeight = 2.0f;
    public float moveSpeed = 5f;

    private Vector3 snapTarget;
    private bool isSnapping = false;

    private Renderer objectRenderer;

    private static GameObject currentObjectInPlacementArea;

    private float minX = -10f;
    private float maxX = 10f;
    private float minZ = -10f;
    private float maxZ = 10f;
    private float minY = -2f;

    private string objectType; // Nesne türü için string deðiþken

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        objectRenderer = GetComponent<Renderer>();

        objectType = gameObject.name; // Nesne türünü adýndan alýyoruz
        UnityEngine.Debug.Log(objectType);

        if (rb != null)
        {
            rb.useGravity = true;
        }
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition) + offset;

            rb.velocity = (worldPosition - transform.position) * 10f;
        }
        else if (isSnapping)
        {
            float newYPosition = Mathf.Lerp(transform.position.y, snapTarget.y + liftHeight, Time.deltaTime * moveSpeed);
            Vector3 targetPosition = new Vector3(snapTarget.x, newYPosition, snapTarget.z);

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

            if (Vector3.Distance(transform.position, targetPosition) < 0.7f)
            {
                isSnapping = false;
                CheckForMatchingObjects(); // Nesne eþleþmesini kontrol et
            }
        }

        if (IsOutOfBounds(transform.position))
        {
            RespawnToCenter();
        }
    }

    void OnMouseDown()
    {
        transform.position = new Vector3(transform.position.x, liftHeight, transform.position.z);

        isDragging = true;

        if (rb != null)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
        }

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
        offset = transform.position - Camera.main.ScreenToWorldPoint(mousePosition);
    }

    void OnMouseUp()
    {
        isDragging = false;

        if (rb != null)
        {
            rb.useGravity = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log("TrigerEntered");
        if (other.CompareTag("PlacementArea"))
        {
            UnityEngine.Debug.Log(gameObject);
            UnityEngine.Debug.Log(currentObjectInPlacementArea);

            if (currentObjectInPlacementArea != null && currentObjectInPlacementArea?.name != gameObject?.name)
            {
                UnityEngine.Debug.Log("Placement Area dolu! Yeni obje (0, 0, 0) noktasýna taþýnýyor.");
                transform.position = new Vector3(0, 1, 0);
                return;
            }

            UnityEngine.Debug.Log("OnTriggerEnter - Placement Area'ya obje ekleniyor.");
            snapTarget = other.transform.position;
            isSnapping = true;
            currentObjectInPlacementArea = gameObject;

            // Get the MatchManager instance and call SetObject
            MatchManager matchManager = FindObjectOfType<MatchManager>();
            if (matchManager != null)
            {
                matchManager.SetObject(gameObject);  // This will call the method from MatchManager
            }
            else
            {
                UnityEngine.Debug.LogWarning("MatchManager instance not found!");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlacementArea"))
        {
            if (currentObjectInPlacementArea == gameObject)
            {
                UnityEngine.Debug.Log("Placement Area'dan obje kaldýrýldý.");
                currentObjectInPlacementArea = null;
                MatchManager matchManager = FindObjectOfType<MatchManager>();
                if (matchManager != null)
                {
                    matchManager.DeleteObject();
                }
                else
                {
                    UnityEngine.Debug.LogWarning("MatchManager instance not found!");
                }
            }

            isSnapping = false;
        }
    }

    void HideObject()
    {
        if (objectRenderer != null)
        {
            objectRenderer.enabled = false;
        }
        else
        {
            UnityEngine.Debug.LogWarning("Renderer bileþeni bulunamadý!");
        }
    }

    bool IsOutOfBounds(Vector3 position)
    {
        return position.x < minX || position.x > maxX || position.z < minZ || position.z > maxZ || position.y < minY;
    }

    public void RespawnToCenter()
    {
        UnityEngine.Debug.Log("Obje oyun alaný dýþýna çýktý! Merkeze taþýnýyor...");
        transform.position = new Vector3(0, 1, 0);
        rb.velocity = Vector3.zero;
    }

    void CheckForMatchingObjects()
    {
        // Eðer yerleþim alanýna eklenen obje, ayný türdeki diðer obje ile eþleþirse skoru arttýr
        if (currentObjectInPlacementArea != null && currentObjectInPlacementArea != gameObject)
        {
            string currentObjectType = currentObjectInPlacementArea.name;
            string objectType = gameObject.name;

            if (currentObjectType == objectType)
            {
                UnityEngine.Debug.Log("Ýki ayný türde nesne yerleþti! Skor artacak...");
                GameManager.Instance.AddScore(10); // Skoru 10 arttýr
                currentObjectInPlacementArea = null; // Yerleþim alanýný temizle
            }
            else
            {
                UnityEngine.Debug.Log("Farklý türde nesneler yerleþti.");
                transform.position = Vector3.zero; // Farklý türdeyse, nesneyi sýfýr noktasýna taþý
            }
        }
    }
}