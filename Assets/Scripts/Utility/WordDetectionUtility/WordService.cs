using UnityEngine;
using System.Collections;
public class WordService : UnitySingletonVisible<WordService>{
	
	public bool ShowUsePlotter = true;//always show for test
	Texture2D m_textureSpectrumLeft = null;
	Texture2D m_textureSpectrumRight = null;
	Texture2D m_textureWave = null;

	public Material MaterialSpectrumLeft = null;
	public Material MaterialSpectrumRight = null;
	public Material MaterialWave = null;
	public SpectrumMicrophone Mic = null;//使用默认话筒，认为手机只有一个话筒


	public int TextureSize = 16;

	protected Color32[] m_colorsSpectrumLeft = null;
	protected Color32[] m_colorsSpectrumRight = null;
	protected Color32[] m_colorsWave = null;

	protected GraphPlotter m_plotter = new GraphPlotter();
	public FFTWindow Window = FFTWindow.BlackmanHarris;//相位差校正信号谐波分析方法，比较准确
	public bool NormalizeGraph = true;
	/// <summary>
	/// Imaginary spectrum
	/// </summary>
	public bool OverrideSpectrumImag = false;
	public float[] SpectrumImag = null;
	/// <summary>
	/// Toggle using plotter
	/// </summary>
	public bool UsePlotter = true;

	private MeshRenderer RendererSpectrumLeft = null;
	private MeshRenderer RendererSpectrumRight = null;
	private MeshRenderer RendererWave = null;

	private GameObject planes;

	public override void OnInit ()
	{
		base.OnInit ();

		Mic = gameObject.AddComponent<SpectrumMicrophone> ();
		Mic.CaptureTime = 1;
		Mic.SampleRate = 1024;

		if (UsePlotter) {
			planes = new GameObject ();
			planes.name = "planes";
			GameObject planeLeft = GameObject.CreatePrimitive (PrimitiveType.Plane);
			planeLeft.transform.position = new Vector3 (5.1f, 0, 10);
			planeLeft.transform.parent = planes.transform;
			planeLeft.layer = LayerMask.NameToLayer ("UI");
			RendererSpectrumLeft = planeLeft.GetComponent<MeshRenderer> ();

			GameObject planeRight = GameObject.CreatePrimitive (PrimitiveType.Plane);
			planeRight.transform.position = new Vector3 (-5.1f, 0, 10);
			planeRight.transform.parent = planes.transform;
			planeRight.layer = LayerMask.NameToLayer ("UI");
			RendererSpectrumRight = planeRight.GetComponent<MeshRenderer> ();

			GameObject planeWave = GameObject.CreatePrimitive (PrimitiveType.Plane);
			planeWave.transform.position = new Vector3 (0, 0, -1);
			planeWave.transform.localScale = new Vector3 (2, 1, 1);
			planeWave.transform.parent = planes.transform;
			planeWave.layer = LayerMask.NameToLayer ("UI");
			RendererWave = planeWave.GetComponent<MeshRenderer> ();

			//planes.AddComponent<Billboard> ();
			planes.transform.localPosition 
			= new Vector3 (1f
				, 3.3f
				, 1f);
			planes.transform.localRotation = Quaternion.Euler (100.30659f, 186.228242f, 185.8563f);
			planes.transform.localScale = new Vector3(0.05f,0.05f,0.05f);
			planes.transform.parent = Camera.main.transform;
			planes.layer = LayerMask.NameToLayer ("UI");
		}

		if (MaterialSpectrumLeft &&
			RendererSpectrumLeft)
		{
			RendererSpectrumLeft.material = (Material)UnityEngine.Object.Instantiate(MaterialSpectrumLeft);
		}

		if (MaterialSpectrumRight &&
			RendererSpectrumRight)
		{
			RendererSpectrumRight.material = (Material)UnityEngine.Object.Instantiate(MaterialSpectrumRight);
		}

		if (MaterialWave &&
			RendererWave)
		{
			RendererWave.material = (Material)UnityEngine.Object.Instantiate(MaterialWave);
		}

		if (null == m_textureSpectrumLeft &&
			RendererSpectrumLeft)
		{
			m_textureSpectrumLeft = new Texture2D(TextureSize, TextureSize, TextureFormat.ARGB32, false);
			m_textureSpectrumLeft.wrapMode = TextureWrapMode.Clamp;
			m_textureSpectrumLeft.filterMode = FilterMode.Point;
			m_textureSpectrumLeft.anisoLevel = 0;
			RendererSpectrumLeft.material.mainTexture = m_textureSpectrumLeft;
			m_colorsSpectrumLeft = m_textureSpectrumLeft.GetPixels32();
		}

		if (null == m_textureSpectrumRight &&
			RendererSpectrumRight)
		{
			m_textureSpectrumRight = new Texture2D(TextureSize, TextureSize, TextureFormat.ARGB32, false);
			m_textureSpectrumRight.wrapMode = TextureWrapMode.Clamp;
			m_textureSpectrumRight.filterMode = FilterMode.Point;
			m_textureSpectrumRight.anisoLevel = 0;
			RendererSpectrumRight.material.mainTexture = m_textureSpectrumRight;
			m_colorsSpectrumRight = m_textureSpectrumRight.GetPixels32();
		}

		if (null == m_textureWave &&
			RendererWave)
		{
			m_textureWave = new Texture2D(TextureSize, TextureSize, TextureFormat.ARGB32, false);
			m_textureWave.wrapMode = TextureWrapMode.Repeat;
			m_textureWave.filterMode = FilterMode.Point;
			m_textureWave.anisoLevel = 0;
			RendererWave.material.mainTexture = m_textureWave;
			m_colorsWave = m_textureWave.GetPixels32();
		}

		m_plotter.TextureSize = TextureSize;

	
	}



	protected float[] m_micData = null;
	protected float[] m_plotData = null;

	protected virtual void GetMicData()
	{
		m_micData = Mic.GetData(0);
	}

	void OnGUI()
	{
		ServiceUpdate ();
		if (RendererSpectrumLeft != null)
		{
			RendererSpectrumLeft.enabled = UsePlotter;
		}

		if (RendererSpectrumRight != null)
		{
			RendererSpectrumRight.enabled = UsePlotter;
		}

		if (RendererWave != null)
		{
			RendererWave.enabled = UsePlotter;
		}
//		if (string.IsNullOrEmpty(Mic.DeviceName))//默认为空就好，不需要选择，为空的话选择默认话筒
//		{
//			GUILayout.Space(150);
//
//			foreach (string device in Microphone.devices)
//			{
//				if (GUILayout.Button(device, GUILayout.Height(60)))
//				{
//					Mic.DeviceName = device;
//				}
//			}
//		}
	}


	protected virtual void ServiceUpdate()
	{
		
			GetMicData();
			if (null == m_micData)
			{
				return;
			}
			if (null == m_plotData ||
				m_plotData.Length != m_micData.Length)
			{
				m_plotData = new float[m_micData.Length];
			}

			PlotWave();//波幅

			PlotSpectrum();//波谱颜色值

		if (UsePlotter && null != m_plotData &&
				m_plotData.Length > 0)
			{
				Vector2 pos = MaterialWave.mainTextureOffset;
				pos.x = Mic.GetPosition();
				RendererWave.material.mainTextureOffset = pos/(float) m_plotData.Length;
			}
	}
	protected virtual void PlotWave()
	{
		if (UsePlotter)
		{
			float min, max;
			m_plotter.PlotGraph(m_micData, m_plotData, m_micData.Length, NormalizeGraph, out min, out max, true,
				m_colorsWave);
			if (NormalizeGraph)
			{
				m_plotter.Min = Mathf.Lerp(m_plotter.Min, min, 0.1f);
				m_plotter.Max = Mathf.Lerp(m_plotter.Max, max, 0.1f);
			}
		}
	}
	protected virtual void PlotSpectrum()
	{
		float[] spectrumReal;
		float[] spectrumImag;
		Mic.GetSpectrumData(Window, out spectrumReal, out spectrumImag);
		if (UsePlotter)
		{
			m_plotter.PlotGraph2(spectrumReal, m_plotData, 0, spectrumReal.Length, NormalizeGraph, m_colorsSpectrumLeft);

			if (OverrideSpectrumImag &&
				null != SpectrumImag)
			{
				m_plotter.PlotGraph2(SpectrumImag, m_plotData, 0, SpectrumImag.Length, NormalizeGraph,
					m_colorsSpectrumRight);
			}
			else
			{
				m_plotter.PlotGraph2(spectrumImag, m_plotData, 0, spectrumImag.Length, NormalizeGraph,
					m_colorsSpectrumRight);
			}

			//float min, max;
			//m_plotter.PlotGraph(spectrumData, m_plotData, spectrumData.Length, NormalizeGraph, out min, out max, false, m_colorsSpectrumRight);

			if (m_textureSpectrumLeft &&
				null != m_colorsSpectrumLeft)
			{
				m_textureSpectrumLeft.SetPixels32(m_colorsSpectrumLeft);
				m_textureSpectrumLeft.Apply();
			}

			if (m_textureSpectrumRight &&
				null != m_colorsSpectrumRight)
			{
				m_textureSpectrumRight.SetPixels32(m_colorsSpectrumRight);
				m_textureSpectrumRight.Apply();
			}

			if (m_textureWave &&
				null != m_colorsWave)
			{
				m_textureWave.SetPixels32(m_colorsWave);
				m_textureWave.Apply();
			}
		}
	}
	public override void OnDispose ()
	{
		base.OnDispose ();
		if(Mic != null)
			Destroy(Mic);
		CleanUp();
		if (planes != null)
			Destroy (planes);
	}
	public void CleanUp()
	{
		if (RendererSpectrumLeft &&
			null != RendererSpectrumLeft.material)
		{
			UnityEngine.Object.DestroyImmediate(RendererSpectrumLeft.material, true);
		}

		if (RendererSpectrumRight &&
			null != RendererSpectrumRight.material)
		{
			UnityEngine.Object.DestroyImmediate(RendererSpectrumRight.material, true);
		}

		if (RendererWave &&
			null != RendererWave.material)
		{
			UnityEngine.Object.DestroyImmediate(RendererWave.material, true);
		}

		if (RendererSpectrumLeft)
		{
			RendererSpectrumLeft.material = MaterialSpectrumLeft;
		}

		if (RendererSpectrumRight)
		{
			RendererSpectrumRight.material = MaterialSpectrumRight;
		}

		if (RendererWave)
		{
			RendererWave.material = MaterialWave;
		}

		if (m_textureSpectrumLeft)
		{
			UnityEngine.Object.DestroyImmediate(m_textureSpectrumLeft, true);
			m_textureSpectrumLeft = null;
			MaterialSpectrumLeft.mainTexture = null;
		}

		if (m_textureSpectrumRight)
		{
			UnityEngine.Object.DestroyImmediate(m_textureSpectrumRight, true);
			m_textureSpectrumRight = null;
			MaterialSpectrumRight.mainTexture = null;
		}

		if (m_textureWave)
		{
			UnityEngine.Object.DestroyImmediate(m_textureWave, true);
			m_textureWave = null;
			MaterialWave.mainTexture = null;
		}
	}
}
