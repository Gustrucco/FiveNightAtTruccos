using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.NeneMalloc.Utils
{
    class CollitionManager
    {
        
        public static List<TgcBoundingBox> obstaculos { get; set; }

        public static Boolean detectColision(TgcBoundingBox boundingBox)
        {
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

        public static List<TgcBoundingBox> getColisions(TgcRay ray)
        {
            Vector3 vector = new Vector3();
            return CollitionManager.obstaculos.FindAll(b =>  TgcCollisionUtils.intersectRayAABB(ray, b , out vector));
        }

        public static Boolean isColliding(TgcBoundingBox boundingBox, TgcBoundingBox obstaculo)
        {
            TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(boundingBox, obstaculo);
            return result == TgcCollisionUtils.BoxBoxResult.Adentro || result == TgcCollisionUtils.BoxBoxResult.Atravesando;
        }

        public static Boolean getClosestBoundingBox(TgcRay rayCast, out TgcBoundingBox boundingBoxResult, TgcBoundingBox boundingBox)
        {
            List<TgcBoundingBox> boundingBoxes = getColisions(rayCast);
            boundingBoxes.Remove(boundingBox);
            if (boundingBoxes.Count == 0)
            {
                boundingBoxResult = null;
                return false;
            }
            else
            {
                List<Vector3> vectors = boundingBoxes.ConvertAll(b => { Vector3 vector = new Vector3(); TgcCollisionUtils.intersectRayAABB(rayCast, b, out vector); return vector;});
                boundingBoxResult = boundingBoxes.Find(b => { Vector3 vector = new Vector3(); TgcCollisionUtils.intersectRayAABB(rayCast, b, out vector); return vectors.TrueForAll(v => Vector3.Length(vector - rayCast.Origin) <= Vector3.Length(v - rayCast.Origin)); });
                return true;
            }
        }

        public static Vector3 getClosesPointBetween(TgcRay rayCast, TgcBoundingBox boundingBox)
        {
            Vector3 vector = new Vector3();
            TgcCollisionUtils.intersectRayAABB(rayCast, boundingBox, out vector);
            return vector;
        }
    }
}
