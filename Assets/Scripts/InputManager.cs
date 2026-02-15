using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour
{
    public enum InputMode 
    {
        MAP = 0,
        CREATURE
    }


    public static InputManager INSTANCE;

    private const float MAX_CAMERA_ZOOM = 44.0f;
    private const float MIN_CAMERA_ZOOM = 4.0f;

    [SerializeField] private PlayerInput _PlayerInput;
    private InputAction _LeftMouse;
    private InputAction _RightMouse;
    private InputAction _MouseDelta;
    private InputAction _MouseScroll;

    [SerializeField] private float CameraDragMultipler;
    [SerializeField] private float CameraZoomMultipler;

    public Vector3Int TileSelectedPos;
    [SerializeField] private TileBase SelectBorder;

    private GameObject _CreatureSelected;
    [SerializeField] private bool _TrackingCreature = false;

    public Bound CameraBound;

    private InputMode _CurrentMode;

    private void Awake()
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
    }

    // Start is called before the first frame update
    void Start()
    {
        _LeftMouse = _PlayerInput.actions.FindAction("LeftMouse");
        _RightMouse = _PlayerInput.actions.FindAction("RightMouse");
        _MouseDelta = _PlayerInput.actions.FindAction("MouseDelta");
        _MouseScroll = _PlayerInput.actions.FindAction("MouseScroll");

        //calculate camera bound
        CameraBound = new Bound();
        UpdateCameraBound();

        _CurrentMode = InputMode.MAP;
    }

    // Update is called once per frame
    void Update()
    {
        CameraMove();
        CameraTrackCreature();
        CameraZoom();
        UpdateCameraBound();
        ClampCamera();

        SelectTile();
        SelectCreature();
    }

    private void CameraZoom()
    {
        Vector2 scrollDelta = _MouseScroll.ReadValue<Vector2>();
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - (scrollDelta.y * CameraZoomMultipler), MIN_CAMERA_ZOOM, MAX_CAMERA_ZOOM);
    }

    private void CameraMove()
    {
        if (!_RightMouse.IsPressed() || 
            (_CurrentMode == InputMode.CREATURE && _TrackingCreature && _CreatureSelected))
        {
            Cursor.visible = true;
            return;
        }

        UIManager.INSTANCE.HideInfoPanel();

        Cursor.visible = false;

        Vector3 delta = (_MouseDelta.ReadValue<Vector2>());
        Camera.main.transform.position = Camera.main.transform.position - (delta * CameraDragMultipler);
    }

    private void CameraTrackCreature()
    {
        if (_CurrentMode != InputMode.CREATURE || !_CreatureSelected)
        {
            if (_TrackingCreature)
                _TrackingCreature = false;

            return;
        }

        if (_RightMouse.WasPressedThisFrame())
        {
            _TrackingCreature = false;
            UIManager.INSTANCE.CreatureDisplay.Deactivate();
        }

        if (_LeftMouse.WasPressedThisFrame())
        {
            _TrackingCreature = true;
            UIManager.INSTANCE.CreatureDisplay.Activate(_CreatureSelected.GetComponent<CreatureGroup>());
        }

        if (!_TrackingCreature)
            return;

        Camera.main.transform.position = new Vector3(_CreatureSelected.transform.position.x, _CreatureSelected.transform.position.y, Camera.main.transform.position.z);
    }

    private void SelectCreature()
    {
        //if not in creature select mode return
        if (_CurrentMode != InputMode.CREATURE || _TrackingCreature)
            return;

        //use circle overlap to find the creatures under mouse
        Vector3 MouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(MouseWorldPos, 0.01f, LayerMask.GetMask("Creature"));

        //if no creature is found clear the selected creature and return
        if (colliders.Length == 0)
        {
            if (_CreatureSelected)
            {
                _CreatureSelected.GetComponent<CreatureHover>().OnHoverExit();
                _CreatureSelected = null;
            }

            return;
        }

        //if the creature found is same as the selected creature, return
        if (_CreatureSelected == colliders[0].gameObject)
        {
            return;
        }

        //old selected creature exit hover
        if (_CreatureSelected)
            _CreatureSelected.GetComponent<CreatureHover>().OnHoverExit();

        //set new selected creature and enter hover
        _CreatureSelected = colliders[0].gameObject;
        _CreatureSelected.GetComponent<CreatureHover>().OnHoverEnter();
    }

    private void SelectTile()
    {
        WorldMap.INSTANCE.UI.ClearAllTiles();

        if (_CurrentMode != InputMode.MAP)
            return;

        Vector3 MouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int Pos = WorldMap.INSTANCE.Base.WorldToCell(MouseWorldPos);
        Pos.x /= 2;
        Pos.y /= 2;
        Pos.z = 0;

        if ((Within(Pos.x, 0, WorldMap.INSTANCE.Width) &&
            Within(Pos.y, 0, WorldMap.INSTANCE.Height)) &&
            !_RightMouse.WasPressedThisFrame() &&
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
            _LeftMouse.WasPressedThisFrame() &&
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
    public void SetInputMode(int mode)
    {
        _CurrentMode = (InputMode)mode;

        switch (_CurrentMode)
        {
            case InputMode.MAP:
                {
                    if (UIManager.INSTANCE.CreatureDisplay.isActiveAndEnabled)
                        UIManager.INSTANCE.CreatureDisplay.Deactivate();

                    break;
                }
            case InputMode.CREATURE:
                {
                    if (UIManager.INSTANCE.IsInfoPanelShowing())
                        UIManager.INSTANCE.HideInfoPanel();

                    break;
                }
        }

    }
}
