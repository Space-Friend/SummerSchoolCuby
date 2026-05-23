using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Burst.Intrinsics.X86;
using static UnityEngine.Audio.ProcessorInstance;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 5f; //left-right
    [SerializeField] private float _acceleration = 20f; //Smoothness
    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _friction = 5f;
    [SerializeField] private float _turnSpeed = 50f; //Rotation acceleration
    [SerializeField] private float _maxTurnSpeed = 550f;
    [SerializeField] private float _jumpSpeed = 8f; //Power Jump
    [SerializeField] private short _maxJumps = 1; //Double jump
    [SerializeField] private TextMeshProUGUI _diamondNum;
    [SerializeField] private TextMeshProUGUI _jumpNum;

    [Header("Sounds")]
    [SerializeField] private AudioClip _jump_mp3; //Jump sound
    [SerializeField] private AudioClip _death_mp3;
    [SerializeField] private AudioClip _win_mp3;
    [SerializeField] private AudioClip _checkPoint_mp3;
    [SerializeField] private AudioClip _pickup_mp3;
    [SerializeField] private AudioClip _diamond_mp3;
    private AudioSource _audioSource;

    private bool _isGrounded;
    private short _jumpsRemaining;

    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private readonly bool _jumpInput;
    private Animator _animator;
    private Vector3 _respawnPoint;
    private short _collectedDiamonds = 0;
    private HashSet<GameObject> _processedPickups = new HashSet<GameObject>();

    //Start
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _jumpsRemaining = 0;
        _audioSource = gameObject.AddComponent<AudioSource>();
        _respawnPoint = gameObject.transform.position;
    }

    //Movement
    private void FixedUpdate()
    {
        if (_moveInput.x != 0)
        {
            _rb.linearVelocityX += _moveInput.x * _speed;
            float targetSpeed = _moveInput.x * _maxSpeed;
             // Change our cpeed
            _rb.linearVelocityX = Mathf.MoveTowards(_rb.linearVelocityX, targetSpeed, _acceleration * Time.fixedDeltaTime);
        }
        // Apply friction when no input
        else
        {
            _rb.linearVelocityX = Mathf.MoveTowards(_rb.linearVelocityX, 0, _friction);
        }

        // Check if we're trying to move in the opposite direction of current spin
        if (_moveInput.x != 0 && Mathf.Sign(_moveInput.x) != Mathf.Sign(-_rb.angularVelocity))
        {
            _rb.angularVelocity = -_moveInput.x * _turnSpeed * 0.3f;
        }
        // Only apply torque if below max speed
        else if (Mathf.Abs(_rb.angularVelocity) < _maxTurnSpeed)
        {
            float speedFactor = 1f - (Mathf.Abs(_rb.angularVelocity) / _maxTurnSpeed);
            _rb.AddTorque(-_moveInput.x * _turnSpeed * speedFactor, ForceMode2D.Force);
        }
    }

    void Update()
    {
        _animator.SetFloat("velocity", _rb.linearVelocity.magnitude);
        _processedPickups.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Ground":
                {
                    _isGrounded = true;
                    _jumpsRemaining = _maxJumps; // Reset jumps when touching ground
                    _jumpNum.text = _maxJumps.ToString();
                    break;
                }
            case "JumpCoin":
                {
                    if (_processedPickups.Add(other.gameObject))
                    {
                        _jumpsRemaining += 1; // jump
                        _jumpNum.text = _jumpsRemaining.ToString();
                        _audioSource.PlayOneShot(_pickup_mp3);
                        StartCoroutine(TemporaryDeactivate(other.gameObject, 5f));
                    }
                    break;
                }
            case "Diamond":
                {
                    if (_processedPickups.Add(other.gameObject))
                    {
                        _collectedDiamonds += 1;
                        _diamondNum.text = _collectedDiamonds.ToString();
                        _audioSource.PlayOneShot(_diamond_mp3);
                        Destroy(other.gameObject);
                    }
                    break;
                }
            case "Damaging":
                {

                    _audioSource.PlayOneShot(_death_mp3);
                    gameObject.transform.position = _respawnPoint; // Kills!
                    _rb.linearVelocity = new Vector2(0, 0);
                    break;
                }
            case "CheckPoint":
                {
                    _audioSource.PlayOneShot(_checkPoint_mp3);
                    _respawnPoint = gameObject.transform.position;
                    other.gameObject.SetActive(false);
                    break;
                }
            case "Win":
                {
                    _audioSource.PlayOneShot(_checkPoint_mp3);
                    AudioSource.PlayClipAtPoint(_win_mp3, transform.position);
                    Destroy(gameObject);
                    break;
                }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground")) _isGrounded = false;
    }

    //Input
    public void Move(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // Only jump on button press (not hold or release)
        if (context.performed)
        {
            if (_jumpsRemaining > 0)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpSpeed);
                _audioSource.PlayOneShot(_jump_mp3, 0.8f);
                if (!_isGrounded)
                {
                    _jumpsRemaining--;
                    _jumpNum.text = _jumpsRemaining.ToString();
                }
            }
        }
    }
    private IEnumerator TemporaryDeactivate(GameObject obj, float delay)
    {
        obj.SetActive(false);
        yield return new WaitForSeconds(delay);
        obj.SetActive(true);
    }
}