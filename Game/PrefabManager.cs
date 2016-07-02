using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using OurGame.Engine;

namespace OurGame
{
    public static class PrefabManager
    {
        #region Prefabs paths
        private static Dictionary<string, string> prefabsData;
        #endregion


        private static Dictionary<string, GameObject> prefabs;

        public static void Initialize()
        {
            prefabsData = new Dictionary<string, string>();
            prefabs = new Dictionary<string, GameObject>();
            prefabsData.Add("Room", "Prefabs\\Room.xml");
            prefabsData.Add("Radiator", "Prefabs\\Radiator.xml");
            prefabsData.Add("Processor", "Prefabs\\Processor.xml");
            prefabsData.Add("Fan", "Prefabs\\Fan.xml");
            prefabsData.Add("Bullet", "Prefabs\\Bullet.xml");
            prefabsData.Add("DeathEnemy", "Prefabs\\DeathEnemy.xml");
            prefabsData.Add("Room1", "Prefabs\\Room1.xml");
            prefabsData.Add("Room2", "Prefabs\\Room2.xml");
            prefabsData.Add("Room3", "Prefabs\\Room3.xml");
            prefabsData.Add("Room4", "Prefabs\\Room4.xml");
            prefabsData.Add("RoomBoss", "Prefabs\\RoomBoss.xml");
            prefabsData.Add("RoomStarting", "Prefabs\\RoomStarting.xml");
            prefabsData.Add("Alien", "Prefabs\\Alien.xml");
            prefabsData.Add("Ghost", "Prefabs\\Ghost.xml");
        }

        public static void LoadContent(ContentManager content)
        {
            foreach(KeyValuePair<string,string> x in prefabsData)
            {
                GameObject newPrefab = GameObject.LoadGameObject(x.Value);
                prefabs.Add(x.Key, newPrefab);
            }
        }
        public static GameObject GetPrafabClone(string name)
        {
            var res = prefabs[name].Clone();
            res.Initialize();
            return res;
        }
        public static GameObject GetPrafabClone(string name, bool hasAutomaticInicialize)
        {
            var res = prefabs[name].Clone();
            if (hasAutomaticInicialize)
            {
                res.Initialize();
            }
            return res;
        }

    }
}
