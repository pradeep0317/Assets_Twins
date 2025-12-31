using System.Collections;
using UnityEngine;
using TMPro;

public class HydraulicPressController : MonoBehaviour
{
    // 🔹 Machine States
    public enum PressState
    {
        Running,
        Stopped,
        Fault
    }

    public PressState currentState;   // 👈 current state

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

    void Start()
    {
        // ✅ Store home position
        pistonStartPos = piston.position;

        faultMat = faultRenderer.material;
        DisableEmission();

        // ✅ INITIAL STATE
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
            movePiston = false; // reach limit
        }
    }

    // ▶ RUN BUTTON
    public void RunPress()
    {
        SetState(PressState.Running);

        piston.position = pistonStartPos;
        movePiston = true;
    }

    // ⏹ STOP BUTTON
    public void StopPress()
    {
        SetState(PressState.Stopped);

        piston.position = pistonStartPos;
        movePiston = false;
    }

    // ⚠️ FAULT BUTTON
    public void FaultPress()
    {
        SetState(PressState.Fault);

        piston.position = pistonStartPos;
        movePiston = false;
    }

    // 🔁 CENTRAL STATE HANDLER (IMPORTANT)
    void SetState(PressState newState)
    {
        currentState = newState;

        // Stop everything first
        movePiston = false;
        StopFault();

        switch (currentState)
        {
            case PressState.Running:
                statusText.text = "Running";
                break;

            case PressState.Stopped:
                statusText.text = "Stopped";
                break;

            case PressState.Fault:
                statusText.text = "Fault";
                faultCoroutine = StartCoroutine(FaultBlink());
                break;
        }
    }

    // 🔁 Fault Blink
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
