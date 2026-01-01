using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LatheMachineController : MonoBehaviour
{
    public enum LatheState
    {
        Running,
        Stopped,
        Fault
    }

    public LatheState currentState;

    [Header("Spindle Rotation")]
    public bool rotateSpindle = false;
    public List<GameObject> rotatableObjects;
    public float rotationSpeed = 20f;

    [Header("Carriage Movement")]
    public Transform carriage;
    public float carriageSpeed = 0.15f;
    public Vector3 carriageDirection = Vector3.right;
    public float maxCarriageDistance = 1.5f;

    private Vector3 carriageStartPos;

    [Header("Handle Wheel")]
    public Transform handlePivot;
    public float handleSpeed = 60f;
    public float handleToCarriageDelay = 1.2f;

    [Header("Fault (Spindle)")]
    public Renderer spindleRenderer;
    public Color faultGlowColor = Color.red;

    [Header("UI")]
    public TextMeshProUGUI statusText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip runningSound;
    public AudioClip faultSound;

    private bool isRunning = false;
    private bool moveCarriage = false;
    private Coroutine faultCoroutine;
    private Material spindleMat;

    void Start()
    {
        spindleMat = spindleRenderer.material;
        DisableEmission();

        carriageStartPos = carriage.position;

        // INITIAL STATE
        SetState(LatheState.Stopped);
    }

    void FixedUpdate()
    {
        if (rotateSpindle)
        {
            foreach (GameObject obj in rotatableObjects)
            {
                obj.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            }
        }
    }

    void Update()
    {
        if (currentState != LatheState.Running)
            return;

        handlePivot.Rotate(handleSpeed * Time.deltaTime, 0f, 0f, Space.Self);

        if (moveCarriage)
        {
            float traveledDistance =
                Vector3.Distance(carriageStartPos, carriage.position);

            if (traveledDistance < maxCarriageDistance)
            {
                carriage.Translate(
                    carriageDirection.normalized * carriageSpeed * Time.deltaTime,
                    Space.World
                );
            }
            else
            {
                moveCarriage = false;
            }
        }
    }

    //  RUN
    public void RunMachine()
    {
        SetState(LatheState.Running);

        carriage.position = carriageStartPos;
        Invoke(nameof(StartCarriage), handleToCarriageDelay);
    }

    void StartCarriage()
    {
        moveCarriage = true;
    }

    // STOP
    public void StopMachine()
    {
        SetState(LatheState.Stopped);
        carriage.position = carriageStartPos;
    }

    // FAULT
    public void FaultMachine()
    {
        SetState(LatheState.Fault);
        carriage.position = carriageStartPos;
    }

    // 🔹 CENTRAL STATE HANDLER
    void SetState(LatheState newState)
    {
        // Reset previous effects
        StopFault();
        StopAllSounds();

        currentState = newState;
        isRunning = false;
        moveCarriage = false;
        rotateSpindle = false;

        switch (currentState)
        {
            case LatheState.Running:
                isRunning = true;
                rotateSpindle = true;
                statusText.text = "Running";
                PlayRunningSound();
                break;

            case LatheState.Stopped:
                statusText.text = "Stopped";
                break;

            case LatheState.Fault:
                statusText.text = "Fault";
                faultCoroutine = StartCoroutine(SpindleBlink());
                PlayFaultSound();
                break;
        }
    }

    IEnumerator SpindleBlink()
    {
        while (true)
        {
            EnableEmission();
            yield return new WaitForSeconds(0.4f);
            DisableEmission();
            yield return new WaitForSeconds(0.4f);
        }
    }

    void StopFault()
    {
        if (faultCoroutine != null)
            StopCoroutine(faultCoroutine);

        DisableEmission();
    }

    // AUDIO
    void PlayRunningSound()
    {
        if (runningSound == null) return;

        audioSource.clip = runningSound;
        audioSource.loop = true;
        audioSource.Play();
    }

    void PlayFaultSound()
    {
        if (faultSound == null) return;

        audioSource.clip = faultSound;
        audioSource.loop = true;
        audioSource.Play();
    }

    void StopAllSounds()
    {
        audioSource.Stop();
        audioSource.loop = false;
    }

    //  Emission
    void EnableEmission()
    {
        spindleMat.EnableKeyword("_EMISSION");
        spindleMat.SetColor("_EmissionColor", faultGlowColor * 3f);
    }

    void DisableEmission()
    {
        spindleMat.SetColor("_EmissionColor", Color.black);
    }
}
