using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject _ParamInfo;

    //InfoPanel
    [SerializeField] private GameObject _InfoPanel;

    //Geo
    [SerializeField] private ParamInfo _Altitude;
    [SerializeField] private ParamInfo _Temperature;
    [SerializeField] private ParamInfo _Humidtiy;

    //Resources
    [SerializeField] private GameObject _ResourcesPanel;
    private List<ParamInfo> _ResourceInfos;

    void Start()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        _ResourceInfos = new List<ParamInfo>();
        _InfoPanel.SetActive(false);
    }

    public void ShowInfoPanel()
    {
        if (_InfoPanel.activeSelf)
            return;

        int x = InputManager.Instance.TileSelectedPos.x;
        int y = InputManager.Instance.TileSelectedPos.y;

        _Altitude.SetValue(WorldMap.Instance.MapTiles[x][y].Altitude);
        _Temperature.SetValue(WorldMap.Instance.MapTiles[x][y].Temperature);
        _Humidtiy.SetValue(WorldMap.Instance.MapTiles[x][y].Humidity);

        Dictionary<Resource, float>.KeyCollection collection = WorldMap.Instance.MapTiles[x][y].ResourceList.Keys;

        foreach(Resource res in collection)
        {
            _ResourceInfos.Add(GameObject.Instantiate(_ParamInfo, _ResourcesPanel.transform).GetComponent<ParamInfo>());
            _ResourceInfos[_ResourceInfos.Count - 1].SetLabel(res.Category.Name + " - " + "Level " + (int)res.Level);
            _ResourceInfos[_ResourceInfos.Count - 1].SetValue(WorldMap.Instance.MapTiles[x][y].ResourceList[res]);
        }


        _InfoPanel.SetActive(true);
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
    }
}
