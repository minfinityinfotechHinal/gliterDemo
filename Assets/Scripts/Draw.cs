using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Draw : MonoBehaviour
{
    
    public Camera m_camera;
    public GameObject brush;

    [SerializeField]
    private string targetLayerName = "colorcard"; // Layer name for the drawable area

    [SerializeField]
    private List<Material> materialList = new();

    private LineRenderer currentLineRenderer;
    private Vector2 lastPos;

    public enum Colors
    {
        green,
        red,
        pink,
        yellow
    }

    private Colors currentColor = Colors.green;
    private bool isDrawing = false;

    private int targetLayerMask;

    private void Start()
    {
        targetLayerMask = LayerMask.GetMask(targetLayerName);
        if (targetLayerMask == 0)
        {
            Debug.LogError($"Layer '{targetLayerName}' does not exist. Ensure the target layer is correctly named.");
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsPointerOverTarget()) // Start drawing only if over the target layer
        {
            StartDrawing();
        }
        else if (Input.GetMouseButtonUp(0) || !IsPointerOverTarget()) // Stop drawing if pointer is not over the target layer
        {
            StopDrawing();
        }

        if (isDrawing)
        {
            PointToMousePos();
        }
    }

    private bool IsPointerOverTarget()
    {
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hitCollider = Physics2D.OverlapPoint(mousePos, targetLayerMask);

        if (hitCollider != null)
        {
            return hitCollider.gameObject.layer == LayerMask.NameToLayer(targetLayerName);
        }

        return false;
    }

    private void StartDrawing()
    {
        if (!isDrawing)
        {
            isDrawing = true;
            CreateBrush();
        }
    }

    private void StopDrawing()
    {
        isDrawing = false;
    }

    public void ChangePaintColor(Colors newColor)
    {
        // Set the new color for future lines, but don't affect existing ones
        currentColor = newColor;
    }

    void CreateBrush()
    {
        if (!IsPointerOverTarget()) return; // Prevent drawing outside the target

        GameObject brushInstance = Instantiate(brush);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();

        // Apply the current color to the new brush
        ApplyCurrentColorToBrush();

        // Set initial position of the brush, based on mouse position on the target layer
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        currentLineRenderer.SetPosition(0, mousePos);
        currentLineRenderer.SetPosition(1, mousePos);

        lastPos = mousePos;
    }

    private void ApplyCurrentColorToBrush()
    {
        Material selectedMaterial = GetMaterialByColor(currentColor);
        if (selectedMaterial != null)
        {
            currentLineRenderer.material = selectedMaterial;
        }
    }

    void AddAPoint(Vector2 pointPos)
    {
        currentLineRenderer.positionCount++;
        currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, pointPos);
    }

    void PointToMousePos()
    {
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        if (lastPos != mousePos)
        {
            AddAPoint(mousePos);
            lastPos = mousePos;
        }
    }

    private Material GetMaterialByColor(Colors color)
    {
        int index = (int)color;
        return (index >= 0 && index < materialList.Count) ? materialList[index] : null;
    }

    public void OnRedButtonClicked() => ChangePaintColor(Colors.red);
    public void OnGreenButtonClicked() => ChangePaintColor(Colors.green);
    public void OnPinkButtonClicked() => ChangePaintColor(Colors.pink);
    public void OnYellowButtonClicked() => ChangePaintColor(Colors.yellow);
}
