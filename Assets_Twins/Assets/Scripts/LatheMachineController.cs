using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LatheMachineController : MonoBehaviour
{
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

    private bool isRunning = false;
    private bool moveCarriage = false;
    private Coroutine faultCoroutine;
    private Material spindleMat;

    void Start()
    {
        rotateSpindle = false;
        spindleMat = spindleRenderer.material;
        DisableEmission();

        // ✅ HOME position store pannrom
        carriageStartPos = carriage.position;
    }

    void FixedUpdate()
    {
        // 🔁 Spindle rotation (your logic)
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
        if (!isRunning) return;

        // 🎡 Handle rotation
        handlePivot.Rotate(handleSpeed * Time.deltaTime, 0f, 0f, Space.Self);

        // ↔ Carriage movement with LIMIT
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
                moveCarriage = false; // stop after distance
            }
        }
    }

    // ▶ RUN
    public void RunMachine()
    {
        isRunning = true;
        moveCarriage = false;
        StopFault();

        // ✅ Always start from HOME
        carriage.position = carriageStartPos;

        rotateSpindle = true;
        statusText.text = "Running";

        Invoke(nameof(StartCarriage), handleToCarriageDelay);
    }

    void StartCarriage()
    {
        moveCarriage = true;
    }

    // ⏹ STOP
    public void StopMachine()
    {
        isRunning = false;
        moveCarriage = false;
        rotateSpindle = false;

        // ✅ RESET carriage to HOME
        carriage.position = carriageStartPos;

        StopFault();
        statusText.text = "Stopped";
    }

    // ⚠️ FAULT
    public void FaultMachine()
    {
        isRunning = false;
        moveCarriage = false;
        rotateSpindle = false;

        // ✅ RESET carriage to HOME
        carriage.position = carriageStartPos;

        statusText.text = "Fault";

        if (faultCoroutine != null)
            StopCoroutine(faultCoroutine);

        faultCoroutine = StartCoroutine(SpindleBlink());
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
