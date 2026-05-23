using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private AudioClip _pipe;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
    }
    public void OnStartClick()
    {
        SceneManager.LoadScene("Game"); 
    }
    public void OnPipeClick()
    {
        
        _audioSource.PlayOneShot(_pipe);
    }
    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
