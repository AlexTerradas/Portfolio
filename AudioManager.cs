using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;

	public AudioMixerGroup mixerGroup;

	public Sound[] sounds;

	public AudioMixer masterMixer;

	private String currentScene;

    private void Start()
    {
		ChooseMusicWithLevel();
	}

    private void Update()
    {
        if (currentScene != SceneManager.GetActiveScene().name)
        {
			ChooseMusicWithLevel();
		}

    }

    void Awake()
	{
		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;

			s.source.outputAudioMixerGroup = mixerGroup;
		}
	}

    public void Play(string sound)
	{

		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Play();
	}

	public void SetSound(float soundLevel)
	{
		masterMixer.SetFloat("MasterVol", soundLevel );
		Debug.Log(soundLevel);
	}

	public void ChooseMusicWithLevel()
    {

		if(SceneManager.GetActiveScene().name == "Level1")
        {
			Play("Theme");
			currentScene = SceneManager.GetActiveScene().name;
		}

		if (SceneManager.GetActiveScene().name == "Menu")
		{
			Play("Menu");
			currentScene = SceneManager.GetActiveScene().name;
		}

		if (SceneManager.GetActiveScene().name == "GameOver")
		{
			Play("GameOver");
			currentScene = SceneManager.GetActiveScene().name;
		}

	}

}