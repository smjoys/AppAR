using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MultiTracker : MonoBehaviour
{
    [Header("Prefabs asociados a imágenes")]
    [SerializeField] private GameObject[] prefabsToSpawn;

    [Header("Asignar manualmente el ARTrackedImageManager")]
    [SerializeField] private ARTrackedImageManager _arTrackedImageManager;

    private Dictionary<string, GameObject> _arObjects;

    private void Awake()
    {
        _arObjects = new Dictionary<string, GameObject>();
    }

    private void Start()
    {
        if (_arTrackedImageManager == null)
        {
            Debug.LogError("❌ ARTrackedImageManager no asignado en el inspector.");
            return;
        }

        _arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;

        foreach (GameObject prefab in prefabsToSpawn)
        {
            GameObject newArObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newArObject.name = prefab.name;
            newArObject.SetActive(false);
            _arObjects.Add(newArObject.name, newArObject);
        }
    }

    private void OnDestroy()
    {
        if (_arTrackedImageManager != null)
            _arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
            UpdateTrackedImage(trackedImage);

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
            UpdateTrackedImage(trackedImage);

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
            _arObjects[trackedImage.referenceImage.name].SetActive(false);
    }

    private void UpdateTrackedImage(ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState == TrackingState.None ||
            trackedImage.trackingState == TrackingState.Limited)
        {
            _arObjects[trackedImage.referenceImage.name].SetActive(false);
            return;
        }

        GameObject prefab = _arObjects[trackedImage.referenceImage.name];
        prefab.SetActive(true);
        prefab.transform.position = trackedImage.transform.position;
        prefab.transform.rotation = trackedImage.transform.rotation;
    }
}
