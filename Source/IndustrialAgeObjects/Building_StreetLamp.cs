﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace IndustrialAge.Objects
{
    class Building_StreetLamp : Building
    {
        private CompBreakdownable compBreakdownable = null;
        private ThingWithComps_Glower glower;
        private readonly ThingDef glowerDef = ThingDef.Named("Jecrell_GasLampGlower");

        private void SpawnGlower()
        {
            Thing thing = ThingMaker.MakeThing(glowerDef, null);
            IntVec3 position = Position + GenAdj.CardinalDirections[0]
                                             + GenAdj.CardinalDirections[0];
            GenPlace.TryPlaceThing(thing, position, Map, ThingPlaceMode.Near);
            glower = thing as ThingWithComps_Glower;
            glower.master = this;
        }

        private void DespawnGlower()
        {
            glower.master = null;
            glower.DeSpawn();
            glower = null;
        }

        private void ResolveGlower()
        {
            if (compBreakdownable != null)
            {
                if (compBreakdownable.BrokenDown)
                {
                    if (glower != null)
                    {
                        DespawnGlower();
                    }

                    return;
                }
                if (glower == null)
                {
                    SpawnGlower();
                    return;
                }
            }
        }

        public override void SpawnSetup(Map map, bool bla)
        {
            base.SpawnSetup(map, bla);
            compBreakdownable = this.TryGetComp<CompBreakdownable>();
        }

        public override void Tick()
        {
            base.Tick();
            if (this.IsHashIntervalTick(60))
            {
                ResolveGlower();
            }
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            base.DeSpawn(mode);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref glower, "glower", false);
        }
    }
}
