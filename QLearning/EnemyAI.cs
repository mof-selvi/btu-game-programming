using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float health = 100;
    private QLearningBrain brain;

    void Start()
    {
        brain = GetComponent<QLearningBrain>();

        // Register actions (methods + parameter counts)
        brain.RegisterAction("Attack", p => Attack((float)p[0]), 1);
        brain.RegisterAction("Dash", p => Dash(), 0);
        brain.RegisterAction("Heal", p => Heal(), 0);
        brain.RegisterAction("Roar", p => Roar((int)p[0]), 1);
    }

    void Update()
    {
        // Build input vector
        float dist = Vector3.Distance(transform.position, player.position);
        float playerHP = 100; // example reference
        var inputs = new List<float> { dist, health, playerHP };

        brain.SetInputs(inputs);

        // Decide & execute
        int actionIndex = brain.DecideAction();

        switch (actionIndex)
        {
            case 0: brain.ExecuteAction(actionIndex, 10f); break;   // Attack damage
            case 1: brain.ExecuteAction(actionIndex); break;         // Dash
            case 2: brain.ExecuteAction(actionIndex); break;         // Heal
            case 3: brain.ExecuteAction(actionIndex, 2); break;      // Roar intensity
        }
    }

    // --- Actions ---
    void Attack(float dmg) { Debug.Log("Enemy attacks " + dmg); }
    void Dash() { Debug.Log("Enemy dashes!"); }
    void Heal() { health += 5; }
    void Roar(int level) { Debug.Log("Enemy roars level " + level); }

    // --- Reward triggers ---
    public void OnDamagePlayer() => brain.Reward(10f);
    public void OnTakeDamage() => brain.Punish(5f);
    public void OnDie() => brain.Punish(50f);
    public void OnKillPlayer() => brain.Reward(100f);
}
