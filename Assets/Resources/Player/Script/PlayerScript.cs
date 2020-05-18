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

    public ItemBar itembar;




    // Start is called before the first frame update
    void Start()
    {
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _player.Inventory.AddItem("Dragons claw");
        _player.Inventory.AddItem("Water ring");
        itembar = GameObject.Find("ItemBar").GetComponent<ItemBar>();
        itembar.CreateItemBar(_player.Inventory);
        

    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.I))
        {
            _animator.Play("SwingUp");
        }else if (Input.GetKeyDown(KeyCode.L))
        {
            _animator.Play("SwingRight");
            
        }else if (Input.GetKeyDown(KeyCode.K))
        {
            _animator.Play("SwingDown");
        }else if (Input.GetKeyDown(KeyCode.J))
        {
            _animator.Play("SwingLeft");
        }
        else
        {
            _change = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0.0f);
        }
        
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        _animator.SetFloat("Horizontal", _change.x);
        _animator.SetFloat("Vertical", _change.y);
        _animator.SetFloat("Speed", _change.magnitude);
        _playerRigidbody2D.MovePosition(transform.position + (_change * speed * Time.deltaTime));
    }


}
