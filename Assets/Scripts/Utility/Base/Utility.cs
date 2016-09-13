using System;
using System.IO;
using System.IO.Compression;

using System.Text;
using System.Text.RegularExpressions;

using System.Collections;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using System.Security.Cryptography;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

//using ICSharpCode.SharpZipLib.GZip;
//using GSLitJson;
//using NetworkUtils;

public enum ENumbericComparison
{
    EqualTo,
    NotEqualTo,

    GreaterThan,
    GreaterThanOrEqualTo,
    LessThan,
    LessThanThanOrEqualTo,
}

public static class UUtil
{
    static MD5CryptoServiceProvider m_MD5Provider;

    //#if USE_JAVA_MOBAGE
    //static public AndroidJavaClass m_JavaClass = new AndroidJavaClass("com.mobage.android.unity3d.GSUnitySDKActivity");	
    //#endif

    #region Constants
    public const int FullMask = 0xffff;
    #endregion

    #region Enums
    public enum Platform
    {
        Standalone,
        Web,
        FlashPlayer,
        iPhone,
        Android
    }

    public enum E_SHOW_DATE_FLAG
    {
        E_YEAR = 1,
        E_MONTH = 1 << 1,
        E_DAY = 1 << 2,
        E_TIME = 1 << 3,
    }

    public enum E_NETWORK_ENV
    {
        E_NETENV_NONE = 0,
        E_NETENV_GPRS,
        E_NETENV_WIFI
    }

    public enum E_VERSION_COMPARE
    {
        E_FAIL,
        E_LESS,
        E_EQUAL,
        E_ADVANCE
    }
    #endregion

    #region Object
    static public void TryToGC()
    {
        System.GC.Collect();

        Debug.LogWarning("!! -------> TryToGC ");

    }

    static public void UnLoadAndTryToGC()
    {
        Resources.UnloadUnusedAssets();

        System.GC.Collect();

        Debug.LogWarning("!! -------> UnLoadAndTryToGC");

    }

    static public GameObject Instantiate(GameObject original, Transform parent)
    {
        if (original != null)
            return Instantiate(original, parent, Vector3.zero, Quaternion.identity, Vector3.one);

        return null;
    }

    static public GameObject Instantiate(GameObject original, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
    {
        if (original == null)
            return null;

        GameObject obj = GameObject.Instantiate(original, localPosition, localRotation) as GameObject;

        obj.transform.parent = parent;
        obj.transform.localPosition = localPosition;
        obj.transform.localRotation = localRotation;
        obj.transform.localScale = localScale;

        return obj;
    }

    static public bool IsActive(GameObject obj)
    {
        if (obj == null) return false;

        if (obj.activeSelf && obj.activeInHierarchy)
        {
            return true;
        }

        return false;
    }

    //public static string DebugObjectToString(object obj)
    //{
    //    if (GSConst.kIsShowLogForce || (GSConst.kDevelopMode != EDevelopMode.eDM_Release))
    //    {
    //        if(obj == null) return string.Empty;

    //        StringBuilder sb = new StringBuilder();
    //        sb.Append("[");
    //        if(obj.GetType().GetInterface("IList") != null)
    //        {
    //            foreach(var v in obj as IEnumerable)
    //            {

    //                sb.Append("[").Append(DebugObjectToString(v)).Append("]");
    //            }

    //        }
    //        else
    //        {
    //            foreach(var f in obj.GetType().GetFields())
    //            {
    //                object fObj = f.GetValue(obj) as object;
    //                string str ;
    //                if(f.FieldType.GetInterface("IList") != null)
    //                {
    //                    str = DebugObjectToString(fObj);
    //                }
    //                else
    //                {
    //                    str = fObj != null ? fObj.ToString() : "null";
    //                }
    //                sb.Append(f.Name).Append(":").Append(str).Append("||");
    //            }
    //        }
    //        sb.Append("]");
    //        return sb.ToString();
    //    }
    //    else
    //    {
    //        return string.Empty;
    //    }
    //}

    static public string GetObjectFullPath(GameObject obj)
    {
        StringBuilder sb = new StringBuilder();
        Transform currT = obj.transform;
        sb.Append(currT.name);
        while (currT.parent != null)
        {
            sb.Insert(0, "/");
            currT = currT.parent;
            sb.Insert(0, currT.name);
        }
        return sb.ToString();
    }

    static public GameObject GetObjRoot(string fullpath)
    {
        string[] split = GetSplit(fullpath, "/");

        GameObject rot = GameObject.Find(split[0]) as GameObject;

        return rot;
    }

    static public GameObject GetObjInScene(string fullpath)
    {
        string[] split = GetSplit(fullpath, "/");

        GameObject rot = GameObject.Find(split[0]) as GameObject;

        Transform tmp = rot.transform;

        string last = "";

        for (int i = 1; i < split.Length; i++)
        {
            last += split[i];

            if (i < split.Length - 1)
            {
                last += "/";
            }
        }
        Transform ret = tmp.Find(last);

        if (ret != null)
        {
            return ret.gameObject;
        }
        else
        {
            Debug.LogWarning("===>>>Can't find tmp.Find(last): tmp: " + UUtil.GetAllName(tmp.gameObject) + "   last: " + last);
            return null;
        }
    }

    static public Transform GetParentDisableInScene(string fullpath)
    {
        string[] split = GetSplit(fullpath, "/");

        GameObject rot = GameObject.Find(split[0]) as GameObject;

        if (rot == null)
        {
            return null;
        }
        if (!rot.activeSelf)
        {
            Debug.LogWarning("==>>>!rot.activeSelf  rot: " + rot.name);

            return rot.transform;
        }
        Transform tmp = rot.transform;

        int i = 1;

        string tmpret = split[0];

        while (tmp != null && tmp.gameObject.activeSelf && i < split.Length)
        {
            tmp = tmp.Find(split[i]);

            if (tmp != null && tmp.gameObject.activeSelf)
            {
                tmpret += ("/" + split[i]);
            }
            i++;
        }
        Debug.LogWarning("==>>>GetParentDisableInScene; " + tmpret);
        return tmp;
    }

    public static T DeepCloneSerializable<T>(T source)
    {
        //the class must be masked as serializable
        if (!typeof(T).IsSerializable)
        {
            throw new ArgumentException("The type must be serializable.", "source");
        }

        // Don't serialize a null object, simply return the default for that object
        if (Object.ReferenceEquals(source, null))
        {
            return default(T);
        }

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream();
        using (stream)
        {
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }

    static public T ForceGetComponent<T>(GameObject obj) where T : Component
    {
        T t = obj.GetComponent<T>();
        if (t == null)
        {
            t = obj.AddComponent<T>();
        }
        return t;
    }

    static public void SetLayer(GameObject target, int layerMask, bool recursively = true)
    {
        List<Transform> stack = new List<Transform>();
        stack.Add(target.transform);

        while (stack.Count > 0)
        {
            Transform tr = stack[0];
            stack.RemoveAt(0);
            tr.gameObject.layer = layerMask;

            if (recursively)
            {
                for (int ii = 0; ii < tr.childCount; ++ii)
                {
                    stack.Add(tr.GetChild(ii));
                }
            }
        }
    }

    static public string GetAllName(GameObject obj)
    {
        if (obj == null)
        {
            return "";
        }
        int i = 0;

        string ret = obj.name;

        Transform tmpparent = obj.transform.parent;

        while (tmpparent != null && i < 30)
        {
            ret = tmpparent.name + "/" + ret;

            tmpparent = tmpparent.parent;

            i++;
        }
        return ret;
    }
    #endregion

    #region Time
    static public DateTime ConvertFromUnixTimestamp(double timestamp)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return origin.AddSeconds(timestamp).ToLocalTime();
    }

    //static public double ConvertToUnixTimestamp (DateTime date)
    //{
    //    DateTime origin = new DateTime (1970, 1, 1, 0, 0, 0, 0).ToLocalTime ();
    //    TimeSpan diff = date.ToLocalTime () - origin;
    //    double clientStamp =  Math.Floor (diff.TotalSeconds);
    //    return clientStamp - GSConst.kServerClientTimeOffset;
    //}

    static public TimeSpan GetTime(double sec)
    {
        TimeSpan origin = TimeSpan.FromSeconds(sec);

        return origin;
        //new DateTime (0, 0, 0, 0, 0, 0, 0).ToLocalTime ();
    }

    static public string TimeDisplay(DateTime date, int showFlag, ILocalization localMgr)
    {
        string result = "";

        if ((showFlag & ((int)E_SHOW_DATE_FLAG.E_YEAR)) != 0)
        {
            result += date.Year + localMgr.GetValue("ShowYearStr");
        }
        if ((showFlag & ((int)E_SHOW_DATE_FLAG.E_MONTH)) != 0)
        {
            result += date.Month + localMgr.GetValue("ShowMonthStr");
        }
        if ((showFlag & ((int)E_SHOW_DATE_FLAG.E_DAY)) != 0)
        {
            result += date.Day + localMgr.GetValue("ShowDayStr");
        }
        if ((showFlag & ((int)E_SHOW_DATE_FLAG.E_TIME)) != 0)
        {
            string minute = date.Minute < 10 ? ("0" + date.Minute.ToString()) : date.Minute.ToString();
            result += " " + date.Hour + ":" + minute;
        }

        return result;
    }

    public interface ILocalization
    {
        string GetValue(string key);
    }

    static public string TimeDisplay(double timestamp, int showFlag, ILocalization localMgr)
    {
        DateTime date = ConvertFromUnixTimestamp(timestamp);
        return TimeDisplay(date, showFlag, localMgr);
    }



    public static string FormatTime(DateTime time)
    {
        //if ((int)DateTime.Now.Subtract(time).TotalDays > 0)
        //{
        //    return (int)DateTime.Now.Subtract(time).TotalDays + Localization.Get("DaysAgo");
        //}
        //else if ((int)DateTime.Now.Subtract(time).TotalHours > 0)
        //{
        //    return (int)DateTime.Now.Subtract(time).TotalHours + Localization.Get("HoursAgo");
        //}
        //else if ((int)DateTime.Now.Subtract(time).TotalMinutes >= 0)
        //{
        //    return (int)DateTime.Now.Subtract(time).TotalMinutes + Localization.Get("MinutesAgo");
        //}
        return string.Empty;
    }

    #endregion

    #region Debug
    public static void DebugBreak()
    {
        if (GetIsShowLog())
        {
            Debug.LogError("here have a break");
            Debug.Break();
        }
    }
    #endregion

    #region Compression
    /*
	    * Create containing directories if not exist
	    * path: path to file not directory
	    */
    /*
	static public void MakeDirectoryFor(string filePath)
	{
		string dir = Path.GetDirectoryName(filePath);
		if( !Directory.Exists(dir) )
		{
			Directory.CreateDirectory(dir);
		}
	}

	static public bool ZipFile(string inputPath, string outputPath)
	{
		int count = 0;
		byte[] buffer = new byte[4096];

		try
		{
			MakeDirectoryFor( outputPath );

			using( GZipOutputStream outputStream = new GZipOutputStream( new FileStream(outputPath, FileMode.OpenOrCreate) ) )
			{
				using( BinaryReader inputStream = new BinaryReader(new FileStream(inputPath, FileMode.Open)) )
				{
					while((count = inputStream.Read(buffer, 0, buffer.Length)) != 0)
					{
						outputStream.Write(buffer, 0, count);
					}
					
					inputStream.Close();
					outputStream.Close();
				}
			}
		}
		catch(Exception e)
		{
			Debug.LogError(e.ToString());
			Debug.LogError(Utility.ConcatString("Failed to zip the file, input: ", inputPath, ", output: ", outputPath));
			return false;
		}

		return true;
	}

	static public bool UnzipFile(string inputPath, string outputPath)
	{
		int count = 0;
		byte[] buffer = new byte[4096];

		try
		{
			MakeDirectoryFor( outputPath );

			using( GZipInputStream inputStream = new GZipInputStream( new FileStream(inputPath, FileMode.Open) ) )
			{
				using( BinaryWriter outputStream = new BinaryWriter(new FileStream(outputPath, FileMode.OpenOrCreate)) )
				{
					while((count = inputStream.Read(buffer, 0, buffer.Length)) != 0)
					{
						outputStream.Write(buffer, 0, count);
					}
					
					inputStream.Close();
					outputStream.Close();
				}
			}
		}
		catch(Exception e)
		{
			Debug.LogError(e.ToString());
			Debug.LogError(Utility.ConcatString("Failed to unzip the file, input: ", inputPath, ", output: ", outputPath));
			return false;
		}

		return true;
	}
	
	static public string UnzipString (byte[] data )
	{		
		GZipInputStream gzi = new GZipInputStream(new MemoryStream(data));
		MemoryStream re = new MemoryStream();
		int count=0;
		
		byte[] buffer = new byte[ 4096 ];
		
		while ((count = gzi.Read(buffer, 0, 4096)) != 0)
		{            
			re.Write(buffer,0,count);
		}
		byte[] depress = re.ToArray();
		gzi.Close();
		re.Close();
		
		return Encoding.UTF8.GetString( depress );
	}*/
    #endregion

    #region Algebra & Geometry
    public static int GetDigit(uint number, int digit)
    {
        if (number < 0 || digit <= 0)
            return 0;

        string strNum = System.Convert.ToString(number);
        if (strNum.Length < digit)
            return 0;

        return (int)Char.GetNumericValue(strNum[digit - 1]);
    }

    static public bool IsZero(float value)
    {
        return (Mathf.Abs(value) <= float.Epsilon) ? true : false;
    }

    // Calculates reflection matrix around the given plane
    public static Matrix4x4 CalculateReflectionMatrix(Vector4 plane)
    {
        Matrix4x4 reflectionMat = new Matrix4x4();

        reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2F * plane[3] * plane[0]);

        reflectionMat.m10 = (-2F * plane[1] * plane[0]);
        reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2F * plane[3] * plane[1]);

        reflectionMat.m20 = (-2F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2F * plane[2] * plane[1]);
        reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2F * plane[3] * plane[2]);

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;

        return reflectionMat;
    }

    public static float GetFractional(float number)
    {
        decimal dec = (decimal)number;
        return (float)(dec - System.Math.Truncate(dec));
    }

    public static bool Compare(int value1, int value2, ENumbericComparison comparison)
    {
        switch (comparison)
        {
            case ENumbericComparison.EqualTo:
                return (value2 == value1) ? true : false;
            case ENumbericComparison.NotEqualTo:
                return (value2 != value1) ? true : false;
            case ENumbericComparison.GreaterThan:
                return (value2 < value1) ? true : false;
            case ENumbericComparison.GreaterThanOrEqualTo:
                return (value2 <= value1) ? true : false;
            case ENumbericComparison.LessThan:
                return (value2 > value1) ? true : false;
            case ENumbericComparison.LessThanThanOrEqualTo:
                return (value2 >= value1) ? true : false;
        }

        return false;
    }

    public static bool IsOccured(float probability)
    {
        return UnityEngine.Random.Range(0.0f, 1.0f) <= probability ? true : false;
    }

    public static Quaternion GetRandomQuaternion()
    {
        return Quaternion.Euler(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f);
    }
    #endregion

    #region String
    public static string ConcatString(params string[] substrings)
    {
        if (substrings.Length == 0)
            return string.Empty;

        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < substrings.Length; ++i)
            builder.Append(substrings[i]);

        return builder.ToString();
    }

    public static StringBuilder BuildString(StringBuilder builder, params string[] substrings)
    {
        if (builder == null)
            builder = new StringBuilder();

        for (int i = 0; i < substrings.Length; ++i)
            builder.Append(substrings[i]);

        return builder;
    }

    public static string GetFullPath(string subpath)
    {
        return ConcatString(Application.dataPath, subpath);
    }

    public static string GetPersistantFullPath(string subpath)
    {
        return ConcatString(Application.persistentDataPath, subpath);
    }

    public static string GetAssetDatabaseFullPath(string subpath)
    {
        return ConcatString("Assets", subpath);
    }

    public static string StripSubstring(string str, string substr)
    {
        if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(substr))
            return str;

        var index = str.IndexOf(substr);
        if (index != -1)
        {
            str = str.Remove(index, substr.Length);
        }

        return str;
    }

    public static string CombinePath(string subpath1, string subpath2)
    {
        if (string.IsNullOrEmpty(subpath1))
            return subpath2;

        if (string.IsNullOrEmpty(subpath2))
            return subpath1;

        return Path.Combine(subpath1, subpath2.TrimStart('/'));
    }

    //Eg. 0.1 = 10%
    static public string GetPercentString(float value)
    {
        return ((decimal)value).ToString("##0.###%");
    }

    static public string[] GetSplit(string desc, string splitchar)
    {
        string[] splitor = new string[] { splitchar };
        string[] after = desc.Split(splitor, StringSplitOptions.RemoveEmptyEntries);
        return after;
    }

    static public string GetLastString(string res, char[] m_spiltwords)
    {
        string words = res;

        string[] split = words.Split(m_spiltwords);

        if (split.Length > 0)
        {
            return split[split.Length - 1];
        }
        return "";

    }

    public static string TransformStringWithoutEmoticonsAndColors(string input_string)
    {
        return input_string;

        //if (input_string == null) return "";

        //string output_string = "";

        //int offset = 0;
        //int textLength = input_string.Length;
        //for ( ; offset < textLength; offset++)
        //{
        //    char ch = input_string[offset];
        //    if (ch == '\\')
        //    {
        //        if (offset + 1 < textLength)
        //        {
        //            if (input_string[offset + 1] == 'n')
        //            {
        //                offset += 1;
        //                continue;
        //            }
        //        }
        //    }
        //    if (ch == '[')
        //    {
        //        // disgard [-]
        //        if (offset + 2 < textLength)
        //        {
        //            if (input_string[offset + 1] == '-' && input_string[offset + 2] == ']')
        //            {
        //                offset += 2;
        //                continue;
        //            }
        //            else if (offset + 7 < textLength && input_string[offset + 7] == ']')
        //            {
        //                if (NGUITools.EncodeColor(NGUITools.ParseColor(input_string, offset + 1)) == input_string.Substring(offset + 1, 6).ToUpper())
        //                {
        //                    offset += 7;
        //                    continue;
        //                }
        //            }
        //        }
        //    }

        //    output_string += ch;
        //}

        //return output_string;
    }

    public static void TrimUtf8BOM(ref string str)
    {
        string _ByteOrderUtf8 = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetPreamble());
        if (str.StartsWith(_ByteOrderUtf8))
        {
            str.Remove(0, _ByteOrderUtf8.Length);
        }
    }

    private static Regex m_rxLegalChara = new Regex(@"^[a-zA-Z0-9]+$");

    public static bool InputStringChk(string str)
    {
        if (m_rxLegalChara.IsMatch(str))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private static Regex m_rxPlayerID = new Regex(@"^.{4}$");

    public static bool InputPlayerIDChk(string str)
    {
        if (m_rxPlayerID.IsMatch(str))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static string EncodeNonAsciiCharacters(string str)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in str)
        {
            if (c > 127)
            {
                // This character is too big for ASCII
                string encodedValue = "\\u" + ((int)c).ToString("X4");
                sb.Append(encodedValue);
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
    #endregion

    #region Platform
    public static string GetPlatformString(Platform platform)
    {
        return platform.ToString();
    }

    static public long GetStorageFreeSize()
    {
        long freeSize = -1;

#if USE_JAVA_MOBAGE
		if(!Application.isEditor)
		{
			AndroidJavaObject obj = m_JavaClass.GetStatic<AndroidJavaObject>("m_pInstance");
			freeSize = obj.Call<long>("getStorageFreeSize");
		}
#elif UNITY_IPHONE
		freeSize = 0;//IOSUtilities.CheckFreespace();
#endif

        Debug.LogWarning("==== KKK  MEMORYCARD FREE SIZE = " + freeSize.ToString());

        return freeSize;
    }
    #endregion

    #region Network
    static public E_NETWORK_ENV GetNetworkEnvironment()
    {

        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                return E_NETWORK_ENV.E_NETENV_NONE;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                return E_NETWORK_ENV.E_NETENV_GPRS;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                return E_NETWORK_ENV.E_NETENV_WIFI;

        }


        //		Debug.LogWarning("==== KKK  NETWORK ENVIRONMENT = " + (E_NETWORK_ENV)netEnv);
        //		
        return E_NETWORK_ENV.E_NETENV_NONE;
    }
    #endregion

    #region System
    public static void QuitGame()
    {
        Application.Quit();
    }
    #endregion

    #region IO
    static public void MakeFolder(string filePath)
    {
        string dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    static public void DeleteFiles(string folderPath)
    {
        foreach (var filePath in Directory.GetFiles(folderPath, "*.*",
            SearchOption.AllDirectories))
        {
            File.Delete(filePath);
        }

        Array.ForEach(Directory.GetDirectories(folderPath), Directory.Delete);
    }

    static public void CopyFiles(string srcFolderPath, string destFolderPath, string searchPattern = "*.*", string extension = "")
    {
        foreach (string dirPath in Directory.GetDirectories(srcFolderPath, "*",
            SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(srcFolderPath, destFolderPath));

        foreach (string filePath in Directory.GetFiles(srcFolderPath, searchPattern,
            SearchOption.AllDirectories))
        {
            File.Copy(filePath, filePath.Replace(srcFolderPath, destFolderPath) + extension, true);
        }
    }

    static public void CopyDirectries(string srcFolderPath, string destFolderPath)
    {
        foreach (string dirPath in Directory.GetDirectories(srcFolderPath, "*",
            SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(srcFolderPath, destFolderPath));
    }

    static public void RemoveDirAndFileAttributes(string folderPath, FileAttributes attributes, string searchPattern = "*.*")
    {
        FileAttributes fileAttributes;
        DirectoryInfo info = new DirectoryInfo(folderPath);
        foreach (var dir in info.GetDirectories("*", SearchOption.AllDirectories))
        {
            dir.Attributes = dir.Attributes & ~attributes;
        }

        foreach (string filePath in Directory.GetFiles(folderPath, searchPattern, SearchOption.AllDirectories))
        {
            fileAttributes = File.GetAttributes(filePath);
            File.SetAttributes(filePath, fileAttributes & ~attributes);
        }
    }

    static public bool FolderExists(string FolderPath)
    {
        return System.IO.Directory.Exists(FolderPath);
    }

    static public bool FileExists(string FilePath)
    {
        return System.IO.File.Exists(FilePath);
    }

    static public string GetFileName(string FilePath)
    {
        return System.IO.Path.GetFileName(FilePath);
    }

    static public string GetFolder(string FilePath)
    {
        return System.IO.Path.GetDirectoryName(FilePath);
    }

    static public bool IsSubFolder(string FolderPath, string ParentPath)
    {
        System.IO.DirectoryInfo ParentFolder = new System.IO.DirectoryInfo(ParentPath);
        System.IO.DirectoryInfo Folder = new System.IO.DirectoryInfo(FolderPath);

        while (Folder.Parent != null)
        {
            if (string.Compare(Folder.Parent.FullName, ParentFolder.FullName, true) == 0)
                return true;
            else
                Folder = Folder.Parent;
        }

        return false;
    }

    //FolderName is case sensitive
    static public string[] GetFolders(string RelativePath, string FolderName)
    {
        return System.IO.Directory.GetDirectories(RelativePath, FolderName, System.IO.SearchOption.AllDirectories);
    }

    /*
        * Check if the file exists in the specified folders 
        * */
    public static bool FileExistsInFolders(string FilePath, string[] FolderPaths)
    {
        if (FilePath == string.Empty || FolderPaths.Length == 0)
            return false;

        string Folder = GetFolder(FilePath);
        foreach (string FolderPath in FolderPaths)
        {
            if (IsSubFolder(Folder, FolderPath))
                return true;
        }

        return false;
    }

    public static string[] GetAllFiles(string FolderPath, string SearchPattern)
    {
        List<string> Files = new List<string>();

        foreach (string File in System.IO.Directory.GetFiles(FolderPath, SearchPattern))
        {
            Files.Add(File);
        }
        foreach (string Folder in System.IO.Directory.GetDirectories(FolderPath))
        {
            Files.AddRange(GetAllFiles(Folder, SearchPattern));
        }

        return Files.ToArray();
    }

    public static string GetPathWithoutExt(string path)
    {
        if (string.IsNullOrEmpty(Path.GetDirectoryName(path)))
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        return Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path);
    }

    public static string Md5Sum(string strToEncrypt)
    {
        Debug.Log(">>> before MD5  string " + strToEncrypt);
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        Debug.Log(">>> after  MD5 " + hashString);

        return hashString.PadLeft(32, '0');
    }

    public static string Md5SumBytes(byte[] data)
    {
        if (data == null)
            return string.Empty;

        if (m_MD5Provider == null)
            m_MD5Provider = new MD5CryptoServiceProvider();

        var hashBytes = m_MD5Provider.ComputeHash(data);
        return ConvertByteArray(ref hashBytes);
    }

    //public static string Md5SumObj(object obj)
    //{
    //    // encrypt bytes
    //    System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

    //    using(System.IO.MemoryStream fs = new System.IO.MemoryStream())
    //    {
    //        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
    //        formatter.Serialize(fs, obj);

    //        byte[] hashBytes = md5.ComputeHash(fs);
    //        string HashCode = ConvertByteArray(ref hashBytes);

    //        return HashCode;
    //    }

    //    return string.Empty;
    //}

    //	public static byte[] Md5Sum(Object Obj)
    //	{
    //		System.IO.MemoryStream fs = new System.IO.MemoryStream();
    //		System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
    //		formatter.Serialize(fs, Obj);
    //
    ////		byte[] bytes = fs.ToArray();
    ////		fs.Close ();
    //
    //		// encrypt bytes
    //		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
    //		//byte[] hashBytes = md5.ComputeHash(bytes);
    //		byte[] hashBytes = md5.ComputeHash (fs);
    //		string HashCode = ConvertByteArray (ref hashBytes);
    //
    //		return hashBytes;
    //	}
    //

    public static string Md5SumFile(string filePath)
    {
        if (m_MD5Provider == null)
            m_MD5Provider = new MD5CryptoServiceProvider();

        //read file
        using (FileStream file = File.OpenRead(filePath))
        {
            byte[] hash = m_MD5Provider.ComputeHash(file);
            return ConvertByteArray(ref hash);
        }
    }

    public static string ConvertByteArray(ref byte[] array)
    {
        string Str = "";

        int i;
        for (i = 0; i < array.Length; i++)
        {
            Str += System.String.Format("{0:X2}", array[i]);
            //if ((i % 4) == 3)
            //    Str += " ";
        }

        return Str;
    }
    #endregion

    #region Misc
    public static int MakeFilter(int Flag, int Filter = 0)
    {
        return Filter | (1 << Flag);
    }

    public static bool CheckFilter(int Filter, int Flag)
    {
        return (Filter & (1 << Flag)) != 0 ? true : false;
    }

    public static bool IsEventHandlerRegistered(Delegate handler, Delegate prospectiveHandler)
    {
        if (handler != null)
        {
            foreach (Delegate existingHandler in handler.GetInvocationList())
            {
                if (existingHandler == prospectiveHandler)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static E_VERSION_COMPARE VersionCompare(string v1, string v2)
    {
        if (null == v1 || null == v2)
            return E_VERSION_COMPARE.E_FAIL;

        string[] v1_splits = v1.Split('.');
        string[] v2_splits = v2.Split('.');

        if (v1_splits.Length <= 0 || v2_splits.Length <= 0)
            return E_VERSION_COMPARE.E_FAIL;

        for (int i = 0; i < v1_splits.Length; i++)
        {
            if (i >= v2_splits.Length)
                return E_VERSION_COMPARE.E_ADVANCE;
            try
            {
                int t1 = Convert.ToInt32(v1_splits[i]);
                int t2 = Convert.ToInt32(v2_splits[i]);
                if (t1 > t2)
                {
                    return E_VERSION_COMPARE.E_ADVANCE;
                }
                else if (t1 < t2)
                {
                    return E_VERSION_COMPARE.E_LESS;
                }
                else
                {
                    continue;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("######### Version Compare Faild v1: " + v1 + "  v2:" + v2 + " " + e.Message.ToString());
                return E_VERSION_COMPARE.E_FAIL;
            }
        }
        return E_VERSION_COMPARE.E_EQUAL;
    }


    static public void AddValueToDicList<K, V, C>(ref Dictionary<K, C> dic, K key, V oneValue) where C : ICollection<V>, new()
    {
        C list;
        if (!dic.TryGetValue(key, out list))
        {
            list = new C();
            dic.Add(key, list);
        }
        list.Add(oneValue);
    }

    public static bool GetIsShowLog()
    {
        //GSEspecialSetingForPlayer set = Singlton.getInstance<GSEspecialSetingForPlayer>();

        //if( set.GetSpeSetting(GSEspecialSetingForPlayer.BoolType.ShowLog))
        //{
        //    return true;
        //}
        //if (GSConst.kIsShowLogForce)
        //{
        //    return true;
        //}
        //if(GSConst.kIsNotShowLogForce)
        //{
        //    return false;
        //}
        //if(GSConst.kDevelopMode != EDevelopMode.eDM_Release)
        //{
        //    return true;
        //}
        //return false;
        return true;
    }

    //public static IEnumerable<TSource> MyOrderBy<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    //{
    //    List<TSource> newList = source.ToList();
    //    newList.Sort((x, y) => (keySelector(x) as IComparable).CompareTo(keySelector(y)));

    //    return newList;
    //}

    //public static IEnumerable<TSource> MyOrderByDescending<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    //{
    //    List<TSource> newList = source.ToList();
    //    newList.Sort((x, y) => (keySelector(y) as IComparable).CompareTo(keySelector(x)));

    //    return newList;
    //}

    public static void CompareAndAssign(ref int intVal, int newIntVal, bool isAlwaysAssign = false)
    {
        if (isAlwaysAssign || newIntVal > intVal)
            intVal = newIntVal;
    }
    #endregion

    #region Json
    public static object JsonToDataObject(Type classType, string strData)
    {
        //LitJsonUtil.JsonMapper.ClearErrorStack();

		object data = JsonConvert.DeserializeObject(strData,classType );
		if (data == null)
		{
			Debug.LogWarning("ToDataObject error:" + System.Environment.NewLine +"json deserialize error");
		}
        return data;
    }
//
//    public static T JsonToDataObject<T>(string strData)
//    {
//        return (T)JsonToDataObject(typeof(T), strData);
//    }
    #endregion

    #region Json temp
    //static public T  LoadJson<T>(string filepathandname)
    //{
    //    T ret = default(T);

    //    TextAsset tex = Resources.Load(filepathandname)as TextAsset;

    //    object runtimeClass = JsonMapperExtend.ToObject( typeof(T), tex.text );

    //    if(runtimeClass!=null)
    //    {
    //        ret = (T)runtimeClass;
    //    }
    //    return ret;
    //}

    //static public void  SaveJson( object obj,string filepathandname)
    //{
    //    Debug.LogWarning("========> SaveJson:   "+filepathandname);

    //    // ELIMINATE WARNING
    //    //string levelnameLower = Application.loadedLevelName.ToLower();

    //    if(Application.isEditor)
    //    {
    //        string file = "./" + filepathandname;

    //        if (File.Exists(file) )
    //        {
    //            File.Delete (file);
    //        }
    //        else
    //        {
    //            if(!Directory.Exists(Path.GetDirectoryName(file)))
    //            {
    //                Debug.Log("Create Directory " + Path.GetDirectoryName(file));
    //                Directory.CreateDirectory(Path.GetDirectoryName(file));	
    //            }

    //        }

    //        System.IO.TextWriter writer = new System.IO.StreamWriter(file, false);

    //        JsonWriter jw = new JsonWriter( writer as System.IO.TextWriter );

    //        jw.PrettyPrint = true;

    //        JsonMapper.ToJson( obj, jw );

    //        writer.Close();
    //    }
    //}
    #endregion
}

// <summary>
/// Extension methods for streams.
/// </summary>
public static class UStreamExtensions
{
    /// <summary>
    /// Reads all the bytes from the current stream and writes them to the destination stream.
    /// </summary>
    /// <param name="original">The current stream.</param>
    /// <param name="destination">The stream that will contain the contents of the current stream.</param>
    /// <exception cref="System.ArgumentNullException">Destination is null.</exception>
    /// <exception cref="System.NotSupportedException">The current stream does not support reading.-or-destination does not support Writing.</exception>
    /// <exception cref="System.ObjectDisposedException">Either the current stream or destination were closed before the System.IO.Stream.CopyTo(System.IO.Stream) method was called.</exception>
    /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
    public static void CopyTo(this Stream original, Stream destination)
    {
        if (destination == null)
        {
            throw new ArgumentNullException("destination");
        }
        if (!original.CanRead && !original.CanWrite)
        {
            throw new ObjectDisposedException("ObjectDisposedException");
        }
        if (!destination.CanRead && !destination.CanWrite)
        {
            throw new ObjectDisposedException("ObjectDisposedException");
        }
        if (!original.CanRead)
        {
            throw new NotSupportedException("NotSupportedException source");
        }
        if (!destination.CanWrite)
        {
            throw new NotSupportedException("NotSupportedException destination");
        }

        byte[] array = new byte[4096];
        int count;
        while ((count = original.Read(array, 0, array.Length)) != 0)
        {
            destination.Write(array, 0, count);
        }
    }
}

public static class EColorCode
{
    static public readonly string cBlack = "[000000]";
    static public readonly string cWhite = "[ffffff]";
    static public readonly string cRed = "[ff0000]";
    static public readonly string cGreen = "[00ff00]";
    static public readonly string cBlue = "[0000ff]";
    static public readonly string cPostfix = "[-]";
}

public static class UColorUtil
{
    static public string GetColorText(Color _color)
    {
        string ret = "[";

        ret += (_color.r * 255 < 16 ? "0" : "") + ((int)(_color.r * 255)).ToString("x");

        ret += (_color.g * 255 < 16 ? "0" : "") + ((int)(_color.g * 255)).ToString("x");

        ret += (_color.b * 255 < 16 ? "0" : "") + ((int)(_color.b * 255)).ToString("x") + "]";

        return ret;
    }
}

public static class UCameraUtil
{
    static public Camera FindCameraByLayer(int layer)
    {
        Camera[] cameras = Camera.allCameras;

        for (int i = 0; i < cameras.Length; ++i)
        {
            if (cameras[i].gameObject.layer == layer)
                return cameras[i];
        }

        return null;
    }
}

public static class UPlatformUtil
{
    public static bool isEditor
    {
        get { return Application.isEditor; }
    }

    public static bool isDesktopPlatform
    {
        get { return (!Application.isMobilePlatform && !Application.isWebPlayer && !Application.isConsolePlatform); }
    }

    public static bool isMobilePlatform
    {
        get { return Application.isMobilePlatform; }
    }

    public static bool isWebPlatform
    {
        get { return Application.isWebPlayer; }
    }
}

public static class UZipUtil
{
    static public string UnzipMemoryData(byte[] data)
    {
        //byte[] unzippedData = null;

        //try
        //{
        //    using (GZipInputStream inputStream = new GZipInputStream(new MemoryStream(data)))
        //    {
        //        using (MemoryStream unzippedStream = new MemoryStream())
        //        {
        //            int count = 0;
        //            byte[] buffer = new byte[4096];

        //            while ((count = inputStream.Read(buffer, 0, buffer.Length)) != 0)
        //            {
        //                unzippedStream.Write(buffer, 0, count);
        //            }

        //            unzippedData = unzippedStream.ToArray();
        //        }
        //    }
        //}
        //catch (Exception e)
        //{
        //    Debug.LogError(e.ToString());
        //    Debug.LogError(UUtil.ConcatString("Failed to unzip string"));
        //}

        //if (unzippedData != null)
        //    return Encoding.UTF8.GetString(unzippedData);

        return null;
    }

    static public bool ZipFile(string inputPath, string outputPath)
    {
        return false;
        //int count = 0;
        //byte[] buffer = new byte[4096];

        //try
        //{
        //    UUtil.MakeFolder(outputPath);

        //    using (GZipOutputStream outputStream = new GZipOutputStream(new FileStream(outputPath, FileMode.OpenOrCreate)))
        //    {
        //        using (BinaryReader inputStream = new BinaryReader(new FileStream(inputPath, FileMode.Open)))
        //        {
        //            while ((count = inputStream.Read(buffer, 0, buffer.Length)) != 0)
        //            {
        //                outputStream.Write(buffer, 0, count);
        //            }

        //            inputStream.Close();
        //            outputStream.Close();
        //        }
        //    }
        //}
        //catch (Exception e)
        //{
        //    Debug.LogError(e.ToString());
        //    Debug.LogError(UUtil.ConcatString("Failed to zip the file, input: ", inputPath, ", output: ", outputPath));
        //    return false;
        //}

        //return true;
    }

    static public bool UnzipFile(string inputPath, string outputPath)
    {
        return false;
        //int count = 0;
        //byte[] buffer = new byte[4096];

        //try
        //{
        //    UUtil.MakeFolder(outputPath);

        //    using (GZipInputStream inputStream = new GZipInputStream(new FileStream(inputPath, FileMode.Open)))
        //    {
        //        using (BinaryWriter outputStream = new BinaryWriter(new FileStream(outputPath, FileMode.OpenOrCreate)))
        //        {
        //            while ((count = inputStream.Read(buffer, 0, buffer.Length)) != 0)
        //            {
        //                outputStream.Write(buffer, 0, count);
        //            }

        //            inputStream.Close();
        //            outputStream.Close();
        //        }
        //    }
        //}
        //catch (Exception e)
        //{
        //    Debug.LogError(e.ToString());
        //    Debug.LogError(UUtil.ConcatString("Failed to unzip the file, input: ", inputPath, ", output: ", outputPath));
        //    return false;
        //}

        //return true;
    }


}
