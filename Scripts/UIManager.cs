using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void OnBeginningPress()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
