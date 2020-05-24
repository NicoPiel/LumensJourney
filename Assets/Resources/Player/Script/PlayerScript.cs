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


    // Start is called before the first frame update
    void Start()
    {
        healthBarScript = GameObject.Find("Healthbar(Clone)").GetComponent<HealthbarScript>();
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        healthBarScript.ChangeHealthBar(2);
        hitCollider = transform.Find("HitCollider").GetComponent<BoxCollider2D>();
        hitCollider.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            hitCollider.transform.rotation = new Quaternion(0,0,180, 0);
            hitCollider.gameObject.SetActive(true);
            _animator.Play("SwingUp");
            hitCollider.gameObject.SetActive(false);
        }else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            hitCollider.transform.rotation = new Quaternion(0,0,270, 0);
            hitCollider.gameObject.SetActive(true);
            _animator.Play("SwingRight");
            hitCollider.gameObject.SetActive(false);
            
        }else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            hitCollider.transform.rotation = new Quaternion(0,0,0, 0);
            hitCollider.gameObject.SetActive(true);
            
            _animator.Play("SwingDown");
            hitCollider.gameObject.SetActive(false);
        }else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            hitCollider.transform.rotation = new Quaternion(0,0,90, 0);
            hitCollider.gameObject.SetActive(true);
            Debug.Log("Penis an");
            _animator.Play("SwingLeft");
            hitCollider.gameObject.SetActive(false);
            Debug.Log("Penis aus");
        }
        else
        {
            _change = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0.0f);
            _change.Normalize();
        }
        
        
        
    }

    private void OnTriggerStay2D(Collider2D other)
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
