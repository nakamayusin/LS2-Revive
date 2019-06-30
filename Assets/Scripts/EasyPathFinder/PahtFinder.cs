using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.EasyPathFinder
{
    public static class PathFinder
    {
        private static readonly HashSet<Vector3Int> Standpoint;
        static Tilemap Map;
        static Tilemap Collider;

        public static void Init(Tilemap map, Tilemap collider)
        {
            Map = map;
            Collider = collider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sp">周圍的人已經占據的格子</param>
        public static List<Vector3Int> FindPath(Vector3Int startPos, Vector3Int targetPos)
        {
            Tile.ColliderType type;
            Tile.ColliderType typeobj;

            bool walkable;
            bool end = false;
            Vector3Int checking;
            int dir;

            List<Vector3Int> list = new List<Vector3Int>();

            while (!end)
            {
                //從起點確認路徑點都是可以移動的，只要有一格不能就把終點設此
                var offset = Map.layoutGrid.CellToWorld(targetPos) - Map.layoutGrid.CellToWorld(startPos);

                dir = CharRender.DirectionToIndex(new Vector2(offset.x, offset.y), 8);
                checking = startPos + Global.DirValue[dir];

                type = Map.GetColliderType(checking);
                typeobj = Collider.GetColliderType(checking);

                walkable = (typeobj == Tile.ColliderType.Sprite) ||
                                        (type == Tile.ColliderType.Sprite && typeobj == Tile.ColliderType.None);

                if (!walkable || checking == startPos || RunBehaviour.Get.NearItems.ContainsValue(checking))
                    end = true;
                else
                {
                    startPos = checking;
                    list.Add(startPos);
                }

                if (checking == targetPos)
                    end = true;
                   
            }
            return list;
        }
    }
}
