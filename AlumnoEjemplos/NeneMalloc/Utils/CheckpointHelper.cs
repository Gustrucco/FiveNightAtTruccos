using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace AlumnoEjemplos.NeneMalloc.Utils
{
    class CheckpointHelper
    {
        public static Dictionary<Floor,List<Checkpoint>> CheckPoints= new Dictionary<Floor,List<Checkpoint>>();

        public static void renderAll()
        {
            foreach (KeyValuePair<Floor,List<Checkpoint>> keyPair in CheckPoints)
            {
                keyPair.Value.ForEach(c => c.render());
            }
        }
        public static void add(Checkpoint checkPoint, Floor key)
        {
            if (!CheckPoints.ContainsKey(key))
            {
                CheckPoints[key] = new List<Checkpoint>();
            }
            CheckPoints[key].Add(checkPoint);
        }

        public static void BuildCheckpoints()
        {
            Checkpoint checkpoint;

            //Checkpoints de la planta baja
            var groundFloorCheckpoints = new List<Checkpoint>();

            checkpoint = new Checkpoint(new Vector3(140.3071f, -91.425f, 246.465f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(135.1541f, -91.5322f, 73.97246f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(136.7793f, -91.5322f, -388.7028f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(367.2954f, -91.5322f, -379.0724f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(142.1351f, -91.5322f, -587.4449f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(48.5405f, -91.5322f, -587.1827f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(47.7466f, -91.5322f, -650.8663f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-207.8635f, -76.8167f, -647.2618f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-205.4841f, -76.8167f, -506.2321f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(63.18164f, -91.5322f, -510.1888f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(138.7557f, -91.5322f, -815.7433f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(157.8764f, -91.5322f, -932.082f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(579.3079f, -91.5322f, -959.9172f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(580.7623f, -91.5322f, -1237.077f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(100.1329f, -91.5322f, -1240.662f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(97.71819f, -91.5322f, -1141.469f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(199.6551f, -91.5322f, -1148.561f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(594.2427f, -91.5322f, -799.2379f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(376.4898f, -91.5322f, -793.5558f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(568.7994f, -91.5322f, -720.6974f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(875.2565f, -91.5322f, -717.1635f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(582.5909f, -91.5322f, -592.5996f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(484.4768f, -91.5322f, -601.9526f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(374.0719f, -91.5322f, -674.12f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(255.9218f, -91.5322f, -593.4872f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(400.259f, -91.5322f, -521.6392f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(581.5532f, -91.5322f, -548.7872f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(733.405f, -91.5322f, -560.9555f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(572.2836f, -91.5322f, -395.963f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(700.7197f, -91.5322f, -400.6456f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(557.5756f, -91.5322f, -248.8698f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(719.8679f, -91.5322f, -243.7147f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(566.7104f, -91.5322f, 186.395f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(720.7777f, -93.5423f, 181.9301f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(464.9825f, -93.5423f, 221.0516f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(599.3282f, -91.5322f, 224.6017f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(454.8033f, -93.5423f, 331.6003f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(707.5667f, -93.5423f, 321.0968f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(585.513f, -93.5423f, 332.7991f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(577.4136f, -93.5423f, 446.3262f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(590.7381f, -93.5423f, 547.0322f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(318.5031f, -92.8721f, 248.1255f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(21.41224f, -92.8721f, 240.3206f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(7.295446f, -92.8721f, 342.8895f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-58.09561f, -92.8721f, 345.3991f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(291.8305f, -92.8721f, 331.9693f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-49.60235f, -92.8721f, 454.0785f));            groundFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-60.96433f, -92.8721f, 538.7425f));            groundFloorCheckpoints.Add(checkpoint);

            //Checkpoints de la escalera
            var stairsCheckpoints = new List<Checkpoint>();
            
            checkpoint = new Checkpoint(new Vector3(-48.11425f, -19.1817f, -798.379f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-48.4113f, -19.1817f, -752.2625f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(438.5003f, -21.042f, 74.19636f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(437.6579f, -21.042f, 15.70669f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-254.5191f, -21.1711f, 449.3713f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-253.2172f, -21.1711f, 498.742f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(139.9471f, -18.3292f, 543.3478f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(141.7452f, -18.3292f, 481.3215f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(771.3737f, -18.3292f, 553.6575f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(778.4038f, -18.3292f, 497.1955f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(381.1559f, -18.3292f, 488.4356f));
            stairsCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(381.1559f, -18.3292f, 442.5632f));
            stairsCheckpoints.Add(checkpoint);

            //Checkpoints del primer piso
            var firstFloorCheckpoints = new List<Checkpoint>();

            checkpoint = new Checkpoint(new Vector3(128.562f, 45.05f, -752.5922f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(103.4315f, 45.05f, -680.0554f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(199.3246f, 45.05f, -680.384f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(190.4221f, 45.05f, -726.1567f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(333.7174f, 45.05f, -727.3807f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(133.1839f, 45.05f, -550.085f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(29.99813f, 45.05f, -543.8524f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(35.04398f, 45.05f, -610.6483f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-56.00881f, 45.05f, -605.1484f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(203.7932f, 45.05f, -549.7377f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(287.8117f, 45.05f, -499.8098f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(349.3032f, 45.05f, -498.6933f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(126.6645f, 45.05f, -360.2591f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(32.61713f, 45.05f, -371.0748f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-8.76408f, 45.05f, -414.1614f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-110.5794f, 45.05f, -425.8705f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(194.7245f, 45.05f, -360.1672f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(244.625f, 45.05f, -309.2738f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(356.0635f, 45.05f, -298.4324f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(124.9204f, 45.05f, -167.1719f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(33.55367f, 45.05f, -178.4633f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-7.165917f, 45.05f, -213.6102f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-121.8994f, 45.05f, -206.8676f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(204.3016f, 45.05f, -171.0816f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(254.7637f, 45.05f, -123.6587f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(363.4653f, 45.05f, -123.1293f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(126.4716f, 45.05f, 54.51212f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(268.3596f, 45.05f, 9.65829f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-57.19542f, 45.05f, 477.3422f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-52.78262f, 45.05f, 876.7896f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(161.8566f, 45.05f, 882.0452f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-255.9604f, 45.05f, 873.9169f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-59.98159f, 45.05f, 1164.413f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(173.3964f, 45.05f, 1183.089f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-515.8253f, 45.05f, 1164.678f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-32.89465f, 45.05f, 1473.632f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-270.3369f, 45.05f, 1476.376f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-56.35907f, 45.05f, 1683.034f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(267.0035f, 45.05f, 1680.632f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-46.9439f, 45.05f, 1748.662f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-249.9617f, 45.05f, 1771.954f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-67.16974f, 45.05f, 2034.404f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(-262.052f, 45.05f, 2039.794f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(277.2758f, 45.05f, 2024.879f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(565.5536f, 45.05f, 2032.795f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(761.587f, 45.05f, 2044.176f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(575.2949f, 45.05f, 1757.459f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(803.9442f, 45.05f, 1762.394f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(590.625f, 45.05f, 1688.794f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(572.6483f, 45.05f, 1442.964f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(793.3728f, 45.05f, 1443.247f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(273.8112f, 45.05f, 1440.544f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(590.0533f, 45.05f, 1184.738f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(960.8246f, 45.05f, 1178.66f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(380.7877f, 45.05f, 1166.235f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(583.562f, 45.05f, 881.5002f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(785.9854f, 45.05f, 859.2502f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(404.5685f, 45.05f, 870.8986f));
            firstFloorCheckpoints.Add(checkpoint);
            checkpoint = new Checkpoint(new Vector3(586.2754f, 45.05f, 491.3209f));
            firstFloorCheckpoints.Add(checkpoint);

            CheckPoints.Add(Floor.GroundFloor, groundFloorCheckpoints);
            CheckPoints.Add(Floor.Stairs, stairsCheckpoints);
            CheckPoints.Add(Floor.FirstFloor, firstFloorCheckpoints);
        }
    }
}
