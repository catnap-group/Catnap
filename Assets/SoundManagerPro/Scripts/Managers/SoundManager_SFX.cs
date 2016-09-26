using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using antilunchbox;

public partial class SoundManager : Singleton<SoundManager> {
	
	/// <summary>
	/// Sets the SFX cap.
	/// </summary>
	/// <param name='cap'>
	/// Cap.
	/// </param>
	public static void SetSFXCap(int cap)
	{
		Instance.capAmount = cap;
	}
    
	/// <summary>
	/// Plays the SFX on an owned object, will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
    public static AudioSource PlaySFX(AudioClip clip, float volume, float pitch, Vector3 location=default(Vector3), bool looping=false, float delay=0f, SongCallBack runOnEndFunction=null, SoundDuckingSetting duckingSetting=SoundDuckingSetting.DoNotDuck, float duckVolume=0f, float duckPitch=1f)
    {
        if (Instance.offTheSFX)
            return null;
            
        if (clip == null)
            return null;
        
        return Instance.PlaySFXAt(clip, volume, pitch, location, false, "", looping, delay, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
    }
	
	/// <summary>
	/// Plays the SFX on an owned object, will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(AudioClip clip, float volume)
    {
        return PlaySFX(clip, volume, Instance.pitchSFX);
    }
	
	/// <summary>
	/// Plays the SFX on an owned object, will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(AudioClip clip)
    {
        return PlaySFX(clip, Instance.volumeSFX);
    }
	
	/// <summary>
	/// Plays the SFX IFF other SFX with the same cappedID are not over the cap limit. Will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
    public static AudioSource PlayCappedSFX(AudioClip clip, string cappedID, float volume, float pitch, Vector3 location=default(Vector3))
    {
        if (Instance.offTheSFX)
            return null;
            
        if (clip == null)
            return null;
		
		if(string.IsNullOrEmpty(cappedID))
			return null;
        
        // Play the clip if not at capacity
		if(!Instance.IsAtCapacity(cappedID, clip.name))
            return Instance.PlaySFXAt(clip, volume, pitch, location, true, cappedID);
		else
			return null;
    }
	
	/// <summary>
	/// Plays the SFX IFF other SFX with the same cappedID are not over the cap limit. Will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlayCappedSFX(AudioClip clip, string cappedID, float volume)
    {
        return PlayCappedSFX(clip, cappedID, volume, Instance.pitchSFX);
    }
	
	/// <summary>
	/// Plays the SFX IFF other SFX with the same cappedID are not over the cap limit. Will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlayCappedSFX(AudioClip clip, string cappedID)
    {
        return PlayCappedSFX(clip, cappedID, Instance.volumeSFX);
    }
	
	/// <summary>
	/// Plays the SFX IFF other SFX with the same cappedID are not over the cap limit. Will default the pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlayCappedSFX(AudioSource aS, AudioClip clip, string cappedID, float volume, float pitch)
    {
        if (Instance.offTheSFX)
            return null;
            
        if (clip == null || aS == null)
            return null;
		
		if(string.IsNullOrEmpty(cappedID))
			return null;
		
		// Play the clip if not at capacity
		if(!Instance.IsAtCapacity(cappedID, clip.name))
		{
			// Keep reference of unownedsfx objects
			Instance.CheckInsertionIntoUnownedSFXObjects(aS);
			
            return Instance.PlaySFXOn(aS, clip, volume, pitch, true, cappedID);
		}
		else
			return null;
    }
	
	/// <summary>
	/// Plays the SFX IFF other SFX with the same cappedID are not over the cap limit. Will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlayCappedSFX(AudioSource aS, AudioClip clip, string cappedID, float volume)
    {
        return PlayCappedSFX(aS, clip, cappedID, volume, Instance.pitchSFX);
    }
	
	/// <summary>
	/// Plays the SFX IFF other SFX with the same cappedID are not over the cap limit. Will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlayCappedSFX(AudioSource aS, AudioClip clip, string cappedID)
    {
        return PlayCappedSFX(aS, clip, cappedID, Instance.volumeSFX);
    }
	/// <summary>
	/// Plays the SFX on an owned & pooled object by clipname reference on the SoundManager, will default the location to (0,0,0), pitch to SoundManager.Instance.pitchSFX, volume to SoundManager.Instance.volumeSFX
	/// </summary>
	/// <returns>
	/// The resulting <a href="http://docs.unity3d.com/ScriptReference/AudioSource.html">AudioSource</a>.
	/// </returns>
	/// <param name='clipName'>
	/// Name of the clip on the SoundManager.
	/// </param>
	/// <param name='looping'>
	/// Whether it is looping.
	/// </param>
	/// <param name='delay'>
	/// Delay.
	/// </param>
	/// <param name='volume'>
	/// Volume. If set to float.MaxValue, it will become the default volume currently set.
	/// </param>
	/// <param name='pitch'>
	/// Pitch. If set to float.MaxValue, it will become the default pitch currently set.
	/// </param>
	/// <param name='location'>
	/// Location.
	/// </param>
	/// <param name='runOnEndFunction'>
	/// Run on end function.
	/// </param>
	/// <param name='duckingSetting'>
	/// Ducking setting.
	/// </param>
	/// <param name='duckVolume'>
	/// Duck volume.
	/// </param>
	/// <param name='duckPitch'>
	/// Duck pitch.
	/// </param>
	public static AudioSource PlaySFX(string clipName, bool looping=false, float delay=0f, float volume=float.MaxValue, float pitch=float.MaxValue, Vector3 location=default(Vector3), SongCallBack runOnEndFunction=null, SoundDuckingSetting duckingSetting=SoundDuckingSetting.DoNotDuck, float duckVolume=0f, float duckPitch=1f)
	{
		if (Instance.offTheSFX || Instance.isPaused)
			return null;

		//if (!SoundManager.ClipNameIsValid(clipName))
		//    return null;

		if(volume == float.MaxValue)
			volume = Instance.volumeSFX;

		if(pitch == float.MaxValue)
			pitch = Instance.pitchSFX;

		return Instance.PlaySFXAt(SoundManager.Load(clipName), volume, pitch, location, false, "", looping, delay, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
	}
	/// <summary>
	/// Plays the SFX another audiosource of your choice, will default the looping to false, pitch to 1f, volume to 1f
	/// </summary>
    public static AudioSource PlaySFX(AudioSource aS, AudioClip clip, bool looping, float volume, float pitch, float delay=0f, SongCallBack runOnEndFunction=null, SoundDuckingSetting duckingSetting=SoundDuckingSetting.DoNotDuck, float duckVolume=0f, float duckPitch=1f)
    {
        if (Instance.offTheSFX)
            return null;
            
        if ((clip == null) || (aS == null))
            return null;
		
		// Keep reference of unownedsfx objects
		Instance.CheckInsertionIntoUnownedSFXObjects(aS);
        
		return Instance.PlaySFXOn(aS, clip, volume, pitch, false, "", looping, delay, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
    }
	
	/// <summary>
	/// Plays the SFX another audiosource of your choice, will default the looping to false, pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(AudioSource aS, AudioClip clip, bool looping, float volume)
    {
        return PlaySFX(aS, clip, looping, volume, Instance.pitchSFX);
    }
	
	/// <summary>
	/// Plays the SFX another audiosource of your choice, will default the looping to false, pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(AudioSource aS, AudioClip clip, bool looping)
    {
        return PlaySFX(aS, clip, looping, Instance.volumeSFX);
    }
	
	/// <summary>
	/// Plays the SFX another audiosource of your choice, will default the looping to false, pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(AudioSource aS, AudioClip clip)
    {
        return PlaySFX(aS, clip, false);
    }
	
	/// <summary>
	/// Stops the SFX on another audiosource
	/// </summary>
    public static void StopSFXObject(AudioSource aS)
    {
        if (aS == null)
            return;
            
        if (aS.isPlaying)
            aS.Stop();
		
		if(Instance.delayedAudioSources.ContainsKey(aS))
			Instance.delayedAudioSources.Remove(aS);
    }
	
	/// <summary>
	/// Plays the SFX another gameObject of your choice, will default the looping to false, pitch to 1f, volume to 1f
	/// </summary>
    public static AudioSource PlaySFX(GameObject gO, AudioClip clip, bool looping, float volume, float pitch, float delay=0f, SongCallBack runOnEndFunction=null, SoundDuckingSetting duckingSetting=SoundDuckingSetting.DoNotDuck, float duckVolume=0f, float duckPitch=1f)
    {
        if (Instance.offTheSFX)
            return null;
            
        if ((clip == null) || (gO == null))
            return null;
        
        if (gO.GetComponent<AudioSource>() == null)
            gO.AddComponent<AudioSource>();
		
		return PlaySFX(gO.GetComponent<AudioSource>(), clip, looping, volume, pitch, delay, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
    }
	
	/// <summary>
	/// Plays the SFX another gameObject of your choice, will default the looping to false, pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(GameObject gO, AudioClip clip, bool looping, float volume)
    {
        return PlaySFX(gO, clip, looping, volume, Instance.pitchSFX);
    }
	
	/// <summary>
	/// Plays the SFX another gameObject of your choice, will default the looping to false, pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(GameObject gO, AudioClip clip, bool looping)
    {
        return PlaySFX(gO, clip, looping, Instance.volumeSFX);
    }
	
	/// <summary>
	/// Plays the SFX another gameObject of your choice, will default the looping to false, pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(GameObject gO, AudioClip clip)
    {
        return PlaySFX(gO, clip, false);
    }
	
	/// <summary>
	/// Stops the SFX on another gameObject
	/// </summary>
    public static void StopSFXObject(GameObject gO)
    {
        if (gO == null)
            return;
        
        StopSFXObject(gO.GetComponent<AudioSource>());
    }
	
	/// <summary>
	/// Stops all SFX.
	/// </summary>
	public static void StopSFX()
	{
		Instance._StopSFX();
	}
	
	/// <summary>
	/// Plays the SFX in a loop on another audiosource of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
    public static AudioSource PlaySFXLoop(AudioSource aS, AudioClip clip, bool tillDestroy, float volume, float pitch, float maxDuration, SongCallBack runOnEndFunction=null, SoundDuckingSetting duckingSetting=SoundDuckingSetting.DoNotDuck, float duckVolume=0f, float duckPitch=1f)
    {
        if (Instance.offTheSFX)
            return null;
            
        if ((clip == null) || (aS == null))
            return null;
		
		Instance.CheckInsertionIntoUnownedSFXObjects(aS);
		
		return Instance.PlaySFXLoopOn(aS, clip, tillDestroy, volume, pitch, maxDuration, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another audiosource of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
	public static AudioSource PlaySFXLoop(AudioSource aS, AudioClip clip, bool tillDestroy, float volume, float pitch)
    {
        return PlaySFXLoop(aS, clip, tillDestroy, volume, pitch, 0f);
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another audiosource of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
	public static AudioSource PlaySFXLoop(AudioSource aS, AudioClip clip, bool tillDestroy, float volume)
    {
        return PlaySFXLoop(aS, clip, tillDestroy, volume, Instance.pitchSFX);
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another audiosource of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
	public static AudioSource PlaySFXLoop(AudioSource aS, AudioClip clip, bool tillDestroy)
    {
        return PlaySFXLoop(aS, clip, tillDestroy, Instance.volumeSFX);
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another audiosource of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
	public static AudioSource PlaySFXLoop(AudioSource aS, AudioClip clip)
    {
        return PlaySFXLoop(aS, clip, true);
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another gameObject of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
    public static AudioSource PlaySFXLoop(GameObject gO, AudioClip clip, bool tillDestroy, float volume, float pitch, float maxDuration, SongCallBack runOnEndFunction=null, SoundDuckingSetting duckingSetting=SoundDuckingSetting.DoNotDuck, float duckVolume=0f, float duckPitch=1f)
    {
        if (Instance.offTheSFX)
            return null;
            
        if ((clip == null) || (gO == null))
            return null;
		
		if (gO.GetComponent<AudioSource>() == null)
            gO.AddComponent<AudioSource>();
		
		Instance.CheckInsertionIntoUnownedSFXObjects(gO.GetComponent<AudioSource>());
		
		return Instance.PlaySFXLoopOn(gO.GetComponent<AudioSource>(), clip, tillDestroy, volume, pitch, maxDuration, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another gameObject of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
	public static AudioSource PlaySFXLoop(GameObject gO, AudioClip clip, bool tillDestroy, float volume, float pitch)
    {
        return PlaySFXLoop(gO, clip, tillDestroy, volume, pitch, 0f);
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another gameObject of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
	public static AudioSource PlaySFXLoop(GameObject gO, AudioClip clip, bool tillDestroy, float volume)
    {
        return PlaySFXLoop(gO, clip, tillDestroy, volume, Instance.pitchSFX);
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another gameObject of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
	public static AudioSource PlaySFXLoop(GameObject gO, AudioClip clip, bool tillDestroy)
    {
        return PlaySFXLoop(gO, clip, tillDestroy, Instance.volumeSFX);
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another gameObject of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
	public static AudioSource PlaySFXLoop(GameObject gO, AudioClip clip)
    {
        return PlaySFXLoop(gO, clip, true);
    }
	
	/// <summary>
	/// Sets mute on all the SFX to 'mute'. Returns the result.
	/// </summary>
	public static bool MuteSFX(bool toggle)
    {
        Instance.mutedSFX = toggle;
		return Instance.mutedSFX;
    }
	
	/// <summary>
	/// Toggles mute on SFX. Returns the result.
	/// </summary>
	public static bool MuteSFX()
    {
        return MuteSFX(!Instance.mutedSFX);
    }
	
	/// <summary>
	/// Determines whether this instance is SFX muted.
	/// </summary>
	public static bool IsSFXMuted()
	{
		return Instance.mutedSFX;
	}
	
	/// <summary>
	/// Sets the maximum volume of SFX in the game relative to the global volume.
	/// </summary>
	public static void SetVolumeSFX(float setVolume)
	{
		setVolume = Mathf.Clamp01(setVolume);
		
		float currentPercentageOfVolume;
		currentPercentageOfVolume = Instance.volumeSFX / Instance.maxSFXVolume;
		
		Instance.maxSFXVolume = setVolume * Instance.maxVolume;
		
		if(float.IsNaN(currentPercentageOfVolume) || float.IsInfinity(currentPercentageOfVolume))
			currentPercentageOfVolume = 1f;
		
		Instance.volumeSFX = Instance.maxSFXVolume * currentPercentageOfVolume;
	}
	/* COMING SOON
	public static void SetVolumeSFX(float setVolume, string groupName)
	{
		
	}
	*/
	public static void SetVolumeSFX(float setVolume, bool ignoreMaxSFXVolume, params AudioSource[] audioSources)
	{
		setVolume = Mathf.Clamp01(setVolume);
		float newVolume = ignoreMaxSFXVolume ? setVolume : (setVolume * Instance.maxSFXVolume);
		
		foreach(AudioSource audioSource in audioSources)
			audioSource.volume = newVolume;
	}
	
	public static void SetVolumeSFX(float setVolume, bool ignoreMaxSFXVolume, params GameObject[] sfxObjects)
	{
		setVolume = Mathf.Clamp01(setVolume);
		float newVolume = ignoreMaxSFXVolume ? setVolume : (setVolume * Instance.maxSFXVolume);
		
		foreach(GameObject sfxObject in sfxObjects)
			sfxObject.GetComponent<AudioSource>().volume = newVolume;
	}
	
	/// <summary>
	/// Gets the volume SFX.
	/// </summary>
	public static float GetVolumeSFX()
	{
		return Instance.maxSFXVolume;
	}
	
	/// <summary>
	/// Sets the pitch of SFX in the game.
	/// </summary>
	public static void SetPitchSFX(float setPitch)
	{
		Instance.pitchSFX = setPitch;
	}
	/* COMING SOON
	public static void SetPitchSFX(float setPitch, string groupName)
	{
		
	}
	*/
	public static void SetPitchSFX(float setPitch, params AudioSource[] audioSources)
	{
		foreach(AudioSource audioSource in audioSources)
			audioSource.pitch = setPitch;
	}
	
	public static void SetPitchSFX(float setPitch, params GameObject[] sfxObjects)
	{
		foreach(GameObject sfxObject in sfxObjects)
			sfxObject.GetComponent<AudioSource>().pitch = setPitch;
	}
	
	/// <summary>
	/// Gets the pitch SFX.
	/// </summary>
	public static float GetPitchSFX()
	{
		return Instance.pitchSFX;
	}
	
	/////////////////////////////////////////////////////
	
	/// <summary>
	/// Saves the SFX to the SoundManager prefab for easy access for frequently used SFX.  Will register the SFX to the group.
	/// </summary>
	public static void SaveSFX(AudioClip clip, string grpName)
	{
		if(clip == null)
			return;
		
		SFXGroup grp = Instance.GetGroupByGroupName(grpName);
		if(grp == null)
			Debug.LogWarning("The SFXGroup, "+grpName+", does not exist. Creating it as a new group");
		
		SaveSFX(clip);
		Instance.AddClipToGroup(clip.name, grpName);
	}
	
	/// <summary>
	/// Saves the SFX to the SoundManager prefab for easy access for frequently used SFX.  Will register the SFX to the group specified.
	/// If the group doesn't exist, it will be added to SoundManager.
	public static void SaveSFX(AudioClip clip, SFXGroup grp)
	{
		if(clip == null)
			return;
		
		if(grp != null)
		{
			if(!Instance.groups.ContainsKey(grp.groupName))
			{
				Instance.groups.Add(grp.groupName, grp);
#if UNITY_EDITOR
				Instance.sfxGroups.Add(grp);
#endif
			}
			else if(Instance.groups[grp.groupName] != grp)
				Debug.LogWarning("The SFXGroup, "+grp.groupName+", already exists. This new group will not be added.");
		}
		
		SaveSFX(clip);
		Instance.AddClipToGroup(clip.name, grp.groupName);
	}
	
	/// <summary>
	/// Saves the SFX to the SoundManager prefab for easy access for frequently used SFX.
	/// </summary>
	public static void SaveSFX(params AudioClip[] clips)
	{
		foreach(AudioClip clip in clips)
		{
			if(clip == null)
				continue;
			
			if(!Instance.allClips.ContainsKey(clip.name))
			{
				Instance.allClips.Add(clip.name, clip);
				Instance.prepools.Add(clip.name, 0);
#if UNITY_EDITOR
				Instance.storedSFXs.Add(clip);
				Instance.sfxPrePoolAmounts.Add(0);
				Instance.showSFXDetails.Add(false);
#endif
			}
		}
	}
	
	public static void DeleteSFX(params AudioClip[] clips)
	{
		foreach(AudioClip clip in clips)
		{
			if(clip == null)
				continue;
			
			if(!Instance.allClips.ContainsKey(clip.name))
			{
				Instance.allClips.Remove(clip.name);
				Instance.prepools.Remove(clip.name);
#if UNITY_EDITOR
				int index = Instance.storedSFXs.IndexOf(clip);
				if(index == -1) continue;
				Instance.storedSFXs.RemoveAt(index);
				Instance.sfxPrePoolAmounts.RemoveAt(index);
				Instance.showSFXDetails.RemoveAt(index);
#endif
			}
		}
	}
	
	public static void DeleteSFX(params string[] clipNames)
	{
		foreach(string clipName in clipNames)
		{
			if(string.IsNullOrEmpty(clipName))
				continue;
			
			if(!Instance.allClips.ContainsKey(clipName))
			{
				AudioClip clip = Instance.allClips[clipName];
				Instance.allClips.Remove(clipName);
				Instance.prepools.Remove(clipName);
#if UNITY_EDITOR
				if(clip == null) continue;
				int index = Instance.storedSFXs.IndexOf(clip);
				if(index == -1) continue;
				Instance.storedSFXs.RemoveAt(index);
				Instance.sfxPrePoolAmounts.RemoveAt(index);
				Instance.showSFXDetails.RemoveAt(index);
#endif
			}
		}
	}
	
	/// <summary>
	/// Creates the SFX group and adds it to SoundManager.
	/// </summary>
	public static SFXGroup CreateSFXGroup(string grpName, int capAmount)
	{
		if(!Instance.groups.ContainsKey(grpName))
		{
			SFXGroup grp = new SFXGroup(grpName, capAmount);
			Instance.groups.Add(grpName, grp);
#if UNITY_EDITOR
			Instance.sfxGroups.Add(grp);
#endif
			return grp;
		}
		Debug.LogWarning("This group already exists. Cannot add it.");
		return null;
	}
	
	/// <summary>
	/// Creates the SFX group and adds it to SoundManager.
	/// </summary>
	public static SFXGroup CreateSFXGroup(string grpName)
	{
		if(!Instance.groups.ContainsKey(grpName))
		{
			SFXGroup grp = new SFXGroup(grpName);
			Instance.groups.Add(grpName, grp);
#if UNITY_EDITOR
			Instance.sfxGroups.Add(grp);
#endif
			return grp;
		}
		Debug.LogWarning("This group already exists. Cannot add it.");
		return null;
	}
	
	/// <summary>
	/// Moves a clip to the specified group. If the group doesn't exist, it will make the group.
	/// </summary>
	public static void MoveToSFXGroup(string clipName, string newGroupName)
	{
		Instance.SetClipToGroup(clipName, newGroupName);
	}
	
	public static void RemoveFromSFXGroup(string clipName)
	{
		Instance.RemoveClipFromGroup(clipName);
	}
	
	/// <summary>
	/// Loads a random SFX from a specified group.
	/// </summary>
	public static AudioClip LoadFromGroup(string grpName)
	{
		SFXGroup grp = Instance.GetGroupByGroupName(grpName);
		if(grp == null)
		{
			Debug.LogError("There is no group by this name: "+grpName+".");
			return null;
		}
		
		AudioClip result = null;
		
		// check if clips is empty
		if(grp.clips.Count == 0)
		{
			Debug.LogWarning("There are no clips in this group: " + grpName);
			return null;
		}
		
		// Get random clip from list
		result = grp.clips[Random.Range(0, grp.clips.Count)];
		
		// return result
		return result;
	}
	
	/// <summary>
	/// Loads all SFX from a specified group.
	/// </summary>
	public static AudioClip[] LoadAllFromGroup(string grpName)
	{
		SFXGroup grp = Instance.GetGroupByGroupName(grpName);
		if(grp == null)
		{
			Debug.LogError("There is no group by this name, "+grpName+".");
			return null;
		}
		
		// check if group is empty
		if(grp.clips.Count == 0)
		{
			Debug.LogWarning("There are no clips in this group: " + grpName);
			return null;
		}
		
		// return all clips in array
		return grp.clips.ToArray();
	}
	
	/// <summary>
	/// Load the specified clipname, at a custom path if you do not want to use resourcesPath.
	/// If custompath fails or is empty/null, it will query the stored SFXs.  If that fails, it'll query the default
	/// resourcesPath.  If all else fails, it'll return null.
	/// </summary>
	/// <param name='clipname'>
	/// Clipname.
	/// </param>
	/// <param name='customPath'>
	/// Custom path.
	/// </param>
	public static AudioClip Load(string clipname, string customPath)
	{
		AudioClip result = null;
		
		// Attempt to use custom path if provided
		if(!string.IsNullOrEmpty(customPath))
			if(customPath[customPath.Length-1] == '/')
				result = (AudioClip)Resources.Load(customPath.Substring(0,customPath.Length) + "/" + clipname);
			else
				result = (AudioClip)Resources.Load(customPath + "/" + clipname);
				
		if(result)
			return result;
		
		// If custom path fails, attempt to find it in our stored SFXs
		if(Instance.allClips.ContainsKey(clipname))
			result = Instance.allClips[clipname];
		
		if(result)
			return result;
		
		// If it is not in our stored SFX, attempt to find it in our default resources path
		result = (AudioClip)Resources.Load(Instance.resourcesPath + "/" + clipname);
		
		return result;
	}
	
	/// <summary>
	/// Load the specified clipname from the stored SFXs.  If that fails, it'll query the default
	/// resourcesPath.  If all else fails, it'll return null.
	/// </summary>
	/// <param name='clipname'>
	/// Clipname.
	/// </param>
	public static AudioClip Load(string clipname)
	{
		return Load(clipname, "");
	}
	
	/// <summary>
	/// Resets the SFX object.
	/// </summary>
	public static void ResetSFXObject(GameObject sfxObj)
	{
		if(sfxObj.GetComponent<AudioSource>() == null)
			return;
		
		sfxObj.GetComponent<AudioSource>().mute = false;
		sfxObj.GetComponent<AudioSource>().bypassEffects = false;
		sfxObj.GetComponent<AudioSource>().playOnAwake = false;
		sfxObj.GetComponent<AudioSource>().loop = false;
		
		sfxObj.GetComponent<AudioSource>().priority = 128;
		sfxObj.GetComponent<AudioSource>().volume = 1f;
		sfxObj.GetComponent<AudioSource>().pitch = 1f;
		
		sfxObj.GetComponent<AudioSource>().dopplerLevel = 1f;
		sfxObj.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Logarithmic;
		sfxObj.GetComponent<AudioSource>().minDistance = 1f;
		sfxObj.GetComponent<AudioSource>().spatialBlend = 1f;
		sfxObj.GetComponent<AudioSource>().spread = 0f;
		sfxObj.GetComponent<AudioSource>().maxDistance = 500f;
		
		sfxObj.GetComponent<AudioSource>().panStereo = 0f;
	}
	
	public static void Crossfade(float duration, AudioSource fromSource, AudioSource toSource, SongCallBack runOnEndFunction=null)
	{
		Instance.StartCoroutine(Instance.XFade(duration, fromSource, toSource, runOnEndFunction));
	}
	
	public static void Crossfade(float duration, GameObject fromSFXObject, GameObject toSFXObject, SongCallBack runOnEndFunction=null)
	{
		Crossfade(duration, fromSFXObject.GetComponent<AudioSource>(), toSFXObject.GetComponent<AudioSource>(), runOnEndFunction);
	}
	
	public static void CrossIn(float duration, AudioSource source, SongCallBack runOnEndFunction=null)
	{
		Instance.StartCoroutine(Instance.XIn(duration, source, runOnEndFunction));
	}
	
	public static void CrossIn(float duration, GameObject sfxObject, SongCallBack runOnEndFunction=null)
	{
		CrossIn(duration, sfxObject.GetComponent<AudioSource>(), runOnEndFunction);
	}
	
	public static void CrossOut(float duration, AudioSource source, SongCallBack runOnEndFunction=null)
	{
		Instance.StartCoroutine(Instance.XOut(duration, source, runOnEndFunction));
	}
	
	public static void CrossOut(float duration, GameObject sfxObject, SongCallBack runOnEndFunction=null)
	{
		CrossOut(duration, sfxObject.GetComponent<AudioSource>(), runOnEndFunction);
	}
}
