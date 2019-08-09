using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Database;

namespace ShinseiChiller
{
    //At this point, maybe combine all these?

    //The hydrogen-using chiller
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public class ShinseiChillerBuildingsPatch
    {
        private static void Prefix()
        {
            Strings.Add(new string[]
            {
                    "STRINGS.BUILDINGS.PREFABS.SHINSEICHILLER.NAME",
                    "Shinsei Chiller"
            });
            Strings.Add(new string[]
            {
                    "STRINGS.BUILDINGS.PREFABS.SHINSEICHILLER.DESC",
                    "This Cools down the Area"
            });
            Strings.Add(new string[]
            {
                    "STRINGS.BUILDINGS.PREFABS.SHINSEICHILLER.EFFECT",
                    "Uses a bit of power and hydrogen to cool down the air"
            });
            ModUtil.AddBuildingToPlanScreen("Utilities", "ShinseiChiller");
        }
    }
    [HarmonyPatch(typeof(Db), "Initialize")]
    public class ShinseiChillerDbPatch
    {
        private static void Prefix()
        {
            List<string> list = new List<string>(Techs.TECH_GROUPING["TemperatureModulation"])
                {
                    "ShinseiChiller"
                };
            Techs.TECH_GROUPING["TemperatureModulation"] = list.ToArray();
        }
    }

    //standalone energy-only cooler
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public class ShinseiCoolerBuildingsPatch
    {
        private static void Prefix()
        {
            Strings.Add(new string[]
            {
                    "STRINGS.BUILDINGS.PREFABS.SHINSEICOOLER.NAME",
                    "Shinsei Cooler"
            });
            Strings.Add(new string[]
            {
                    "STRINGS.BUILDINGS.PREFABS.SHINSEICOOLER.DESC",
                    "This Cools down the Area"
            });
            Strings.Add(new string[]
            {
                    "STRINGS.BUILDINGS.PREFABS.SHINSEICOOLER.EFFECT",
                    "Uses a smidge of power to cool down the air"
            });
            ModUtil.AddBuildingToPlanScreen("Utilities", "ShinseiCooler");
        }
    }
    [HarmonyPatch(typeof(Db), "Initialize")]
    public class ShinseiCoolerDbPatch
    {
        private static void Prefix()
        {
            List<string> list = new List<string>(Techs.TECH_GROUPING["TemperatureModulation"])
                {
                    "ShinseiCooler"
                };
            Techs.TECH_GROUPING["TemperatureModulation"] = list.ToArray();
        }
    }

    //water-only cooler
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public class ShinseiWaterCoolerBuildingsPatch
    {
        private static void Prefix()
        {
            Strings.Add(new string[]
            {
                    "STRINGS.BUILDINGS.PREFABS.SHINSEIWATERCOOLER.NAME",
                    "Shinsei Water Cooler"
            });
            Strings.Add(new string[]
            {
                    "STRINGS.BUILDINGS.PREFABS.SHINSEIWATERCOOLER.DESC",
                    "This Cools down the water it's placed into"
            });
            Strings.Add(new string[]
            {
                    "STRINGS.BUILDINGS.PREFABS.SHINSEIWATERCOOLER.EFFECT",
                    "Uses a lot of power to cool down water at half the speed of the tepidizer"
            });
            ModUtil.AddBuildingToPlanScreen("Utilities", "ShinseiWaterCooler");
        }
    }
    [HarmonyPatch(typeof(Db), "Initialize")]
    public class ShinseiWaterCoolerDbPatch
    {
        private static void Prefix()
        {
            List<string> list = new List<string>(Techs.TECH_GROUPING["TemperatureModulation"])
                {
                    "ShinseiWaterCooler"
                };
            Techs.TECH_GROUPING["TemperatureModulation"] = list.ToArray();
        }
    }




    //for modding the colors
    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public class ShinseiChillerColorPatch
    {
        private static void Postfix(ref BuildingComplete __instance)
        {
            if (string.Compare(__instance.name, "ShinseiChillerComplete") == 0)
            {
                __instance.GetComponent<KAnimControllerBase>().TintColour = ShinseiChillerConfig.Color();
                //__instance.gameObject.AddOrGet<>
            }
        }
    }

    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public class ShinseiCoolerColorPatch
    {
        private static void Postfix(ref BuildingComplete __instance)
        {
            if (string.Compare(__instance.name, "ShinseiCoolerComplete") == 0)
            {
                __instance.GetComponent<KAnimControllerBase>().TintColour = ShinseiCoolerConfig.Color();
                //__instance.gameObject.AddOrGet<>
            }
        }
    }

    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public class ShinseiWaterCoolerColorPatch
    {
        private static void Postfix(ref BuildingComplete __instance)
        {
            if (string.Compare(__instance.name, "ShinseiWaterCoolerComplete") == 0)
            {
                __instance.GetComponent<KAnimControllerBase>().TintColour = ShinseiWaterCoolerConfig.Color();
                //__instance.gameObject.AddOrGet<>
            }
        }
    }
}
