using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Resource;

public class WorldMap : MonoBehaviour
{
    public static WorldMap INSTANCE;

    public Tilemap Base;
    public Tilemap Detail;
    public Tilemap UI;

    public TilePalette BasePalette;
    public TilePalette DetailPalette;

    public const int MIN_WIDTH_MAP = 30;
    public const int MIN_HEIGHT_MAP = 10;
    public const int MAX_WIDTH_MAP = 150;
    public const int MAX_HEIGHT_MAP = 75;

    public Texture2D HeightMap;

    public int Width = -1;
    public int Height = -1;
    public float RandomGenerateOffset;


    public List<List<Tiledata>> MapTiles;

    //Global properties
    public float AverageTemperature = 26.0f;

    //map generate parameter
    [Range(0.0f, 1.0f)]
    public float CoastHeight;

    [Range(0.0f, 1.0f)]
    public float HighlandHight;

    [Range(0.0f, 1.0f)]
    public float MountainHeight;

    public Bound TilemapBound;

    // Start is called before the first frame update
    void Start()
    {
        Base.ClearAllTiles();
        Detail.ClearAllTiles();
        UI.ClearAllTiles();

        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            Destroy(this);
            return;
        }


        CheckMapSize();
        GenerateMap();
        
        Base.CompressBounds();
        Bounds tilemapBound = Base.localBounds;

        TilemapBound = new Bound();
        TilemapBound.Min = Base.transform.TransformPoint(tilemapBound.min);
        TilemapBound.Max = Base.transform.TransformPoint(tilemapBound.max);
        TilemapBound.Width = TilemapBound.Max.x - TilemapBound.Min.x;
        TilemapBound.Height = TilemapBound.Max.y - TilemapBound.Min.y;

    }

    private void CheckMapSize()
    {
        //check and restrict the custom map size
        if (Width < MIN_WIDTH_MAP)
        {
            Width = MIN_WIDTH_MAP;
        }
        else if (Width > MIN_WIDTH_MAP)
        {
            Width = MAX_WIDTH_MAP;
        }

        if (Height < MIN_HEIGHT_MAP)
        {
            Height = MIN_HEIGHT_MAP;
        }
        else if (Height > MAX_HEIGHT_MAP)
        {
            Height = MAX_HEIGHT_MAP;
        }

        if (Width > HeightMap.width)
        {
            Width = HeightMap.width;
        }
        
        if (Height > HeightMap.height)
        {
            Height = HeightMap.height;
        }
    }

    private void GenerateMap()
    {
        MapTiles = new List<List<Tiledata>>();
        for (int i = 0; i < Width; ++i)
        {
            MapTiles.Add(new List<Tiledata>());
            MapTiles[i] = new List<Tiledata>();

            for (int ii = 0; ii < Height; ++ii)
            {
                //set the tile data
                MapTiles[i].Add(new Tiledata());
                MapTiles[i][ii].Latitude = 180.0f * (ii / (float)Height) - 90.0f;
                MapTiles[i][ii].Altitude = CalculateAltitude(i, ii);
                MapTiles[i][ii].Temperature = CalculateTemperature(MapTiles[i][ii]);
                MapTiles[i][ii].Humidity = CalculateHumiditiy(i, ii);

                //distribut resources
                MapTiles[i][ii].ResourceList = new Dictionary<Resource, int>();
                MineralDistribution(i, ii);

                //set the tile appearance
                MapTiles[i][ii].BaseTile = GetBaseTileGroup(MapTiles[i][ii]);
                MapTiles[i][ii].DetailTile = GetDetailTileGroup(MapTiles[i][ii]);

                UpdateTileBase(i, ii);
            }
        }
    }

    private float CalculateAltitude(float x, float y)
    {
        if (!HeightMap)
        {
            return Mathf.PerlinNoise((x / Width) + Random.Range(-RandomGenerateOffset, RandomGenerateOffset), (y / Height) + Random.Range(-RandomGenerateOffset, RandomGenerateOffset)) + 0.12f;
        }
        else
        {
            return HeightMap.GetPixel((int)(HeightMap.width * (x / Width)), (int)(HeightMap.height * (y / Height))).r;
        }
    }

    private float CalculateHumiditiy(int x, int y)
    {
        float humidity = 0.0f;
        float altitude;

        //surrounding
        int count = 0;
        for (int i = -2; i < 3; ++i)
        {
            if (x + i < 0 || x + i == Width)
            {
                continue;
            }

            for (int ii = -2; ii < 3; ++ii)
            {
                if (i == 0 && ii == 0)
                {
                    continue;
                }
                else if (y + ii < 0 || y + ii == Height)
                {
                    continue;
                }

                altitude = CalculateAltitude(x + i, y + ii);

                if (altitude > MountainHeight)
                {
                    humidity += 2.8f;
                }
                else if (altitude > HighlandHight)
                {
                    humidity += 1.25f;
                }
                else if(altitude > CoastHeight)
                {
                    humidity += 0.95f;
                }
                else if (altitude < CoastHeight)
                {
                    humidity += 3.5f;
                }

                ++count;
            }
        }
        humidity /= count;


        //Altitude
        if (MapTiles[x][y].Altitude > MountainHeight)
        {
            humidity *= 1.5f;
        }
        else if (MapTiles[x][y].Altitude > HighlandHight)
        {
            humidity *= 1.25f;
        }
        else if (MapTiles[x][y].Altitude > CoastHeight)
        {
            humidity *= 0.98f;
        }
        else
        {
            return -1.0f;
        }

        //temperature
        if (MapTiles[x][y].Temperature >= 0)
        {
            return (201 - Mathf.Pow(0.93647f, MapTiles[x][y].Temperature)) * humidity;
        }
        else
        {
            return (Mathf.Pow(1.18193f, MapTiles[x][y].Temperature) + 199) * humidity;
        }

    }

    private float CalculateTemperature(Tiledata data)
    {
        if(data.Altitude < CoastHeight)
        {
            return AverageTemperature - (0.7f * Mathf.Abs(data.Latitude));
        }

        return AverageTemperature - (57.6f * ((data.Altitude - CoastHeight + 0.04f) / (1.0f - CoastHeight))) - (0.7f * Mathf.Abs(data.Latitude)); 
    }


    private void UpdateTileBase(int x, int y)
    {
        int xWorld = x * 2;
        int yWorld = y * 2;

        //determine the base layer tile direction
        TileGroup.TileDir baseTileDir = TileGroup.TileDir.CENTER;
        TileGroup.TileDir detailTileDir = TileGroup.TileDir.CENTER;

        for (int ii = 0; ii < 2; ++ii)
        {
            for (int i = 0; i < 2; ++i)
            {
                //update base tile
                Base.SetTile(new Vector3Int(xWorld + i, yWorld + ii, 0), MapTiles[x][y].BaseTile.GetTile(baseTileDir, ii + ii + i));

                //update detail tile
                if(MapTiles[x][y].DetailTile)
                    Detail.SetTile(new Vector3Int(xWorld + i, yWorld + ii, 0), MapTiles[x][y].DetailTile.GetTile(detailTileDir, ii + ii + i));
            }
        }
    }

    private TileGroup GetBaseTileGroup(Tiledata data)
    {
        if(data.Altitude < CoastHeight)
        {
            //Water
            return BasePalette.GetTile("Ocean");
        }
        else
        {
            //Land
            if (data.Temperature < -20.0f)
            {
                return BasePalette.GetTile("SnowSoil");
            }
            else if(data.Humidity > 250.0f)
            {
                return BasePalette.GetTile("PlainSoil");
            }
            else
            {
                return BasePalette.GetTile("Sand");
            }
        }
    }

    private TileGroup GetDetailTileGroup(Tiledata data)
    {
        if (data.Altitude < CoastHeight)
        {
            //Water
            data.Altitude = 0.0f;
            return null;
        }
        else
        {
            //Land
            if(data.Altitude > MountainHeight)
            {
                return DetailPalette.GetTile("Mountain");
            }
            else if(data.Altitude > HighlandHight)
            {
                return DetailPalette.GetTile("Highland");
            }
            else
            {
                return null;
            }
        }
    }


    public void MineralDistribution(int x, int y)
    {
        //surrounding
        int mountain = 0;
        int highland = 0;
        int flat = 0;
        int ocean = 0;

        for (int i = -2; i < 3; ++i)
        {
            if (x + i < 0 || x + i == Width)
            {
                continue;
            }

            for (int ii = -2; ii < 3; ++ii)
            {
                if (i == 0 && ii == 0)
                {
                    continue;
                }
                else if (y + ii < 0 || y + ii == Height)
                {
                    continue;
                }

                float altitude = CalculateAltitude(x + i, y + ii);

                if (altitude > MountainHeight)
                {
                    ++mountain;
                }
                else if (altitude > HighlandHight)
                {
                    ++highland;
                }
                else if (altitude > CoastHeight)
                {
                    ++flat;
                }
                else if (altitude < CoastHeight)
                {
                    ++ocean;
                }
            }
        }


        List<Resource> minerals = ResourceList.GetInstance().GetResources(Categories.MINERAL);
        if (MapTiles[x][y].Altitude > MountainHeight)
        {
            //Level 5
            MapTiles[x][y].ResourceList.Add(minerals[4], 4000);
            
            //Level 2
            MapTiles[x][y].ResourceList.Add(minerals[1], 8000);
        }
        else if (MapTiles[x][y].Altitude > HighlandHight)
        {
            //Level 5
            MapTiles[x][y].ResourceList.Add(minerals[4], 2000);

            //Level 2
            int mineralAmount = (2000 + (4000 * mountain) + (500 * highland) - (1000 * flat) - (2000 * ocean));
            mineralAmount = mineralAmount < 0 ? 0 : mineralAmount;
            MapTiles[x][y].ResourceList.Add(minerals[1], mineralAmount);
        }
        else if (MapTiles[x][y].Altitude > CoastHeight)
        {
            //Level 5
            MapTiles[x][y].ResourceList.Add(minerals[4], 500);

            //Level 2
            int mineralAmount = (1000 + (4000 * mountain) + (1000 * highland) - (1000 * ocean));
            mineralAmount = mineralAmount < 0 ? 0 : mineralAmount;
            MapTiles[x][y].ResourceList.Add(minerals[1], mineralAmount);
        }
        else if (MapTiles[x][y].Altitude < CoastHeight)
        {
            //Level 5
            MapTiles[x][y].ResourceList.Add(minerals[4], 4000);

            //Level 2
            int mineralAmount = (8000 + (2000 * mountain) + (2000 * highland) + (1000 * flat));
            mineralAmount = mineralAmount < 0 ? 0 : mineralAmount;
            MapTiles[x][y].ResourceList.Add(minerals[1], mineralAmount);
        }
    }


    public void DynamicResource()
    {
        for(int x = 0; x < Width; ++x)
        {
            for(int y = 0; y < Height; ++y)
            {
                //resources flow
                ResourceFlow(x, y);

                //resources combination
                ResourceCombination(x, y);
            }
        }



    }

    public void ResourceCombination(int x, int y)
    {
        Tiledata tile = MapTiles[x][y];

        if (tile.Temperature <= 0)
            return;

        float energyGain = tile.Temperature * 1000;

        List<Resource> minerals = ResourceList.GetInstance().GetResources(Categories.MINERAL);
        int combineToNextLevel = 0;
        for (int i = 0; i < 4; ++i)
        {
            if (!tile.ResourceList.ContainsKey(minerals[i]))
                continue;

            combineToNextLevel = (int)Mathf.Clamp(Mathf.Pow(energyGain / ((minerals[i + 1].EnergyProvide - (minerals[i].EnergyProvide * 2)) * 10), 1.0f - ((float)i / 30.0f)), 0, tile.ResourceList[minerals[i]] / 2);

            if (!tile.ResourceList.ContainsKey(minerals[i + 1]))
                tile.ResourceList.Add(minerals[i + 1], 0);

            tile.ResourceList[minerals[i]] -= combineToNextLevel * 2;
            tile.ResourceList[minerals[i + 1]] += combineToNextLevel;
        }
    }


    public void ResourceFlow(int x, int y)
    {
        List<Resource> minerals = ResourceList.GetInstance().GetResources(Categories.MINERAL);
        
        if (!MapTiles[x][y].ResourceList.ContainsKey(minerals[0]))
            return; ;

        int totalValue = 0;
        float totalUnit = 0;
        float oldRatio = 1.0f;
        float newRatio = 1.0f;
        float targetRatio = 1.0f;
        int flowAmount = 0;

        for (int i = -1; i < 2; ++i)
        {
            if((x + i) < 0 || (x + i) >= Width)
                continue;

            for(int ii = -1; ii < 2; ++ii)
            {
                if ((y + ii) < 0 || (y + ii) >= Height)
                    continue;

                if (!MapTiles[x + i][y + ii].ResourceList.ContainsKey(minerals[0]))
                    continue;

                //calculate the target ratio base on altitude
                targetRatio = ((MapTiles[x][y].Altitude - MapTiles[x + i][y + ii].Altitude) / 0.05f) - 1.0f;

                //if the ratio is smaller than -1.0f, means the (x, y) is lower than (x + i, y + ii). Skip
                if (targetRatio < -1.0f)
                    continue;

                targetRatio = Mathf.Clamp(targetRatio, 1.0f, 5.0f);

                //calculate old ratio
                if (MapTiles[x + i][y + ii].ResourceList[minerals[0]] == 0)
                    oldRatio = (float)MapTiles[x][y].ResourceList[minerals[0]];
                else
                    oldRatio = (float)MapTiles[x][y].ResourceList[minerals[0]] / (float)MapTiles[x + i][y + ii].ResourceList[minerals[0]];

                if (oldRatio < targetRatio && !Mathf.Approximately(targetRatio,1.0f))
                    continue;

                newRatio = Mathf.Lerp(oldRatio, 1.0f / targetRatio, Time.deltaTime);
                
                totalUnit = (1.0f / newRatio) + 1;
                totalValue = MapTiles[x][y].ResourceList[minerals[0]] + MapTiles[x + i][y + ii].ResourceList[minerals[0]];

                flowAmount = MapTiles[x][y].ResourceList[minerals[0]] - (int)(totalValue / totalUnit);
                MapTiles[x][y].ResourceList[minerals[0]] -= flowAmount;
                MapTiles[x + i][y + ii].ResourceList[minerals[0]] += flowAmount;
            }
        }
    }
}
