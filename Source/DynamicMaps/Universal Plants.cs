﻿using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace DynamicMaps
{
    [HarmonyPatch(typeof(WildPlantSpawner), "CalculatePlantsWhichCanGrowAt")]
    internal static class UniversalPlants
    {
        internal static void Postfix(IntVec3 c, Map ___map, List<ThingDef> outPlants)
        {
            ThingDef thingDef;
            outPlants.Remove(ThingDef.Named("Plant_Dandelion"));
            thingDef = ThingDef.Named("Plant_Dandelion");
            if (!thingDef.CanEverPlantAt_NewTemp(c, ___map))
            {
                return;
            }
            if (GenLocalDate.Season(___map) == Season.Spring)
            {
                Log.Error("spring test");
                outPlants.Add(thingDef);
            }
            //thingDef = ThingDef.Named("DM_Pansy");
            //if (!thingDef.CanEverPlantAt_NewTemp(c, ___map))
            //{
            //    return;
            //}
            //outPlants.Add(thingDef);
            //thingDef = ThingDef.Named("DM_Daffodil");
            //if (!thingDef.CanEverPlantAt_NewTemp(c, ___map))
            //{
            //    return;
            //}
            //outPlants.Add(thingDef);
        }
    }
    [HarmonyPatch(typeof(WildPlantSpawner), "GetCommonalityOfPlant")]
    internal static class UniversalPlantsCommonality
    {
        internal static void Postfix(ref float __result, ThingDef plant)
        {
            if (plant.defName == "DM_Pansy")
                __result = 5;
            if (plant.defName == "DM_Daffodil")
                __result = 5;
            if (plant.defName == "DM_Dandelion")
                __result = 5;
        }
    }
    [StaticConstructorOnStartup]
    public class DM_Plant : Plant
    {
        public Graphic semiMatureGraphic;
        private static Graphic GraphicSowing = GraphicDatabase.Get<Graphic_Single>("Things/Plant/Plant_Sowing", ShaderDatabase.Cutout, Vector2.one, Color.white);
        public override Graphic Graphic
        {
            get
            {
                Graphic graphic = def.graphic;

                if (LifeStage == PlantLifeStage.Sowing)
                {
                    graphic = GraphicSowing;
                }
                if (def.plant.leaflessGraphic != null && LeaflessNow && (!sown || !HarvestableNow))
                {
                    graphic = def.plant.leaflessGraphic;
                }
                if (def.HasModExtension<DM_ModExtension>())
                {

                    DM_ModExtension ext = def.GetModExtension<DM_ModExtension>();
                    if (ext.SemiMaturePath != null && Growth < 0.8f)
                    {
                        graphic = GraphicDatabase.Get(def.graphicData.graphicClass, ext.SemiMaturePath, def.graphic.Shader, def.graphicData.drawSize, def.graphicData.color, graphic.colorTwo);
                    }
                }
                if (def.plant.immatureGraphic != null && Growth < 0.5f)
                {
                    graphic = def.plant.immatureGraphic;
                }
                return graphic;
            }
        }
    }
}
