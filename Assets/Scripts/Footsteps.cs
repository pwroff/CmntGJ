using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{

    public AudioSource audioSource;

    public enum FootstepTypes { shoes, barefoot, grass, sand }
    public FootstepTypes footstepType;

    public AudioClip[] shoeSounds;
    public AudioClip[] barefootSounds;
    public AudioClip[] grassSounds;
    public AudioClip[] sandSounds;
    float lastPlayed;

    GameObject footstepParent;
    public GameObject footstepGraphic;
    public float footstepYOffset = 0.01f;

    private void Awake()
    {
        footstepParent = new GameObject("Footstep Group");
    }

    void Update() {
		CheckGroundType();
	}
    void CheckGroundType() {
		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;
		Debug.DrawRay(ray.origin, ray.direction);
		if (Physics.Raycast(ray, out hit)) {
            if (hit.collider.CompareTag("Shoes")) footstepType = FootstepTypes.shoes;
            if (hit.collider.CompareTag("Barefoot")) footstepType = FootstepTypes.barefoot;
            if (hit.collider.CompareTag("Grass")) footstepType = FootstepTypes.grass;
            if (hit.collider.CompareTag("Sand")) footstepType = FootstepTypes.sand;
        }
    }


	public void PlayFootstepSound() {
        //stop accidental double fires of audio
        if (Time.time - lastPlayed < 0.1) return;

        AudioClip selectedClip;
        switch (footstepType) {
            default://shoes
                { 
                    int whichSound = Random.Range(0, shoeSounds.Length - 1);
                    selectedClip = shoeSounds[whichSound];
                    break;
                }
            case  FootstepTypes.barefoot:
                {
                    int whichSound = Random.Range(0, barefootSounds.Length - 1);
                    selectedClip = barefootSounds[whichSound];
                    break;
                }
            case FootstepTypes.grass:
                {
                    int whichSound = Random.Range(0, grassSounds.Length - 1);
                    selectedClip = grassSounds[whichSound];
                    break;
                }
            case FootstepTypes.sand:
                {
                    int whichSound = Random.Range(0, sandSounds.Length - 1);
                    selectedClip = sandSounds[whichSound];
                    break;
                }
        }
        lastPlayed = Time.time;
        audioSource.PlayOneShot(selectedClip);
        PlaceFootstepGraphic();
    }
    
    void PlaceFootstepGraphic() {        
        GameObject newFootstepGraphic = Instantiate(footstepGraphic, transform.position + new Vector3(0, footstepYOffset, 0), Quaternion.identity, footstepParent.transform);
        newFootstepGraphic.transform.forward = transform.forward;
    }
}
