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
    }
}