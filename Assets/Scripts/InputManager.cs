using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerInput _PlayerInput;
    private InputAction RightMouse;
    private InputAction MouseDelta;


    public Camera MainCamera;
    [SerializeField] private float CameraDragMultipler;

    private Vector3Int TileSelectedPos;
    public TileBase SelectBorder;

    // Start is called before the first frame update
    void Start()
    {


        RightMouse = _PlayerInput.actions.FindAction("RightMouse");
        MouseDelta = _PlayerInput.actions.FindAction("MouseDelta");
    }

    // Update is called once per frame
    void Update()
    {
        CameraMove();
        SelectTile();
    }

    private void CameraMove()
    {
        if (!RightMouse.IsPressed())
        {
            Cursor.visible = true;
            return;
        }

        Cursor.visible = false;

        Vector3 delta = (MouseDelta.ReadValue<Vector2>());
        MainCamera.transform.position = MainCamera.transform.position - (delta * CameraDragMultipler * Time.deltaTime);
    }

    private void SelectTile()
    {
        Vector3 MouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        TileSelectedPos = WorldMap.Instance.Base.WorldToCell(MouseWorldPos);
        TileSelectedPos.x /= 2;
        TileSelectedPos.y /= 2;
        TileSelectedPos.z = 0;

        WorldMap.Instance.UI.ClearAllTiles();
        WorldMap.Instance.UI.SetTile(TileSelectedPos * 2, SelectBorder);
    }
}
