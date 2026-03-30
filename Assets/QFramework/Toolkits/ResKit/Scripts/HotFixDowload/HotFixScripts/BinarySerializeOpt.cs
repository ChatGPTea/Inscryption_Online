using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace QFramework
{
    public class BinarySerializeOpt
    {
        /// <summary>
        ///     类序列化成xml
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool Xmlserialize(string path, object obj)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (var sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        //XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                        //namespaces.Add(string.Empty, string.Empty);
                        var xs = new XmlSerializer(obj.GetType());
                        xs.Serialize(sw, obj);
                    }
                }

                return true;
                ;
            }
            catch (Exception e)
            {
                Debug.LogError("此类无法转换成xml " + obj.GetType() + "," + e);
            }

            return false;
        }

        /// <summary>
        ///     编辑器使读取xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string path) where T : class
        {
            var t = default(T);
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    var xs = new XmlSerializer(typeof(T));
                    t = (T)xs.Deserialize(fs);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("此xml无法转成二进制: " + path + "," + e);
            }

            return t;
        }

        /// <summary>
        ///     Xml的反序列化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object XmlDeserialize(string path, Type type)
        {
            object obj = null;
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    var xs = new XmlSerializer(type);
                    obj = xs.Deserialize(fs);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("此xml无法转成二进制: " + path + "," + e);
            }

            return obj;
        }


        /// <summary>
        ///     类转换成二进制
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool BinarySerilize(string path, object obj)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    var bf = new BinaryFormatter();
                    bf.Serialize(fs, obj);
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("此类无法转换成二进制 " + obj.GetType() + "," + e);
            }

            return false;
        }
    }
}