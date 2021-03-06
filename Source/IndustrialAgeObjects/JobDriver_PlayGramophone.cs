﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
//using VerseBase;
using Verse;
using Verse.AI;
using Verse.Sound;
using RimWorld;
//using RimWorld.Planet;
//using RimWorld.SquadAI;


namespace IndustrialAge.Objects
{

    public class JobDriver_PlayGramophone : JobDriver
    {

        public override bool TryMakePreToilReservations(bool debug)
        {
            return true;
        }

        protected int Duration { get; } = 400;

        private string report = "";
        public override string GetReport()
        {
            if (report != "")
            {
                return base.ReportStringProcessed(report);
            }
            return base.GetReport();
        }

        //What should we do?
        protected override IEnumerable<Toil> MakeNewToils()
        {

            //Check it out. Can we go there?
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);

            if (job.targetA.Thing is Building_Radio)
            {
                report = "Playing the radio.";
            }

            // Toil 1:
            // Reserve Target (TargetPack A is selected (It has the info where the target cell is))
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1);

            // Toil 2:
            // Go to the thing.
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            // Toil 3:
            // Wind up the gramophone
            var toil = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = Duration
            };
            toil.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            if (job.targetA.Thing is Building_Radio)
            {
                toil.PlaySustainerOrSound(DefDatabase<SoundDef>.GetNamed("Estate_RadioSeeking"));
            }
            else
            {
                toil.PlaySustainerOrSound(DefDatabase<SoundDef>.GetNamed("Estate_GramophoneWindup"));
            }

            toil.initAction = delegate
            {
                var gramophone = job.targetA.Thing as Building_Gramophone;
                gramophone.StopMusic();
            };
            yield return toil;

            // Toil 4:
            // Play music.

            var toilPlayMusic = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Instant,
                initAction = delegate
                {
                    var gramophone = job.targetA.Thing as Building_Gramophone;
                    gramophone.PlayMusic(pawn);
                }
            };
            yield return toilPlayMusic;

            yield break;
        }

    }
}




/*

This is the needed XML file to make a real Job from the JobDriver
     
<?xml version="1.0" encoding="utf-8" ?>
<JobDefs>
<!--========= Job ============-->
	<JobDef>
	<defName>PlayGramophone</defName>
	<driverClass>ArkhamEstate.JobDriver_PlayGramophone</driverClass>
	<reportString>Winding up gramophone.</reportString>
	</JobDef>
</JobDefs>
     
*/
