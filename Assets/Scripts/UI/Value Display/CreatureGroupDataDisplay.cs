using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class CreatureGroupDataDisplay : MonoBehaviour
{
    public bool IsActivated;

    private CreatureGroup _Group;

    [SerializeField] private TextValueDisplay _IDDisplay;
    [SerializeField] private TextValueDisplay _CreatureDisplay;
    [SerializeField] private TextValueDisplay _ResourcesCanCarriedDisplay;
    [SerializeField] private TextValueDisplay _LifespanDisplay;
    [SerializeField] private TextValueDisplay _SpeedDisplay;
    [SerializeField] private TextValueDisplay _MapPosDisplay;
    [SerializeField] private TextValueDisplay _WorldPosDisplay;

    [SerializeField] private SliderValueDisplay _PopulationDisplay;
    [SerializeField] private SliderValueDisplay _TemperatureDisplay;
    [SerializeField] private SliderValueDisplay _MoistureDisplay;

    [SerializeField] private ListValueDisplay _AbilitiesDisplay;

    [SerializeField] private Sprite _PopulationGain;
    [SerializeField] private Sprite _PopulationDecline;

    [SerializeField] private Sprite _Hot;
    [SerializeField] private Sprite _Warm;
    [SerializeField] private Sprite _Cold;

    [SerializeField] private Sprite _Moist;
    [SerializeField] private Sprite _Dry;

    public void Activate(CreatureGroup group)
    {
        IsActivated = true;

        UIManager.INSTANCE.PopoutUI.BottomPanel.SetActive(true);
        
        _IDDisplay.gameObject.SetActive(true);
        _CreatureDisplay.gameObject.SetActive(true);
        _ResourcesCanCarriedDisplay.gameObject.SetActive(true);
        _LifespanDisplay.gameObject.SetActive(true);
        _SpeedDisplay.gameObject.SetActive(true);
        _MapPosDisplay.gameObject.SetActive(true);
        _WorldPosDisplay.gameObject.SetActive(true);

        _PopulationDisplay.gameObject.SetActive(true);
        _TemperatureDisplay.gameObject.SetActive(true);
        _MoistureDisplay.gameObject.SetActive(true);


        SetGroupDisplay(group);
        UIManager.INSTANCE.OnUpdate += UpdateGroupDisplay;
        group.OnLeaderChanged += SetGroupDisplay;
    }

    public void Deactivate()
    {
        IsActivated = false;

        UIManager.INSTANCE.PopoutUI.BottomPanel.SetActive(false);

        _IDDisplay.gameObject.SetActive(false);
        _CreatureDisplay.gameObject.SetActive(false);
        _ResourcesCanCarriedDisplay.gameObject.SetActive(false);
        _LifespanDisplay.gameObject.SetActive(false);
        _SpeedDisplay.gameObject.SetActive(false);
        _MapPosDisplay.gameObject.SetActive(false);
        _WorldPosDisplay.gameObject.SetActive(false);

        _PopulationDisplay.gameObject.SetActive(false);
        _TemperatureDisplay.gameObject.SetActive(false);
        _MoistureDisplay.gameObject.SetActive(false);
        _AbilitiesDisplay.gameObject.SetActive(false);

        UIManager.INSTANCE.OnUpdate -= UpdateGroupDisplay;

        if (_Group)
            _Group.OnLeaderChanged -= SetGroupDisplay;
    }

    public void SetGroupDisplay(CreatureGroup group)
    {
        _Group = group;

        _IDDisplay.UpdateValue(_Group.Index.ToString());
        _CreatureDisplay.UpdateValue(_Group.LeaderCreature.Code.GetCode());
        _ResourcesCanCarriedDisplay.UpdateValue(_Group.LeaderCreature.ResourceCarryNum);
        _LifespanDisplay.UpdateValue(_Group.LeaderCreature.Lifespan);
        _SpeedDisplay.UpdateValue(_Group.LeaderCreature.Speed);

        _PopulationDisplay.Setup(0, _Group.LeaderCreature.GroupMax, _Group.LeaderCreature.GroupMin, _Group.LeaderCreature.GroupMax, _Group.Creatures.Sum(i => i.Value));
        _TemperatureDisplay.Setup(-30.0f, 60.0f, _Group.LeaderCreature.LowestTemperatureAccept, _Group.LeaderCreature.HighestTemperatureAccept, WorldMap.INSTANCE.MapTiles[_Group.MapPosition.x][_Group.MapPosition.y].Temperature);
        _MoistureDisplay.Setup(0, 1500.0f, _Group.LeaderCreature.HumidityRequired, 1500.0f, WorldMap.INSTANCE.MapTiles[_Group.MapPosition.x][_Group.MapPosition.y].Humidity);
        UpdateMoistureValue();
    }

    public void SetGroupDisplay()
    {
        _IDDisplay.UpdateValue(_Group.Index.ToString());
        _CreatureDisplay.UpdateValue(_Group.LeaderCreature.Code.GetCode());

        _PopulationDisplay.Setup(0, _Group.LeaderCreature.GroupMax, _Group.LeaderCreature.GroupMin, _Group.LeaderCreature.GroupMax, _Group.Creatures.Sum(i => i.Value));
        _TemperatureDisplay.Setup(-30.0f, 60.0f, _Group.LeaderCreature.LowestTemperatureAccept, _Group.LeaderCreature.HighestTemperatureAccept, WorldMap.INSTANCE.MapTiles[_Group.MapPosition.x][_Group.MapPosition.y].Temperature);
        _MoistureDisplay.Setup(0, 1500.0f, _Group.LeaderCreature.HumidityRequired, 1500.0f, WorldMap.INSTANCE.MapTiles[_Group.MapPosition.x][_Group.MapPosition.y].Humidity);
        UpdateMoistureValue();
    }

    public void UpdateGroupDisplay()
    {
        _MapPosDisplay.UpdateValue(string.Format("({0:n3}, {1:n3})", _Group.MapPosition.x, _Group.MapPosition.y));
        _WorldPosDisplay.UpdateValue(string.Format("({0:n3}, {1:n3})", _Group.transform.position.x, _Group.transform.position.y));

        _PopulationDisplay.UpdateValue((float)_Group.Creatures.Sum(i => i.Value));
        _TemperatureDisplay.UpdateValue(WorldMap.INSTANCE.MapTiles[_Group.MapPosition.x][_Group.MapPosition.y].Temperature);
        UpdateMoistureValue();
    }

    private void UpdateMoistureValue()
    {
        float moist = WorldMap.INSTANCE.MapTiles[_Group.MapPosition.x][_Group.MapPosition.y].Humidity;
        moist = moist < 0 ? 1000.0f : moist;

        _MoistureDisplay.UpdateValue(moist);
    }


}
