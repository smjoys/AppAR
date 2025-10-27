using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MultipleImagesTrackingManager : MonoBehaviour
{
    [Header("Referencia manual al Image Manager")]
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    [Header("Prefabs asociados a cada imagen (orden igual al de la librería)")]
    [SerializeField] private List<GameObject> prefabsToSpawn = new List<GameObject>();

    // Diccionario para mantener las instancias activas
    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

    [Header("Ajustes de movimiento suave (opcional)")]
    [Range(0f, 1f)] public float smoothFactor = 0.5f; // 0 = sin suavizado, 1 = máximo suavizado

    private void OnEnable()
    {
        if (trackedImageManager == null)
        {
            Debug.LogError("⚠️ Debes asignar manualmente el ARTrackedImageManager en el Inspector.");
            return;
        }

        trackedImageManager.trackedImagesChanged += OnImagesChanged;
        SetupPrefabs();
    }

    private void OnDisable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackedImagesChanged -= OnImagesChanged;
    }

    private void SetupPrefabs()
    {
        // Instancia todos los prefabs al inicio y los guarda desactivados
        foreach (var prefab in prefabsToSpawn)
        {
            if (prefab == null) continue;

            GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            obj.name = prefab.name;
            obj.SetActive(false);
            spawnedObjects.Add(obj.name, obj);
        }
    }

    private void OnImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
            UpdateTrackedImage(trackedImage);

        foreach (var trackedImage in eventArgs.updated)
            UpdateTrackedImage(trackedImage);

        foreach (var trackedImage in eventArgs.removed)
        {
            if (spawnedObjects.ContainsKey(trackedImage.referenceImage.name))
                spawnedObjects[trackedImage.referenceImage.name].SetActive(false);
        }
    }

    private void UpdateTrackedImage(ARTrackedImage trackedImage)
    {
        if (trackedImage == null) return;

        string imageName = trackedImage.referenceImage.name;

        if (!spawnedObjects.ContainsKey(imageName))
        {
            Debug.LogWarning($"No se encontró prefab asociado a la imagen: {imageName}");
            return;
        }

        GameObject obj = spawnedObjects[imageName];

        // Si la imagen no está siendo rastreada activamente, ocultar el modelo
        if (trackedImage.trackingState == TrackingState.None || trackedImage.trackingState == TrackingState.Limited)
        {
            obj.SetActive(false);
            return;
        }

        // Activar y actualizar posición y rotación
        obj.SetActive(true);

        // Movimiento suave (interpolación opcional)
        obj.transform.position = Vector3.Lerp(obj.transform.position, trackedImage.transform.position, 1f - smoothFactor);
        obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, trackedImage.transform.rotation, 1f - smoothFactor);
    }
}
