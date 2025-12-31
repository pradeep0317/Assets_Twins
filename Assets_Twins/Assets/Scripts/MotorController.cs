using UnityEngine;
using TMPro;
using System.Collections;

public class MotorController : MonoBehaviour
{
    [Header("Motor Settings")]
    public Transform shaft;
    public float rotationSpeed = 600f;
    private bool isRunning = false;

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
    }

    void Update()
    {
        if (isRunning)
        {
            shaft.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    //  RUN
    public void RunMotor()
    {
        isRunning = true;
        StopFault();
        statusText.text = "Running";
    }

    //  STOP
    public void StopMotor()
    {
        isRunning = false;
        StopFault();
        statusText.text = "Stopped";
    }

    // FAULT
    public void FaultMotor()
    {
        isRunning = false;
        statusText.text = "Fault";

        if (faultCoroutine != null)
            StopCoroutine(faultCoroutine);

        faultCoroutine = StartCoroutine(FaultGlowBlink());
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