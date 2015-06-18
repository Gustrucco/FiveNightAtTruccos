using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.NeneMalloc.Utils
{
    public class MathUtil
    {

        public static Boolean Equals(float number1, float number2)
        {
            return Equals(number1,number2, Constants.EPSILON_DEFAULT);
        }
        public static Boolean Equals(float number1, float number2, float epsilon)
        {
            return Math.Abs(number1 - number2) <= epsilon;
        }
    }
}
