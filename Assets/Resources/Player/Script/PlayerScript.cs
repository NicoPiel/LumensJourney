using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float speed;

    private Rigidbody2D _playerRigidbody2D;

    private Vector3 _change;

    private Animator _animator;
    
    // Start is called before the first frame update
    private void Start()
    {
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            _animator.Play("SwingUp");
        }else if (Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            _animator.Play("SwingRight");
        }else if (Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _animator.Play("SwingDown");
        }else if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.LeftArrow))
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
