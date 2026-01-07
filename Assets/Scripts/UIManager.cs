using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager INSTANCE;

    [SerializeField] private GameObject _ParamInfo;

    //InfoPanel
    [SerializeField] private GameObject _InfoPanel;
    [SerializeField] public GameObject _SpawnCreatureBtn;

    //Geo
    [SerializeField] private ParamInfo _Altitude;
    [SerializeField] private ParamInfo _Temperature;
    [SerializeField] private ParamInfo _Humidtiy;

    //Resources
    [SerializeField] private GameObject _ResourcesPanel;
    private List<ParamInfo> _ResourceInfos;

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

        _ResourceInfos = new List<ParamInfo>();
        _InfoPanel.SetActive(false);
        _SpawnCreatureBtn.SetActive(false);
    }

    private void Update()
    {
        UpdateInfoPanel();
    }

    public void ShowInfoPanel()
    {
        if (_InfoPanel.activeSelf)
            return;

        int x = InputManager.INSTANCE.TileSelectedPos.x;
        int y = InputManager.INSTANCE.TileSelectedPos.y;

        _Altitude.SetValue(WorldMap.INSTANCE.MapTiles[x][y].Altitude);
        _Temperature.SetValue(WorldMap.INSTANCE.MapTiles[x][y].Temperature);
        _Humidtiy.SetValue(WorldMap.INSTANCE.MapTiles[x][y].Humidity);

        KeyValuePair<Resource, int>[] kvps = WorldMap.INSTANCE.MapTiles[x][y].ResourceList.OrderBy(kvp => kvp.Key.Category.name).ThenBy(kvp => kvp.Key.Level).ToArray();

        foreach (var kvp in kvps)
        {
            _ResourceInfos.Add(GameObject.Instantiate(_ParamInfo, _ResourcesPanel.transform).GetComponent<ParamInfo>());
            _ResourceInfos[_ResourceInfos.Count - 1].SetLabel(kvp.Key.Category.Name + " - " + "Level " + ((int)kvp.Key.Level + 1));
            _ResourceInfos[_ResourceInfos.Count - 1].SetValue(kvp.Value);
        }

        _InfoPanel.SetActive(true);
        _SpawnCreatureBtn.SetActive(true);

        Vector2 tileWorldPosition = WorldMap.INSTANCE.Base.CellToWorld(new Vector3Int(x + x + 1, y + y + 1, 0));
        Vector2 tileOnCamera = tileWorldPosition - InputManager.INSTANCE.CameraBound.Min;
        tileOnCamera.x = tileOnCamera.x / InputManager.INSTANCE.CameraBound.Width;
        tileOnCamera.y = tileOnCamera.y / InputManager.INSTANCE.CameraBound.Height;

        RectTransform infoPanelRect = _InfoPanel.GetComponent<RectTransform>();
        if (tileOnCamera.x < 0.4f)
        {
            infoPanelRect.pivot = new Vector2(0, 0.5f);
            infoPanelRect.anchorMin = new Vector2(1, 0.5f);
            infoPanelRect.anchorMax = new Vector2(1, 0.5f);

            infoPanelRect.anchoredPosition = new Vector2(-700, 0);
        }
        else
        {
            infoPanelRect.pivot = new Vector2(1, 0.5f);
            infoPanelRect.anchorMin = new Vector2(0, 0.5f);
            infoPanelRect.anchorMax = new Vector2(0, 0.5f);

            infoPanelRect.anchoredPosition = new Vector2(700, 0);
        }
    }

    private void UpdateInfoPanel()
    {
        if (!_InfoPanel.activeSelf || _ResourceInfos.Count == 0)
            return;

        int x = InputManager.INSTANCE.TileSelectedPos.x;
        int y = InputManager.INSTANCE.TileSelectedPos.y;

        KeyValuePair<Resource, int>[] kvps = WorldMap.INSTANCE.MapTiles[x][y].ResourceList.OrderBy(kvp => kvp.Key.Category.name).ThenBy(kvp => kvp.Key.Level).ToArray();

        int count = 0;
        foreach (var kvp in kvps)
        {
            if (_ResourceInfos.Count == count)
            {
                _ResourceInfos.Add(GameObject.Instantiate(_ParamInfo, _ResourcesPanel.transform).GetComponent<ParamInfo>());
            }

            _ResourceInfos[count].SetLabel(kvp.Key.Category.Name + " - " + "Level " + ((int)kvp.Key.Level + 1));
            _ResourceInfos[count].SetValue(kvp.Value);
            ++count;
        }
    }

    public void HideInfoPanel()
    {
        if (_ResourceInfos.Count > 0)
        {
            foreach (ParamInfo info in _ResourceInfos)
            {
                Destroy(info.gameObject);
            }
            _ResourceInfos.Clear();
        }


        _InfoPanel.SetActive(false);
        _SpawnCreatureBtn.SetActive(false);
    }

    public bool IsInfoPanelShowing()
    {
        return _InfoPanel.activeSelf;
    }
}
