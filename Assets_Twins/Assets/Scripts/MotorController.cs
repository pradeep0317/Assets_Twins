using UnityEngine;
using TMPro;
using System.Collections;

public class MotorController : MonoBehaviour
{
    // 🔹 Motor States
    public enum MotorState
    {
        Running,
        Stopped,
        Fault
    }

    public MotorState currentState;   // 👈 current state

    [Header("Motor Settings")]
    public Transform shaft;
    public float rotationSpeed = 600f;

    [Header("Fault Settings")]
    public Renderer faultPart;
    public Color faultGlowColor = Color.red;
    private Material faultMat;
    private Coroutine faultCoroutine;

    [Header("UI")]
    public TextMeshProUGUI statusText;

    void Start()
    {
        faultMat = faultPart.material;
        DisableEmission();

        // ✅ INITIAL STATE
        SetState(MotorState.Stopped);
    }

    void Update()
    {
        if (currentState == MotorState.Running)
        {
            shaft.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    // ▶ RUN BUTTON
    public void RunMotor()
    {
        SetState(MotorState.Running);
    }

    // ⏹ STOP BUTTON
    public void StopMotor()
    {
        SetState(MotorState.Stopped);
    }

    // ⚠️ FAULT BUTTON
    public void FaultMotor()
    {
        SetState(MotorState.Fault);
    }

    // 🔹 CENTRAL STATE HANDLER
    void SetState(MotorState newState)
    {
        currentState = newState;

        // Reset common things
        StopFault();

        switch (currentState)
        {
            case MotorState.Running:
                statusText.text = "Running";
                break;

            case MotorState.Stopped:
                statusText.text = "Stopped";
                break;

            case MotorState.Fault:
                statusText.text = "Fault";
                faultCoroutine = StartCoroutine(FaultGlowBlink());
                break;
        }
    }

    IEnumerator FaultGlowBlink()
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
        faultMat.SetColor("_EmissionColor", faultGlowColor * 2f);
    }

    void DisableEmission()
    {
        faultMat.SetColor("_EmissionColor", Color.black);
    }
}
