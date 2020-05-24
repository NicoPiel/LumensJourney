using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class PlayerScript : MonoBehaviour
{
    public float speed;

    private Rigidbody2D _playerRigidbody2D;

    private Vector3 _change;

    private Animator _animator;

    private Player _player = new Player("Pacolos");
    public HealthbarScript healthBarScript;
    public BoxCollider2D hitCollider;
    private static readonly int StateExit = Animator.StringToHash("StateExit");


    // Start is called before the first frame update
    private void Start()
    {
        healthBarScript = GameObject.Find("Healthbar(Clone)").GetComponent<HealthbarScript>();
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        healthBarScript.ChangeHealthBar(2);
        hitCollider = transform.Find("HitCollider").GetComponent<BoxCollider2D>();
        hitCollider.gameObject.SetActive(false);
        _animator.SetBool(StateExit, false);
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
}