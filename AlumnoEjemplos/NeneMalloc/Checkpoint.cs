﻿using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.NeneMalloc.Utils;

namespace AlumnoEjemplos.NeneMalloc
{
    class Checkpoint
    {
        public Boolean Checked {get;set;}
        public HashSet<Checkpoint> Neighbors {get;set;}
        public Vector3 Position { get; set;}
        public TgcArrow Arrow { get; set;}
        public TgcBox Point { get; set; }

        public static int idCounter= 0;

        public int id;

        public static Vector3 DEFAULT_UP = new Vector3(0f,1f,0f);

        public Checkpoint(Vector3 aPosition)
        {
            this.Position = aPosition;
            this.Checked = false;
            this.id = idCounter;
            idCounter ++;
                 
            this.Arrow = new TgcArrow();

            
            this.Arrow.BodyColor = Color.Blue;
            this.Arrow.HeadColor = Color.Yellow;
            this.Arrow.Thickness = 0.4f;
            this.Arrow.HeadSize = new Vector2(2, 5);
            this.Arrow.PStart = aPosition;
            this.Arrow.PEnd = aPosition + new Vector3(0, 15, 0);
            this.Arrow.updateValues();

            this.Point =  TgcBox.fromSize(new Vector3(4, 4, 4), Color.Red);
            this.Point.Position = this.Position;

        }

        public bool hasDirectSightWith(Checkpoint aCheckPoint)
        {
            TgcRay rayCast = new TgcRay();
            rayCast.Origin = this.Position;
            rayCast.Direction = aCheckPoint.Position - this.Position;
            float distance = Vector3.Length(aCheckPoint.Position - this.Position);
            TgcBoundingBox boundingBox= new TgcBoundingBox();
            CollitionManager.getClosestBoundingBox(rayCast, out boundingBox, null);
            return boundingBox == null || (Vector3.Length(CollitionManager.getClosesPointBetween(rayCast, boundingBox) - rayCast.Origin) > distance);

        }

        public void DeleteNeighbor(int NeighborId)
        {
            var neighbor = this.Neighbors.Single(n => n.id == NeighborId);
            this.Neighbors.Remove(neighbor);
            neighbor.Neighbors.Remove(this);
        }

        public void render()
        {
            Arrow.render();
            Point.render();
        }

        public bool CanArriveTo(Checkpoint anotherCheckpoint)
        {
            return this.Neighbors.Contains(anotherCheckpoint) || this.Neighbors.Any(n => n.CanArriveTo(anotherCheckpoint));
        }
    }
}
