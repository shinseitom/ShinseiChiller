using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using STRINGS;
using TUNING;
using UnityEngine;

namespace ShinseiChiller
{
    class ShinseiChillerConfig : IBuildingConfig
    {
        public override BuildingDef CreateBuildingDef()
        {
            string id = "ShinseiChiller";
            int width = 2;
            int height = 2;
            string anim = "spaceheater_kanim";
            int hitpoints = 30;
            float construction_time = 30f;
            float[] tier = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
            string[] all_METALS = MATERIALS.ALL_METALS;
            float melting_point = 1600f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, all_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.BONUS.TIER1, tier2, 0.2f);
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 240f;
            buildingDef.ExhaustKilowattsWhenActive = -50f;
            buildingDef.SelfHeatKilowattsWhenActive = -50f;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.InputConduitType = ConduitType.Gas;
            buildingDef.OverheatTemperature = 800.15f;
            return buildingDef;
        }
        
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            //not super sure what these do
            go.AddOrGet<LoopingSounds>();
            ShinseiChiller shinseiChiller = go.AddOrGet<ShinseiChiller>();

            //go.AddOrGet<KAnimControllerBase>().TintColour = Color();

            //For a heater, this only allows it to run until it hits that temp
            //For a chiller, if it gets that hot (can't cool enough) it stops trying
            //Basically, should equal melting for a cooler
            shinseiChiller.targetTemperature = 1600.15f;

            //this requires an ElementConverter, it's basically an internal anti-entrophy machine
            go.AddOrGet<MassiveHeatSink>();

            //can hold 1kg of whatever (in this case, hydrogen, which is a bit)
            go.AddOrGet<Storage>().capacityKg = 1f;

            //required ElementConverter that is the consumer, with a list (of size one) of elements it eats
            go.AddOrGet<ElementConverter>().consumedElements = new ElementConverter.ConsumedElement[]
            {
                new ElementConverter.ConsumedElement(ElementLoader.FindElementByHash(SimHashes.Hydrogen).tag, 0.01f)
            };

            //here's how that hydrogen above is going to actually get to the converter
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.consumptionRate = 1f;
            conduitConsumer.capacityTag = GameTagExtensions.Create(SimHashes.Hydrogen);
            conduitConsumer.capacityKG = 1f;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

            //required ElementConverter that is the producer, with a list (of nothing, no output) of elements it generates
            ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
            elementConverter.outputElements = new ElementConverter.OutputElement[0];

            //sets the minimum temperature of the building before it stops working
            //CRUCIAL if a heat-negating building, otherwise you get a crash
            go.AddOrGet<MinimumOperatingTemperature>().minimumTemperature = 120f;
        }
        
        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_1_0);
        }
        
        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_1_0);
        }
        
        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_1_0);
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGetDef<PoweredActiveController.Def>();
        }

        private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[]
        {
            LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(1, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, "When active, is cooling", "When standby, disabled", false)
        };
        
        public const string ID = "ShinseiChiller";

        public static Color32 Color()
        {
            return new Color32(255, 64, 255, byte.MaxValue);
        }
    }
}
