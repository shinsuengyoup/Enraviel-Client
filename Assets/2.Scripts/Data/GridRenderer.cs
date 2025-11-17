// 2025-11-16 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    private int gridSize = 10; // 격자의 크기
    private float cellSize = 0.5f; // 각 셀의 크기
    private Vector3 startPos;
    public Material lineMaterial; // LineRenderer에 사용할 머티리얼

    public void SetGridOption(Vector3 v3Pos, int gridSize)
    {
        startPos = v3Pos;
        this.gridSize = gridSize;
        DrawGrid();
    }

    void DrawGrid()
    {
        for (int i = 0; i <= gridSize; i++)
        {
            // 수평선 그리기
            Vector3 hStart = startPos + new Vector3(0, i * cellSize, 0);
            Vector3 hEnd = startPos + new Vector3(gridSize * cellSize, i * cellSize, 0);
            CreateLine(hStart, hEnd);

            // 수직선 그리기
            Vector3 vStart = startPos + new Vector3(i * cellSize, 0, 0);
            Vector3 vEnd = startPos + new Vector3(i * cellSize, gridSize * cellSize, 0);
            CreateLine(vStart, vEnd);
        }
    }

    void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject line = new GameObject("GridLine");
        line.transform.parent = transform;
        line.transform.localPosition = Vector3.zero;
        line.layer = LayerMask.NameToLayer("system");

        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.015f; // 선의 두께
        lineRenderer.endWidth = 0.015f;

        lineRenderer.startColor = ConstData.CLR_Gray_tile;
        lineRenderer.endColor = ConstData.CLR_Gray_tile;
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        Gradient gradient = new Gradient();
        colorKeys[0].color = Color.black;
        colorKeys[1].color = Color.black;
        alphaKeys[0].time = 0; alphaKeys[0].alpha = 0.5f;
        colorKeys[1].time = 1; alphaKeys[1].alpha = 0.5f;
        gradient.SetKeys(colorKeys, alphaKeys);
        lineRenderer.colorGradient = gradient;


        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.useWorldSpace = true;
        lineRenderer.sortingLayerName = "Background";
        lineRenderer.sortingOrder = 1;
    }
}