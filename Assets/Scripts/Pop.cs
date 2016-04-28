using UnityEngine;
using System.Collections;

public enum PopType {
    CURE,
    INFECT,
}

public class Pop : MonoBehaviour {
    const float EXPIRE_TIME = 0.2f;
    public PopType type;
    
	// Use this for initialization
	void Start () {
        StartCoroutine(Expire());
	}
	
	IEnumerator Expire () {
        yield return new WaitForSeconds(EXPIRE_TIME);
        ParticleSystem.EmissionModule module = GetComponent<ParticleSystem>().emission;
        module.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(EXPIRE_TIME);
        Destroy(gameObject);
    }
}
