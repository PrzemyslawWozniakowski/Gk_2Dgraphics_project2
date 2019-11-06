using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gk_projekt_2
{
    public class Vec3
    {
        public double x;
        public double y;
        public double z;

        public Vec3(double X, double Y, double Z)
        {
            x = X;
            y = Y;
            z = Z;
        }


        public static Vec3 operator+(Vec3 a, Vec3 b)
        {
            return new Vec3(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static Vec3 operator -(Vec3 a, Vec3 b)
        {
            return new Vec3(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public static Vec3 operator*(double a, Vec3 b)
        {
            return new Vec3(a*b.x, a*b.y, a*b.z);
        }
        public  double ScalarProduct( Vec3 b)
        {
            return x * b.x + y * b.y + z * b.z; 
        }

        public void NormalizeVector()
        {
            double length = Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2);
            length = Math.Sqrt(length);
            x = x / length;
            y = y / length;
            z = z / length;
        }
        public override String ToString()
        {
            return $"({x},{y},{z})";
        }
    }
}
