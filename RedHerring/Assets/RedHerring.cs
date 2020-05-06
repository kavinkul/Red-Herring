using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class RedHerring : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable Chungun;
    public GameObject Chungus;
    public Material[] Colors;
    public GameObject[] Status;
    KMAudio.KMAudioRef sound;
    public GameObject[] NoiseMakers;

	//Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved = false;
	
    bool active = false;
    int DistractionPicker = 0;
    private List<string> Distractions = new List<string>{"Swan","Door1","Door2","Glass","DoubleOh","Needy","DiscordCall","DiscordJoin","DiscordLeave","FuckingNothing"};
    float Time = 0f;
	float ActualTime;
	
    bool WindowofPress = false;
    bool Started = false;
    bool Pressed = false;
    bool TogglePress = false;
    bool CanPress = false;
	
	#pragma warning disable 0649
    private bool TwitchPlaysActive;
    #pragma warning restore 0649

    void Awake()
	{
        moduleId = moduleIdCounter++;
		GetComponent<KMBombModule>().OnActivate += RedHerringInTP;
        Chungun.OnInteract += delegate () { PressChungun(); return false; };
    }
	
    void Start()
	{
      DistractionPicker = UnityEngine.Random.Range(0,Distractions.Count());
      Status[0].SetActive(false);
      Status[1].SetActive(false);
      Status[2].SetActive(true);
	}
	
	void RedHerringInTP()
	{
		ActualTime = TwitchPlaysActive ? 5f : 0.5f;
	}
	
	void PressChungun()
	{
		Chungun.AddInteractionPunch();
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Chungun.transform);
		if (TogglePress == false)
		{
			Debug.LogFormat("[Red Herring #{0}] You have started the timer.", moduleId);
			TogglePress = true;
			StartCoroutine(StartThing());
			switch (DistractionPicker)
			{
				case 0:
				StartCoroutine(Swan());
				break;
				case 1:
				StartCoroutine(Door1Noise());
				break;
				case 2:
				StartCoroutine(Door2Noise());
				break;
				case 3:
				StartCoroutine(GlassNoise());
				break;
				case 4:
				StartCoroutine(DoubleOhStrikeTime());
				break;
				case 5:
				StartCoroutine(NeedyDistract());
				break;
				case 6:
				StartCoroutine(Discord1());
				break;
				case 7:
				StartCoroutine(Discord2());
				break;
				case 8:
				StartCoroutine(Discord3());
				break;
				case 9:
				return;
				break;
			}
		}
		
		else
		{
			if (CanPress == true)
			{
				GetComponent<KMBombModule>().HandlePass();
				Chungus.GetComponent<MeshRenderer>().material = Colors[1];
				Status[2].SetActive(false);
				Status[1].SetActive(true);
				moduleSolved = true;
			}
			
			else
			{
				StartCoroutine(StrikeCoroutine());
			}
		}
	}
	
	IEnumerator Swan()
	{
		yield return new WaitForSeconds(1.2f);
		while(Started == true)
		{
			yield return new WaitForSeconds(1.8f);
			Audio.PlaySoundAtTransform("Swan", transform);
		}
	}
	
	IEnumerator StartThing()
	{
		while (TogglePress == true)
		{
			Started = true;
			Time = UnityEngine.Random.Range(7f,15f);
			yield return new WaitForSeconds(Time);
			Chungus.GetComponent<MeshRenderer>().material = Colors[1];
			CanPress = true;
			yield return new WaitForSeconds(ActualTime);
			Chungus.GetComponent<MeshRenderer>().material = Colors[0];
			CanPress = false;
			Started = false;
			if (moduleSolved == true)
			{
				  TogglePress = false;
				  yield return null;
			}
			
			else
			{
			  TogglePress = false;
			  Debug.LogFormat("[Red Herring #{0}] You didn't press in time. Strike, slow poke.", moduleId);
			  GetComponent<KMBombModule>().HandleStrike();
			  yield return null;
			}
		}
	}
	
	IEnumerator Door1Noise()
	{
		yield return new WaitForSeconds(Time - 1.5f);
		Audio.PlaySoundAtTransform("Door1", NoiseMakers[0].transform);
	}
	
	IEnumerator Door2Noise()
	{
		yield return new WaitForSeconds(Time - 1.75f);
		Audio.PlaySoundAtTransform("Door2", NoiseMakers[1].transform);
	}
	
	IEnumerator GlassNoise()
	{
		yield return new WaitForSeconds(Time - 1.25f);
		Audio.PlaySoundAtTransform("Glass", NoiseMakers[0].transform);
	}
	
	IEnumerator NeedyDistract()
	{
		yield return new WaitForSeconds(Time - 4f);
		sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.NeedyWarning, transform);
		yield return new WaitForSeconds(4f);
		sound.StopSound();
		sound = null;
	}
	
	IEnumerator StrikeCoroutine()
	{
		Debug.LogFormat("[Red Herring #{0}] You pressed too early. Strike, pin head.", moduleId);
		GetComponent<KMBombModule>().HandleStrike();
		TogglePress = false;
		Status[0].SetActive(true);
		Status[2].SetActive(false);
		yield return new WaitForSeconds(1f);
		Status[0].SetActive(false);
		Status[2].SetActive(true);
		DistractionPicker = UnityEngine.Random.Range(0,Distractions.Count());
		TogglePress = false;
		StopAllCoroutines();
	}
	
	IEnumerator DoubleOhStrikeTime()
	{
		yield return new WaitForSeconds(4f);
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.Strike, Chungun.transform);
		Status[0].SetActive(true);
		Status[2].SetActive(false);
		yield return new WaitForSeconds(1f);
		Status[0].SetActive(false);
		Status[2].SetActive(true);
	}
	
	IEnumerator Discord1()
	{
		yield return new WaitForSeconds(Time - 6f);
		Audio.PlaySoundAtTransform("DiscordCall", transform);
	}
	
	IEnumerator Discord2()
	{
		yield return new WaitForSeconds(Time - 2f);
		Audio.PlaySoundAtTransform("DiscordJoin", transform);
	}
	
	IEnumerator Discord3()
	{
		yield return new WaitForSeconds(Time - 2f);
		Audio.PlaySoundAtTransform("DiscordLeave", transform);
	}
	
	//twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"To push the button in the module, do !{0} push. (You have to respond before 5 seconds passes after it changes color to avoid striking)";
    #pragma warning restore 414
	
	IEnumerator ProcessTwitchCommand(string command)
	{
		string[] parameters = command.Split(' ');
		if (Regex.IsMatch(parameters[0], @"^\s*push\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
			yield return null;
			Chungun.OnInteract();
		}
	}
}
