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
using Photon.Realtime;



public class NetworkTankInfo : MonoBehaviourPunCallbacks
{
    public static NetworkTankInfo instance;
    public float maxHealth = 100f; // Maximum health of the tank
    public float currentHealth; // Current health of the tank
    public GameObject destroyPrefab;
    public TMP_Text healthText; // Reference to the TextMeshPro component for displaying health
    [SerializeField] private TMP_Text tankNameText;
    public string playerName;
    public KeyCode repairKey = KeyCode.X; // Key to trigger repair
    public GameObject healthIndicator; // Reference to the game object to enable/disable
    public GameObject healthDeductUI;
    public GameObject healthAddUI;
    public bool repairCooldown;
    public AudioSource repairAudio;
    private bool isRepairing = false; // Flag to indicate if the tank is currently in repair mode
    public float repairTime = 4f; // Time in seconds for repair
    private float healAmountMin = 5f; // Minimum amount of healing
    private float healAmountMax = 15f; // Maximum amount of healing
    private HealthUIAnimation healthUIAnimation;
    private NetworkSquareMovement networkSquareMovementScript;
    private AudioSource moveAudio;
    [SerializeField] private CinemachineFreeLook tankCamera;
    PhotonView view;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        repairCooldown = true;
        view = GetComponent<PhotonView>();
        networkSquareMovementScript = GetComponent<NetworkSquareMovement>();
        moveAudio = GetComponent<AudioSource>();
        // Initialize current health to max health at the start
        currentHealth = maxHealth;
        healthText = GameObject.Find("Health").GetComponent<TMP_Text>();
        healthUIAnimation = GetComponent<HealthUIAnimation>();

        // Update the health UI text
        UpdateHealthText();
        SetPlayerName();
        UpdateNamesForAllPlayers();
    }

    void Update()
    {
        if (view.IsMine)
        {
            UpdateHealthText();
            // Check if the repair key is pressed
            if (Input.GetKeyDown(repairKey) && currentHealth < maxHealth && repairCooldown)
            {
                view.RPC("StartRepair", RpcTarget.All);
            }

            // If currently repairing, decrease repair time
            if (isRepairing)
            {
                repairTime -= Time.deltaTime;

                // Check if repair time is over
                if (repairTime <= 0f)
                {
                    view.RPC("EndRepair", RpcTarget.All);
                }
            }
        }
        if (!view.IsMine && tankNameText != null)
        {
            if (Camera.main != null)
            {
                tankNameText.transform.LookAt(Camera.main.transform);
            }
            tankNameText.transform.Rotate(Vector3.up, 180f);
        }
    }

    [PunRPC]
    public void TakeDamage(int damage, int shooterId)
    {
        int hitResistor = 1;
        currentHealth -= damage;
        StartCoroutine(HealthDeduction(damage));

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Clamp health between 0 and maxHealth

        // Notify the shooter to show the hit marker
        PhotonView shooterView = PhotonView.Find(shooterId);
        if (shooterView != null && shooterView.IsMine)
        {
            HUDManager shooterHUDManager = FindObjectOfType<HUDManager>();
            if (shooterHUDManager != null)
            {
                shooterHUDManager.ShowHitmarker();
            }
        }

        if (currentHealth <= 0)
        {
            if (view.IsMine)
            {
                // Show kill marker if destroyed by the current player
                if (PhotonNetwork.LocalPlayer.ActorNumber == shooterId)
                {
                    HUDManager localHUDManager = FindObjectOfType<HUDManager>();
                    if (localHUDManager != null)
                    {
                        localHUDManager.ShowKillmarker();
                    }
                }
                SpawnPlayer.instance.SetDeadTankId(PhotonNetwork.LocalPlayer.ActorNumber);
            }
            DestructionPhase();
            StartCoroutine(DestroyTank());
        }
        else if (currentHealth <= 25)
        {
            maxHealth = 75;
            hitResistor = hitResistor + 3;
        }
        else if (currentHealth <= 45)
        {
            maxHealth = 82;
            hitResistor = hitResistor + 2;
        }
        else if (currentHealth <= 60)
        {
            maxHealth = 93;
            hitResistor++;
        }

        maxHealth = maxHealth - hitResistor;
    }

    [PunRPC]
    public void ShakeCamera(Vector3 impactPosition, Vector3 impulseDirection, float impulseForce)
    {
        if (view.IsMine && tankCamera != null)
        {
            CinemachineImpulseSource impulseSource = GetComponent<CinemachineImpulseSource>();
            if (impulseSource != null)
            {
                Vector3 force = impulseDirection * impulseForce;
                impulseSource.GenerateImpulseAt(impactPosition, force);
            }
        }
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
            healthUIAnimation.StartRepairAnimation();
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
            healthUIAnimation.StopRepairAnimation();
        }
        view.RPC("EnableMovementAndStopRepairAudio", RpcTarget.All);
        repairCooldown = true;
    }

    [PunRPC]
    public void DisableMovementAndPlayRepairAudio()
    {
        networkSquareMovementScript.enabled = false;
        if (moveAudio != null && moveAudio.isPlaying)
        {
            moveAudio.Stop();
        }
        if (repairAudio != null)
        {
            repairAudio.Play();
        }
    }

    [PunRPC]
    public void EnableMovementAndStopRepairAudio()
    {
        networkSquareMovementScript.enabled = true;
        if (repairAudio != null)
        {
            repairAudio.Stop();
        }
    }

    public IEnumerator HealthDeduction(float deduction)
    {
        healthDeductUI.SetActive(true);
        healthDeductUI.GetComponent<TextMeshProUGUI>().text = "-" + deduction;
        yield return new WaitForSeconds(1.5f);
        healthDeductUI.SetActive(false);
    }

    public IEnumerator HealthAddition(float addition)
    {
        currentHealth += addition;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Ensure currentHealth doesn't exceed maxHealth
        UpdateHealthText();
        healthAddUI.SetActive(true);
        healthAddUI.GetComponent<TextMeshProUGUI>().text = "+" + addition;
        yield return new WaitForSeconds(1.5f);
        healthAddUI.SetActive(false);
    }

    private void UpdateHealthText()
    {
        healthText.text = "Health: " + currentHealth.ToString();
    }

    private void SetPlayerName()
    {
        if (view.IsMine)
        {
            playerName = PhotonNetwork.NickName;
            view.RPC("UpdateTankName", RpcTarget.AllBuffered, playerName);
        }
    }

    [PunRPC]
    private void UpdateTankName(string name)
    {
        tankNameText.text = name;
    }

    private void UpdateNamesForAllPlayers()
    {
        NetworkTankInfo[] allPlayers = FindObjectsOfType<NetworkTankInfo>();
        foreach (NetworkTankInfo player in allPlayers)
        {
            player.SetPlayerName();
        }
    }

    private void DestructionPhase()
    {
        GameObject explosion = Instantiate(destroyPrefab, transform.position, transform.rotation);
        Destroy(explosion, 2f);
    }

    private IEnumerator DestroyTank()
    {
        yield return new WaitForSeconds(2f);
        PhotonNetwork.Destroy(gameObject);
    }
}




