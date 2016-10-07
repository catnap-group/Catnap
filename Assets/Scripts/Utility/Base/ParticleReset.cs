using UnityEngine;
using System.Collections;
using System;

public class ParticleReset : MonoBehaviour {
	#if !EE
	bool stop= false;
    private ParticleSystem[] m_particleArr;
	public float fadeTime = 0.0f;
    void Start()
    {
    }
    void OnEnable()
    {
        if (m_particleArr==null)
            m_particleArr = gameObject.GetComponentsInChildren<ParticleSystem>();
		stop = false;
        foreach (var item in m_particleArr)
        {
            ParticleSystem.EmissionModule psemit = item.emission;
            psemit.enabled = true;
            item.Simulate(0.0f, true, true);
			if (item.GetComponent<Renderer> ().material.HasProperty ("_TintColor")) {
				Color c = item.GetComponent<Renderer> ().material.GetColor ("_TintColor");
				c.a = 1.0f;
				item.GetComponent<Renderer> ().material.SetColor ("_TintColor", c);
			}
            item.Clear();
            item.Play();
            item.enableEmission = true;
        }
    }
//	IEnumerator CheckIfAlive(ParticleSystem item)
//	{
//		while (item.IsAlive ()) {
//			yield return new WaitForSeconds (0.2f);
//		}
//		item.Clear();
//		item.enableEmission = false;
//	}
	void Update()
	{
		if (stop) {
			foreach (var item in m_particleArr) {
				ParticleSystem.Particle[] particles = new ParticleSystem.Particle[item.particleCount];
				ParticleSystem.EmissionModule psemit = item.emission;
				psemit.enabled = false;
				//item.Stop();
				//get the particles
				item.GetParticles (particles);
                if (fadeTime>0)
                {
				    if (item.GetComponent<Renderer> ().material.HasProperty ("_TintColor")) {
					    Color c = item.GetComponent<Renderer> ().material.GetColor ("_TintColor");
					    c.a = Mathf.Lerp (1.0f, 0.0f, Time.deltaTime * 1.0f/fadeTime);
					    item.GetComponent<Renderer> ().material.SetColor ("_TintColor", c);
				    }
                }

			}
		}

	}

    public void Reset()
    {

    }
    void OnDisable()
    {
        if (m_particleArr == null)
            m_particleArr = gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (var item in m_particleArr)
        {
            ParticleSystem.EmissionModule psemit = item.emission;
            psemit.enabled = false;
            item.time = 0;
            item.Stop();
//			if(item.IsAlive())
//			StartCoroutine( CheckIfAlive(item));
        }
    }
	#endif
	public void Fade(Action onFadeOver=null)
	{
		stop = true;
	}
}
