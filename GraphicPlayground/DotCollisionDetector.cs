using System.Collections.Generic;
using System.Drawing;

namespace GraphicPlayground
{
    internal class DotCollisionDetector : IEqualityComparer<IModelDrawer>
    {
        public bool Equals(IModelDrawer x, IModelDrawer y)
        {
            if (x.Equals(y))
                return false;
            var intersect = Rectangle.Intersect(
                new Rectangle((int)x.X - x.Size / 2, (int)x.Y - x.Size / 2, x.Size, x.Size),
                new Rectangle((int)y.X - y.Size / 2, (int)y.Y - y.Size / 2, y.Size, y.Size));
            return intersect.Width + intersect.Height > 0;
        }

        public bool Equals(IModelDrawer x, Rectangle y)
        {
            if (x.Equals(y))
                return false;
            var intersect = Rectangle.Intersect(
                new Rectangle((int)x.X - x.Size / 2, (int)x.Y - x.Size / 2, x.Size, x.Size),
                y);
            return intersect.Width + intersect.Height > 0;
        }

        public int GetHashCode(IModelDrawer obj)
        {
            return obj.GetHashCode();
        }
    }
}