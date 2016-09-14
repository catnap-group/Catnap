using UnityEngine;
using System.Collections;
using System.IO;

//Serized class
public class BinaryStream
{
    public static void WriteVector3ToBinary(BinaryWriter bw, Vector3 value)
    {
        WriteFloatToBinary(bw, value.x);
        WriteFloatToBinary(bw, value.y);
        WriteFloatToBinary(bw, value.z);
    }
    public static void WriteVector4ToBinary(BinaryWriter bw, Vector4 value)
    {
        WriteFloatToBinary(bw, value.x);
        WriteFloatToBinary(bw, value.y);
        WriteFloatToBinary(bw, value.z);
        WriteFloatToBinary(bw, value.w);
    }
    public static void WriteQuaternionToBinary(BinaryWriter bw, Quaternion value)
    {
        WriteFloatToBinary(bw, value.x);
        WriteFloatToBinary(bw, value.y);
        WriteFloatToBinary(bw, value.z);
        WriteFloatToBinary(bw, value.w);
    }


//    public static void WriteWTransformToBinary(BinaryWriter bw, WTransform value)
//    {
//        WriteVector3ToBinary(bw, value.position);
//        WriteQuaternionToBinary(bw, value.rotation);
//        WriteVector3ToBinary(bw, value.localScale);
//    }
    public static void WriteTransformToBinary(BinaryWriter bw, Transform value)
    {
        WriteVector3ToBinary(bw, value.position);
        WriteQuaternionToBinary(bw, value.rotation);
        WriteVector3ToBinary(bw, value.localScale);
    }

    public static void WriteFloatToBinary(BinaryWriter bw, float value)
    {
        short data = (short)(value * 100);
        bw.Write(data);
    }
    public static void WriteByteToBinary(BinaryWriter bw, byte value)
    {
        bw.Write(value);
    }

	public static void WriteMemoryStreamToBinary(BinaryWriter bw, MemoryStream value)
	{

		//This gives you the byte array.
		byte[] data = value.ToArray();
		WriteDataToBinary(bw,data);
	}
    public static void WriteIntToBinary(BinaryWriter bw, int value)
    {
        bw.Write(value);
    }
    public static void WriteShortToBinary(BinaryWriter bw, short value)
    {
        bw.Write(value);
    }
    public static void WriteStringToBinary(BinaryWriter bw, string value)
    {
        bw.Write(value);
    }
    public static void WriteDataToBinary(BinaryWriter bw, byte[] value)
    {
		int dataLength = value.Length;
		byte[] data = new byte[dataLength];
		System.Array.Copy(value, data, dataLength);
		bw.Write(dataLength);
		bw.Write(data);
    }

    public static string ParseString(byte[] originalBinary, ref int index)
    {

        int stringLength = originalBinary[index];
        byte[] stringName = new byte[stringLength];
        index += 1;
        System.Array.Copy(originalBinary, index, stringName, 0, stringName.Length);
        string name = System.Text.Encoding.Default.GetString(stringName);
        index = index + stringName.Length;
        return name;
    }
	public static MemoryStream ParseBinaryToMemoryStream(byte[] originalBinary, ref int index)
	{
		byte[] data = ParseData(originalBinary,ref index);
		MemoryStream stream = new MemoryStream(data);
		return stream;
	}
    public static byte[] ParseData(byte[] originalBinary, ref int index )
    {
		int dataLength = ParseInt(originalBinary, ref index);
        byte[] data = new byte[dataLength];

        System.Array.Copy(originalBinary, index, data, 0, dataLength);

        index += dataLength;

        return data;
    }


    public static Vector3 ParseVector3(byte[] originalBinary, ref int index)
    {
        Vector3 destValue = new Vector3();

        destValue.x = ParseFloat(originalBinary, ref index);
        destValue.y = ParseFloat(originalBinary, ref index);
        destValue.z = ParseFloat(originalBinary, ref index);

        return destValue;
    }
    
    public static Vector3 ParseVector4(byte[] originalBinary, ref int index)
    {
        Vector4 destValue = new Vector4();

        destValue.x = ParseFloat(originalBinary, ref index);
        destValue.y = ParseFloat(originalBinary, ref index);
        destValue.z = ParseFloat(originalBinary, ref index);
        destValue.w = ParseFloat(originalBinary, ref index);

        return destValue;
    }

    public static Quaternion ParseQuaternion(byte[] originalBinary, ref int index)
    {
        Quaternion destValue = new Quaternion();

        destValue.x = ParseFloat(originalBinary, ref index);
        destValue.y = ParseFloat(originalBinary, ref index);
        destValue.z = ParseFloat(originalBinary, ref index);
        destValue.w = ParseFloat(originalBinary, ref index);

        return destValue;
    }

    public static float ParseFloat(byte[] originalBinary, ref int index)
    {
        int size = sizeof(short);
        byte[] binaryValue = new byte[size];

        System.Array.Copy(originalBinary, index, binaryValue, 0, binaryValue.Length);

        //short destValue = (short)(System.BitConverter.ToInt16(binaryValue, 0) / 100.0f);


        float destValue = System.BitConverter.ToInt16(binaryValue, 0) / 100.0f;

        index = index + size;

        return destValue;
    }

    public static short ParseShort(byte[] originalBinary, ref int index)
    {
        int size = sizeof(short);
        byte[] binaryValue = new byte[size];

        System.Array.Copy(originalBinary, index, binaryValue, 0, binaryValue.Length);

        index = index + size;

        short destValue = System.BitConverter.ToInt16(binaryValue, 0);
        return destValue;
    }


    public static byte ParseByte(byte[] originalBinary, ref int index)
    {
        int size = sizeof(byte);
        byte[] binaryValue = new byte[size];

        System.Array.Copy(originalBinary, index, binaryValue, 0, binaryValue.Length);

        index = index + size;

        byte destValue = binaryValue[0];
        return destValue;
    }

    public static int ParseInt(byte[] originalBinary, ref int index)
    {
        int size = sizeof(int);
        byte[] binaryValue = new byte[size];

        System.Array.Copy(originalBinary, index, binaryValue, 0, binaryValue.Length);

        index = index + size;

        int destValue = System.BitConverter.ToInt32(binaryValue, 0);
        return destValue;
    }

//    public static WTransform ParseWTransform(byte[] originalBinary, ref int index)
//    {
//        WTransform destValue = new WTransform();
//
//        destValue.position = ParseVector3(originalBinary, ref index);
//        destValue.rotation = ParseQuaternion(originalBinary, ref index);
//        destValue.localScale = ParseVector3(originalBinary, ref index);
//
//        return destValue;
//    }

    public static byte[] GetFileBinaryData(string path)
    {
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        if (fs == null)
            return null;
        BinaryReader br = new BinaryReader(fs);
        if (br == null)
            return null;
        byte[] data = br.ReadBytes((int)fs.Length);
        br.Close();
        fs.Close();
        return data;

    }
    
}