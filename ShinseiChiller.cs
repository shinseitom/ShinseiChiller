using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;
using STRINGS;
using KSerialization;

namespace ShinseiChiller
{
    [SerializationConfig(MemberSerialization.OptIn)]
    class ShinseiChiller : StateMachineComponent<ShinseiChiller.StatesInstance>
    {
        public float TargetTemperature
        {
            get
            {
                return this.targetTemperature;
            }
        }
        
        protected override void OnSpawn()
        {
            base.OnSpawn();
            base.smi.StartSM();
        }
        
        public void SetLiquidHeater()
        {
            this.heatLiquid = true;
        }

        private ShinseiChiller.MonitorState MonitorHeating(float dt)
        {
            this.monitorCells.Clear();
            int cell = Grid.PosToCell(base.transform.GetPosition());
            GameUtil.GetNonSolidCells(cell, this.radius, this.monitorCells);
            int num = 0;
            float num2 = 0f;
            for (int i = 0; i < this.monitorCells.Count; i++)
            {
                if (Grid.Mass[this.monitorCells[i]] > this.minimumCellMass && ((Grid.Element[this.monitorCells[i]].IsGas && !this.heatLiquid) || (Grid.Element[this.monitorCells[i]].IsLiquid && this.heatLiquid)))
                {
                    num++;
                    num2 += Grid.Temperature[this.monitorCells[i]];
                }
            }
            if (num == 0)
            {
                return (!this.heatLiquid) ? ShinseiChiller.MonitorState.NotEnoughGas : ShinseiChiller.MonitorState.NotEnoughLiquid;
            }
            bool flag = num2 / (float)num >= this.targetTemperature;
            if (flag)
            {
                return ShinseiChiller.MonitorState.TooHot;
            }
            return ShinseiChiller.MonitorState.ReadyToHeat;
        }
        
        public float targetTemperature = 308.15f;
        
        public float minimumCellMass;
        
        public int radius = 2;
        
        [SerializeField]
        private bool heatLiquid;
        
        [MyCmpReq]
        private Operational operational;
        
        private List<int> monitorCells = new List<int>();
        
        public class StatesInstance : GameStateMachine<ShinseiChiller.States, ShinseiChiller.StatesInstance, ShinseiChiller, object>.GameInstance
        {
            public StatesInstance(ShinseiChiller master) : base(master)
            {
            }
        }
        
        public class States : GameStateMachine<ShinseiChiller.States, ShinseiChiller.StatesInstance, ShinseiChiller>
        {
            public override void InitializeStates(out StateMachine.BaseState default_state)
            {
                default_state = this.offline;
                base.serializable = false;
                this.statusItemUnderMassLiquid = new StatusItem("statusItemUnderMassLiquid", BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_LIQUID.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_LIQUID.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 129022);
                this.statusItemUnderMassGas = new StatusItem("statusItemUnderMassGas", BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_GAS.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_GAS.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 129022);
                this.statusItemOverTemp = new StatusItem("statusItemOverTemp", BUILDING.STATUSITEMS.HEATINGSTALLEDHOTENV.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDHOTENV.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 129022);
                this.statusItemOverTemp.resolveStringCallback = delegate (string str, object obj)
                {
                    ShinseiChiller.StatesInstance statesInstance = (ShinseiChiller.StatesInstance)obj;
                    return string.Format(str, GameUtil.GetFormattedTemperature(statesInstance.master.TargetTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
                };
                this.offline.EventTransition(GameHashes.OperationalChanged, this.online, (ShinseiChiller.StatesInstance smi) => smi.master.operational.IsOperational);
                this.online.EventTransition(GameHashes.OperationalChanged, this.offline, (ShinseiChiller.StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(this.online.heating).Update("ShinseiChiller_online", delegate (ShinseiChiller.StatesInstance smi, float dt)
                {
                    switch (smi.master.MonitorHeating(dt))
                    {
                        case ShinseiChiller.MonitorState.ReadyToHeat:
                            smi.GoTo(this.online.heating);
                            break;
                        case ShinseiChiller.MonitorState.TooHot:
                            smi.GoTo(this.online.overtemp);
                            break;
                        case ShinseiChiller.MonitorState.NotEnoughLiquid:
                            smi.GoTo(this.online.undermassliquid);
                            break;
                        case ShinseiChiller.MonitorState.NotEnoughGas:
                            smi.GoTo(this.online.undermassgas);
                            break;
                    }
                }, UpdateRate.SIM_4000ms, false);
                this.online.heating.Enter(delegate (ShinseiChiller.StatesInstance smi)
                {
                    smi.master.operational.SetActive(true, false);
                }).Exit(delegate (ShinseiChiller.StatesInstance smi)
                {
                    smi.master.operational.SetActive(false, false);
                });
                this.online.undermassliquid.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, this.statusItemUnderMassLiquid, null);
                this.online.undermassgas.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, this.statusItemUnderMassGas, null);
                this.online.overtemp.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, this.statusItemOverTemp, null);
            }
            
            public GameStateMachine<ShinseiChiller.States, ShinseiChiller.StatesInstance, ShinseiChiller, object>.State offline;
            
            public ShinseiChiller.States.OnlineStates online;
            
            private StatusItem statusItemUnderMassLiquid;
            
            private StatusItem statusItemUnderMassGas;
            
            private StatusItem statusItemOverTemp;
            
            public class OnlineStates : GameStateMachine<ShinseiChiller.States, ShinseiChiller.StatesInstance, ShinseiChiller, object>.State
            {
                public GameStateMachine<ShinseiChiller.States, ShinseiChiller.StatesInstance, ShinseiChiller, object>.State heating;
                
                public GameStateMachine<ShinseiChiller.States, ShinseiChiller.StatesInstance, ShinseiChiller, object>.State overtemp;
                
                public GameStateMachine<ShinseiChiller.States, ShinseiChiller.StatesInstance, ShinseiChiller, object>.State undermassliquid;
                
                public GameStateMachine<ShinseiChiller.States, ShinseiChiller.StatesInstance, ShinseiChiller, object>.State undermassgas;
            }
        }
        
        private enum MonitorState
        {
            ReadyToHeat,
            TooHot,
            NotEnoughLiquid,
            NotEnoughGas
        }
    }
}
