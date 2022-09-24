using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Giant : MonoBehaviour
{
    public AudioSource source;
    public NavMeshAgent agent;
    public List<Collider> RagDollParts = new List<Collider>();
    public GiantData giantData;
    public int health;
    public int id;
    public Slider slider;
    public List<Transform> littleGuyTransforms = new List<Transform>();
    Transform currentLittleGuy;

    // Start is called before the first frame update
    void Start()
    {       
        
        health = giantData.health;
        slider.value = health * 0.01f;
        SetRagDollParts();
        ChooseLittleGuy();
        GameEvents.current.onHealthDecrease += DecreaseHealth;
    }

    public void Update()
    {
        if(GameEvents.current.gameStart && currentLittleGuy != null)
        agent.SetDestination(currentLittleGuy.position);
    }

    public void SetRagDollParts()
    {
        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();

        foreach (Collider c in colliders)
        {
            if (c.gameObject != this.gameObject)
            {
                if (c.gameObject.GetComponent<Rigidbody>() != null)
                    c.gameObject.GetComponent<Rigidbody>().useGravity = false;
                c.isTrigger = true;
                RagDollParts.Add(c);
            }
        }
    }

    public void TurnOnRagDoll()
    {
        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();

        foreach (Collider c in colliders)
        {
            if (c.gameObject != this.gameObject)
            {
                if (c.gameObject.GetComponent<Rigidbody>() != null)
                    c.gameObject.GetComponent<Rigidbody>().useGravity = true;
                c.isTrigger = false;
                RagDollParts.Add(c);
            }
        }
    }

    IEnumerator RagdollOn()
    {
        yield return new WaitForSeconds(5f);
        TurnOnRagDoll();
    }

    public void DecreaseHealth(int health, int id)
    {
        if (this.id == id)
        {
            this.health -= health;
            slider.value = this.health * 0.01f;
            if (this.health <= 0)
            {
                slider.value = 0;
                if (GameEvents.current.gameStart)
                {
                    GameEvents.current.numGiants--;
                    GameEvents.current.Win();
                }
                TurnOnRagDoll();
                var growlClip = Resources.Load<AudioClip>("Sounds/Growl");
                source.PlayOneShot(growlClip, 1f);
                GameEvents.current.onHealthDecrease -= DecreaseHealth;
            }
        }       
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.collider.CompareTag(Tags.LittleGuy))
        {
            if (GameEvents.current.gameStart)
            {
                Destroy(other.gameObject);
                GameEvents.current.numLittleGuys--;
                GameEvents.current.Lose();
            }           
            littleGuyTransforms.Remove(other.gameObject.transform);
            if(littleGuyTransforms.Count != 0)
            ChooseLittleGuy();
        }
    }

    public void ChooseLittleGuy()
    {
        int randNum = Random.Range(0, littleGuyTransforms.Count - 1);
        currentLittleGuy = littleGuyTransforms[randNum];

    }
}
