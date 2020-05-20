using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
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
    public ItemBarScript itemBarScript;
    
    

    // Start is called before the first frame update
    void Start()
    {
        healthBarScript = GameObject.Find("Healthbar(Clone)").GetComponent<HealthbarScript>();
        itemBarScript = GameObject.Find("ItemBar(Clone)").GetComponent<ItemBarScript>();
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _player.Inventory.AddItem("Dragons claw");
        _player.Inventory.AddItem("Water ring");
        itemBarScript.CreateItemBar(_player.Inventory);
        
        
        healthBarScript.ChangeHealthBar(2);

    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            _animator.Play("SwingUp");
        }else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _animator.Play("SwingRight");
            
        }else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _animator.Play("SwingDown");
        }else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _animator.Play("SwingLeft");
        }
        else
        {
            _change = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0.0f);
            _change.Normalize();
        }
        
        
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
