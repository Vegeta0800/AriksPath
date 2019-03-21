using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Buttons : MonoBehaviour {

    public delegate void OnResumed();
    public static OnResumed onResumed;


    [SerializeField] private Animator ani;
    private string level;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject settingsMenu;

    [SerializeField] private AudioMixer mixer;

    [SerializeField] private EventSystem eventSystem;


    [SerializeField] private Transform player;

    private bool mainMenuActive;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Start()
    {
        Scene scene = SceneManager.GetActiveScene();

        if(scene.name == "Main Menu")
        {
            mainMenuActive = true;

            GoToMenu("main");

            if (PlayerPrefs.HasKey("level"))
            {
                level = PlayerPrefs.GetString("level");
            }
            else
            {
                level = "Level 1";
            }

        }
        else
        {
            mainMenuActive = false;
            level = "Main Menu";
        }
    }

    public void SetMusicValue(float musicLvl)
    {
        mixer.SetFloat("music", musicLvl);
    }

    public void SetSfxValue(float sfxLvl)
    {
        mixer.SetFloat("sfx", sfxLvl);
    }

    public void FadeToLevel()
    {
        Time.timeScale = 1f;

        ani.SetTrigger("FadeOut");
    }

    public void GoToMenu(string menu)
    {
        if (menu == "main")
        {
            mainMenu.SetActive(true);
            controlsMenu.SetActive(false);
            playMenu.SetActive(false);
            settingsMenu.SetActive(false);
            eventSystem.SetSelectedGameObject(GameObject.Find("Play"));
        }
        else if (menu == "play")
        {
            mainMenu.SetActive(false);
            playMenu.SetActive(true);
            controlsMenu.SetActive(false);
            settingsMenu.SetActive(false);
            eventSystem.SetSelectedGameObject(GameObject.Find("Play"));
        }
        else if (menu == "controls")
        {
            mainMenu.SetActive(false);
            playMenu.SetActive(false);
            controlsMenu.SetActive(true);
            settingsMenu.SetActive(false);
            eventSystem.SetSelectedGameObject(GameObject.Find("Back"));
        }
        else if (menu == "settings")
        {
            mainMenu.SetActive(false);
            playMenu.SetActive(false);
            controlsMenu.SetActive(false);
            settingsMenu.SetActive(true);
            eventSystem.SetSelectedGameObject(GameObject.Find("Back"));
        }
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(level, LoadSceneMode.Single);
    }

    public void Exit()
    {
        //if (UnityEditor.EditorApplication.isPlaying == true)
        //{
        //    UnityEditor.EditorApplication.isPlaying = false;
        //}

        //else
        //{
            Application.Quit();
        //}
    }

    public void Credits()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Resets()
    {
        PlayerPrefs.DeleteKey("LastPositionX");
        PlayerPrefs.DeleteKey("LastPositionY");
        PlayerPrefs.DeleteKey("LastPositionZ");
        level = "Level 1";
        PlayerPrefs.Save();
    }

    public void Save()
    {
        Scene activeScene = SceneManager.GetActiveScene();

        PlayerPrefs.SetFloat("LastPositionX", player.position.x);
        PlayerPrefs.SetFloat("LastPositionY", player.position.y);
        PlayerPrefs.SetFloat("LastPositionZ", player.position.z);
        PlayerPrefs.SetString("level", activeScene.name);
        PlayerPrefs.Save();
    }

    public void Resume()
    {
        if(onResumed != null)
        {
            onResumed();
        }
    }

}
