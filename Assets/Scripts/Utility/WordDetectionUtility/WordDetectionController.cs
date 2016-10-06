﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.IO;


[RequireComponent(typeof(AudioSource))]
public class WordDetectionController : UnitySingletonVisible<WordDetectionController>
{

	//探测到的字
	public WordDetection AudioWordDetection = null;

	private WordService _WordServe = null;//wordservice引用
	//话筒
	private SpectrumMicrophone _Mic = null;
	//指令系统
	enum Commands
	{
		Noise,
		Hello,//打招呼
		Come,//过来
		Sit,//坐下
		MoveAway,//走开
	}

	Commands m_command = Commands.Noise;
	/// <summary>
	/// wave归一化
	/// </summary>
	public bool NormalizeWave = true;

	//是否要移除噪音
	public bool RemoveSpectrumNoise = true;

	protected int m_WordIndex = -1;

	protected int m_startPosition = 0;//开始录音的位置

	protected DateTime m_timerStart = DateTime.MinValue;

	protected const string WORD_NOISE = "Noise";

	protected virtual WordDetails GetWord(string label)
	{
		foreach (WordDetails details in AudioWordDetection.Words)
		{
			if (null == details)
			{
				continue;
			}
			if (details.Label.Equals(label))
			{
				return details;
			}
		}

		return null;
	}

	/// <summary>
	/// Initialize the example
	/// </summary>
	public override void OnInit()
	{
		//AudioWordDetection = gameObject.AddComponent<WordDetection> ();
		_WordServe = WordService.Instance;
		_Mic = _WordServe.Mic;
		//AudioWordDetection.Mic = _Mic;
		if (
			null == _Mic)
		{
			Debug.LogError("Missing meta references");
			return;
		}

	}
	public void Start()
	{
		AudioWordDetection = 	GameObject.Find ("WordDetection").GetComponent<WordDetection>();
		AudioWordDetection.Mic = _Mic;
		// prepopulate words
		AudioWordDetection.Words.Add(new WordDetails() { Label = "Noise" });

		//subscribe detection event
		AudioWordDetection.WordDetectedEvent += WordDetectedHandler;
	}
	public override void OnDispose ()
	{
		base.OnDispose ();
	}
	/// <summary>
	/// Handle word detected event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	void WordDetectedHandler(object sender, WordDetection.WordEventArgs args)
	{
		if (string.IsNullOrEmpty(args.Details.Label))
		{
			return;
		}

		Debug.Log(string.Format("Detected: {0}", args.Details.Label));
	}

	/// <summary>
	/// Refilter all the samples, removing noise
	/// </summary>
	protected void Refilter()
	{
		if (null == AudioWordDetection)
		{
			return;
		}
		for (int index = 1; index < AudioWordDetection.Words.Count; ++index)
		{
			WordDetails details = AudioWordDetection.Words[index];
			_Mic.RemoveSpectrumNoise(GetWord(WORD_NOISE).SpectrumReal, details.SpectrumReal);
		}
	}

	protected virtual void SetupWordProfile(bool playAudio)
	{
		SetupWordProfile(playAudio, m_WordIndex == 0, m_WordIndex);
	}

	/// <summary>
	/// Setup the word profile
	/// </summary>
	protected virtual void SetupWordProfile(bool playAudio, bool isNoise, int wordIndex)
	{
		if (null == AudioWordDetection ||
			null == _Mic )//||string.IsNullOrEmpty(Mic.DeviceName)
		{
			return;
		}

		if (wordIndex < 0 ||
			wordIndex >= AudioWordDetection.Words.Count)
		{
			return;
		}

		WordDetails details = AudioWordDetection.Words[wordIndex];

		float[] wave = _Mic.GetLastData();
		if (null != wave)
		{
			//allocate for the wave copy
			int size = wave.Length;
			if (null == details.Wave ||
				details.Wave.Length != size)
			{
				details.Wave = new float[size];
				if (null != details.Audio)
				{
					UnityEngine.Object.DestroyImmediate(details.Audio, true);
					details.Audio = null;
				}
			}

			//trim the wave
			int position = _Mic.GetPosition();

			//get the trim size
			int trim = 0;
			if (m_startPosition < position)
			{
				trim = position - m_startPosition;
			}
			else
			{
				trim = size - m_startPosition + position;
			}

			//zero the existing wave
			for (int index = 0; index < size; ++index)
			{
				details.Wave[index] = 0f;
			}

			//shift array
			for (int index = 0, i = m_startPosition; index < trim; ++index, i = (i + 1) % size)
			{
				details.Wave[index] = wave[i];
			}

			//clear existing mic data
			for (int index = 0; index < size; ++index)
			{
				wave[index] = 0;
			}

			if (NormalizeWave &&
				!isNoise)
			{
				//normalize the array
				_Mic.NormalizeWave(details.Wave);
			}

			SetupWordProfile(details, isNoise);

			//play the audio
			if (null == details.Audio)
			{

				details.Audio = AudioClip.Create(string.Empty, size, 1, _Mic.SampleRate, false, false);
			}
			details.Audio.SetData(details.Wave, 0);
			GetComponent<AudioSource>().loop = false;
			GetComponent<AudioSource>().mute = false;
			if (playAudio)
			{
				if (NormalizeWave)
				{
					GetComponent<AudioSource>().PlayOneShot(details.Audio, 0.1f);
				}
				else
				{
					GetComponent<AudioSource>().PlayOneShot(details.Audio);
				}
			}

			// show profile
			_WordServe.OverrideSpectrumImag = true;
			_WordServe.SpectrumImag = details.SpectrumReal;
		}
	}

	protected virtual void SetupWordProfile(WordDetails details, bool isNoise)
	{
		//||string.IsNullOrEmpty(Mic.DeviceName)
		if (null == AudioWordDetection ||
			null == _Mic )
		{
			return;
		}

		int size = details.Wave.Length;
		int halfSize = size/2;

		//allocate profile spectrum, real
		if (null == details.SpectrumReal ||
			details.SpectrumReal.Length != halfSize)
		{
			details.SpectrumReal = new float[halfSize];
		}

		//allocate profile spectrum, imaginary
		if (null == details.SpectrumImag ||
			details.SpectrumImag.Length != halfSize)
		{
			details.SpectrumImag = new float[halfSize];
		}

		//get the spectrum for the trimmed word
		if (null != details.Wave &&
			details.Wave.Length > 0)
		{
			_Mic.GetSpectrumData(details.Wave, details.SpectrumReal, details.SpectrumImag, FFTWindow.Rectangular);
		}

		//filter noise
		if (RemoveSpectrumNoise)
		{
			if (isNoise)
			{
				Refilter();
			}
			else
			{
				_Mic.RemoveSpectrumNoise(GetWord(WORD_NOISE).SpectrumReal, details.SpectrumReal);
			}
		}
	}

	protected void DisplayProfileLoadSave(string key)
	{
		GUILayout.BeginHorizontal();

		GUILayout.Label(string.Empty, GUILayout.Width(150));
		GUILayout.Label("Profiles:");
		GUILayout.Label(string.Empty, GUILayout.Width(30));
		if (GUILayout.Button("Load", GUILayout.MinHeight(40)))
		{
			if (//AudioWordDetection.LoadProfiles(new FileInfo(key)) //||
				AudioWordDetection.LoadProfilesPrefs(key))
				//)
			{
				for (int wordIndex = 0; wordIndex < AudioWordDetection.Words.Count; ++wordIndex)
				{
					WordDetails details = AudioWordDetection.Words[wordIndex];

					if (null != details.Wave &&
						details.Wave.Length > 0)
					{
						if (null == details.Audio)
						{
							details.Audio = AudioClip.Create(string.Empty, details.Wave.Length, 1, _Mic.SampleRate, false,
								false);
						}
						details.Audio.SetData(details.Wave, 0);
						GetComponent<AudioSource>().loop = false;
						GetComponent<AudioSource>().mute = false;
					}

					SetupWordProfile(details, false);
				}
			}
		}
		GUILayout.Label(string.Empty, GUILayout.Width(30));
		if (GUILayout.Button("Save", GUILayout.MinHeight(40)))
		{
			//AudioWordDetection.SaveProfiles(new FileInfo(key));
			AudioWordDetection.SaveProfilesPrefs(key);
		}

		GUILayout.EndHorizontal();
	}

	private const string FILE_PROFILES = "VerbalCommand_Example4.profiles";

	/// <summary>
	/// GUI event
	/// </summary>
	protected virtual void OnGUI()
	{
		// ||string.IsNullOrEmpty(Mic.DeviceName)
		if (null == AudioWordDetection ||
			null == _Mic)
		{
			return;
		}

		DisplayProfileLoadSave(FILE_PROFILES);

		if (GUILayout.Button("Add Word", GUILayout.Height(45)))
		{
			WordDetails details = new WordDetails();
			details.Label = AudioWordDetection.Words.Count.ToString();
			AudioWordDetection.Words.Add(details);
		}

		GUILayout.Space(10);

		Color backgroundColor = GUI.backgroundColor;

		for (int wordIndex = 0; wordIndex < AudioWordDetection.Words.Count; ++wordIndex)
		{
			if (AudioWordDetection.ClosestIndex == wordIndex)
			{
				GUI.backgroundColor = Color.red;
			}
			else
			{
				GUI.backgroundColor = backgroundColor;
			}

			if (wordIndex > 0)
			{
				GUI.enabled = null != GetWord(WORD_NOISE).SpectrumReal;
			}

			GUILayout.BeginHorizontal();
			WordDetails details = AudioWordDetection.Words[wordIndex];
			if (wordIndex == 0)
			{
				GUILayout.Label(details.Label, GUILayout.Width(150), GUILayout.Height(45));
			}
			else
			{
				details.Label = GUILayout.TextField(details.Label, GUILayout.Width(150), GUILayout.Height(45));
			}
			GUILayout.Button(string.Format("{0}",
				(null == details.SpectrumReal) ? "not set" : "set"), GUILayout.Height(45));

			Event e = Event.current;
			if (null != e)
			{
				Rect rect = GUILayoutUtility.GetLastRect();
				bool overButton = rect.Contains(e.mousePosition);

				if (m_WordIndex == -1 &&
					m_timerStart == DateTime.MinValue &&
					Input.GetMouseButton(0) &&
					overButton)
				{
					Debug.Log("Initial button down");
					m_WordIndex = wordIndex;
					m_startPosition = _Mic.GetPosition();
					m_timerStart = DateTime.Now + TimeSpan.FromSeconds(_Mic.CaptureTime);
				}
				if (m_WordIndex == wordIndex)
				{
					bool buttonUp = Input.GetMouseButtonUp(0);
					if (m_timerStart > DateTime.Now &&
						!buttonUp)
					{
						Debug.Log("Button still pressed");
					}
					else if (m_timerStart != DateTime.MinValue &&
						m_timerStart < DateTime.Now)
					{
						Debug.Log("Button timed out");
						SetupWordProfile(false);
						m_timerStart = DateTime.MinValue;
						m_WordIndex = -1;
					}
					else if (m_timerStart != DateTime.MinValue &&
						buttonUp &&
						m_WordIndex != -1)
					{
						Debug.Log("Button is no longer pressed");
						SetupWordProfile(true);
						m_timerStart = DateTime.MinValue;
						m_WordIndex = -1;
					}
				}
			}
			GUI.enabled = null != details.Audio;
			if (GUILayout.Button("Play", GUILayout.Height(45)))
			{
				if (null != details.Audio)
				{
					if (NormalizeWave)
					{
						GetComponent<AudioSource>().PlayOneShot(details.Audio, 0.1f);
					}
					else
					{
						GetComponent<AudioSource>().PlayOneShot(details.Audio);
					}
				}

				// show profile
				_WordServe.OverrideSpectrumImag = true;
				_WordServe.SpectrumImag = details.SpectrumReal;
			}
			GUI.enabled = wordIndex > 0;
			if (wordIndex > 0 &&
				GUILayout.Button("Remove", GUILayout.Height(45)))
			{
				AudioWordDetection.Words.RemoveAt(wordIndex);
				--wordIndex;
			}
			GUILayout.Label(details.Score.ToString());
			GUILayout.EndHorizontal();

			if (wordIndex > 0)
			{
				GUI.enabled = null != GetWord(WORD_NOISE).SpectrumReal;
			}

			GUILayout.Space(10);
		}

		GUI.backgroundColor = backgroundColor;
	}
}
