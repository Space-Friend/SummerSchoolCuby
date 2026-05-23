using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] private float _bounceForce = 10f;
    [SerializeField] private AudioClip _bounce;
    private Animator _animator;
    private AudioSource _audioSource;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _animator.SetTrigger("Bounce");

            // Apply force to player
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 bounceDirection = transform.up;  // The trampoline's local "up" direction
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0) + bounceDirection * _bounceForce;
                _audioSource.PlayOneShot(_bounce,0.15f);
            }
        }
    }
}