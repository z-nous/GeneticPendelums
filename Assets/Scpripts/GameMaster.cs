﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//Class to hold pendelum Fitness and DNA Data
public class BestPendelumDNA
{

    public float Fitness;
    public float[] DNA;
    public int Type;

    public BestPendelumDNA(float fitness, float[] dna, int tyyppi)
    {
        DNA = new float[dna.Length];
        Fitness = fitness;
        Type = tyyppi;
        for (int i = 0; i < dna.Length; i++)
        {
            DNA[i] = dna[i];
        }
        
    }

} 

public class NeuralnetList
{
    public float Fitness;
    public NeuronalNetwork Controller;

    public NeuralnetList(float fitness, NeuronalNetwork controller)
    {
        Controller = new NeuronalNetwork(controller);
        Fitness = fitness;
    }
}

public class GameMaster : MonoBehaviour {

    //public int PendelumAmount; //How many pendelums to instantiate per generation
    public GameObject Singlependelum; //prefab for single pendelum
    public GameObject DoublePendelum; //Prefab for double pendelum
    public GameObject SpawnPoint; //spawn point for pendelums
    public GameObject BestSpawnpoint; //Spawn point for the best pendelums
    public GameObject AvarageFitnessGraph;
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
    public Text Mutationtext;
    public Slider MutationSlider;
    public Slider MatingrateSlider;
    public Text MatingText;
    public Dropdown Pendelumtype;
    public Dropdown ControllerType;
    public Text MutationPossibilityText;
    public Slider MutationPossibilitySlider;
    public int MutationPossibility = 10;
    public Slider SimulationLength;
    public Text SimulationLengthText;
    public Slider NumberOfPendelumsSlider;
    public Text NumberofPendelumsSliderText;
    public Slider NumberOfNewPendelumsSlider;
    public Text NUmberOfNewPendelumsText;
    public InputField FirstLayer;
    public InputField SecondLayer;
    public InputField ThirdLayer;

    public float MinMaxMovementForce = 5f; //Max force applied to the pendelums

    //Things that has something to do with neural network
    private List<NeuralnetList> ListOfNeuralNetworks;
    private List<NeuralnetList> NeuralMatingPool;
    private bool UsingNeuralNetwork = true;


    private float AliverTimer; // used to measure simulation time
    private bool IsSimulationRunning = false; //True if simulation is running.
    private int generation = 1; //generation count
    private bool RunNextGeneration = false; //if true, run next generation of pendelums
    private int AccelerateTime; //used for time flow control
    private float AvarageFitness = 0f;
    private bool UseParents = true; //does mating happen, or is the change done via mutation.
    private int MatingRate = 100;
    private GameObject pendelum; //Pendelum type to be instantiated
    private float BestPendelumFitness = 0f; //the best fitness of any pendelem
    private bool IsPendelumsCreated = false;
    private int NumberOfPendelumsActive;
    private List<float> AvarageFitnessValues;
    private int NumberOfNewPendelums = 0;
    private int NumberofnueronsFirst = 5;
    private int NumberofneuronsSecond = 10;
    private int NumberofneuronsThird = 10;

    // Use this for initialization
    void Start () {

        AliverTimer = 0f; //Reset the time
        AccelerateTime = 2; //set timemultiplier to 1
        pendelum = Singlependelum;

        //Initialize all the lists
        ListOfPendelums = new List<GameObject>();
        MatingPool = new List<GameObject>();
        BestPendelums = new List<BestPendelumDNA>();
        ListOfBestPendelums = new List<GameObject>();
        ListOfNeuralNetworks = new List<NeuralnetList>();
        NeuralMatingPool = new List<NeuralnetList>();
        AvarageFitnessValues = new List<float>();
        //instantiate the first generation of pendelums

    }

    // Update is called once per frame
    void Update() {

        //If simulation is running, keep oon running till it stops
        if (IsSimulationRunning == true) RunSimulation();

        if(UsingNeuralNetwork == false) { 
       
            //If the last simulation has stopped, and the flag to run the next on is set, get new generation going
            if (RunNextGeneration == true && IsSimulationRunning == false)
            {
                NewGeneration();
                InstantiateBestPendelums();
            }
        }
        else
        {
            if (RunNextGeneration == true && IsSimulationRunning == false)
            {
                NewNeuralGeneration();

            }
        }


    }

   void NewNeuralGeneration()
    {
        
        AliverTimer = 0f; // nollataan aikalaskuri, koska simulaatio ei ole käynnissä.
        generation += 1;
        
        //Get the neural networks of the pendelums and their fitnesses
        for (int i = 0; i < NumberOfPendelums; i++)
        {
            ListOfNeuralNetworks.Add(new NeuralnetList(ListOfPendelums[i].GetComponent<Pendelum>().GetFitness(), ListOfPendelums[i].GetComponent<Pendelum>().GetNeuralNetwork()));
        }
        
        //Sort the list from best to worst
        ListOfNeuralNetworks.Sort(delegate (NeuralnetList b1, NeuralnetList b2) { return b1.Fitness.CompareTo(b2.Fitness); });
        ListOfNeuralNetworks.Reverse();
        //print(ListOfNeuralNetworks[0].Fitness);
        //print(ListOfNeuralNetworks[ListOfNeuralNetworks.Count - 1].Fitness);

        // Update the best fitness value
        if (ListOfNeuralNetworks[0].Fitness > BestPendelumFitness) BestPendelumFitness = ListOfNeuralNetworks[0].Fitness;

        //calculate avarege fitness
        AvarageFitness = 0;
        for (int i = 0; i < ListOfNeuralNetworks.Count; i++)
        {
            AvarageFitness += ListOfNeuralNetworks[i].Fitness;
        }
        AvarageFitness = AvarageFitness / ListOfNeuralNetworks.Count;
        AvarageFitnessValues.Add(AvarageFitness);
        
        //Update information on screen
        Info.text = "Best this generation:\n" + ListOfNeuralNetworks[0].Fitness + "\nThis generation avarage:\n" + AvarageFitness + "\nBest ever:\n" + BestPendelumFitness + "\nGeneration: " + generation;
        AvarageFitnessGraph.GetComponent<Graph>().AddValue(AvarageFitness, ListOfNeuralNetworks[0].Fitness, ListOfNeuralNetworks[ListOfNeuralNetworks.Count - 1].Fitness);
        //Mutate the Controllers



        for (int i = 0; i < ListOfNeuralNetworks.Count; i++)
        {
            int RandMutation = Random.Range(0, 100);
            if (MutationPossibility >= RandMutation)
            {
                ListOfNeuralNetworks[i].Controller.Mutate(99 - Mutationthreshold);
            }
        }

        //Create mating pool of the networks
        for (int i = 0; i < MatingRate; i++)
        {
            for (int j = 0; j < MatingRate - i; j++)
            {
                NeuralMatingPool.Add(ListOfNeuralNetworks[i]);
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

        //Add the networks to the pendelums

        for (int i = 0; i < NumberOfPendelums; i++)
        {
            if(NumberOfNewPendelums > i)
            {
                ListOfPendelums[i].gameObject.GetComponent<Pendelum>().SetNeuralNetwork(new NeuronalNetwork(3, 1, NumberofnueronsFirst, NumberofneuronsSecond, NumberofneuronsThird), Random.Range(-2, 2));
            }
            else
            {
                ListOfPendelums[i].GetComponent<Pendelum>().SetNeuralNetwork(NeuralMatingPool[Random.Range(0, NeuralMatingPool.Count)].Controller, Random.Range(-1, 1)); //Add random controller from mating pool
            }
        }
        NeuralMatingPool.Clear(); //Kill the last generation of pendelums
        ListOfNeuralNetworks.Clear(); //Clear the old list of networks

        IsSimulationRunning = true;

    }


    //Simulation running
    void RunSimulation()
    {
        AliverTimer += Time.deltaTime;
        if (AliverTimer >= AliveTime || NumberOfPendelumsActive == 0) //Simulation has run enough. stop it or all pendelums have fallen
        {
            IsSimulationRunning = false;
            NumberOfPendelumsActive = NumberOfPendelums; //reset the active pendelum count.
            //Shut down all the pendelums.
            for (int i = 0; i < NumberOfPendelums; i++){
               ListOfPendelums[i].GetComponent<Pendelum>().Deactivate();
            }
            for (int i = 0; i < ListOfBestPendelums.Count; i++)
            {
                ListOfBestPendelums[i].GetComponent<Pendelum>().Deactivate();
            }
        }
    }

    //Gets new generation of pendelums going
    void NewGeneration()
    {
        //print(AliverTimer);
        AliverTimer = 0f; // nollataan aikalaskuri, koska simulaatio ei ole käynnissä.
        generation += 1;

        SortPendelumsbyfitness();
        AddTheBestToBestList();
        CalculateAvarageFitness();

        //Update Best Fitness value
        if (ListOfPendelums[0].GetComponent<Pendelum>().Fitness > BestPendelumFitness) BestPendelumFitness = ListOfPendelums[0].GetComponent<Pendelum>().Fitness;

        UpdateInformationToGUI();
        
 
        
        //Create a mating pool for the pendelums

        for (int i = 0; i < MatingRate; i++)
        {
            for (int j = 0; j < MatingRate - i; j++)
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

    void SortPendelumsbyfitness()
    {
        //go through all pendelums and sort by their fitness
        ListOfPendelums.Sort(delegate (GameObject a, GameObject b)
        {
            return (a.GetComponent<Pendelum>().Fitness).CompareTo(b.GetComponent<Pendelum>().Fitness);
        });
        ListOfPendelums.Reverse();
    }

    void AddTheBestToBestList()
    {
        //Add the best pendelum DNA and fitness to a list containing the best of each generation.
        BestPendelums.Add(new BestPendelumDNA(ListOfPendelums[0].GetComponent<Pendelum>().Fitness, ListOfPendelums[0].GetComponent<Pendelum>().GetDNA(), Pendelumtype.value));

        //Order the bet pendelums by the best
        BestPendelums.Sort(delegate (BestPendelumDNA b1, BestPendelumDNA b2) { return b1.Fitness.CompareTo(b2.Fitness); });
        BestPendelums.Reverse();
    }

    void CalculateAvarageFitness()
    {
        //calculate avarage fitness of the pendelums
        AvarageFitness = 0;
        for (int i = 0; i < ListOfPendelums.Count; i++)
        {
            AvarageFitness += ListOfPendelums[i].GetComponent<Pendelum>().Fitness;
        }
        AvarageFitness = AvarageFitness / ListOfPendelums.Count;
    }

    void UpdateInformationToGUI()
    {
        //Update info to the gui of the just run generation
        Info.text = "Best this generation:\n" + ListOfPendelums[1].GetComponent<Pendelum>().Fitness + "\nThis generation avarage:\n" + AvarageFitness + "\nBest ever:\n" + BestPendelumFitness + "\nGeneration: " + generation;

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

        

        //delete all but 5 best pendelums
        for (int i = 0; i < BestPendelums.Count; i++)
        {
            if (i > 4)
            {
                BestPendelums.RemoveAt(i);
            }
        }
        
        //Print Fitness of all pendelums
        /*
        for (int i = 0; i < BestPendelums.Count; i++)
        {
            print(BestPendelums[i].Fitness);
        }*/

        //print(BestPendelums.Count);


            float offset = 0.3f;
        for (int i = 0; i < BestPendelums.Count; i++)
        {
            offset = offset * -1;
            //print(BestPendelums[i].Fitness);
            if(BestPendelums[i].Type == 0)
            { 
                ListOfBestPendelums.Add((GameObject)Instantiate(Singlependelum, new Vector3(BestSpawnpoint.transform.position.x + offset, BestSpawnpoint.transform.position.y , BestSpawnpoint.transform.position.z - i*12), Quaternion.identity));
            }
            if(BestPendelums[i].Type == 1)
            {
                ListOfBestPendelums.Add((GameObject)Instantiate(DoublePendelum, new Vector3(BestSpawnpoint.transform.position.x + offset, BestSpawnpoint.transform.position.y, BestSpawnpoint.transform.position.z - i * 12), Quaternion.identity));
            }
            ListOfBestPendelums[i].gameObject.GetComponent<Pendelum>().SetDNA(BestPendelums[i].DNA);//Add the dna of the winners
            
        }
        
    }

    void AddPendelums(int amount)
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
            ListOfPendelums.Add((GameObject)Instantiate(pendelum, new Vector3(SpawnPoint.transform.position.x + offset, SpawnPoint.transform.position.y + k * 12, SpawnPoint.transform.position.z - j * 12), Quaternion.identity));
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
        int RandMutation = Random.Range(0, 100);

        for (int i = 0; i < DNA1.Length; i++)
        {
            if (UseParents == true)
            {

                if (i < Cut)
                {
                    MixedDNA[i] = DNA1[i];
                }
                else
                {
                    MixedDNA[i] = DNA2[i];
                }
            }
            else
            {
                MixedDNA[i] = DNA1[i];
            }
            if (MutationPossibility > RandMutation) { 
                if (Random.Range(0, 100) < Mutationthreshold) MixedDNA[i] = Random.Range(-MinMaxMovementForce, MinMaxMovementForce);
            }
        }
        return MixedDNA; //Change to MixedDNA once impelemted
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

    public void SetFucking()
    {
        UseParents = !UseParents;
    }

    public void SetPendelumType()
    {
        //if()
        //print("muutos");
        if (Pendelumtype.value == 0) pendelum = Singlependelum;
        if (Pendelumtype.value == 1) pendelum = DoublePendelum;
    }

    public void SetMutationPercent()
    {
        Mutationthreshold = (int)MutationSlider.value;
        Mutationtext.text = "Mutation Percent: " + Mutationthreshold;

    }

    public void SetMutationPossibility()
    {
        MutationPossibility = (int)MutationPossibilitySlider.value;
        MutationPossibilityText.text = "Possibility for mutation: " + MutationPossibility + "%";

    }

    public void SetMatingRate()
    {
        MatingRate = (int)MatingrateSlider.value;
        MatingText.text = "Cutoff % for mating: " + MatingRate;
        //print(MatingRate);
    }

    public void ResetBestValue()
    {
        BestPendelumFitness = 0f;

    }

    public void SetSimulationLength()
    {
        AliveTime = SimulationLength.value;
        
        SimulationLengthText.text = "MAX Simulation time: " + AliveTime.ToString("#.0") + " seconds";
        
    }

    public void SetNeuralNetworkUsage()
    {
        if (ControllerType.value == 0) UsingNeuralNetwork = true;
        if (ControllerType.value == 1) UsingNeuralNetwork = false;
        //UsingNeuralNetwork = !UsingNeuralNetwork;
        print(UsingNeuralNetwork);
    }

    public void StartNewGeneration()
    {
        //Destroy the old ones
        if(IsPendelumsCreated == true) { 
            for (int i = 0; i < NumberOfPendelums; i++)
            {
                //print(ListOfPendelums[i].GetComponent<Pendelum>().Fitness);
                GameObject thing = ListOfPendelums[i];
                Destroy(thing); //Destroy the pendelums at the end.  
            }
            ListOfPendelums.Clear();
        }
        //Add new pendelums
        IsPendelumsCreated = true;
        AddPendelums(NumberOfPendelums);
        generation = 0; 

        //Add dna to the just instantiated pendelums
        if (UsingNeuralNetwork == false)
        {
            for (int i = 0; i < NumberOfPendelums; i++)
            {
                ListOfPendelums[i].gameObject.GetComponent<Pendelum>().SetDNA(RandomizeDNA(DNALength));
            }
        }
        else
        {
            for (int i = 0; i < NumberOfPendelums; i++)
            {
                ListOfPendelums[i].gameObject.GetComponent<Pendelum>().SetNeuralNetwork(new NeuronalNetwork(3,1, NumberofnueronsFirst, NumberofneuronsSecond, NumberofneuronsThird),0f);
            }
        }
        AvarageFitnessValues.Clear();
        

    }

    public void SetNumberOfPendelums()
    {
        NumberOfPendelums = (int)NumberOfPendelumsSlider.value;
        NumberofPendelumsSliderText.text = "Number of Pendelums" + NumberOfPendelums;
    }

    public void NumberOfNewPendelumsPerGeneration()
    {
        NumberOfNewPendelums = (int)NumberOfNewPendelumsSlider.value;
        NUmberOfNewPendelumsText.text = "Number of new pendelums: " + NumberOfNewPendelums;
    }

    public void SetNeuralNetworkLayers()
    {
        NumberofnueronsFirst = System.Convert.ToInt32(FirstLayer.text);
        NumberofneuronsSecond = System.Convert.ToInt32(SecondLayer.text);
        NumberofneuronsThird = System.Convert.ToInt32(ThirdLayer.text);
        //print(NumberofnueronsFirst);
        //print(NumberofneuronsSecond);
        //print(NumberofneuronsThird);
    }

    public void MinusNumberOfActivePendelums()
    {
        NumberOfPendelumsActive = NumberOfPendelumsActive - 1;
        //print(NumberOfPendelumsActive);
    }

    public void KillAllPendelums()
    {
        //Kill all pendelums
        for (int i = 0; i < NumberOfPendelums; i++)
        {
            //print(ListOfPendelums[i].GetComponent<Pendelum>().Fitness);
            GameObject thing = ListOfPendelums[i];
            Destroy(thing); //Destroy the pendelums at the end.  
        }
        ListOfPendelums.Clear();
        MatingPool.Clear();
        NeuralMatingPool.Clear();
        BestPendelums.Clear();
        ListOfNeuralNetworks.Clear();
        IsPendelumsCreated = false;
        IsSimulationRunning = false;
        RunNextGeneration = false;
        UseParents = true;
        AvarageFitnessGraph.GetComponent<Graph>().ResetValues();
    }

}

