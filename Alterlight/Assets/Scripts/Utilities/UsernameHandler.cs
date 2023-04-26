using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Newtonsoft.Json;

public class UsernameHandler : MonoBehaviour
{
    public TMP_InputField input;
    public GameObject SaveDataFound;
    public GameObject NoSaveDataFound;

    private void Start() 
    {
        string path = @"C:\Users\sidc2\Documents\GitHub\alterlight\Alterlight\Assets\UserData\user_data.json";
        FileInfo file = new FileInfo(path);

        if (file.Length > 0)
        {
            SaveDataFound.SetActive(true);
            NoSaveDataFound.SetActive(false);
        }
        else
        {
            NoSaveDataFound.SetActive(true);
            SaveDataFound.SetActive(false);
        }
    }

    public void GetUsername()
    {
        string name = input.text;
        PlayerSaveData player = new PlayerSaveData{ username = name, Level = 0, EXP = 0, MobsKilled = 0 };
        string jsonObj = JsonConvert.SerializeObject(player, Formatting.Indented);
        string path = @"C:\Users\sidc2\Documents\GitHub\alterlight\Alterlight\Assets\UserData\user_data.json";
        File.WriteAllText(path, jsonObj);

        SceneManager.LoadScene("Main");
    }
    public void Skip() => SceneManager.LoadScene("Main");
}
