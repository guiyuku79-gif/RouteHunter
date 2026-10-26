using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(DrawingManager))]
public class DrawingInput : MonoBehaviour
{
    [SerializeField] Camera drawingCamera;

    DrawingManager drawingManager;
    Pointer drawingPointer;
    readonly List<RaycastResult> uiHits = new List<RaycastResult>();

    void Awake()
    {
        drawingManager = GetComponent<DrawingManager>();

        if (drawingCamera == null)
        {
            drawingCamera = Camera.main;
        }
    }

    void Update()
    {
        if (!drawingManager.IsInputEnabled) return;

        HandlePointer();
        HandleKeyboard();
    }

    void HandlePointer()
    {
        // 描画中は、描き始めたデバイスを使う
        Pointer pointer = drawingManager.IsDrawing
            ? drawingPointer
            : Pointer.current;

        if (pointer == null)
        {
            CancelDrawing();
            return;
        }

        bool touchCanceled =
            pointer is Touchscreen touchscreen &&
            touchscreen.primaryTouch.phase.ReadValue()
                == UnityEngine.InputSystem.TouchPhase.Canceled;

        if (touchCanceled)
        {
            CancelDrawing();
            return;
        }

        Vector2 worldPosition =
            ScreenToWorldPosition(pointer.position.ReadValue());

        if (!drawingManager.IsDrawing &&
            pointer.press.wasPressedThisFrame)
        {
            // ボタンが同じフレーム内に非表示になっても、そのクリックでは描画しない。
            if (Time.frameCount == drawingManager.InputEnabledFrame ||
                IsOverUI(pointer.position.ReadValue())) return;

            drawingPointer = pointer;
            drawingManager.BeginDrawing(worldPosition);
        }

        // 開始位置が範囲外の場合などは、描画が始まらない
        if (!drawingManager.IsDrawing)
        {
            drawingPointer = null;
            return;
        }

        drawingManager.UpdateDrawing(worldPosition);

        if (pointer.press.wasReleasedThisFrame)
        {
            drawingManager.FinishDrawing(worldPosition);
            drawingPointer = null;
        }
        else if (!pointer.press.isPressed)
        {
            CancelDrawing();
        }
    }

    void HandleKeyboard()
    {
        if (!drawingManager.IsInputEnabled) return;
        if (drawingManager.IsDrawing) return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.vKey.wasPressedThisFrame)
        {
            drawingManager.LineMode();
        }

        if (keyboard.cKey.wasPressedThisFrame)
        {
            drawingManager.CompassMode();
        }

        if (keyboard.fKey.wasPressedThisFrame)
        {
            drawingManager.FixedCompassMode();
        }

        if (keyboard.zKey.wasPressedThisFrame)
        {
            drawingManager.Undo();
        }

        if (keyboard.rKey.wasPressedThisFrame)
        {
            drawingManager.ResetBoard();
        }
    }

    bool IsOverUI(Vector2 screenPosition)
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null) return false;

        // EventSystem の Update 順序に依存せず、現在のマウス・タッチ位置を調べる。
        var eventData = new PointerEventData(eventSystem) { position = screenPosition };
        uiHits.Clear();
        eventSystem.RaycastAll(eventData, uiHits);
        foreach (RaycastResult hit in uiHits)
        {
            if (hit.module is GraphicRaycaster) return true;
        }

        return false;
    }

    Vector2 ScreenToWorldPosition(Vector2 screenPosition)
    {
        // 現在の実装と同じ、Z=0の描画面を想定
        Vector3 worldPosition = drawingCamera.ScreenToWorldPoint(
            new Vector3(
                screenPosition.x,
                screenPosition.y,
                -drawingCamera.transform.position.z
            )
        );

        return new Vector2(worldPosition.x, worldPosition.y);
    }

    void CancelDrawing()
    {
        if (drawingManager.IsDrawing)
        {
            drawingManager.CancelDrawing();
        }

        drawingPointer = null;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            CancelDrawing();
        }
    }

    void OnDisable()
    {
        if (drawingManager != null)
        {
            CancelDrawing();
        }
    }
}
