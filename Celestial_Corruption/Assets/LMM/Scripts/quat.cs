using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Barracuda;

namespace Quat
{
    public class QuatFuncs
    {

        public static Vector3 cross(Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x);
        }

        public static Quaternion quat_from_cols(Vector3 c0, Vector3 c1, Vector3 c2)
        {
            if (c2.z < 0.0f)
            {
                if (c0.x > c1.y)
                {
                    return quat_normalize(new Quaternion(
                        c1.z - c2.y,
                        1.0f + c0.x - c1.y - c2.z,
                        c0.y + c1.x,
                        c2.x + c0.z));
                }
                else
                {
                    return quat_normalize(new Quaternion(
                        c2.x - c0.z,
                        c0.y + c1.x,
                        1.0f - c0.x + c1.y - c2.z,
                        c1.z + c2.y));
                }
            }
            else
            {
                if (c0.x < -c1.y)
                {
                    return quat_normalize(new Quaternion(
                        c0.y - c1.x,
                        c2.x + c0.z,
                        c1.z + c2.y,
                        1.0f - c0.x - c1.y + c2.z));
                }
                else
                {
                    return quat_normalize(new Quaternion(
                        1.0f + c0.x + c1.y + c2.z,
                        c1.z - c2.y,
                        c2.x - c0.z,
                        c0.y - c1.x));
                }
            }
        }

        public static Quaternion quat_from_xform_xy(Vector3 x, Vector3 y)
        {
            Vector3 c2 = cross(x, y).normalized;
            Vector3 c1 = cross(c2, x).normalized;
            Vector3 c0 = x;
            return quat_from_cols(c0, c1, c2);
        }

        public static Quaternion quat_mul(Quaternion q, Quaternion p)
        {
            return new Quaternion(
            p.w * q.w - p.x * q.x - p.y * q.y - p.z * q.z,
            p.w * q.x + p.x * q.w - p.y * q.z + p.z * q.y,
            p.w * q.y + p.x * q.z + p.y * q.w - p.z * q.x,
            p.w * q.z - p.x * q.y + p.y * q.x + p.z * q.w);
        }

        public static Quaternion quat_exp(Vector3 v, float eps = 1e-8f)
        {
            float halfangle = MathF.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);

            if (halfangle < eps)
            {
                return quat_normalize(new Quaternion(1.0f, v.x, v.y, v.z));
            }
            else
            {
                float c = Mathf.Cos(halfangle);
                float s = Mathf.Sin(halfangle) / halfangle;
                return new Quaternion(c, s * v.x, s * v.y, s * v.z);
            }
        }

        public static Quaternion quat_inv(Quaternion q)
        {
            return new Quaternion(-q.w, q.x, q.y, q.z);
        }

        public static Quaternion quat_from_scaled_angle_axis(Vector3 v, float eps = 1e-8f)
        {
            return quat_exp(v / 2.0f, eps);
        }

        public static Quaternion quat_normalize(Quaternion q, float eps = 1e-8f)
        {

            float magnitude = (quat_length(q) + eps);
            q.x /= magnitude;
            q.y /= magnitude;
            q.z /= magnitude;
            q.w /= magnitude;

            return q;
        }

        public static float quat_length(Quaternion q)
        {
            return Mathf.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z);
        }

        public static Vector3 quat_mul_vec3(Quaternion q, Vector3 v)
        {
            Vector3 t = 2.0f * cross(new Vector3(q.x, q.y, q.z), v);
            return v + q.w * t + cross(new Vector3(q.x, q.y, q.z), t);
        }

    }
}