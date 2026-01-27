using System.Collections.Generic;
using SwarmLab;
using UnityEngine;

namespace Runtime.Rules
{
    [System.Serializable]
    public class AlignmentRule : SteeringRule
    {
        public override Vector3 CalculateForce(Entity entity, List<Entity> neighbors)
        {
            if (neighbors == null || neighbors.Count == 0)
                return Vector3.zero;
            
            Vector3 averageVelocity = Vector3.zero;
            float totalWeight = 0f;
            int count = 0;

            foreach (var neighbor in neighbors)
            {
                if (neighbor == entity) continue;
                
                float weight = GetWeightFor(neighbor.Species);
                if (weight > 0f)
                {
                    averageVelocity += neighbor.Velocity * weight;
                    totalWeight += weight;
                    count++;
                }
            }

            if (count > 0 && totalWeight > 0f)
            {
                averageVelocity /= totalWeight;
                return averageVelocity.normalized;
            }
            
            return Vector3.zero;
        }
    }
}