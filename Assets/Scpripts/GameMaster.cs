﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//Class to hold pendelum Fitness and DNA Data
public class BestPendelumDNA
{

    public float Fitness;
    public float[] DNA;

    public BestPendelumDNA(float fitness, float[] dna)
    {
        DNA = new float[dna.Length];
        Fitness = fitness;
        for (int i = 0; i < dna.Length; i++)
        {
            DNA[i] = dna[i];
        }
        
    }

} 


public class GameMaster : MonoBehaviour {

    public int PendelumAmount; //How many pendelums to instantiate per generation
    public GameObject pendelum; //pendelum gameobject to be instantiated
    public GameObject SpawnPoint; //spawn point for pendelums
    public GameObject BestSpawnpoint; //Spawn point for the best pendelums
    public int DNALength = 500; //length of the pendelum DNA ... ie number of instructions
    public int NumberOfPendelums = 100; //How many pendelums are instantiated per generation
    public float AliveTime = 5f; //How long to simulate in seconds
    public List<GameObject> ListOfPendelums; //this is a list that holds all the instantiated pendelums
    public List<GameObject> MatingPool; //This list is used for mating the pendelums after simulation
    public List<GameObject> ListOfBestPendelums; //This list holds instantiated best pendelums
    public List<BestPendelumDNA> BestPendelums; //This list holds the fitness and dna of the best pendelums
    public int Mutationthreshold = 10; //how many % of the DNA of a new pendelum is mutated
    public Text Info; //GUI text of the current best pendelums fitness and number of generations
    public Text StartSimulationText; //button text for starting simulation
    public Text TimeScaleText; // button text for timescale button

    public float MinMaxMovementForce = 5f; //Max force applied to the pendelums
    

    private float AliverTimer; // used to measure simulation time
    private bool IsSimulationRunning = true; //True if simulation is running.
    private int generation = 1; //generation count
    private bool RunNextGeneration = false; //if true, run next generation of pendelums
    private int AccelerateTime; //used for time flow control
    private float AvarageFitness = 0f;
   
	// Use this for initialization
	void Start () {
        
        AliverTimer = 0f; //Reset the time
        AccelerateTime = 1; //set timemultiplier to 1

        //Initialize all the lists
        ListOfPendelums = new List<GameObject>();
        MatingPool = new List<GameObject>();
        BestPendelums = new List<BestPendelumDNA>();
        ListOfBestPendelums = new List<GameObject>();

        //instantiate the first generation of pendelums
        AddPendelums(NumberOfPendelums);
        //Add dna to the just instantiated pendelums
        for (int i = 0; i < NumberOfPendelums; i++)
        {
            ListOfPendelums[i].gameObject.GetComponent<Pendelum>().SetDNA(RandomizeDNA(DNALength));
        }
    }

    //changes the state of the simulation from running to stopped. Controlled by the start simulation button
    public void StartSimulation() 
    {

        RunNextGeneration = !RunNextGeneration;
        if (RunNextGeneration == false) StartSimulationText.text = "Start simulation";
        if (RunNextGeneration == true) StartSimulationText.text = "Running";
        //print(IsSimulationRunning);
    }

    //Changes the speed of simulation 1/2/4/8/16x
    public void SetTimeScale()
    {

        if (AccelerateTime == 1)
        {
            AccelerateTime = 2;
            TimeScaleText.text = "1x";
            Time.timeScale = 1f;
            return;
        }

        if (AccelerateTime == 2)
        {
            AccelerateTime = 3;
            TimeScaleText.text = "2x";
            Time.timeScale = 2f;
            return;
        }

        if (AccelerateTime == 3)
        {
            AccelerateTime = 4;
            TimeScaleText.text = "4x";
            Time.timeScale = 4f;
            return;
        }
        if (AccelerateTime == 4)
        {
            AccelerateTime = 5;
            TimeScaleText.text = "8x";
            Time.timeScale = 8f;
            return;
        }
        if (AccelerateTime == 5)
        {
            AccelerateTime = 1;
            TimeScaleText.text = "16x";
            Time.timeScale = 16f;
            return;
        }
    }

	// Update is called once per frame
	void Update () {

        //If simulation is running, keep oon running till it stops
        if (IsSimulationRunning == true) RunSimulation();

        //If the last simulation has stopped, and the flag to run the next on is set, get new generation going
        if (RunNextGeneration == true && IsSimulationRunning == false)
        {
            NewGeneration();
            InstantiateBestPendelums();
        }

    }

    //instantiates new generation of pendelums int amount is the amount of new pendelums
    void AddPendelums (int amount)
    {
        int j = 0;
        int k = 0;
        float offset = 0.3f;

        for (int i = 0; i < amount; i++)
        {
            if (k >= 5)
            {
                k = 0;
                j++;
            }
            k++;
            offset = offset * -1;
            ListOfPendelums.Add((GameObject)Instantiate(pendelum, new Vector3(SpawnPoint.transform.position.x + offset, SpawnPoint.transform.position.y + k*12, SpawnPoint.transform.position.z - j*12), Quaternion.identity));
        }
     }

    //this randomizes the DNA for new pendelums
    float[] RandomizeDNA(int length)
    {
        float[] DNA;
        DNA = new float[length];

        for (int i = 0; i < DNA.Length; i++)
        {
            DNA[i] = Random.Range(-MinMaxMovementForce, MinMaxMovementForce);
            //print(i);
            //print("DNA:" + DNA[i] + " " + i );

        }
        return DNA;
    }

    //Mixes 2 DNAs by introducing mutation. or by slicing two DNAs together... or both
    float[] MixDNA(float[] DNA1, float[] DNA2)
    {
        float[] MixedDNA;
        MixedDNA = new float[DNA1.Length];

        //MIXING THE DNA
        int Cut;
        Cut = Random.Range(0, DNA1.Length);
        //print(Cut);

        for (int i = 0; i < DNA1.Length; i++)
        {
            if (i < Cut)
            {
                MixedDNA[i] = DNA1[i];
            }
            else
            {
                MixedDNA[i] = DNA2[i];
            }
            if (Random.Range(0, 100) > Mutationthreshold) MixedDNA[i] = Random.Range(-MinMaxMovementForce, MinMaxMovementForce);

            
        }

        return MixedDNA; //Change to MixedDNA once impelemted
    }

    //Simulation running
    void RunSimulation()
    {
        AliverTimer += Time.deltaTime;
        if (AliverTimer >= AliveTime) //Simulation has run enough. stop it
        {
            IsSimulationRunning = false;

            //Shut down all the pendelums.
            for (int i = 0; i < NumberOfPendelums; i++){
               ListOfPendelums[i].GetComponent<Pendelum>().Deactivate();
            }
            
            
            //NewGeneration();
            //If simulation has runned for long enough, stop it
        }
    }

    //Gets new generation of pendelums going
    void NewGeneration()
    {
        //print(AliverTimer);
        AliverTimer = 0f; // nollataan aikalaskuri, koska simulaatio ei ole käynnissä.
        generation += 1;

        //go through all pendelums and sort by their fitness
        ListOfPendelums.Sort(delegate (GameObject a, GameObject b)
        {
            return (a.GetComponent<Pendelum>().Fitness).CompareTo(b.GetComponent<Pendelum>().Fitness);
        });
        ListOfPendelums.Reverse();
        //print(ListOfPendelums[0].GetComponent<Pendelum>().Fitness);//print the best fitness to console

        //Add the best pendelum DNA and fitness to a list containing the best of each generation.
        BestPendelums.Add(new BestPendelumDNA(ListOfPendelums[0].GetComponent<Pendelum>().Fitness, ListOfPendelums[0].GetComponent<Pendelum>().GetDNA()));

        //calculate avarage fitness of the pendelums
        for (int i = 0; i < ListOfPendelums.Count; i++)
        {
            AvarageFitness += ListOfPendelums[i].GetComponent<Pendelum>().Fitness;
        }
        AvarageFitness = AvarageFitness / ListOfPendelums.Count;

        //Update info to the gui of the just run generation
        Info.text = "Best this generation:\n" + ListOfPendelums[1].GetComponent<Pendelum>().Fitness + "\nThis generation avarage:\n"+ AvarageFitness +"\nBest ever:\n"+ BestPendelums[0].Fitness + "\nGeneration: " + generation;

        //Create a mating pool for the pendelums

        for (int i = 0; i < NumberOfPendelums/2; i++)
        {
            for (int j = 0; j < NumberOfPendelums/2 - i; j++)
            {
                MatingPool.Add(ListOfPendelums[i]);
            }

        }

        //Destroy all the pendelums before recreating them.
        for (int i = 0; i < NumberOfPendelums; i++)
        {
            //print(ListOfPendelums[i].GetComponent<Pendelum>().Fitness);
            GameObject thing = ListOfPendelums[i];
            Destroy(thing); //Destroy the pendelums at the end.  
        }
        ListOfPendelums.Clear();

        //New pendelums to the arena!!!!!!
        AddPendelums(NumberOfPendelums); //Add new pendelums to the simulation
        //add Mixed dna form the mating pool to the new generation of pendelums
        for (int i = 0; i < NumberOfPendelums; i++)
        {
            ListOfPendelums[i].gameObject.GetComponent<Pendelum>().SetDNA(MixDNA(MatingPool[Random.Range(0, MatingPool.Count)].gameObject.GetComponent<Pendelum>().GetDNA(), MatingPool[Random.Range(0, MatingPool.Count)].gameObject.GetComponent<Pendelum>().GetDNA()));
        }
        MatingPool.Clear(); //Kill the last generation of pendelums

        //Instantiate the best pendelums

        IsSimulationRunning = true; //Lets start the simulation again
    }

    //Used to instantiate the best pendelums.
    void InstantiateBestPendelums()
    {
        //print(BestPendelums.Count);
        //Destroy the old pendelums
        for (int i = 0; i < ListOfBestPendelums.Count; i++)
        {
            //print(ListOfPendelums[i].GetComponent<Pendelum>().Fitness);
            GameObject thing = ListOfBestPendelums[i];
            Destroy(thing); //Destroy the pendelums at the end.  
        }
        ListOfBestPendelums.Clear();

        
        //Order the pendelums by the best
        BestPendelums.Sort(delegate(BestPendelumDNA b1, BestPendelumDNA b2) { return b1.Fitness.CompareTo(b2.Fitness); });
        BestPendelums.Reverse();
        //delete all but 10 best pendelums
        for (int i = 0; i < BestPendelums.Count; i++)
        {
            if (i > 11)
            {
                BestPendelums.RemoveAt(i);
            }
        }
        

        //print(BestPendelums.Count);

            float offset = 0.3f;
        for (int i = 0; i < BestPendelums.Count; i++)
        {
            offset = offset * -1;
            //print(BestPendelums[i].Fitness);
            ListOfBestPendelums.Add((GameObject)Instantiate(pendelum, new Vector3(BestSpawnpoint.transform.position.x + offset, BestSpawnpoint.transform.position.y , BestSpawnpoint.transform.position.z - i*12), Quaternion.identity));
            ListOfBestPendelums[i].gameObject.GetComponent<Pendelum>().SetDNA(BestPendelums[BestPendelums.Count - i - 1].DNA);//Add the dna of the winners
            
        }
        
    }

}