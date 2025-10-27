using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ARSceneController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogImage;
    public float dialogDuration = 3f;

    void Start()
    {
        StartCoroutine(ShowDialog());
    }

    IEnumerator ShowDialog()
    {
        if (dialogImage != null)
        {
            dialogImage.SetActive(true);
            yield return new WaitForSeconds(dialogDuration);
            dialogImage.SetActive(false);
        }
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
