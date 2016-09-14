using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

static internal class EventListener  {

	#region Internal Variables
	#pragma warning disable 0414//no need not use variates
	#pragma warning restore 0414

	static public Dictionary<string ,Delegate> eventTable = new Dictionary<string, Delegate>();
	static public void Cleanup()
	{
		#if LOG_ALL_MESSAGES
		Debug.Log("MESSAGER cleanup. Make sure that none of necessary listeners are removed.");
		#endif
		List<string> messageToRemove = new List<string> ();

		foreach (KeyValuePair<string, Delegate> pair in eventTable) {

			messageToRemove.Add (pair.Key);
		}

		foreach (string message in messageToRemove) {
			eventTable.Remove (message);
		}
	}
	static public void PrintEventTable()
	{
		Debug.Log("\t\t\t=== MESSENGER PrintEventTable ===");

		foreach (KeyValuePair<string, Delegate> pair in eventTable) {
			Debug.Log("\t\t\t" + pair.Key + "\t\t" + pair.Value);
		}
		Debug.Log ("\n");
	}
	#endregion 
	#region Message logging and exception throwing
	static public void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
	{
		#if LOG_ALL_MESSAGES || LOG_ADD_LISTENER
		Debug.Log("MESSENGER OnListenerAdding \t\"" + eventType + "\"\t{" + listenerBeingAdded.Target + " -> " + listenerBeingAdded.Method + "}");
		#endif

		if (!eventTable.ContainsKey(eventType))
		{
			eventTable.Add(eventType, null);
		}

		Delegate d = eventTable[eventType];
		if (d != null && d.GetType() != listenerBeingAdded.GetType())
		{
			throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
		}
	}
	static public void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
	{
		#if LOG_ALL_MESSAGES
		Debug.Log("MESSENGER OnListenerRemoving \t\"" + eventType + "\"\t{" + listenerBeingRemoved.Target + " -> " + listenerBeingRemoved.Method + "}");
		#endif
		if (eventTable.ContainsKey (eventType)) {
			Delegate d = eventTable [eventType];
			if (d == null) {
				throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
			}
			else if (d.GetType() != listenerBeingRemoved.GetType())
			{
				throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
			}
		}
		else
		{
			//throw new ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
		}
	}
	static public void OnListenerRemoved(string eventType)
	{
		if (eventTable [eventType] == null) {
			eventTable.Remove (eventType);
		}
	}
	static public void OnBroadcasting(string eventType)
	{
		#if REQUEST_LISTENER
		if(!eventTable.ContainsKey(eventType)){
		throw  new BroadcastException (string.Format ("Broadcast message\"{0}\" but no listener found."));
		}
		#endif
	}
	static public BroadcastException CreateBroadcastSignatureException(string eventType)
	{
		return new BroadcastException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
	}
	public class BroadcastException : Exception
	{
		public BroadcastException(string msg)
			: base(msg)
		{
		}
	}
	public class ListenerException : Exception
	{
		public ListenerException(string msg)
			: base(msg)
		{
		}
	}
	#endregion 
	#region AddListener
	//No parameters
	static public void AddListener(string eventType, Callback handler)
	{
		OnListenerAdding(eventType, handler);

		eventTable[eventType] = (Callback)eventTable[eventType] + handler;
	}

	//Single parameter
	static public void AddListener<T>(string eventType, Callback<T> handler)
	{
		OnListenerAdding(eventType, handler);
		eventTable[eventType] = (Callback<T>)eventTable[eventType] + handler;
	}

	//Two parameters
	static public void AddListener<T, U>(string eventType, Callback<T, U> handler)
	{
		OnListenerAdding(eventType, handler);
		eventTable[eventType] = (Callback<T, U>)eventTable[eventType] + handler;
	}

	//Three parameters
	static public void AddListener<T, U, V>(string eventType, Callback<T, U, V> handler)
	{
		OnListenerAdding(eventType, handler);
		eventTable[eventType] = (Callback<T, U, V>)eventTable[eventType] + handler;
	}

	static public void AddListener(Enum eventType, Callback handler)
	{
		AddListener(eventType.ToString(), handler);
	}

	//Single parameter
	static public void AddListener<T>(Enum eventType, Callback<T> handler)
	{
		AddListener<T>(eventType.ToString(), handler);
	}

	//Two parameters
	static public void AddListener<T, U>(Enum eventType, Callback<T, U> handler)
	{
		AddListener<T, U>(eventType.ToString(), handler);
	}

	//Three parameters
	static public void AddListener<T, U, V>(Enum eventType, Callback<T, U, V> handler)
	{
		AddListener<T, U, V>(eventType.ToString(), handler);
	}
	#endregion
	#region RemoveListener

	//No parameters
	static public void RemoveListener(string eventType, Callback handler)
	{
		if (!eventTable.ContainsKey(eventType))
			return;
		OnListenerRemoving(eventType, handler);
		eventTable[eventType] = (Callback)eventTable[eventType] - handler;
		OnListenerRemoved(eventType);
	}

	//Single parameter
	static public void RemoveListener<T>(string eventType, Callback<T> handler)
	{
		if (!eventTable.ContainsKey(eventType))
			return;

		OnListenerRemoving(eventType, handler);
		eventTable[eventType] = (Callback<T>)eventTable[eventType] - handler;
		OnListenerRemoved(eventType);
	}

	//Two parameters
	static public void RemoveListener<T, U>(string eventType, Callback<T, U> handler)
	{
		if (!eventTable.ContainsKey(eventType))
			return;
		OnListenerRemoving(eventType, handler);
		eventTable[eventType] = (Callback<T, U>)eventTable[eventType] - handler;
		OnListenerRemoved(eventType);
	}

	//Three parameters
	static public void RemoveListener<T, U, V>(string eventType, Callback<T, U, V> handler)
	{
		if (!eventTable.ContainsKey(eventType))
			return;
		OnListenerRemoving(eventType, handler);
		eventTable[eventType] = (Callback<T, U, V>)eventTable[eventType] - handler;
		OnListenerRemoved(eventType);
	}

	//No parameters
	static public void RemoveListener(Enum eventType, Callback handler)
	{
		RemoveListener(eventType.ToString(), handler);
	}

	//Single parameter
	static public void RemoveListener<T>(Enum eventType, Callback<T> handler)
	{
		RemoveListener<T>(eventType.ToString(), handler);
	}

	//Two parameters
	static public void RemoveListener<T, U>(Enum eventType, Callback<T, U> handler)
	{
		RemoveListener<T, U>(eventType.ToString(), handler);
	}

	//Three parameters
	static public void RemoveListener<T, U, V>(Enum eventType, Callback<T, U, V> handler)
	{
		RemoveListener<T, U, V>(eventType.ToString(), handler);
	}

	#endregion
	#region Broadcast
	//No parameters
	static public void Broadcast(string eventType)
	{
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnBroadcasting(eventType);

		Delegate d;
		if (eventTable.TryGetValue(eventType, out d))
		{
			Callback callback = d as Callback;

			if (callback != null)
			{
				callback();
			}
			else
			{
				throw CreateBroadcastSignatureException(eventType);
			}
		}
	}

	//Single parameter
	static public void Broadcast<T>(string eventType, T arg1)
	{
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnBroadcasting(eventType);

		Delegate d;
		if (eventTable.TryGetValue(eventType, out d))
		{
			Callback<T> callback = d as Callback<T>;

			if (callback != null)
			{
				callback(arg1);
			}
			else
			{
				throw CreateBroadcastSignatureException(eventType);
			}
		}
	}

	//Two parameters
	static public void Broadcast<T, U>(string eventType, T arg1, U arg2)
	{
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnBroadcasting(eventType);
		Delegate d;
		if (eventTable.TryGetValue(eventType, out d))
		{
			Callback<T, U> callback = d as Callback<T, U>;
			if (callback != null)
			{
				callback(arg1, arg2);
			}
			else
			{
				throw CreateBroadcastSignatureException(eventType);
			}
		}
	}

	//Three parameters
	static public void Broadcast<T, U, V>(string eventType, T arg1, U arg2, V arg3)
	{
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnBroadcasting(eventType);

		Delegate d;
		if (eventTable.TryGetValue(eventType, out d))
		{
			Callback<T, U, V> callback = d as Callback<T, U, V>;

			if (callback != null)
			{
				callback(arg1, arg2, arg3);
			}
			else
			{
				throw CreateBroadcastSignatureException(eventType);
			}
		}
	}

	//p2p
	static public void SendMsgToObject(GameObject obj, string eventType,object param ,SendMessageOptions opt )
	{
		obj.SendMessage (eventType, param, opt);//SendMessageOptions.RequireReceiver);
	}

	//No parameters
	static public void Broadcast(Enum eventType)
	{
		Broadcast(eventType.ToString());
	}

	//Single parameter
	static public void Broadcast<T>(Enum eventType, T arg1)
	{
		Broadcast<T>(eventType.ToString(), arg1);
	}

	//Two parameters
	static public void Broadcast<T, U>(Enum eventType, T arg1, U arg2)
	{
		Broadcast<T, U>(eventType.ToString(), arg1, arg2);
	}

	//Three parameters
	static public void Broadcast<T, U, V>(Enum eventType, T arg1, U arg2, V arg3)
	{
		Broadcast<T, U, V>(eventType.ToString(), arg1, arg2, arg3);
	}


	#endregion	
}
