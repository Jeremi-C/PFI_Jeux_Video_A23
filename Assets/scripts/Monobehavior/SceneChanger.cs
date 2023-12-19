using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    const string sceneName = "Level";
    public static int GlobalLevelNumber = 2;
    [SerializeField] private int levelNumber = GlobalLevelNumber;
    private Button button;
    //Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(()=>ChangeLevel());
        //DontDestroyOnLoad(gameObject);
    }
    public void ChangeLevel()
    {
        GlobalLevelNumber = levelNumber;
        SceneManager.LoadScene(sceneName);
    }    
}
