using UnityEngine;
using UnityEngine.InputSystem;

public class Compass : MonoBehaviour
{
    [SerializeField] GameObject circlePrefab;
    [SerializeField] int segments = 100;

    Vector3 center;
    bool dragging;

    LineRenderer currentCircle;

    void Update()
    {
        // ドラッグ開始
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            center = GetMouseWorldPosition();

            // 新しい円を作る
            GameObject circle = Instantiate(circlePrefab);

            currentCircle = circle.GetComponent<LineRenderer>();

            dragging = true;
        }

        if (dragging)
        {
            Vector3 mousePos = GetMouseWorldPosition();

            float radius = Vector3.Distance(center, mousePos);

            DrawCircle(radius);

            // ドラッグ終了
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                dragging = false;

                // ここで円が確定
                currentCircle = null;
            }
        }
    }

    void DrawCircle(float radius)
    {
        currentCircle.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * 2f * Mathf.PI / segments;

            Vector3 pos = center + new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0
            );

            currentCircle.SetPosition(i, pos);
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(
                mousePos.x,
                mousePos.y,
                -Camera.main.transform.position.z
            )
        );

        worldPos.z = 0;

        return worldPos;
    }
}