using System.Collections.Generic;
using SwarmLab;
using UnityEngine;

namespace Runtime.Rules
{
    [System.Serializable]
    public class CohesionRule : SteeringRule
    {
        public override Vector3 CalculateForce(Entity entity, List<Entity> neighbors)
        {
            if (neighbors.Count == 0) return Vector3.zero;

            Vector3 centerOfMass = Vector3.zero;
            float totalWeight = 0f;

            foreach (var neighbor in neighbors)
            {
                // Skip self if included (usually logic handles this, but safe to check if strictly needed, though list usually excludes self)
                // Assuming list contains only neighbors.
                
                float weight = GetWeightFor(neighbor.Species);
                if (weight <= 0.001f) continue;

                centerOfMass += neighbor.Position * weight;
                totalWeight += weight;
            }

            if (totalWeight <= 0.001f) return Vector3.zero;

            centerOfMass /= totalWeight;
            
            // Cohesion force drives the entity towards the center of mass
            return (centerOfMass - entity.Position).normalized;
        }
    }
}