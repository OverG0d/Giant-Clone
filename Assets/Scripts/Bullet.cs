using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public Rigidbody rigid;
    public GameObject explosion;

    public void Start()
    {
        Destroy(gameObject, 15f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Building))
        {
            GameEvents.current.HealthDecrease(damage, other.GetComponent<Building>().id);
        }
        else if(other.CompareTag(Tags.Head))
        {
            GameEvents.current.HealthDecrease(damage, other.transform.root.gameObject.GetComponent<Giant>().id);
        }
        else if (other.CompareTag(Tags.Leg))
        {
            GameEvents.current.HealthDecrease(damage / 3, other.transform.root.gameObject.GetComponent<Giant>().id);
        }
        else if (other.CompareTag(Tags.Arm))
        {
            GameEvents.current.HealthDecrease(damage / 3, other.transform.root.gameObject.GetComponent<Giant>().id);
        }
        else if (other.CompareTag(Tags.Body))
        {
            GameEvents.current.HealthDecrease(damage / 2, other.transform.root.gameObject.GetComponent<Giant>().id);
        }
        GameObject explosion_Clone = (GameObject)Instantiate(explosion, transform.position, transform.rotation);
        Destroy(explosion_Clone, 2f);
        Destroy(gameObject);
    }
}
