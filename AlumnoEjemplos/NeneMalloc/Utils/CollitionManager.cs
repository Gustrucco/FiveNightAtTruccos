using System;
using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.NeneMalloc.Utils
{
    class CollitionManager
    {
        public static List<TgcBoundingBox> obstaculos { get; set; }
       
        public static Boolean detectColision(TgcBoundingBox boundingBox){
            Boolean collide = false;
            foreach (TgcBoundingBox obstaculo in CollitionManager.obstaculos)
            {
                
                TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(boundingBox, obstaculo);
                if (result == TgcCollisionUtils.BoxBoxResult.Adentro || result == TgcCollisionUtils.BoxBoxResult.Atravesando)
                {
                    collide = true;
                    break;
                }
            }
             return collide;
        }
        public static List<TgcBoundingBox> getColisions(TgcBoundingBox boundingBox)
        {
            List<TgcBoundingBox> boundingBoxes = new List<TgcBoundingBox>();
            foreach (TgcBoundingBox obstaculo in CollitionManager.obstaculos)
            {

                TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(boundingBox, obstaculo);
                if (result == TgcCollisionUtils.BoxBoxResult.Adentro || result == TgcCollisionUtils.BoxBoxResult.Atravesando)
                {
                    boundingBoxes.Add(obstaculo);
                }
            }
            return boundingBoxes;
        }

        public static Boolean isColliding(TgcBoundingBox boundingBox, TgcBoundingBox obstaculo)
        {
            TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(boundingBox, obstaculo);
            return result == TgcCollisionUtils.BoxBoxResult.Adentro || result == TgcCollisionUtils.BoxBoxResult.Atravesando;
        }
    }
}
