using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PlayerScript : MonoBehaviour
{
    public float speed;
    private Rigidbody2D _playerRigidbody2D;
    private Vector3 _change;
    private AudioSource _audioSource;
    private Animator _animator;
    private Player _player = new Player("Pacolos");
    public HealthbarScript healthBarScript;
    public BoxCollider2D hitCollider;
    private static readonly int StateExit = Animator.StringToHash("StateExit");


    public Dictionary<string, AudioClip> _audioClips;



    // Start is called before the first frame update
    private void Start()
    {
        healthBarScript = GameObject.Find("Healthbar(Clone)").GetComponent<HealthbarScript>();
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _audioClips = new Dictionary<String, AudioClip>();
        healthBarScript.ChangeHealthBar(2);
        hitCollider = transform.Find("HitCollider").GetComponent<BoxCollider2D>();
        hitCollider.gameObject.SetActive(false);
        _animator.SetBool(StateExit, false);

        _audioSource = GetComponent<AudioSource>();

        AddAudioClips();

    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            hitCollider.transform.eulerAngles = new Vector3(0, 0, 180);
            hitCollider.size= new Vector2(2, 1);
            hitCollider.offset =new Vector2(0, -1);
            hitCollider.gameObject.SetActive(true);
            _animator.SetBool(StateExit, false);
            _animator.Play("SwingUp");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            hitCollider.transform.eulerAngles = new Vector3(0, 0, 90);
            hitCollider.size = new Vector2(1,1.5f);
            hitCollider.offset =  new Vector2(0,-1);
            hitCollider.gameObject.SetActive(true);
            _animator.SetBool(StateExit, false);
            _animator.Play("SwingRight");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            hitCollider.transform.eulerAngles = new Vector3(0, 0, 0);
            hitCollider.size=new Vector2(2,1);
            hitCollider.offset=new Vector2(0,-0.5f);
            hitCollider.gameObject.SetActive(true);
            _animator.SetBool(StateExit, false);
            _animator.Play("SwingDown");
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            hitCollider.transform.eulerAngles = new Vector3(0, 0, 270);
            hitCollider.size=new Vector2(1,1.5f);
            hitCollider.offset=new Vector2(0,-1);
            hitCollider.gameObject.SetActive(true);
            _animator.SetBool(StateExit, false);
            _animator.Play("SwingLeft");
        }
        else
        {
            _change = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0.0f);
            _change.Normalize();
        }

        if (_animator.GetBool(StateExit))
        {
            hitCollider.gameObject.SetActive(false);
            _animator.SetBool(StateExit, false);
            Debug.Log("State Exit");
        }
    }

    public void PlayFootsteps()
    {
        int rnd = Random.Range(0, 3);
        _audioSource.clip = _audioClips["footstep0"+rnd];
        _audioSource.Play();
    }

    public void PlaySwordWoosh()
    {
        int rnd = Random.Range(1, 8);
        _audioSource.clip = _audioClips["woosh"+rnd];
        _audioSource.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Penis");
    }

    public void AddToInventory(GameItem item)
    {
        _player.Inventory.AddItem(item);
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        _animator.SetFloat("Horizontal", _change.x);
        _animator.SetFloat("Vertical", _change.y);
        _animator.SetFloat("Speed", _change.magnitude);
        _playerRigidbody2D.MovePosition(transform.position + (_change * speed * Time.fixedDeltaTime));
    }

    private void AddAudioClips()
    {
        for( int i = 1; i <= 8; i++ )
        {
            _audioClips.Add("woosh"+i, Resources.Load<AudioClip>("Audio/Wooshes/woosh"+i));
        }

        for (int i = 0; i <= 2; i++)
        {
            _audioClips.Add("footstep0"+i, Resources.Load<AudioClip>("Audio/Footsteps/footstep0"+i));
        }
    }
}