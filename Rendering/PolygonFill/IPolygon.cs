using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rendering.PolygonFill
{
    interface IPolygon
    {
        public IEnumerable<Vector3> Fill();
    }
}
