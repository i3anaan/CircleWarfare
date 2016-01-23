using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


public class Utils
{
	public static byte[] ObjectToBytes(object o)
	{
		MemoryStream stream = new MemoryStream();
		IFormatter formatter = new BinaryFormatter();
		formatter.Serialize(stream, o);
		return stream.ToArray();
	}

	public static object BytesToObject(byte[] bytes)
	{
		MemoryStream stream = new MemoryStream (bytes);
		IFormatter formatter = new BinaryFormatter();
		stream.Seek(0, SeekOrigin.Begin);
		object o = formatter.Deserialize(stream);
		return o;
	}

	public static byte[] StringToBytes(string str) {
		return System.Text.Encoding.UTF8.GetBytes(str);
	}

	public static string BytesToString(byte[] bytes, int start, int length) {
		return System.Text.Encoding.UTF8.GetString (bytes, start, length);
	}

	public static byte[] ConcatBytes(byte arr1, byte arr2) {
		return ConcatBytes (new byte[]{ arr1 }, new byte[]{ arr2 });
	}

	public static byte[] ConcatBytes(byte arr1, byte[] arr2) {
		return ConcatBytes (new byte[]{ arr1 }, arr2);
	}

	public static byte[] ConcatBytes(byte[] arr1, byte[] arr2) {
		byte[] z = new byte[arr1.Length + arr2.Length];
		arr1.CopyTo(z, 0);
		arr2.CopyTo(z, arr1.Length);
		return z;
	}

	public static T[] SubArray<T>(T[] data, int index, int length)	{
		T[] result = new T[length];
		Array.Copy(data, index, result, 0, length);
		return result;
	}
}

