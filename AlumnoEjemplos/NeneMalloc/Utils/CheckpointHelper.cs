using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.NeneMalloc.Utils
{
    class CheckpointHelper
    {
        public static Dictionary<Floor,List<Checkpoint>> CheckPoints= new Dictionary<Floor,List<Checkpoint>>();
        public static List<TgcArrow> lastCheckPointArrows;
        public static void renderAll()
        {
            foreach (KeyValuePair<Floor,List<Checkpoint>> keyPair in CheckPoints)
            {
                keyPair.Value.ForEach(c => c.render());
            }
        }
        public static void Add(Checkpoint checkPoint)
        {
            Floor key = (Floor) GuiController.Instance.Modifiers.getValue("PisoCheckPoint");
            if (!CheckPoints.ContainsKey(key))
            {
                CheckPoints[key] = new List<Checkpoint>();
            }
            CheckPoints[key].Add(checkPoint);
        }


        public static void GenerateGraph()
        {
            List<Checkpoint> checkPoints = new List<Checkpoint>();
            foreach (KeyValuePair<Floor, List<Checkpoint>> key in CheckPoints)
            {
                checkPoints.AddRange(key.Value);
            }
            checkPoints.ForEach(c => c.Checked = false);
            foreach (Checkpoint checkPoint in checkPoints)
            {
                checkPoint.Checked = true;
                List<Checkpoint> unCheckedCheckpoints = checkPoints;
                unCheckedCheckpoints = unCheckedCheckpoints.FindAll(c => checkPoint.hasDirectSightWith(c));
                checkPoint.Neighbors =
                    new HashSet<Checkpoint>(
                        unCheckedCheckpoints.FindAll(c => checkPoint.hasDirectSightWith(c))
                            .FindAll(
                                neighbor => 
                                            !unCheckedCheckpoints.Any(
                                                c =>
                                                    c != neighbor &&
                                                    neighbor.hasDirectSightWith(c) &&
                                                    Math.Abs(AngleBetweenInXandZ(checkPoint, neighbor, c)) < 15 &&
                                                    DistanceBetweenInXandZ(checkPoint, c) <
                                                    DistanceBetweenInXandZ(checkPoint, neighbor))

                            ));
            }
            DestroyLinkBetween(0,2);
            DestroyLinkBetween(8,25);
            DestroyLinkBetween(8,29);
            DestroyLinkBetween(17,43);
            DestroyLinkBetween(26,29);
            DestroyLinkBetween(23, 28);
            DestroyLinkBetween(38, 40);
            DestroyLinkBetween(45, 53);
            DestroyLinkBetween(51, 67);
            DestroyLinkBetween(52, 65);
            DestroyLinkBetween(41, 69);
            DestroyLinkBetween(75, 77);
            DestroyLinkBetween(73, 75);
            DestroyLinkBetween(81, 88);
            DestroyLinkBetween(90, 92);
            DestroyLinkBetween(91, 93);
            DestroyLinkBetween(83, 85);
            DestroyLinkBetween(82, 84);
            DestroyLinkBetween(102, 104);
            DestroyLinkBetween(103, 105);
            DestroyLinkBetween(104, 106);
        }

        public static float DistanceBetweenInXandZ(Checkpoint checkPoint, Checkpoint otherCheckpoint)
        {
            Vector3 positionOne = new Vector3(checkPoint.Position.X, 0f, checkPoint.Position.Z);
            Vector3 positionTwo = new Vector3(otherCheckpoint.Position.X, 0f, otherCheckpoint.Position.Z);
            return Vector3.Length(positionOne - positionTwo);
        }

        public static float AngleBetweenInXandZ(Checkpoint checkPointBase, Checkpoint otherCheckpoint, Checkpoint otherCheckpoint2)
        {
            Vector3 vector1 = otherCheckpoint.Position - checkPointBase.Position;
            Vector3 vector2 = otherCheckpoint2.Position - checkPointBase.Position;
            vector1.Normalize();
            vector2.Normalize();
            return Geometry.RadianToDegree(Convert.ToSingle(Math.Acos(Vector3.Dot(vector1, vector2))));
        }

        public static List<TgcArrow> PrepareClosestCheckPoint( Vector3 position, Checkpoint lastCheckPoint, out Checkpoint updatedChekPoint)
        {
            updatedChekPoint = GetClosestCheckPoint(position);
            if (lastCheckPoint != updatedChekPoint)
            {
                lastCheckPoint = updatedChekPoint;
                lastCheckPointArrows = updatedChekPoint.Neighbors.Select(c =>
                {
                    TgcArrow arrow = new TgcArrow();
                    arrow.PStart = lastCheckPoint.Position;
                    arrow.PEnd = c.Position;
                    arrow.BodyColor = Color.Black;
                    arrow.HeadColor = Color.White;
                    arrow.Thickness = 0.4f;
                    arrow.HeadSize = new Vector2(8, 10);

                    arrow.updateValues();
                    return arrow;
                }).ToList(); 
            }
            
            return lastCheckPointArrows;
            
        }
        public static Checkpoint GetClosestCheckPoint(Vector3 position)
        {
            List<Checkpoint> checkpoints = new List<Checkpoint>();
            foreach (KeyValuePair<Floor, List<Checkpoint>> key in CheckPoints)
            {
                checkpoints.AddRange(key.Value);
            }

           return checkpoints.Aggregate((checkPointMin, aCheckpoint) => (checkPointMin == null ||  Vector3.Length(position - aCheckpoint.Position) < (Vector3.Length(position - checkPointMin.Position)) ? aCheckpoint : checkPointMin));
        }

        public static void DestroyLinkBetween(int IdCheckpoint, int IdNeighbor)
        {
            var keyPair = CheckPoints.Single(k => k.Value.Any(c => c.id == IdCheckpoint));
            var checkpoint = keyPair.Value.Single(c => c.id == IdCheckpoint);
            checkpoint.DeleteNeighbor(IdNeighbor);
        }

        public static void BuildCheckpoints()
        {
            //Checkpoints de la planta baja
            var groundFloorCheckpoints = new List<Checkpoint>();

            var checkpoint = new Checkpoint(new Vector3(-441.2449f, -91.5322f, -1020.275f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-180.3557f, -91.5322f, -1240.219f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-3.346033f, -91.5322f, -924.025f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(107.6922f, -91.5322f, -1238.628f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-472.2211f, -91.5322f, -1237.265f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-6.083243f, -91.5322f, -794.3417f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-9.515788f, -91.5322f, -711.4396f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(207.0244f, -91.5322f, -717.0748f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-13.4697f, -91.5322f, -593.6032f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-12.59746f, -91.5322f, -538.896f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(78.98927f, -91.5322f, -579.123f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-11.9973f, -91.5322f, -394.514f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(102.992f, -91.5322f, -435.2148f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-14.12555f, -91.5322f, -245.6966f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(105.7645f, -91.5322f, -288.6515f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-11.39549f, -91.5322f, 192.1234f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(118.3192f, -93.5423f, 181.4908f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(117.8299f, -93.5423f, 326.5257f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-139.8307f, -93.5423f, 329.9497f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-139.4873f, -93.5423f, 222.7041f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-15.96007f, -93.5423f, 374.0578f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-15.85328f, -93.5423f, 444.1907f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-18.95858f, -93.5423f, 547.3691f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-127.6335f, -91.5322f, -604.7589f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-236.5264f, -91.5322f, -662.8294f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-252.1422f, -91.5322f, -548.9617f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-337.5294f, -91.5322f, -604.6776f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-450.4541f, -91.5322f, -798.5519f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-443.5921f, -91.5322f, -564.0257f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-536.9713f, -91.1875f, -592.6354f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-695.4191f, -91.1875f, -586.1326f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-438.5636f, -91.5322f, -388.287f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-238.9347f, -91.5322f, -389.5356f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-448.3088f, -91.5322f, 70.48834f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-447.3773f, -91.425f, 235.5176f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-278.5223f, -92.8721f, 237.1485f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-275.5135f, -92.8721f, 335.0953f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-558.3151f, -92.8721f, 336.4161f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-578.1342f, -92.8721f, 238.3849f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-652.7402f, -92.8721f, 440.7519f));
            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-644.4784f, -92.8721f, 543.4628f));
            groundFloorCheckpoints.Add(checkpoint);
            
            //Checkpoints de la escalera
            var stairsCheckpoints = new List<Checkpoint>();

            checkpoint = new Checkpoint(new Vector3(186.7543f, -18.3292f, 556.3228f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(183.7467f, -18.3292f, 497.9099f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-206.5249f, -18.3292f, 499.5992f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-206.1419f, -18.3292f, 441.5247f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-642.075f, -19.18169f, -788.9778f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-643.2663f, -19.18169f, -741.1729f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-845.9832f, -21.1711f, 451.8687f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-845.9832f, -21.1711f, 504.301f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-446.6695f, -18.3292f, 492.7986f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-446.7674f, -18.3292f, 533.6132f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-146.6003f, -21.04201f, 77.85492f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-146.4155f, -21.04201f, 6.449927f));
            stairsCheckpoints.Add(checkpoint);

            //Checkpoints del primer piso
            var firstFloorCheckpoints = new List<Checkpoint>();

            checkpoint = new Checkpoint(new Vector3(-448.5588f, 45.05f, -747.7059f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-466.3888f, 45.05f, -673.1213f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-296.2352f, 45.05f, -722.7036f));
            firstFloorCheckpoints.Add(checkpoint);
            //
            checkpoint = new Checkpoint(new Vector3(-453.1046f, 45.05f, -553.6915f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-331.3843f, 45.05f, -520.8478f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-618.611f, 45.05f, -578.6929f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-465.3097f, 45.05f, -389.0335f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-346.1839f, 45.05f, -330.8428f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-594.2684f, 45.05f, -398.4708f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-466.703f, 45.05f, -168.9748f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-320.4767f, 45.05f, -126.5933f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-619.2263f, 45.05f, -199.9368f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-478.084f, 45.05f, 146.0769f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-313.4892f, 45.05f, 182.8999f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-336.9914f, 45.05f, -2.76211f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-489.539f, 45.05f, 20.39594f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-13.86294f, 45.05f, 481.7654f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-0.7266606f, 45.05f, 872.2333f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(197.5538f, 45.05f, 874.0514f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-185.7934f, 45.05f, 879.1564f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-0.9810541f, 45.05f, 1170.692f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(187.189f, 45.05f, 1151.578f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(352.2277f, 45.05f, 1238.289f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(549.8654f, 45.05f, 1147.011f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(345.2043f, 45.05f, 1084.979f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-222.101f, 45.05f, 1166.275f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-30.57178f, 45.05f, 1446.456f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(192.7904f, 45.05f, 1441.94f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(15.79467f, 45.05f, 1697.493f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-162.1533f, 45.05f, 1691.188f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-342.474f, 45.05f, 1632.498f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-484.9714f, 45.05f, 1693.247f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-329.9544f, 45.05f, 1767.038f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-22.84791f, 45.05f, 1752.089f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(166.6231f, 45.05f, 1753.702f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-2.216207f, 45.05f, 2035.025f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(181.1671f, 45.05f, 2038.52f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-176.5126f, 45.05f, 2038.75f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-349.1664f, 45.05f, 2124.763f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-458.8842f, 45.05f, 2035.803f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-365.2083f, 45.05f, 1972.74f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-332.2917f, 45.05f, 1445.364f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-618.7535f, 45.05f, 1429.367f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-791.9449f, 45.05f, 1514.034f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-650.1395f, 45.05f, 1689.329f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-629.4071f, 45.05f, 1760.479f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-828.4836f, 45.05f, 1765.216f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-631.8762f, 45.05f, 2039.168f));
            firstFloorCheckpoints.Add(checkpoint);
           
            checkpoint = new Checkpoint(new Vector3(-833.1726f, 45.05f, 2035.984f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-637.7863f, 45.05f, 1154.442f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-848.3715f, 45.05f, 1143.457f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-1052.012f, 45.05f, 1272.097f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-1206.233f, 45.05f, 1150.388f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-987.455f, 45.05f, 1071.077f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-435.0444f, 45.05f, 1181.913f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-645.9997f, 45.05f, 878.9117f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-845.0198f, 45.05f, 874.7791f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-465.817f, 45.05f, 872.4465f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-641.4966f, 45.05f, 494.6979f));
            firstFloorCheckpoints.Add(checkpoint);

            CheckPoints.Add(Floor.GroundFloor, groundFloorCheckpoints);
            CheckPoints.Add(Floor.Stairs, stairsCheckpoints);
            CheckPoints.Add(Floor.FirstFloor, firstFloorCheckpoints);
        }
    }
   
}
