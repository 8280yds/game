using UnityEngine;

namespace Freamwork
{
    public static class VectorUtil
    {
        /// <summary>
        /// 获取两个二维向量的夹角,夹角范围在-180~180之间
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float Vector2Angle(Vector2 from, Vector2 to)
        {
            Vector3 cross = Vector3.Cross(from, to);
            float angle = Vector2.Angle(from, to);
            return cross.z > 0 ? -angle : angle;
        }

        /// <summary>
        /// 判断两条线是否相交
        /// </summary>
        /// <param name="a">线段1起点坐标</param>
        /// <param name="b">线段1终点坐标</param>
        /// <param name="c">线段2起点坐标</param>
        /// <param name="d">线段2终点坐标</param>
        /// <param name="intersection">相交点坐标</param>
        /// <returns>是否相交 Z轴的值(0:两线平行或共点共线; -1:不平行且未相交; 1:两线相交)</returns>
        public static Vector3 GetIntersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            Vector3 point = Vector3.zero;

            //两线平行或共点共线
            if (a == b || c == d || ((a.y - b.y) * (c.x - d.x) == (a.x - b.x) * (c.y - d.y)))
            {
                return point;
            }

            point.x = ((b.x - a.x) * (c.x - d.x) * (c.y - a.y) - c.x * (b.x - a.x) * (c.y - d.y) +
                a.x * (b.y - a.y) * (c.x - d.x)) / ((b.y - a.y) * (c.x - d.x) - (b.x - a.x) * (c.y - d.y));
            point.y = ((b.y - a.y) * (c.y - d.y) * (c.x - a.x) - c.y * (b.y - a.y) * (c.x - d.x) +
                a.y * (b.x - a.x) * (c.y - d.y)) / ((b.x - a.x) * (c.y - d.y) - (b.y - a.y) * (c.x - d.x));

            if ((double)(point.x - a.x) * (point.x - b.x) <= 0 && (double)(point.x - c.x) * (point.x - d.x) <= 0 &&
                (double)(point.y - a.y) * (point.y - b.y) <= 0 && (double)(point.y - c.y) * (point.y - d.y) <= 0)
            {
                point.z = 1; //'相交
            }
            else
            {
                point.z = -1; //'相交但不在线段上
            }
            return point;
        }

    }
}