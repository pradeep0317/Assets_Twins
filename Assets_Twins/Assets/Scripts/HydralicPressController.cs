using System.Collections;
using UnityEngine;
using TMPro;

public class HydraulicPressController : MonoBehaviour
{
    [Header("Piston Movement")]
    public Transform piston;
    public float pressSpeed = 0.5f;
    public float pressDistance = 0.8f;     // 👈 nee define panna alavu
    public Vector3 pressDirection = Vector3.down;

    private Vector3 pistonStartPos;
    private bool isRunning = false;
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
        pistonStartPos = piston.position;
        faultMat = faultRenderer.material;
        DisableEmission();
    }

    void Update()
    {
        if (!isRunning || !movePiston) return;

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
            // Target reach aagiduchu
            movePiston = false;
        }
    }

    // ▶ RUN BUTTON
    public void RunPress()
    {
        isRunning = true;
        movePiston = true;
        StopFault();

        // Always start from top
        piston.position = pistonStartPos;

        statusText.text = "Running";
    }

    // ⏹ STOP BUTTON
    public void StopPress()
    {
        isRunning = false;
        movePiston = false;

        // Return piston to home (top)
        piston.position = pistonStartPos;

        StopFault();
        statusText.text = "Stopped";
    }

    // ⚠️ FAULT BUTTON
    public void FaultPress()
    {
        isRunning = false;
        movePiston = false;

        // Reset piston
        piston.position = pistonStartPos;

        statusText.text = "Fault";

        if (faultCoroutine != null)
            StopCoroutine(faultCoroutine);

        faultCoroutine = StartCoroutine(FaultBlink());
    }

    // 🔁 Fault Blink Logic
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
