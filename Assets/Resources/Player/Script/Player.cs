using UnityEngine;

public class Player : MonoBehaviour
{
    private int CurrentHealth {get; set;}
    private int MaxHealth  {get; set;}
    private int AttackDamage {get; set;}
    private string Playername {get; set;}
    
    // Start is called before the first frame update
    public Player(int currentHealth, string playername, int maxHealth = 20, int attackDamage = 5)
    {
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
        AttackDamage = attackDamage;
        Playername = playername;
    }
}
