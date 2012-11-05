using System.IO;

namespace ResEx.Common
{
    public static class SerializationTools
    {
        public static string Serialize<T>(T obj) where T : class 
        {
            if (obj == null)
            {
                return null;
            }

            // the output of the serialization will be written to a memory stream
            MemoryStream memoryStream = new MemoryStream();

            // serialize the object
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            serializer.Serialize(memoryStream, obj);

            // convert memorystream contents to string
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            return enc.GetString(memoryStream.GetBuffer());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Suppressing this rule because I don't care about type inference here more than helping the caller to call without casting.")]
        public static T Deserialize<T>(string serializedObject)
        {
            if (string.IsNullOrEmpty(serializedObject))
            {
                return default(T);
            }

            // convert string to memorystream
            MemoryStream memoryStream = new MemoryStream();
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            byte[] objBytes = enc.GetBytes(serializedObject);
            memoryStream.Write(objBytes, 0, objBytes.Length);
            memoryStream.Position = 0;

            // deserialize to a new instance
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            T newObject = (T)serializer.Deserialize(memoryStream);

            return newObject;
        }
    }
}