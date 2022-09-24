using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingData buildingData;
    public int health;
    public int id;

    // Start is called before the first frame update
    void Start()
    {
        health = buildingData.health;
        GameEvents.current.onHealthDecrease += DecreaseHealth;
    }

    public void DecreaseHealth(int health, int id)
    {
        if (this.id == id)
        {
            this.health -= health;
            if (this.health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnDestroy()
    {
        GameEvents.current.onHealthDecrease -= DecreaseHealth;
    }
}
