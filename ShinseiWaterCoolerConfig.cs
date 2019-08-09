using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TUNING;

namespace ShinseiChiller
{
    class ShinseiWaterCoolerConfig : LiquidHeaterConfig
    {
        public override BuildingDef CreateBuildingDef()
        {
            string id = "ShinseiWaterCooler";
            int width = 4;
            int height = 1;
            string anim = "boiler_kanim";
            int hitpoints = 30;
            float construction_time = 30f;
            float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
            string[] all_METALS = MATERIALS.ALL_METALS;
            float melting_point = 3200f;
            BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
            EffectorValues none = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, all_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER1, none, 0.2f);
            buildingDef.RequiresPowerInput = true;
            buildingDef.Floodable = false;
            buildingDef.EnergyConsumptionWhenActive = 960f;//960f;
            buildingDef.ExhaustKilowattsWhenActive = -2000;//4000f;
            buildingDef.SelfHeatKilowattsWhenActive = -32f;//64f;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "SolidMetal";
            buildingDef.OverheatTemperature = 1273.15f;//1073.15f;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<LoopingSounds>();
            ShinseiChiller shinseiChiller = go.AddOrGet<ShinseiChiller>();
            shinseiChiller.targetTemperature = 3200f;
            shinseiChiller.SetLiquidHeater();
            shinseiChiller.minimumCellMass = 0f;//400f;

            //required!
            go.AddOrGet<MinimumOperatingTemperature>().minimumTemperature = 16f;
        }

        public const string ID = "ShinseiWaterCooler";


        public static Color32 Color()
        {
            return new Color32(25, 255, 255, byte.MaxValue);
        }
    }
}
