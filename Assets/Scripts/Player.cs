using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Animator animator;
    public AudioSource source;
    public float mouseSensitivity = 100f;
    public float bulletSpeed;
    public float volume;
    public int currentAmmo;
    public bool reloading;
    int currentGun;

    public Transform playerBody;

    public float xRotation = 0f;

    private bool scoped = false;

    public GameObject scopeOverlay;
    public GameObject[] guns;
    public GameObject weaponCamera;
    public GameObject bullet;
    public Transform spawnPoint;
    public Camera mainCamera;

    public float scopedFOV = 15f;
    private float normalFOV;

    public Text reload;
    public Text pressToShoot;

    // Start is called before the first frame update
    void Start()
    {
        currentGun = 0;
        Cursor.lockState = CursorLockMode.Locked;
        playerBody.rotation = Quaternion.Euler(0f, 180f, 0f);
        currentAmmo = guns[currentGun].GetComponent<Gun>().gunData.ammo;
    }

    // Update is called once per frame
    void Update()
    {    
        PlayerControls();
        
        if (scoped)
            StartCoroutine(Scoped());
        else
            UnScoped();
    }

    public void PlayerControls()
    {
        if (GameEvents.current.numLittleGuys != 0 && GameEvents.current.numGiants != 0)
        {
            if (Input.GetButtonDown("Fire1") && !reloading)
            {
                pressToShoot.text = "";
                if (GameEvents.current.numLittleGuys != 0 && GameEvents.current.numGiants != 0)
                    GameEvents.current.gameStart = true;
                scoped = !scoped;
                animator.SetBool("Scoped", scoped);
                if (!scoped)
                {
                    Shoot();
                }
            }

            if (Input.GetButtonDown("Fire2") && !scoped)
            {
                guns[currentGun].SetActive(false);
                if (currentGun == 1)
                {
                    currentGun = 0;
                    guns[currentGun].SetActive(true);
                    currentAmmo = guns[currentGun].GetComponent<Gun>().gunData.ammo;
                }
                else
                {
                    currentGun++;
                    guns[currentGun].SetActive(true);
                    currentAmmo = guns[currentGun].GetComponent<Gun>().gunData.ammo;
                }
            }
        }

        if (GameEvents.current.gameStart)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            var rot = playerBody.transform.eulerAngles;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -45f, 45f);

            rot.y = Mathf.Clamp(playerBody.transform.eulerAngles.y, 150f, 220f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.transform.eulerAngles = rot;
            playerBody.Rotate(Vector3.up * mouseX);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void Shoot()
    {
        GameObject bullet_Clone = (GameObject)Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);
        bullet_Clone.GetComponent<Bullet>().rigid.velocity = spawnPoint.forward * bulletSpeed * Time.deltaTime;
        bullet_Clone.GetComponent<Bullet>().damage = guns[currentGun].GetComponent<Gun>().gunData.damage;
        var sniperShotClip = Resources.Load<AudioClip>("Sounds/Sniper Shot");
        source.PlayOneShot(sniperShotClip, volume);
        currentAmmo--;

        if(currentAmmo == 0)
        {
            var reloadClip = Resources.Load<AudioClip>("Sounds/Reload");
            source.PlayOneShot(reloadClip, volume);
            reloading = true;
            StartCoroutine(Reloading());
        }
    }

    IEnumerator Scoped()
    {
        yield return new WaitForSeconds(.15f);
        scopeOverlay.SetActive(true);
        weaponCamera.SetActive(false);

        normalFOV = mainCamera.fieldOfView;
        mainCamera.fieldOfView = scopedFOV;
    }

    void UnScoped()
    {
        scopeOverlay.SetActive(false);
        weaponCamera.SetActive(true);
        mainCamera.fieldOfView = 60;
    }

    IEnumerator Reloading()
    {
        reload.text = "Reloading...";
        yield return new WaitForSeconds(guns[currentGun].GetComponent<Gun>().gunData.reloadTime);
        reload.text = "";
        reloading = false;
        currentAmmo = guns[currentGun].GetComponent<Gun>().gunData.ammo;
    }
}

