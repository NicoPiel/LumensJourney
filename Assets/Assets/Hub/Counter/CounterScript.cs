using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class CounterScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer L;
    [SerializeField] private SpriteRenderer U;
    [SerializeField] private SpriteRenderer M;
    [SerializeField] private SpriteRenderer E;
    [SerializeField] private SpriteRenderer N;

    [SerializeField] private Sprite LOn;
    [SerializeField] private Sprite UOn;
    [SerializeField] private Sprite MOn;
    [SerializeField] private Sprite EOn;
    [SerializeField] private Sprite NOn;
    
    [SerializeField] private Sprite LOff;
    [SerializeField] private Sprite UOff;
    [SerializeField] private Sprite MOff;
    [SerializeField] private Sprite EOff;
    [SerializeField] private Sprite NOff;
    // Start is called before the first frame update
    void Start()
    {
        L.sprite = LOff;
        U.sprite = UOff;
        M.sprite = MOff;
        E.sprite = EOff;
        N.sprite = NOff;
        var runs = GameManager.GetSaveSystem().RunsCompleted;
        if (runs >= 1)
        {
            L.sprite = LOn;
        }

        if (runs >= 2)
        {
            U.sprite = UOn;
        }

        if (runs >= 3)
        {
            M.sprite = MOn;
        }

        if (runs >= 4)
        {
            E.sprite = EOn;
        }

        if (runs >= 5)
        {
            N.sprite = NOn;
        }
   
        
    }
}
