using UnityEngine;
using System.Collections;

public class PendelumError : MonoBehaviour
{
    public Rigidbody rb;
    public int DNALength = 1000;
    public int MovementArea = 5;
    public float ForceApplyInterval = 0.2f;
    public float MinMaxMovementForce = 5f;
    public float Fitness = 1000f;
    public GameObject UpperStick;
    public GameObject LoweStick;
    public GameObject UpperGoal;
    public GameObject LowerGoal;

    private float[] DNA;
    private int j = 0;

    public float GetFitness()
    {
        return Fitness;
    }
    public float[] GetDNA()
    {
        return DNA;
    }

    // Use this for initialization
    void Start()
    {
        RandomizeDNA();

    }

    // Update is called once per frame
    void Update()
    {
        Transfer(DNA[j]);
        if (j < DNALength) j = 0;
        j++;
        //print(Time.deltaTime);
        CalculateFitness();
        //print(Fitness);
    }

    void Transfer(float amount)
    {
        rb.AddForce(0, 0,amount);
        //print(amount);
        if (rb.transform.position.z > MovementArea)
        {
            rb.AddForce(0, 0, -100);
            //print("vasen");
        }

        if (rb.transform.position.z < -MovementArea)
        {
            rb.AddForce(0, 0, 100);
            //print("oikea");
        }

        //print(rb.transform.position.z);
    }

    void CalculateFitness()
    {
        float UpperDistance = 0f;
        float LowerDistance = 0f;
        UpperDistance = Vector3.Distance(UpperStick.transform.position, UpperGoal.transform.position) / 10;
        LowerDistance = Vector3.Distance(LoweStick.transform.position, LowerGoal.transform.position) / 10;
        Fitness = Fitness - UpperDistance - LowerDistance;
    }

    void RandomizeDNA()
    {
        DNA = new float[DNALength];

        for (int i = 0; i < DNALength; i++)
        {
            DNA[i] = Random.Range(-MinMaxMovementForce, MinMaxMovementForce);
            //print(i);
            //print("DNA:" + DNA[i] + " " + i );

        }
    }

}