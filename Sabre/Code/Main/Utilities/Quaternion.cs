using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabre
{
    class Quaternion
    {
        float[] q = new float[4];
        public Quaternion(float s1, float s2, float s3, float s4)
        {
            q[0] = s1;
            q[1] = s2;
            q[2] = s3;
            q[3] = s4;
        }
    }
}
