using SwarmLab;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MyCustomRule : SteeringRule
{
    // 1. Define custom parameters per species
    [System.Serializable]
    public class MyParams : SpeciesParams
    {
        public float weight = 1f;
        public float customRadius = 10f;
    }

    // 2. The list that will be serialized
    public List<MyParams> speciesParams = new List<MyParams>();

    // 3. Implement Sync logic (Boilerplate)
    public override void SyncSpeciesList(List<SpeciesDefinition> allSpecies)
    {
        // Add new species
        foreach (var def in allSpecies)
        {
            if (!speciesParams.Exists(p => p.species == def))
            {
                speciesParams.Add(new MyParams { species = def });
            }
        }
        // Remove deleted species
        speciesParams.RemoveAll(p => p.species == null || !allSpecies.Contains(p.species));
    }

    // 4. Calculate Force
    public override Vector3 CalculateForce(Entity entity, List<Entity> neighbors)
    {
        Vector3 force = Vector3.zero;
        
        // Find params for this entity (or caching them) is recommended
        // Logic here...
        
        return force;
    }
}