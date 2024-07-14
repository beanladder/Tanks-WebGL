using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using JetBrains.Annotations;
using Photon.Pun.Demo.Cockpit;
using Unity.VisualScripting;
using Photon.Pun.Demo.PunBasics;

using Cinemachine;
using ExitGames.Client.Photon.StructWrapping;


public class NetworkTankInfo : MonoBehaviourPunCallbacks
{
    public static NetworkTankInfo instance;
    public float maxHealth = 100f; // Maximum health of the tank
    public float currentHealth; // Current health of the tank
    public GameObject destroyPrefab;
    public TMP_Text healthText; // Reference to the TextMeshPro component for displaying health
    public TMP_Text tankNameText;
    public KeyCode repairKey = KeyCode.X; // Key to trigger repair
    public GameObject healthIndicator; // Reference to the game object to enable/disable
    public GameObject healthDeductUI;
    public GameObject healthAddUI;
    public bool repairCooldown;
    public AudioSource repairAudio;
    public string playerName;
    private bool isRepairing = false; // Flag to indicate if the tank is currently in repair mode
    public float repairTime = 4f; // Time in seconds for repair
    private float healAmountMin = 5f; // Minimum amount of healing
    private float healAmountMax = 15f; // Maximum amount of healing
    private NetworkSquareMovement networkSquareMovementScript;
    private AudioSource moveAudio;
    // [SerializeField]private CinemachineFreeLook mapCamera;
    PhotonView view;
    void Awake(){
        instance = this;
    }
    void Start()
    {
        // GameObject mapCameraObject = GameObject.Find("MapCamera");
        // if (mapCameraObject != null)
        // {
        //     mapCamera = mapCameraObject.GetComponent<CinemachineFreeLook>();
        // }
        tankNameText = GameObject.Find("PlayerName").GetComponent<TMP_Text>();
        repairCooldown = true;
        view = GetComponent<PhotonView>();
        networkSquareMovementScript = GetComponent<NetworkSquareMovement>();
        moveAudio = GetComponent<AudioSource>();
        // Initialize current health to max health at the start
        currentHealth = maxHealth;

        // if (view.IsMine)
        // {
        //     CameraManager.instance.SetCurrentTankCamera(GetComponentInChildren<CinemachineFreeLook>());
        // }
        // Find the TextMeshPro component in the scene and assign it to the healthText field
        healthText = GameObject.Find("Health").GetComponent<TMP_Text>();

        // Update the health UI text
        UpdateHealthText();

        if(view.IsMine && PhotonNetwork.LocalPlayer!=null){
            playerName = PhotonNetwork.LocalPlayer.NickName;
            Debug.Log("Player name : "+playerName);
        }

        if(tankNameText!=null){
            tankNameText.text = playerName;
        }
    }

    void Update()
    {
        if(view.IsMine)
        {
            UpdateHealthText();
            // Check if the repair key is pressed
            if (Input.GetKeyDown(repairKey) && currentHealth < maxHealth && repairCooldown)
            {
                // Start the repair process
                //StartRepair();
                view.RPC("StartRepair",RpcTarget.All);
            }

            // If currently repairing, decrease repair time
            if (isRepairing)
            {
                repairTime -= Time.deltaTime;

                // Check if repair time is over
                if (repairTime <= 0f)
                {
                    // End repair process
                    //EndRepair();
                    view.RPC("EndRepair",RpcTarget.All);
                }
            }
        }
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        int hitResistor = 1;
        currentHealth -= damage;
        StartCoroutine(HealthDeduction(damage));
        
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Clamp health between 0 and maxHealth

        // Update the health UI text
        if (currentHealth <= 0)
        {
            if (view.IsMine)
            {
                SpawnPlayer.instance.SetDeadTankId(PhotonNetwork.LocalPlayer.ActorNumber);
                // Blend to the map camera when the tank dies
                // CameraManager.instance.BlendToMapCamera();s
            }
            DestructionPhase();
            Destroy(gameObject);
        }
        else if(currentHealth <=25)
        {
            maxHealth = 75;
            hitResistor=hitResistor+3;
        }
        else if(currentHealth <=45)
        {
            maxHealth = 82;
            hitResistor=hitResistor+2;
        }
        else if(currentHealth <= 60)
        {
            maxHealth = 93;
            hitResistor++;
        }
        
        maxHealth=maxHealth-hitResistor;
    }

    public void mobileRepair()
    {
        if (currentHealth < maxHealth && repairCooldown)
        {
            // Start the repair process
            StartRepair();
        }
    }
    [PunRPC]
    public void StartRepair()
    {
        repairCooldown = false;
        isRepairing = true;
        Debug.Log("Starting repair...");
        if (view.IsMine)
        {
            HealthUIAnimation.instance.StartRepairAnimation();
        }
        view.RPC("DisableMovementAndPlayRepairAudio", RpcTarget.All);
    }

[PunRPC]
public void EndRepair()
{
    isRepairing = false;

    // Generate random healing amount
    float healAmount = Random.Range(healAmountMin, healAmountMax);

    // Clamp heal amount so it doesn't exceed maxHealth
    StartCoroutine(HealthAddition(healAmount));

    // Update the health UI text
    Debug.Log("Repair complete. Healed " + healAmount + " health.");
    repairTime = 4f;
    if (view.IsMine)
    {
        HealthUIAnimation.instance.StopRepairAnimation();
    }
    view.RPC("EnableMovementAndStopRepairAudio", RpcTarget.All);
    repairCooldown = true;
}

    [PunRPC]
    public void DisableMovementAndPlayRepairAudio()
    {
        networkSquareMovementScript.enabled = false;
        moveAudio.enabled = false;
        repairAudio.Play();
    }

    [PunRPC]
    public void EnableMovementAndStopRepairAudio()
    {
        networkSquareMovementScript.enabled = true;
        repairAudio.Stop();
        moveAudio.enabled = true;
    }

    public void DestructionPhase()
    {
        GameObject newDestroy = Instantiate(destroyPrefab, transform.position, transform.rotation);
        Destroy(newDestroy, 2.5f);
    }

    public IEnumerator HealthAddition(float heal)
    {
        Debug.Log("Repair Animation should start");
        healthAddUI.SetActive(true);
        TMP_Text healText = healthAddUI.GetComponent<TMP_Text>();
        healText.text = heal.ToString("0");
        yield return new WaitForSeconds(1f);
        healthAddUI.SetActive(false);
        float potentialHealth = currentHealth + heal;
        currentHealth = Mathf.Clamp(potentialHealth, 0f, maxHealth);
    }

    public IEnumerator HealthDeduction(float damage)
    {
        Debug.Log("Tank should take damage");
        healthDeductUI.SetActive(true);
        TMP_Text damagetext = healthDeductUI.GetComponent<TMP_Text>();
        damagetext.text = damage.ToString("0");
        yield return new WaitForSeconds(1f);
        healthDeductUI.SetActive(false);
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString("0");
        }

        // Check if health is below 60
        if (currentHealth < 60f)
        {
            // Enable the health indicator game object
            healthIndicator.SetActive(true);
        }
        else
        {
            // Disable the health indicator game object
            healthIndicator.SetActive(false);
        }
    }
}
