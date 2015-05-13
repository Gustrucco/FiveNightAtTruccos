using System;
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
    class Orders
    {
        public Vector3 movement { get; set; }
        public Vector3 lastCoordinates { get; set; }

        public Vector3 getDirection()
        {
            Vector3 direction = this.movement;
            direction.Normalize();
            return direction;
        }
        public Vector3 getFinalPosition()
        {
            Vector3 lastPos = this.movement+ this.lastCoordinates;
            return lastPos;
        }

    }

    
}
