using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [SerializeField] private PlayerInput _PlayerInput;
    private InputAction LeftMouse;
    private InputAction RightMouse;
    private InputAction MouseDelta;


    public Camera MainCamera;
    [SerializeField] private float CameraDragMultipler;

    public Vector3Int TileSelectedPos;
    [SerializeField] private TileBase SelectBorder;

    // Start is called before the first frame update
    void Start()
    {
        if(!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }


        LeftMouse = _PlayerInput.actions.FindAction("LeftMouse");
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

        UIManager.Instance.HideInfoPanel();

        Cursor.visible = false;

        Vector3 delta = (MouseDelta.ReadValue<Vector2>());
        MainCamera.transform.position = MainCamera.transform.position - (delta * CameraDragMultipler * Time.deltaTime);


    }

    private void SelectTile()
    {
        Vector3 MouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int Pos = WorldMap.Instance.Base.WorldToCell(MouseWorldPos);
        Pos.x /= 2;
        Pos.y /= 2;
        Pos.z = 0;

        WorldMap.Instance.UI.ClearAllTiles();
        WorldMap.Instance.UI.SetTile(Pos * 2, SelectBorder);

        if (
            LeftMouse.IsPressed() && 
            !EventSystem.current.IsPointerOverGameObject() && 
            Within(Pos.x, 0, WorldMap.Instance.Width) &&
            Within(Pos.y, 0, WorldMap.Instance.Height))
        {
            TileSelectedPos = Pos;
            UIManager.Instance.ShowInfoPanel();
        }
    }

    private bool Within(int value, int Min, int Max)
    {
        return (Max > value) && (value >= Min);
    }
}
