using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldMap : MonoBehaviour
{
    public static WorldMap Instance;

    public Tilemap Base;
    public Tilemap Detail;
    public Tilemap UI;

    public TilePalette BasePalette;
    public TilePalette DetailPalette;

    public const int MIN_WIDTH_MAP = 50;
    public const int MIN_HEIGHT_MAP = 25;
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

    //Resource generation parameter
    public ResourceCategoryList ResCategoryList;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }


        CheckMapSize();
        GenerateMap();
    }

    private void CheckMapSize()
    {
        if (!HeightMap)
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
        }
        else
        {
            if (HeightMap.width > MAX_WIDTH_MAP)
            {
                Width = MAX_WIDTH_MAP;
                Height = (int)(HeightMap.height * ((float)MAX_WIDTH_MAP / HeightMap.width));
            }
            else
            {
                Width = HeightMap.width;
                Height = HeightMap.height;
            }
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
                MapTiles[i][ii].Altitude = CalculateAltitude(i, ii);
                MapTiles[i][ii].Temperature = CalculateTemperature(MapTiles[i][ii]);
                MapTiles[i][ii].Humidity = CalculateHumiditiy(i, ii);

                //distribut resources
                MapTiles[i][ii].ResourceList = new Dictionary<Resource, float>();
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
            return AverageTemperature;
        }

        return AverageTemperature - (57.6f * ((data.Altitude - CoastHeight + 0.04f) / (1.0f - CoastHeight))); 
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


        if (MapTiles[x][y].Altitude > MountainHeight)
        {
            //Level 5
            MapTiles[x][y].ResourceList.Add(ResCategoryList.FindResourceCategory("Mineral").List[(int)Resource.ResourceLevel.LEVEL_5], 4000);
            
            //Level 1
            MapTiles[x][y].ResourceList.Add(ResCategoryList.FindResourceCategory("Mineral").List[(int)Resource.ResourceLevel.LEVEL_1], 8000);
        }
        else if (MapTiles[x][y].Altitude > HighlandHight)
        {
            //Level 5
            MapTiles[x][y].ResourceList.Add(ResCategoryList.FindResourceCategory("Mineral").List[(int)Resource.ResourceLevel.LEVEL_5], 2000);

            //Level 1
            int mineralAmount = (2000 + (4000 * mountain) + (500 * highland) - (1000 * flat) - (2000 * ocean));
            mineralAmount = mineralAmount < 0 ? 0 : mineralAmount;
            MapTiles[x][y].ResourceList.Add(ResCategoryList.FindResourceCategory("Mineral").List[(int)Resource.ResourceLevel.LEVEL_1], mineralAmount);
        }
        else if (MapTiles[x][y].Altitude > CoastHeight)
        {
            //Level 5
            MapTiles[x][y].ResourceList.Add(ResCategoryList.FindResourceCategory("Mineral").List[(int)Resource.ResourceLevel.LEVEL_5], 500);

            //Level 1
            int mineralAmount = (1000 + (4000 * mountain) + (1000 * highland) - (1000 * ocean));
            mineralAmount = mineralAmount < 0 ? 0 : mineralAmount;
            MapTiles[x][y].ResourceList.Add(ResCategoryList.FindResourceCategory("Mineral").List[(int)Resource.ResourceLevel.LEVEL_1], mineralAmount);
        }
        else if (MapTiles[x][y].Altitude < CoastHeight)
        {
            //Level 5
            MapTiles[x][y].ResourceList.Add(ResCategoryList.FindResourceCategory("Mineral").List[(int)Resource.ResourceLevel.LEVEL_5], 4000);

            //Level 1
            int mineralAmount = (8000 + (2000 * mountain) + (2000 * highland) + (1000 * flat));
            mineralAmount = mineralAmount < 0 ? 0 : mineralAmount;
            MapTiles[x][y].ResourceList.Add(ResCategoryList.FindResourceCategory("Mineral").List[(int)Resource.ResourceLevel.LEVEL_1], mineralAmount);
        }
    }
}
