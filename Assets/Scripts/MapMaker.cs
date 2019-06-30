#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapMaker : MonoBehaviour
{
    public int mapid = 8025;

    readonly string adrnPath = @"D:\download\LS2Reader_20190607\adrn.bin";

    readonly string pathTiles = "Assets/Tilemaps/Tiles/";

    //readonly string pathObjects = "Assets/Tilemaps/Objects/";

    readonly string pathTileInfo = @"D:\download\LS2Reader_20190607\tileInfo.txt";

    readonly string pathObjInfo = @"D:\download\LS2Reader_20190607\objectInfo.txt";

    readonly List<int> Info = new List<int>();
    readonly List<int> InfoObj = new List<int>();

    public SpriteRenderer BasicPrefab;

    public Grid Grid;
    public GameObject TilesObj;

    public List<GameObject> ObjectsObj;

    public void Export()
    {

        string path = @"D:\download\LS2Reader_20190607\map\" + mapid + ".dat";

        try
        {
            Info.Clear();
            InfoObj.Clear();

            foreach (var objG in ObjectsObj)
            {
                foreach(Transform singleObj in objG.transform)
                    Destroy(singleObj.gameObject);
            }

            using (StreamReader sr = new StreamReader(adrnPath))
            {
                Adrn.Init(sr.BaseStream);
            }

            using (StreamReader sr = new StreamReader(pathTileInfo))
            {
                ReadInfo(sr);
            }

            using (StreamReader sr = new StreamReader(pathObjInfo))
            {
                ReadObjInfo(sr);
            }

            using (StreamReader sr = new StreamReader(path))
            {
                var map = ReadFromStream(sr.BaseStream);

                Grid.name = "Map " + mapid;
                Grid.cellSize = new Vector3(1, 0.734375f, 1);
                Grid.cellLayout = GridLayout.CellLayout.Isometric;

                TilesObj.name = "Tiles";
                var rendertile = TilesObj.GetComponent<TilemapRenderer>();
                rendertile.mode = TilemapRenderer.Mode.Individual;
                rendertile.sortOrder = TilemapRenderer.SortOrder.TopRight;

                Tilemap tilesMap = TilesObj.GetComponent<Tilemap>();
                //tilesMap.ClearAllTiles();
                tilesMap.size = new Vector3Int(map.width, map.height, 0);
                tilesMap.color = new Color(1, 1, 1, 1);
                tilesMap.enabled = true;
                tilesMap.tileAnchor = new Vector3(0.25f, 0.25f, 0);

                //ObjsObj.name = "Objects";
                //var render = ObjsObj.GetComponent<TilemapRenderer>();
                //render.mode = TilemapRenderer.Mode.Individual;
                //render.sortOrder = TilemapRenderer.SortOrder.TopRight;
                //render.sortingLayerName = "Foreground";

                //Tilemap objectsMap = ObjsObj.GetComponent<Tilemap>();
                //objectsMap.ClearAllTiles();
                //objectsMap.size = new Vector3Int(map.width, map.height, 0);
                //objectsMap.color = new Color(1, 1, 1, 1);
                //objectsMap.enabled = true;
                //objectsMap.tileAnchor = new Vector3(0.25f, 0.25f, 0);

                Tile singleTile;
                int index = 0;
                TextureImporter nowpng;
                Real real;
                Sprite spr;
                string objPath = "Assets/Tilemaps/ObjectsPng/";

                List<int> RepairedPivot = new List<int>();
                for (int y = map.height-1; y >= 0; y--)
                {
                    for (int x = 0; x < map.width; x++)
                    {
                        if (index >= map.titles.Length)
                            break;

                        var number = map.titles[index];
                        if (number > 0 && Info.Contains(number))
                        {
                            singleTile = AssetDatabase.LoadAssetAtPath(pathTiles + number + ".asset", typeof(Tile)) as Tile;
                            var point = new Vector3Int(x, y, 0);
                            tilesMap.SetTile(point, singleTile);
                        }
                        else if (number > 0)
                            Debug.Log("缺少Tile資源：" + number);


                        var numberObj = map.objects[index];
                        if (numberObj > 0 && InfoObj.Contains(numberObj))
                        {
                            spr = Resources.Load<Sprite>("ObjectsPng/"+ numberObj);
                            if (spr == null)
                            {
                                index++;
                                continue;
                            }

                            Vector3Int localPlace = (new Vector3Int(x, y, (int)Grid.transform.position.y));
                            Vector3 place = Grid.CellToWorld(localPlace);
                            var obj = Instantiate(BasicPrefab);
                            obj.sprite = spr;
                            obj.name = numberObj.ToString();
                            obj.spriteSortPoint = SpriteSortPoint.Pivot;
                            obj.transform.position = place;

                            GameObject followObj = null;
                            if (numberObj == 11046)
                            {
                                followObj = ObjectsObj[0];
                            }
                            else if (GameTile.DecoObjects.Contains(numberObj))
                                followObj = ObjectsObj[1];
                            else if (GameTile.Traps.Contains(numberObj))
                                followObj = ObjectsObj[2];
                            else if (GameTile.LowerWalls.Contains(numberObj))
                                followObj = ObjectsObj[4];
                            else if (GameTile.HigherWalls.Contains(numberObj))
                                followObj = ObjectsObj[5];
                            else
                                followObj = ObjectsObj[3];

                            obj.transform.SetParent(followObj.transform, true);



                            nowpng = AssetImporter.GetAtPath(objPath + numberObj + ".png") as TextureImporter;
                            real = Adrn.Reals[Adrn.Images[numberObj]];
                            if (RepairedPivot.Contains(numberObj) || nowpng == null)
                            {
                                index++;
                                continue;
                            }

                            RepairedPivot.Add(numberObj);
                            var a = (float)real.offsetX/ (float)real.height * -1f;
                            var b = ((float)real.width + (float)real.offsetY)/ (float)real.width;
                            if (real.offsetX == 0)
                            {
                                a = 0.5f;
                            }

                            if (real.offsetY == 0)
                            {
                                b = 0.5f;
                            }

                            nowpng.spritePivot = new Vector2(a,b);

                            StartCoroutine(SaveFile(objPath, numberObj.ToString()));

                        }
                        else
                            Debug.Log("缺少Obj資源："+ numberObj);

                            index++;

                        if (index >= map.titles.Length)
                            break;
                    }
                }

                PrefabUtility.SaveAsPrefabAsset(Grid.gameObject, "Assets/Maps/" + Grid.name + ".prefab");
                tilesMap.RefreshAllTiles();
                AssetDatabase.Refresh();
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }

    }

    public void ReadInfo(StreamReader stream)
    {
        string line;
        string[] strArray;
        while ((line = stream.ReadLine()) != null)
        {
            strArray = line.Split(':');
            var number = Convert.ToInt32(strArray[1]);
            if (Info.Contains(number))
            {
                Debug.Log("這讀圖檔不該出現1");
                break;
            }
            else
                Info.Add(number);
        }
    }

    public void ReadObjInfo(StreamReader stream)
    {
        string line;
        string[] strArray;
        while ((line = stream.ReadLine()) != null)
        {
            strArray = line.Split(':');
            var number = Convert.ToInt32(strArray[1]);
            if (InfoObj.Contains(number))
            {
                Debug.Log("這讀圖檔不該出現1");
                break;
            }
            else
                InfoObj.Add(number);
        }
    }

    public Map ReadFromStream(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        byte[] buffer = new byte[8];
        stream.Read(buffer, 0, buffer.Length);

        Map map = new Map
        {
            width = BitConverter.ToInt32(buffer, 0),
            height = BitConverter.ToInt32(buffer, 4)
        };

        int unitCount = map.height * map.width;
        map.objects = new int[unitCount];
        map.titles = new int[unitCount];

        buffer = new byte[unitCount * 2 * 2];
        stream.Read(buffer, 0, buffer.Length);
        for (int i = 0; i < unitCount; i++)
        {
            map.titles[i] = BitConverter.ToInt16(buffer, i * 2); ;
            var tile = BitConverter.ToInt16(buffer, (unitCount * 2) + (i * 2));
            map.objects[i] = tile;
        }
        return map;
    }

    public IEnumerator SaveFile(string objPath, string numberObj)
    {
        AssetDatabase.WriteImportSettingsIfDirty(objPath + numberObj + ".png");
        AssetDatabase.ImportAsset(objPath + numberObj + ".png", ImportAssetOptions.ForceUpdate);
        yield break;
    }

}

public class Map
{
    public int[] objects;
    public int[] titles;
    public int height;
    public int width;
}

#endif
