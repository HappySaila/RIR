using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {

	public static SoundManager INSTANCE;

    public float Volume;

	public AudioClip[] AudioClips;
	public AudioClip[] AudioClipsForGettingUp;

	public AudioClip UIClick;
	public AudioClip hoveringRobot;
	public AudioClip ramHit;
	public AudioClip hammer;
	public AudioClip stab;
	public AudioClip robotScene;
	public AudioClip gladiatorScene;
	public AudioClip swoosh;
	public AudioClip Swing;

	public AudioClip GameOverSound;
	AudioSource gameOverAudioSource;

    float lowPitch = 0.8f;
	float highPitch = 1.2f;
	float lowVolume = 0.6f;
	float highVolume = 1f;

	int hitSoundCallCount;

	void Awake(){
		
		DontDestroyOnLoad (gameObject);
		if (INSTANCE == null) {
			INSTANCE = this;
			gameOverAudioSource = GetComponent<AudioSource>();
        } else {
            Destroy (gameObject);
        }
	}

    private void Start()
    {
		gameOverAudioSource = GetComponent<AudioSource>();
	}
    private void Update()
	{
		//to ensure that when alot of things are hit that it dosnt sound terrible.... Improves sfx feal
		if (hitSoundCallCount >0) {
			hitSoundCallCount--;
		}
	}
	public void PlayRamHit(AudioSource s){
		hitSoundCallCount++;
		if(hitSoundCallCount<2){
			s.PlayOneShot(ramHit);
		}


	}
	public void PlayLabourPickUp(AudioSource s){
		s.PlayOneShot(AudioClipsForGettingUp[Random.Range(0,AudioClipsForGettingUp.Length)]);
	}
	public void PlayHammer(AudioSource s){
		s.PlayOneShot(hammer);
	}
	public void PlayButtonClicked(AudioSource s){
		s.PlayOneShot(UIClick);
	}

	public void PlayGameOverSound(){
        gameOverAudioSource.PlayOneShot(GameOverSound, 0.5f);
	}

	public void PlayRobotScene()
	{
        gameOverAudioSource.PlayOneShot(robotScene, 1);
	}

	public void PlayGladiatorScene()
	{
        gameOverAudioSource.PlayOneShot(gladiatorScene, 1);
	}

	public void PlaySwoosh()
	{
		gameOverAudioSource.PlayOneShot(swoosh, 1);
	}

	public void PlayStab(AudioSource s)
	{
        if (canStab){
			s.PlayOneShot(stab);
            canStab = false;
		}
        Invoke("StopStab", 0.5f);    
	}
    bool canStab = true;

    void StopStab(){
        canStab = true;
    }

    public void PlaySwing(AudioSource s){
        BendPitch(s);
		s.PlayOneShot(Swing);
	}



    void BendPitch(AudioSource source){
		source.pitch = Random.Range (lowPitch, highPitch);
	}

	void BendVolume(AudioSource source){
		source.volume = Random.Range (lowVolume, highVolume);
	}

	void SetVolume(AudioSource source, float value){
		source.volume = value;
	}

    public IEnumerator FadeOut(AudioSource audioSource, float FadeTime) {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
