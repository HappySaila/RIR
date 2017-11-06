using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemManager : MonoBehaviour {
    public static ParticleSystemManager instance;

    public GameObject LaborerSparks;
    public GameObject DeathParticles;
    public GameObject RespawnRed;
    public GameObject RespawnBlue;
    public GameObject TimeMachineSteam;
    public GameObject HelpingUp;
    public GameObject EnvironmentDust;

    public Color RedSteamStartColor;
    public Color BlueSteamStartColor;

    private void Awake()
    {
        if (instance == null){
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }
   

}
