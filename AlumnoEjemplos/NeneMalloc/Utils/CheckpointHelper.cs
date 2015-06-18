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


            ////Checkpoints de la escalera
            //var stairsCheckpoints = new List<Checkpoint>();

            //checkpoint = new Checkpoint(new Vector3(-48.11425f, -19.1817f, -798.379f));
            //stairsCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-48.4113f, -19.1817f, -752.2625f));
            //stairsCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(438.5003f, -21.042f, 74.19636f));
            //stairsCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(437.6579f, -21.042f, 15.70669f));
            //stairsCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-254.5191f, -21.1711f, 449.3713f));
            //stairsCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-253.2172f, -21.1711f, 498.742f));
            //stairsCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(139.9471f, -18.3292f, 543.3478f));
            //stairsCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(141.7452f, -18.3292f, 481.3215f));
            //stairsCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(771.3737f, -18.3292f, 553.6575f));
            //stairsCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(778.4038f, -18.3292f, 497.1955f));
            //stairsCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(381.1559f, -18.3292f, 488.4356f));
            //stairsCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(381.1559f, -18.3292f, 442.5632f));
            //stairsCheckpoints.Add(checkpoint);

            ////Checkpoints del primer piso
            //var firstFloorCheckpoints = new List<Checkpoint>();

            //checkpoint = new Checkpoint(new Vector3(128.562f, 45.05f, -752.5922f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(103.4315f, 45.05f, -680.0554f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(199.3246f, 45.05f, -680.384f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(190.4221f, 45.05f, -726.1567f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(333.7174f, 45.05f, -727.3807f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(133.1839f, 45.05f, -550.085f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(29.99813f, 45.05f, -543.8524f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(35.04398f, 45.05f, -610.6483f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-56.00881f, 45.05f, -605.1484f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(203.7932f, 45.05f, -549.7377f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(287.8117f, 45.05f, -499.8098f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(349.3032f, 45.05f, -498.6933f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(126.6645f, 45.05f, -360.2591f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(32.61713f, 45.05f, -371.0748f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-8.76408f, 45.05f, -414.1614f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-110.5794f, 45.05f, -425.8705f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(194.7245f, 45.05f, -360.1672f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(244.625f, 45.05f, -309.2738f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(356.0635f, 45.05f, -298.4324f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(124.9204f, 45.05f, -167.1719f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(33.55367f, 45.05f, -178.4633f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-7.165917f, 45.05f, -213.6102f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-121.8994f, 45.05f, -206.8676f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(204.3016f, 45.05f, -171.0816f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(254.7637f, 45.05f, -123.6587f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(363.4653f, 45.05f, -123.1293f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(126.4716f, 45.05f, 54.51212f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(268.3596f, 45.05f, 9.65829f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-57.19542f, 45.05f, 477.3422f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-52.78262f, 45.05f, 876.7896f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(161.8566f, 45.05f, 882.0452f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-255.9604f, 45.05f, 873.9169f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-59.98159f, 45.05f, 1164.413f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(173.3964f, 45.05f, 1183.089f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-515.8253f, 45.05f, 1164.678f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-32.89465f, 45.05f, 1473.632f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-270.3369f, 45.05f, 1476.376f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-56.35907f, 45.05f, 1683.034f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(267.0035f, 45.05f, 1680.632f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-46.9439f, 45.05f, 1748.662f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-249.9617f, 45.05f, 1771.954f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-67.16974f, 45.05f, 2034.404f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(-262.052f, 45.05f, 2039.794f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(277.2758f, 45.05f, 2024.879f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(565.5536f, 45.05f, 2032.795f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(761.587f, 45.05f, 2044.176f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(575.2949f, 45.05f, 1757.459f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(803.9442f, 45.05f, 1762.394f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(590.625f, 45.05f, 1688.794f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(572.6483f, 45.05f, 1442.964f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(793.3728f, 45.05f, 1443.247f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(273.8112f, 45.05f, 1440.544f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(590.0533f, 45.05f, 1184.738f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(960.8246f, 45.05f, 1178.66f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(380.7877f, 45.05f, 1166.235f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(583.562f, 45.05f, 881.5002f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(785.9854f, 45.05f, 859.2502f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(404.5685f, 45.05f, 870.8986f));
            //firstFloorCheckpoints.Add(checkpoint);
            //checkpoint = new Checkpoint(new Vector3(586.2754f, 45.05f, 491.3209f));
            //firstFloorCheckpoints.Add(checkpoint);

            CheckPoints.Add(Floor.GroundFloor, groundFloorCheckpoints);
            //CheckPoints.Add(Floor.Stairs, stairsCheckpoints);
            //CheckPoints.Add(Floor.FirstFloor, firstFloorCheckpoints);
        }
    }
   
}
