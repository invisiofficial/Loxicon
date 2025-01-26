using System.Collections.Generic;

using UnityEngine;

using Invisi.Pseudocode;

public class AlgorithmFactory : MonoBehaviour
{
    public Algorithm Create()
    {
        // Initializing list of parts
        List<IAlgorithmPart> algorithmParts = new();

        // Collecting parts from factories
        for (int i = 0; i < this.transform.childCount; i++) algorithmParts.Add(this.transform.GetChild(i).GetComponent<IAlgorithmPartFactory>().Create());

        // Creating new algorithm
        return new Algorithm(algorithmParts.ToArray());
    }
}