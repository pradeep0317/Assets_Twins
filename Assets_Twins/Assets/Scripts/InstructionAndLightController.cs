using UnityEngine;

public class InstructionAndLightController : MonoBehaviour
{
    [Header("UI")]
    public GameObject instructionPanel;

    [Header("Lights to Control")]
    public Light[] sceneLights;

    void Start()
    {
        // Instruction ON
        instructionPanel.SetActive(true);

        // ALL lights OFF
        foreach (Light l in sceneLights)
        {
            l.enabled = false;
        }
    }

    // Start button click
    public void OnStartClicked()
    {
        // Instruction OFF
        instructionPanel.SetActive(false);

        // ALL lights ON
        foreach (Light l in sceneLights)
        {
            l.enabled = true;
        }
    }
}