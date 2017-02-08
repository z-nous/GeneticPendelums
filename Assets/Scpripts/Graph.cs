using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Graph : MonoBehaviour {

    public LineRenderer AvarageLine;
    public LineRenderer WorstLine;
    public LineRenderer BestLine;
    public Material AvarageMaterial;
    public Material BestMaterial;
    public Material WorstMaterial;
    public Transform GraphStartPostion;

    private List<float> AvarageValues = new List<float>();
    private List<float> BestValues = new List<float>();
    private List<float> WorstValues = new List<float>();
/*
    public void AddValues(List<float> NewValues)
    {
        for (int i = 0; i < NewValues.Count; i++)
        {
            //Values.Add(NewValues[i]);
        }
        DrawLine();
    }
*/
    public void AddValue(float newavaragevalue, float newbestvalue, float newworstvalue)
    {
        AvarageValues.Add(newavaragevalue);
        BestValues.Add(newbestvalue);
        WorstValues.Add(newworstvalue);
        DrawLine();
    }

    public void ResetValues()
    {
        AvarageValues.Clear();
        BestValues.Clear();
        WorstValues.Clear();
    }


    private void DrawLine()
    {
        AvarageLine.SetVertexCount(AvarageValues.Count);
        BestLine.SetVertexCount(BestValues.Count);
        WorstLine.SetVertexCount(WorstValues.Count);

        float spacing = 490f / AvarageValues.Count;


        for(int i = 0; i < AvarageValues.Count; i++)
        {
            AvarageLine.SetPosition(i, new Vector3(GraphStartPostion.localPosition.x + spacing * i, GraphStartPostion.localPosition.y + AvarageValues[i] * 9.5f, GraphStartPostion.localPosition.z));
            BestLine.SetPosition(i, new Vector3(GraphStartPostion.localPosition.x + spacing * i, GraphStartPostion.localPosition.y + BestValues[i] * 9.5f, GraphStartPostion.localPosition.z));
            WorstLine.SetPosition(i, new Vector3(GraphStartPostion.localPosition.x + spacing * i, GraphStartPostion.localPosition.y + WorstValues[i] * 9.5f, GraphStartPostion.localPosition.z));

        }

    }

    // Use this for initialization
    void Start () {
        AvarageLine.SetWidth(0.2f, 0.2f);
        AvarageLine.material = AvarageMaterial;
        BestLine.SetWidth(0.2f, 0.2f);
        BestLine.material = BestMaterial;
        WorstLine.SetWidth(0.2f, 0.2f);
        WorstLine.material = WorstMaterial;
    }
	
	// Update is called once per frame
	void Update () {
	
	}



}
