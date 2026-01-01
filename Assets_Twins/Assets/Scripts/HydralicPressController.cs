using System.Collections;
using UnityEngine;
using TMPro;

public class HydraulicPressController : MonoBehaviour
{
    //  Machine States
    public enum PressState
    {
        Running,
        Stopped,
        Fault
    }

    public PressState currentState;

    [Header("Piston Movement")]
    public Transform piston;
    public float pressSpeed = 0.5f;
    public float pressDistance = 0.8f;
    public Vector3 pressDirection = Vector3.down;

    private Vector3 pistonStartPos;
    private bool movePiston = false;

    [Header("Fault")]
    public Renderer faultRenderer;
    public Color faultGlowColor = Color.red;
    private Material faultMat;
    private Coroutine faultCoroutine;

    [Header("UI")]
    public TextMeshProUGUI statusText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip runOnceSound;  
    public AudioClip faultSound;     

    void Start()
    {
        pistonStartPos = piston.position;
        faultMat = faultRenderer.material;
        DisableEmission();

        //  INITIAL STATE
        SetState(PressState.Stopped);
    }

    void Update()
    {
        if (currentState != PressState.Running || !movePiston)
            return;

        float movedDistance =
            Vector3.Distance(pistonStartPos, piston.position);

        if (movedDistance < pressDistance)
        {
            piston.Translate(
                pressDirection.normalized * pressSpeed * Time.deltaTime,
                Space.World
            );
        }
        else
        {
            movePiston = false;
        }
    }

    // RUN
    public void RunPress()
    {
        SetState(PressState.Running);

        piston.position = pistonStartPos;
        movePiston = true;
    }

    // STOP
    public void StopPress()
    {
        SetState(PressState.Stopped);

        piston.position = pistonStartPos;
        movePiston = false;
    }

    //  FAULT
    public void FaultPress()
    {
        SetState(PressState.Fault);

        piston.position = pistonStartPos;
        movePiston = false;
    }

    //  CENTRAL STATE HANDLER
    void SetState(PressState newState)
    {
        StopFault();
        StopAllSounds();

        currentState = newState;
        movePiston = false;

        switch (currentState)
        {
            case PressState.Running:
                statusText.text = "Running";
                PlayRunOnceSound();  
                break;

            case PressState.Stopped:
                statusText.text = "Stopped";
                break;

            case PressState.Fault:
                statusText.text = "Fault";
                faultCoroutine = StartCoroutine(FaultBlink());
                PlayFaultSound();     
                break;
        }
    }

    //  Fault Blink
    IEnumerator FaultBlink()
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

    //AUDIO METHODS
    void PlayRunOnceSound()
    {
        if (runOnceSound == null) return;

        audioSource.loop = false;
        audioSource.PlayOneShot(runOnceSound);
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
        faultMat.EnableKeyword("_EMISSION");
        faultMat.SetColor("_EmissionColor", faultGlowColor * 3f);
    }

    void DisableEmission()
    {
        faultMat.SetColor("_EmissionColor", Color.black);
    }
}
