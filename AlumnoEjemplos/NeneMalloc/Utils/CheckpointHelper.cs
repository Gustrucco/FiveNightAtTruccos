using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.NeneMalloc.Utils
{
    class CheckpointHelper
    {
        public static Dictionary<string,List<Checkpoint>> CheckPoints= new Dictionary<string,List<Checkpoint>>();

        public static void renderAll()
        {
            foreach (KeyValuePair<String,List<Checkpoint>> keyPair in CheckPoints)
            {
                keyPair.Value.ForEach(c => c.render());
            }
        }
        public static void add(Checkpoint checkPoint, String key)
        {
            if (!CheckPoints.ContainsKey(key))
            {
                CheckPoints[key] = new List<Checkpoint>();
            }
            CheckPoints[key].Add(checkPoint);
        }
    }
}
