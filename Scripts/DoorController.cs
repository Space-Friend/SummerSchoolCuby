using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Closed Door Sprites")]
    [SerializeField] private GameObject closedSprite1;
    [SerializeField] private GameObject closedSprite2;

    [Header("Open Door Sprites")]
    [SerializeField] private GameObject openSprite1;
    [SerializeField] private GameObject openSprite2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Turn off closed sprites
        closedSprite1.SetActive(false);
        closedSprite2.SetActive(false);

        // Turn on open sprites
        openSprite1.SetActive(true);
        openSprite2.SetActive(true);
    }
}