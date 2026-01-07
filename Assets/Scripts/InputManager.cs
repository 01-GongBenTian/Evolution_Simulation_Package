using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;



public class InputManager : MonoBehaviour
{
    public static InputManager INSTANCE;

    private const float MAX_CAMERA_ZOOM = 36.0f;
    private const float MIN_CAMERA_ZOOM = 8.0f;

    [SerializeField] private PlayerInput _PlayerInput;
    private InputAction LeftMouse;
    private InputAction RightMouse;
    private InputAction MouseDelta;
    private InputAction MouseScroll;

    [SerializeField] private float CameraDragMultipler;
    [SerializeField] private float CameraZoomMultipler;

    public Vector3Int TileSelectedPos;
    [SerializeField] private TileBase SelectBorder;

    public Bound CameraBound;

    // Start is called before the first frame update
    void Start()
    {
        if (!INSTANCE)
        {
            INSTANCE = this;
        }
        else
        {
            Destroy(this);
            return;
        }


        LeftMouse = _PlayerInput.actions.FindAction("LeftMouse");
        RightMouse = _PlayerInput.actions.FindAction("RightMouse");
        MouseDelta = _PlayerInput.actions.FindAction("MouseDelta");
        MouseScroll = _PlayerInput.actions.FindAction("MouseScroll");

        //calculate camera bound
        CameraBound = new Bound();
        UpdateCameraBound();
    }

    // Update is called once per frame
    void Update()
    {
        CameraMove();
        CameraZoom();
        UpdateCameraBound();
        ClampCamera();

        SelectTile();
    }

    private void CameraZoom()
    {
        Vector2 scrollDelta = MouseScroll.ReadValue<Vector2>();
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - (scrollDelta.y * CameraZoomMultipler), MIN_CAMERA_ZOOM, MAX_CAMERA_ZOOM);
    }

    private void CameraMove()
    {
        if (!RightMouse.IsPressed())
        {
            Cursor.visible = true;
            return;
        }

        UIManager.INSTANCE.HideInfoPanel();

        Cursor.visible = false;

        Vector3 delta = (MouseDelta.ReadValue<Vector2>());
        Camera.main.transform.position = Camera.main.transform.position - (delta * CameraDragMultipler);
    }

    private void SelectTile()
    {
        Vector3 MouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int Pos = WorldMap.INSTANCE.Base.WorldToCell(MouseWorldPos);
        Pos.x /= 2;
        Pos.y /= 2;
        Pos.z = 0;

        WorldMap.INSTANCE.UI.ClearAllTiles();

        if ((Within(Pos.x, 0, WorldMap.INSTANCE.Width) &&
            Within(Pos.y, 0, WorldMap.INSTANCE.Height)) &&
            !RightMouse.IsPressed() &&
            !UIManager.INSTANCE.IsInfoPanelShowing()
            )
        {
            WorldMap.INSTANCE.UI.SetTile(Pos * 2, SelectBorder);
        }
        else if (UIManager.INSTANCE.IsInfoPanelShowing())
        {
            WorldMap.INSTANCE.UI.SetTile(TileSelectedPos * 2, SelectBorder);
        }

        if (
            LeftMouse.IsPressed() &&
            !EventSystem.current.IsPointerOverGameObject() &&
            Within(Pos.x, 0, WorldMap.INSTANCE.Width) &&
            Within(Pos.y, 0, WorldMap.INSTANCE.Height))
        {
            TileSelectedPos = Pos;

            UIManager.INSTANCE.ShowInfoPanel();
        }
    }

    private bool Within(int value, int Min, int Max)
    {
        return (Max > value) && (value >= Min);
    }

    public void UpdateCameraBound()
    {
        CameraBound.Height = Camera.main.orthographicSize * 2;
        CameraBound.Width = CameraBound.Height * Camera.main.aspect;
        CameraBound.Min = new Vector2(Camera.main.transform.position.x - CameraBound.Width / 2, Camera.main.transform.position.y - CameraBound.Height / 2);
        CameraBound.Max = new Vector2(Camera.main.transform.position.x + CameraBound.Width / 2, Camera.main.transform.position.y + CameraBound.Height / 2);
    }

    public void ClampCamera()
    {
        if (CameraBound.Within(WorldMap.INSTANCE.TilemapBound))
        {
            return;
        }

        //check size
        if (CameraBound.Width > WorldMap.INSTANCE.TilemapBound.Width)
        {
            float ratio = WorldMap.INSTANCE.TilemapBound.Width / CameraBound.Width;
            Camera.main.orthographicSize *= ratio;
            UpdateCameraBound();
        }

        if (CameraBound.Height > WorldMap.INSTANCE.TilemapBound.Height)
        {
            float ratio = WorldMap.INSTANCE.TilemapBound.Height / CameraBound.Height;
            Camera.main.orthographicSize *= ratio;
            UpdateCameraBound();
        }

        //Check position
        if (CameraBound.Min.x < WorldMap.INSTANCE.TilemapBound.Min.x)
        {
            float offset = CameraBound.Min.x - WorldMap.INSTANCE.TilemapBound.Min.x;
            Camera.main.transform.position -= new Vector3(offset, 0, 0);
            UpdateCameraBound();
        }
        else if (CameraBound.Max.x > WorldMap.INSTANCE.TilemapBound.Max.x)
        {
            float offset = CameraBound.Max.x - WorldMap.INSTANCE.TilemapBound.Max.x;
            Camera.main.transform.position -= new Vector3(offset, 0, 0);
            UpdateCameraBound();
        }

        if (CameraBound.Min.y < WorldMap.INSTANCE.TilemapBound.Min.y)
        {
            float offset = CameraBound.Min.y - WorldMap.INSTANCE.TilemapBound.Min.y;
            Camera.main.transform.position -= new Vector3(0, offset, 0);
            UpdateCameraBound();
        }
        else if (CameraBound.Max.y > WorldMap.INSTANCE.TilemapBound.Max.y)
        {
            float offset = CameraBound.Max.y - WorldMap.INSTANCE.TilemapBound.Max.y;
            Camera.main.transform.position -= new Vector3(0, offset, 0);
            UpdateCameraBound();
        }
    }
}
