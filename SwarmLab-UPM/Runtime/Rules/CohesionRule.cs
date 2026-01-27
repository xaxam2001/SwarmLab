using System.Collections.Generic;
using SwarmLab;
using UnityEngine;

namespace Runtime.Rules
{
    [System.Serializable]
    public class CohesionRule : SteeringRule
    {
        [Tooltip("Distance maximum pour voir les voisins")]
        public float visionRadius = 50; // 

        [Tooltip("Force maximum de virage")]
        public float maxForce = 2f; 

        public override Vector3 CalculateForce(Entity entity, List<Entity> neighbors)
        {
            Vector3 centerOfMass = Vector3.zero;
            float totalWeight = 0f;
            int count = 0;

            foreach (var neighbor in neighbors)
            {
                if (neighbor == entity) continue;

                float distance = Vector3.Distance(entity.Position, neighbor.Position);
                
                if (distance > 0 && distance < visionRadius)
                {
                    float weight = GetWeightFor(neighbor.Species);
                    if (weight <= 0.001f) continue;

                    centerOfMass += neighbor.Position * weight;
                    totalWeight += weight;
                    count++;
                }
            }

            if (count == 0) return Vector3.zero;

            // 1. Calculate weighted center
            centerOfMass /= totalWeight;
            
            // 2. Calculate the Average Weight of the group
            // If all neighbors had weight 0.5, averageWeight is 0.5
            float averageWeight = totalWeight / count;

            // Reynolds Steering
            Vector3 desired = centerOfMass - entity.Position;
            desired = desired.normalized * entity.Species.maxSpeed;

            Vector3 steer = desired - entity.Velocity;
            steer = Vector3.ClampMagnitude(steer, maxForce);

            // 3. APPLY WEIGHT: Scale the final force by the species importance
            return steer * averageWeight; 
        }
    }
}