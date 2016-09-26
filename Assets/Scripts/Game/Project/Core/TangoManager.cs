using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using Tango;
using UnityEngine;
using UnityEngine.EventSystems;
//这个类就是原来的PlaceMarkerInGameController,用来游戏交互，保存数据，闭环处理，主要是unity映射到现实世界
public class TangoManager : UnityAllSceneSingletonVisible<TangoManager>, ITangoPose, ITangoEvent, ITangoDepth {

	/// <summary>
	/// 场景中的点云.
	/// </summary>
	public TangoPointCloud m_pointCloud;
	#region 这部分使用ugui来代替，为了避免忘记，在这里记录下
	/// <summary>
	/// 放置 2D 游戏对象的画布.
	/// </summary>
	/// 
	/// <summary>
	/// 触摸效果.
	/// </summary>
	/// 
	/// <summary>
	/// 保存过程的界面文本.
	/// </summary>
	#endregion
	#if UNITY_EDITOR//考虑使用ugui，此处不用只是这里备注下，这三个变量是用来做模拟键盘的，因为电脑上输入框
	private bool m_displayGuiTextInput;
	private string m_guiTextInputContents;
	private bool m_guiTextInputResult;
	#endif
	/// <summary>
	/// TangoARPoseController 实例的一个引用.
	/// 当放置一个 marker 时, 我们需要该引用获取时间戳和姿态, 会用于后续闭环位置校正.
	/// </summary>
	private TangoARPoseController m_poseController;
	/// <summary>
	/// 若为 true, 则深度摄像头被启动, 等待下一次深度更新.
	/// </summary>
	private bool m_findPlaneWaitingForDepth;
	/// <summary>
	/// 被选中的 unit.
	/// </summary>
	private SceneUnit m_selectedUnit;

	/// <summary>
	/// 被选中 unit 的矩形边界.
	/// </summary>
	private Rect m_selectedRect;
	/// <summary>
	/// 交互是否已初始化.
	/// 初始化由重定位事件引发. 在设备重定位之前, 不能放置目标，为了避免与类本身的initialized冲突，换了个名字.
	/// </summary>
	private bool m_tangoReady = false;

	/// <summary>
	/// Tango Service 当前导入到区域描述子.
	/// </summary>
	[HideInInspector]
	public AreaDescription m_curAreaDescription;


	/// <summary>
	/// TangoApplication 实例的一个引用.
	/// </summary>
	private TangoApplication m_tangoApplication;
	/// <summary>
	/// 保存线程.
	/// </summary>
	private Thread m_saveThread;

	public override void OnInit ()
	{
		//直接可以拿到，但是为了使用方便
		m_tangoApplication = TangoService.Instance.m_tangoApplication;
		//这里采用一个引用
		m_poseController = TangoService.Instance.m_poseController;
		if (m_tangoApplication != null)
		{
			m_tangoApplication.Register(this);
		}
		base.OnInit ();
	}
	public bool IsTangoReady(){
		return  m_tangoReady;
	}
	/// <summary>
	/// Unity Update 函数.
	/// 主要处理触摸事件和放置 marker.
	/// </summary>
	public void Update()
	{
		if (m_saveThread != null && m_saveThread.ThreadState != ThreadState.Running)
		{
			// 由闭环更新 marker.
			_UpdateUnitsForLoopClosures();
			// 保存 sceneUit.
			_SaveUnitToDisk();
			// 重新加载场景来重启游戏.
			#pragma warning disable 618
			CatSceneManager.Instance.SetNextScene (SceneID.Initialize);//切回初始场景
			#pragma warning restore 618
			#if UNITY_EDITOR
			if (Input.GetKey(KeyCode.Escape))
			{
				#pragma warning disable 618
				CatSceneManager.Instance.SetNextScene (SceneID.Initialize);//切回初始场景
				#pragma warning restore 618
			}
			#endif
			if (!m_tangoReady)
				return;
			//确保没有点在ui上
			if (EventSystem.current.IsPointerOverGameObject (0) || GUIUtility.hotControl != 0)
				return;
			if (Input.touchCount == 1) {
				Touch t = Input.GetTouch(0);
				Vector2 guiPosition = new Vector2(t.position.x, Screen.height - t.position.y);
				Camera cam = Camera.main;
				RaycastHit hitInfo;

				if (t.phase != TouchPhase.Began)
				{
					return;
				}

				if (m_selectedRect.Contains(guiPosition))
				{
					// 不用处理, 由“隐藏" 按钮处理.这是因为点在了UI上，这个UI由gui画出来
				}
				else if (Physics.Raycast(cam.ScreenPointToRay(t.position), out hitInfo))
				{
					// 查找一个 unit, 选中它.
					GameObject tapped = hitInfo.collider.gameObject;
					m_selectedUnit = tapped.GetComponent<SceneUnit>();
				}
			}

		}
	}
	public void ScreenPoint2ARUnit(SceneUnit unit, Vector2 screenPos)//对于屏幕上一点,放置某个物体
	{
		
		StartCoroutine(_WaitForDepthAndFindPlane(unit, screenPos));

		// 由于可能会等待一段时间, 播放一个小动画以提醒用户.
		UIManager.Instance.Open(UIID.TouchEffect);
		Vector2 normalizedPosition = screenPos;
		normalizedPosition.x /= Screen.width;
		normalizedPosition.y /= Screen.height;
		UIManager.Instance.SetRectAnchor (UIID.TouchEffect, normalizedPosition);
	}
	public void SceneUnit2ARUnit(SceneUnit unit)//直接放置某个对象
	{
		Vector3 transScreenPos = Camera.main.WorldToScreenPoint (unit.thisT.position);
		Vector2 screenPos = new Vector2 (transScreenPos.x, transScreenPos.y);
		ScreenPoint2ARUnit (unit,screenPos);
	}
	/// <summary>
	/// 应用暂停/唤醒.
	/// </summary>
	/// <param name="pauseStatus"><c>true</c> if the application about to pause, otherwise <c>false</c>.</param>
	public void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus && m_tangoReady)
		{
			// When application is backgrounded, we reload the level because the Tango Service is disconected. All
			// learned area and placed marker should be discarded as they are not saved.
			#pragma warning disable 618
			CatSceneManager.Instance.SetNextScene (SceneID.Initialize);
			#pragma warning restore 618
		}
	}
	private IEnumerator _WaitForDepthAndFindPlane(SceneUnit unit, Vector2 touchPosition)
	{
		m_findPlaneWaitingForDepth = true;

		// T打开深度摄像头等待下一次深度更新.
		m_tangoApplication.SetDepthCameraRate(TangoEnums.TangoDepthCameraRate.MAXIMUM);
		while (m_findPlaneWaitingForDepth)
		{
			yield return null;
		}

		m_tangoApplication.SetDepthCameraRate(TangoEnums.TangoDepthCameraRate.DISABLED);


		// 下面是具体放置标记的代码, 我们游戏放置猫盆等主要参考这里.

		// 找到触摸点所在平面.
		Camera cam = Camera.main;
		Vector3 planeCenter;
		Plane plane;
		if (!m_pointCloud.FindPlane(cam, touchPosition, out planeCenter, out plane))
		{
			yield break;
		}

		// Ensure the location is always facing the camera.  This is like a LookRotation, but for the Y axis.
		Vector3 up = plane.normal;
		Vector3 forward;
		if (Vector3.Angle(plane.normal, cam.transform.forward) < 175)
		{
			Vector3 right = Vector3.Cross(up, cam.transform.forward).normalized;
			forward = Vector3.Cross(right, up).normalized;
		}
		else
		{
			// Normal is nearly parallel to camera look direction, the cross product would have too much
			// floating point error in it.
			forward = Vector3.Cross(up, cam.transform.right);
		}

		// 实例化 marker 对象.
//		newMarkObject = Instantiate(m_markPrefabs[m_currentMarkType],
//			planeCenter,
//			Quaternion.LookRotation(forward, up)) as GameObject;
		unit.thisT.position = planeCenter;
		unit.thisT.rotation = Quaternion.LookRotation (forward, up);

		unit.m_timestamp = (float)m_poseController.m_poseTimestamp;

		Matrix4x4 uwTDevice = Matrix4x4.TRS(m_poseController.m_tangoPosition,
			m_poseController.m_tangoRotation,
			Vector3.one);
		Matrix4x4 uwTUnit = Matrix4x4.TRS(unit.thisT.position,
			unit.thisT.rotation,
			Vector3.one);
		unit.m_deviceT = Matrix4x4.Inverse(uwTDevice) * uwTUnit;//用controller探测到位置和朝向后
		//，反转为原始矩阵，再用矩阵乘，则相当于把unity内的位置朝向投射到现实空间去了，之后利用闭环修正这个位置


	}
	/// <summary>
	/// 将一个由 Bounds 对象表示的 3D 包围盒转换为一个由 Rect 对象表示的 2D 矩形.选择框
	/// </summary>
	/// <returns>屏幕坐标下的 2D 矩形.</returns>
	/// <param name="cam">Camera to use.</param>
	/// <param name="bounds">3D bounding box.</param>
	private Rect _WorldBoundsToScreen(Camera cam, Bounds bounds)
	{
		Vector3 center = bounds.center;
		Vector3 extents = bounds.extents;
		Bounds screenBounds = new Bounds(cam.WorldToScreenPoint(center), Vector3.zero);

		screenBounds.Encapsulate(cam.WorldToScreenPoint(center + new Vector3(+extents.x, +extents.y, +extents.z)));
		screenBounds.Encapsulate(cam.WorldToScreenPoint(center + new Vector3(+extents.x, +extents.y, -extents.z)));
		screenBounds.Encapsulate(cam.WorldToScreenPoint(center + new Vector3(+extents.x, -extents.y, +extents.z)));
		screenBounds.Encapsulate(cam.WorldToScreenPoint(center + new Vector3(+extents.x, -extents.y, -extents.z)));
		screenBounds.Encapsulate(cam.WorldToScreenPoint(center + new Vector3(-extents.x, +extents.y, +extents.z)));
		screenBounds.Encapsulate(cam.WorldToScreenPoint(center + new Vector3(-extents.x, +extents.y, -extents.z)));
		screenBounds.Encapsulate(cam.WorldToScreenPoint(center + new Vector3(-extents.x, -extents.y, +extents.z)));
		screenBounds.Encapsulate(cam.WorldToScreenPoint(center + new Vector3(-extents.x, -extents.y, -extents.z)));
		return Rect.MinMaxRect(screenBounds.min.x, screenBounds.min.y, screenBounds.max.x, screenBounds.max.y);
	}
	public void OnGUI()
	{
		if (m_selectedUnit != null)
		{
			Renderer selectedRenderer = m_selectedUnit.GetComponent<Renderer>();

			// GUI's Y is flipped from the mouse's Y
			Rect screenRect = _WorldBoundsToScreen(Camera.main, selectedRenderer.bounds);
			float yMin = Screen.height - screenRect.yMin;
			float yMax = Screen.height - screenRect.yMax;
			screenRect.yMin = Mathf.Min(yMin, yMax);
			screenRect.yMax = Mathf.Max(yMin, yMax);

//			if (GUI.Button(screenRect, "<size=30>删除</size>"))//选择后可以删除场景对象，暂时注释掉
//			{
//				//m_markerList.Remove(m_selectedMarker.gameObject);
//				MapSceneManager.Instance.RemoveSceneUnit(m_selectedUnit);
//				m_selectedUnit = null;
//				m_selectedRect = new Rect();
//			}
//			else
//			{
				m_selectedRect = screenRect;
//			}
		}
		else
		{
			m_selectedRect = new Rect();
		}
		#region 这部分代码保留，只在编辑器起作用，作用是保存的时候输入需要保存名字，在手机上自然有输入框，游戏的ui主体用ugui，测试用ui用GUI
		#if UNITY_EDITOR
		// Handle text input when there is no device keyboard in the editor.
		if (m_displayGuiTextInput)
		{
			Rect textBoxRect = new Rect(100,
				Screen.height - 200,
				Screen.width - 200,
				100);

			Rect okButtonRect = textBoxRect;
			okButtonRect.y += 100;
			okButtonRect.width /= 2;

			Rect cancelButtonRect = okButtonRect;
			cancelButtonRect.x = textBoxRect.center.x;

			GUI.SetNextControlName("TextField");
			GUIStyle customTextFieldStyle = new GUIStyle(GUI.skin.textField);
			customTextFieldStyle.alignment = TextAnchor.MiddleCenter;
			m_guiTextInputContents = 
				GUI.TextField(textBoxRect, m_guiTextInputContents, customTextFieldStyle);
			GUI.FocusControl("TextField");

			if (GUI.Button(okButtonRect, "OK")
				|| (Event.current.type == EventType.keyDown && Event.current.character == '\n'))
			{
				m_displayGuiTextInput = false;
				m_guiTextInputResult = true;
			}
			else if (GUI.Button(cancelButtonRect, "Cancel"))
			{
				m_displayGuiTextInput = false;
				m_guiTextInputResult = false;
			}
		}
		#endif
		#endregion
	}
	/// <summary>
	/// Tango 事件处理.
	/// </summary>
	/// <param name="tangoEvent">Tango 事件.</param>
	public void OnTangoEventAvailableEventHandler(Tango.TangoEvent tangoEvent)
	{
		// We will not have the saving progress when the learning mode is off.
		if (!m_tangoApplication.m_areaDescriptionLearningMode)
		{
			return;
		}

		if (tangoEvent.type == TangoEnums.TangoEventType.TANGO_EVENT_AREA_LEARNING
			&& tangoEvent.event_key == "AreaDescriptionSaveProgress")
		{
			// 显示保存进度.
			string msg = "Saving. " + (float.Parse(tangoEvent.event_value) * 100) + "%";
			UIManager.Instance.ShowMessage (msg);
		}
	}

	/// <summary>
	/// OnTangoPoseAvailable 事件.
	/// 
	/// 此例只是监听相应于区域描述帧对的 Start-Of-Service.
	/// 这个帧对表明一个重定位或闭环事件发生, 基于这, 我们还开始交互初始化或 marker 位置调整.
	/// </summary>
	/// <param name="poseData">Returned pose data from TangoService.</param>
	public void OnTangoPoseAvailable(Tango.TangoPoseData poseData)
	{
		// 该帧对待回调表明一个闭环或重定位已发生.
		//
		// 学习模式时, 该回调表明闭环事件发生.
		// 闭环发生在系统识别到一个已访问区域时, 闭环操作将修正以前保存的姿态以获取更精确的结果.
		// (基于以前保存的时间戳, 可以通过 GetPoseAtTime 查询姿态.)
		//
		// 非学习模式且已加载一个区域描述时, 该回调表明重定位事件发生.
		// 重定位是设备找到它在那里, 相应于加载的区域描述.
		// 此例中, 当设备重定位后, marker 将被加载, 因为知道了其相对设备位置.

		if (poseData.framePair.baseFrame ==
			TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_AREA_DESCRIPTION &&
			poseData.framePair.targetFrame ==
			TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_START_OF_SERVICE &&
			poseData.status_code == TangoEnums.TangoPoseStatusType.TANGO_POSE_VALID)
		{
			// 在第一次闭环/重定位事件, 初始化游戏内交互.
			if (!m_tangoReady)
			{
				m_tangoReady = true;
				if (m_curAreaDescription == null)
				{
					Debug.Log("AndroidInGameController.OnTangoPoseAvailable(): m_curAreaDescription is null");
					return;
				}

				_LoadUnitFromDisk();
			}
		}
	}
	/// <summary>
	/// 保存游戏.
	/// 1. 如果学习模式打开, 则保存区域描述.
	/// 2. unit 位置调整, 详见 _UpdateUnitsForLoopClosures() 函数头.
	/// 3. 保存 unit, xml 格式.
	/// 4. 重新加载场景.
	/// </summary>
	public void Save()
	{
		StartCoroutine(_DoSaveCurrentAreaDescription());
	}
	/// <summary>
	/// 保存区域描述.
	/// </summary>
	/// <returns>Coroutine IEnumerator.</returns>
	private IEnumerator _DoSaveCurrentAreaDescription()
	{
		#if UNITY_EDITOR
		// Work around lack of on-screen keyboard in editor:
		if (m_displayGuiTextInput || m_saveThread != null)
		{
			yield break;
		}

		m_displayGuiTextInput = true;
		m_guiTextInputContents = "Unnamed";
		while (m_displayGuiTextInput)
		{
			yield return null;
		}

		bool saveConfirmed = m_guiTextInputResult;
		#else
		if (TouchScreenKeyboard.visible || m_saveThread != null)
		{
		yield break;
		}

		TouchScreenKeyboard kb = TouchScreenKeyboard.Open("Unnamed");
		while (!kb.done && !kb.wasCanceled)
		{
		yield return null;
		}

		bool saveConfirmed = kb.done;
		#endif
		// 保存.
		if (saveConfirmed)
		{
			// 保存前禁用交互.
			m_tangoReady = false;
			UIManager.Instance.ShowMessage ("Saving...");
			if (m_tangoApplication.m_areaDescriptionLearningMode) // 学习模式, 保存当前区域描述.
			{
				m_saveThread = new Thread(delegate ()
					{
						// 启动一个保存线程.
						m_curAreaDescription = AreaDescription.SaveCurrent();
						AreaDescription.Metadata metadata = m_curAreaDescription.GetMetadata();
						#if UNITY_EDITOR
						metadata.m_name = m_guiTextInputContents;
						#else
						metadata.m_name = kb.text;
						#endif
						m_curAreaDescription.SaveMetadata(metadata);
					});
				m_saveThread.Start();
			}
			else // 保存 marker.
			{
				_SaveUnitToDisk();
				#pragma warning disable 618
				CatSceneManager.Instance.SetNextScene (SceneID.Initialize);
				//Application.LoadLevel(Application.loadedLevel);
				#pragma warning restore 618
			}
		}
	}
	/// <summary>
	/// 加载 sceneunit.
	/// </summary>
	private void _LoadUnitFromDisk()
	{
		// Attempt to load the exsiting markers from storage.
		string path = Application.persistentDataPath + "/" + m_curAreaDescription.m_uuid + ".xml";

		var serializer = new XmlSerializer(typeof(List<UnitData>));
		var stream = new FileStream(path, FileMode.Open);

		List<UnitData> xmlDataList = serializer.Deserialize(stream) as List<UnitData>;

		if (xmlDataList == null)
		{
			Debug.Log("AndroidInGameController._LoadMarkerFromDisk(): xmlDataList is null");
			return;
		}

		//m_markerList.Clear();
		foreach (UnitData unit in xmlDataList)
		{
			if(unit.m_type <(int) UnitClassType.SceneObj 
				&& unit.m_type>(int)UnitClassType.ScenePet)//不是宠物，自带的宠物在gamescene内生成并且映射过去,这里只是场景物件
				//如猫砂盘
				MapSceneManager.Instance.CreateSceneCatLittle(unit.m_id, unit.m_position, unit.m_orientation);
			
		}
	}
	/// <summary>
	/// 当有新的深度信息时被调用. 对 Tango 平板, 深度回调频率为 5 Hz.
	/// </summary>
	/// <param name="tangoDepth">Tango 深度.</param>
	public void OnTangoDepthAvailable(TangoUnityDepth tangoDepth)
	{
		// 此处不处理深度, 因为 PointCloud 可能尚未更新. 只通知协程可以继续即可.
		m_findPlaneWaitingForDepth = false;
	}
	/// <summary> 
	/// 当闭环发生时, 校正已保存的 marker.
	/// 
	/// 当 Tango 服务在学习模式时, 漂移会随时间积累, 但当系统看到一个已存在区域时, 它将校正先前保存的姿态.
	/// 这个操作称为闭环. 当闭环发生时, 我们需要重新查询先前保存的 marker 位置以获得最好结果.
	/// </summary>
	private void _UpdateUnitsForLoopClosures()
	{
		// 每当一个闭环事件时, 调整 marker 位置.
		foreach (SceneUnit unit in MapSceneManager.Instance.GetAllUnit())
		{
			
			if (unit.m_timestamp != -1.0f)
			{
				TangoCoordinateFramePair pair;
				TangoPoseData relocalizedPose = new TangoPoseData();

				pair.baseFrame = TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_AREA_DESCRIPTION;
				pair.targetFrame = TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_DEVICE;
				PoseProvider.GetPoseAtTime(relocalizedPose, unit.m_timestamp, pair);

				Matrix4x4 uwTDevice = m_poseController.m_uwTss
					* relocalizedPose.ToMatrix4x4()
					* m_poseController.m_dTuc;

				Matrix4x4 uwTMarker = uwTDevice * unit.m_deviceT;

				unit.thisT.position = uwTMarker.GetColumn(3);
				unit.thisT.rotation = Quaternion.LookRotation(uwTMarker.GetColumn(2), uwTMarker.GetColumn(1));
			}
		}
	}
	/// <summary>
	/// 保存 marker.
	/// </summary>
	private void _SaveUnitToDisk()
	{
		// Compose a XML data list.
		List<UnitData> xmlDataList = new List<UnitData>();
		foreach (SceneUnit unit in MapSceneManager.Instance.GetAllUnit())
		{
			if (unit.m_Type < UnitClassType.SceneObj 
				&& unit.m_Type > UnitClassType.ScenePet) {//除了宠物之外的才保存
				// Add marks data to the list, we intentionally didn't add the timestamp, because the timestamp will not be
				// useful when the next time Tango Service is connected. The timestamp is only used for loop closure pose
				// correction in current Tango connection.
				UnitData temp = new UnitData ();
				temp.m_id = unit.id;
				temp.m_type = (int)unit.m_Type;
				temp.m_position = unit.thisT.position;
				temp.m_orientation = unit.thisT.rotation;
				xmlDataList.Add (temp);
			}
		}

		string path = Application.persistentDataPath + "/" + m_curAreaDescription.m_uuid + ".xml";
		var serializer = new XmlSerializer(typeof(List<UnitData>));
		using (var stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, xmlDataList);
		}
	}


	/// <summary>
	/// sceneunit 的数据容器.
	/// 
	/// Used for serializing/deserializing marker to xml.
	/// </summary>
	[System.Serializable]
	public class UnitData
	{
		[XmlElement("id")]
		public int m_id;
		/// <summary>
		/// Marker's type.
		/// 
		/// Red, green or blue markers. In a real game scenario, this could be different game objects
		/// (e.g. banana, apple, watermelon, persimmons).
		/// </summary>
		[XmlElement("type")]
		public int m_type;

		/// <summary>
		/// Position of the this mark, with respect to the origin of the game world.
		/// </summary>
		[XmlElement("position")]
		public Vector3 m_position;

		/// <summary>
		/// Rotation of the this mark.
		/// </summary>
		[XmlElement("orientation")]
		public Quaternion m_orientation;
	}
}
