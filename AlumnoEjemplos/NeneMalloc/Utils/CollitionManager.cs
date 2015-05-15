using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.NeneMalloc.Utils
{
    class CollitionManager
    {
        public static List<TgcBox> obstaculos { get; set; }
       
        public static Boolean detectColision(TgcBoundingBox boundingBox){
            Boolean collide = false;
             foreach (TgcBox obstaculo in CollitionManager.obstaculos)
                {
                    TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(boundingBox, obstaculo.BoundingBox);
                    if (result == TgcCollisionUtils.BoxBoxResult.Adentro || result == TgcCollisionUtils.BoxBoxResult.Atravesando)
                    {
                        collide = true;
                        break;
                    }
                }
             return collide;
        }
    }
}
