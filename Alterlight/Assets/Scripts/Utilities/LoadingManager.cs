using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    [SerializeField] private GameObject canvas;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string[] tips;

    private float target;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public async void LoadScene(string sceneName)
    {
        target = 0;
        slider.value = 0;
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        canvas.SetActive(true);

        text.text = tips[Random.Range(0, tips.Length)];

        do
        {
            await Task.Delay(100);
            target = scene.progress;
        } while (scene.progress < 0.9f);

        await Task.Delay(1500);
        scene.allowSceneActivation = true;
        canvas.SetActive(false);
    }

    private void Update() {
        slider.value = Mathf.MoveTowards(slider.value, target, 1.5f * Time.deltaTime);
    }
}
