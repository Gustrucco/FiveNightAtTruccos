﻿using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcSkeletalAnimation;
namespace AlumnoEjemplos.NeneMalloc
{
    public class Order
    {
        public int moveForward = 0;
        public int moveAside = 0; 
        public int rotateY = 0;
        public int rotateX = 0;
        

        //public Vector3 getDirection()
        //{
        //    Vector3 direction = this.movement;
        //    direction.Normalize();
        //    return direction;
        //}
        //public Vector3 getFinalPosition()
        //{
        //    Vector3 lastPos = this.movement+ this.lastCoordinates;
        //    return lastPos;
        //}

        public Boolean moving()
        {
            return moveForward != 0 || moveAside != 0;
        }
        public Boolean running()
        {
            return Math.Abs(moveForward) >= 2;
        }

        public Boolean rotating()
        {
            return rotateX != 0 || rotateY != 0;
        }

    }

    
}
