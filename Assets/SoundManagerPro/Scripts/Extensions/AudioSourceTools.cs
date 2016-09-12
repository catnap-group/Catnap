using UnityEngine;
using System.Collections;

public static class AudioSourceTools {

	public static void PlaySFX ( ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name, bool loop, float volume, float pitch)
    {
        SoundManager.PlaySFX(theAudioSource, fromGroup ? SoundManager.LoadFromGroup(clipOrGroup_Name) : SoundManager.Load(clipOrGroup_Name), loop, volume, pitch);
    }
	
	public static void PlaySFX ( ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name, bool loop, float volume)
    {
        SoundManager.PlaySFX(theAudioSource, fromGroup ? SoundManager.LoadFromGroup(clipOrGroup_Name) : SoundManager.Load(clipOrGroup_Name), loop, volume);
    }
	
	public static void PlaySFX ( ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name, bool loop)
    {
        SoundManager.PlaySFX(theAudioSource, fromGroup ? SoundManager.LoadFromGroup(clipOrGroup_Name) : SoundManager.Load(clipOrGroup_Name), loop);
    }
	
	public static void PlaySFX ( ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name)
    {
        SoundManager.PlaySFX(theAudioSource, fromGroup ? SoundManager.LoadFromGroup(clipOrGroup_Name) : SoundManager.Load(clipOrGroup_Name));
    }
	
	public static void PlaySFX ( ref AudioSource theAudioSource, AudioClip clip, bool loop, float volume, float pitch)
    {
        SoundManager.PlaySFX(theAudioSource, clip, loop, volume, pitch);
    }
	
	public static void PlaySFX ( ref AudioSource theAudioSource, AudioClip clip, bool loop, float volume)
    {
        SoundManager.PlaySFX(theAudioSource, clip, loop, volume);
    }
	
	public static void PlaySFX ( ref AudioSource theAudioSource, AudioClip clip, bool loop)
    {
        SoundManager.PlaySFX(theAudioSource, clip, loop);
    }
	
	public static void PlaySFX ( ref AudioSource theAudioSource, AudioClip clip)
    {
        SoundManager.PlaySFX(theAudioSource, clip);
    }
	
    public static void StopSFX ( ref AudioSource theAudioSource )
    {
        SoundManager.StopSFXObject(theAudioSource);
    }
	
    public static void PlaySFXLoop( ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name, bool tillDestroy, float volume, float pitch, float maxDuration)
    {
		SoundManager.PlaySFXLoop(theAudioSource, fromGroup ? SoundManager.LoadFromGroup(clipOrGroup_Name) : SoundManager.Load(clipOrGroup_Name), tillDestroy, volume, pitch, maxDuration);
    }
	
	public static void PlaySFXLoop( ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name, bool tillDestroy, float volume, float pitch)
    {
        SoundManager.PlaySFXLoop(theAudioSource, fromGroup ? SoundManager.LoadFromGroup(clipOrGroup_Name) : SoundManager.Load(clipOrGroup_Name), tillDestroy, volume, pitch);
    }
	
	public static void PlaySFXLoop( ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name, bool tillDestroy, float volume)
    {
        SoundManager.PlaySFXLoop(theAudioSource, fromGroup ? SoundManager.LoadFromGroup(clipOrGroup_Name) : SoundManager.Load(clipOrGroup_Name), tillDestroy, volume);
    }
	
	public static void PlaySFXLoop( ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name, bool tillDestroy)
    {
        SoundManager.PlaySFXLoop(theAudioSource, fromGroup ? SoundManager.LoadFromGroup(clipOrGroup_Name) : SoundManager.Load(clipOrGroup_Name), tillDestroy);
    }
	
	public static void PlaySFXLoop( ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name)
    {
        SoundManager.PlaySFXLoop(theAudioSource, fromGroup ? SoundManager.LoadFromGroup(clipOrGroup_Name) : SoundManager.Load(clipOrGroup_Name));
    }
}
