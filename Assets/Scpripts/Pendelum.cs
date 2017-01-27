using UnityEngine;
using System.Collections;


public class Pendelum : MonoBehaviour
{
    public Rigidbody rb;
    public int DNALength = 10;
    public int MovementArea = 5;
    public float ForceApplyInterval = 0.2f;
    public float MinMaxMovementForce = 5f;
    public float Fitness = 0f;
    public GameObject UpperStick;
    public GameObject LoweStick;
    public GameObject UpperGoal;
    public GameObject LowerGoal;
    public bool IsDNASet = false;
    public bool IsSingle = false; //puts the code into a single pendelum mode.

    private float[] DNA;
    private int j = 0;
    private bool activated = true;
    private bool Fallen = false;

    public float GetFitness()
    {
        return Fitness;
    }
    public float[] GetDNA()
    {
        return DNA;
    }

    public void Deactivate()
    {
        activated = false;
        //rb.constraints = RigidbodyConstraints.FreezePositionZ;
        //UpperGoal.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //LowerGoal.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    public void ReRun()
    {
        //Move to starting location
        //reset j to 0
        //set active
    }
    

    public void SetDNA(float[] DNAtoInject)
    {
        DNA = new float[DNAtoInject.Length];
        for (int i = 0; i < DNAtoInject.Length; i++)
        {
            DNA[i] = DNAtoInject[i];
        }
        IsDNASet = true;
        DNALength = DNAtoInject.Length;
    }

    // Use this for initialization
    void Start()
    {
        //RandomizeDNA();

    }

    // Update is called once per frame
    void Update()
    {
        if (activated == true)
        {
            if (IsDNASet == true)
            {
                Transfer(DNA[j]);
                if (j > DNALength)
                {
                    print(DNALength);
                    j = 0;
                }
                j++;
                //print(Time.deltaTime);
                CalculateFitness();
                //print(Fitness);
            }
        }
    }

    void Transfer(float amount)
    {
        rb.AddForce(0, 0, amount * Time.deltaTime * 50);
        
        //print(amount);
        /*
        if (rb.transform.position.z > MovementArea)
        {
            rb.AddForce(0, 0, -100 * Time.deltaTime * 50);
            //print("vasen");
        }

        if (rb.transform.position.z < -MovementArea)
        {
            rb.AddForce(0, 0, 100 * Time.deltaTime * 50);
            //print("oikea");
        }
        */
        //print(rb.transform.position.z);
    }

    void CalculateFitness()
    {

        //#############################Fitness based on distance from goal
        /*
        if (IsSingle == false)//fitness for double pendelum
        {
            float UpperDistance = 0f;
            float LowerDistance = 0f;
            float lowersticangle = LoweStick.transform.eulerAngles.x;
            UpperDistance = Vector3.Distance(UpperStick.transform.position, UpperGoal.transform.position) * Time.deltaTime * 10;
            LowerDistance = Vector3.Distance(LoweStick.transform.position, LowerGoal.transform.position) * Time.deltaTime * 10;
            if (lowersticangle > 180) LowerDistance = LowerDistance * 5f; //punsih if the stick is fallen
            Fitness = Fitness - UpperDistance - LowerDistance;
        }
        else//Fitness for single pendelum
        {
            float LowerDistance = 0f;
            float lowersticangle = LoweStick.transform.eulerAngles.x;
            LowerDistance = Vector3.Distance(LoweStick.transform.position, LowerGoal.transform.position) * Time.deltaTime * 10;
            if (lowersticangle > 180) LowerDistance = LowerDistance * 5f; //punsih if the stick is fallen
            Fitness = Fitness - LowerDistance;
        }
        */

        //#############################Fitness based on time standing up
        if (IsSingle == false)//fitness for double pendelum
        {
            float lowersticangle = LoweStick.transform.eulerAngles.x;
            float upperstickangle = UpperStick.transform.eulerAngles.x;
            if (lowersticangle < 180 && upperstickangle < 180 && Fallen == false)
            {
                Fitness = Fitness + Time.deltaTime;
            }
            else Fallen = true;
        }
        else//Fitness for single pendelum
        {
            float lowersticangle = LoweStick.transform.eulerAngles.x;
            if (lowersticangle < 180 && Fallen == false)
            {
                Fitness = Fitness + Time.deltaTime;
            }
            else Fallen = true;


        }
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