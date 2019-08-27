using UnityEngine;
using System.Collections;

public class CharacterAudio : MonoBehaviour {
	public float navMeshSpeed;
	public float speed = 0;
	public float lastSpeed = 0;
	float lastFootStep = -1;
	public float waitBetweenFootsteps = 0.02f;
	public AudioClip[] sandFootstepSounds;
	public AudioClip[] grassFootstepSounds;
	public AudioClip[] rockFootstepSounds;
	//public bool isGrass;
	public bool isBarefoot;
	public bool debugFootsepts;
	public AudioClip softLanding;
	public AudioClip hardLanding;
	public AudioClip jumpTakeoff;
	public bool demoFootstep;
	public CharacterController charController;
	public enum footstepSurfaces { sand , rock }
	public footstepSurfaces groundType;

	void Update(){
		if (demoFootstep){
			demoFootstep = false;
			PlayFootstep();
		}
	}

	public void HardLanding(){
		
		GetComponent<AudioSource>().PlayOneShot(hardLanding);
	}
	public void SoftLanding(){
		
		GetComponent<AudioSource>().PlayOneShot(softLanding);		
	}

	public void StartJumping(){
		GetComponent<AudioSource>().PlayOneShot(jumpTakeoff);		

	}

	public void PlayFootstep(){
		if (debugFootsepts) Debug.Log("FOOTSTEP " + Time.time);

		if (charController.isGrounded){			
			//check for hard floor surface (buildings, rocks)

			if (GetComponent<AudioSource>()) {
				if (Time.time > lastFootStep + waitBetweenFootsteps){				
					/*
					if (hardFloor){
						if (isGrass){
							GetComponent<AudioSource>().clip = grassFootstepSounds[Random.Range(0,grassFootstepSounds.Length)];												
						} else {
							GetComponent<AudioSource>().clip = rockFootstepSounds[Random.Range(0,rockFootstepSounds.Length)];						
						}
					} else { //outside use sand
						GetComponent<AudioSource>().clip = sandFootstepSounds[Random.Range(0,sandFootstepSounds.Length)];												
					}
					*/
					switch(groundType){
					case footstepSurfaces.rock: 						
						GetComponent<AudioSource>().clip = rockFootstepSounds[Random.Range(0,rockFootstepSounds.Length)]; 
						break;
					case footstepSurfaces.sand: 
						//GetComponent<AudioSource>().clip = grassFootstepSounds[Random.Range(0,grassFootstepSounds.Length)]; 
						GetComponent<AudioSource>().clip = sandFootstepSounds[Random.Range(0,sandFootstepSounds.Length)]; 
						break;						
					}


					GetComponent<AudioSource>().Play();
					lastFootStep = Time.time;				
				}
			}
		}	
	}
}
