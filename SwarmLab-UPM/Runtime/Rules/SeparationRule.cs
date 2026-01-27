using System.Collections.Generic;
using SwarmLab;
using UnityEngine;

namespace Runtime.Rules
{
    [System.Serializable]
    public class SeparationRule : SteeringRule
    {
        [Tooltip("Distance at which repulsion starts")]
        public float minDistance = 2f; 

        [Tooltip("Maximum force applied for separation")]
        public float maxForce = 2f;

        public override Vector3 CalculateForce(Entity entity, List<Entity> neighbors)
        {
            Vector3 steer = Vector3.zero;
            int count = 0;
            float totalWeight = 0f;

            foreach (var neighbor in neighbors)
            {
                float distance = Vector3.Distance(entity.Position, neighbor.Position);

                if (distance > 0 && distance < minDistance)
                {
                    float weight = GetWeightFor(neighbor.Species);
                    if (weight <= 0.001f) continue;

                    Vector3 diff = entity.Position - neighbor.Position;
                    diff.Normalize();
                    diff /= distance; // Weight by distance (closer = stronger)

                    steer += diff; // Add the vector directly
                    totalWeight += weight;
                    count++;
                }
            }

            if (count > 0)
            {
                steer /= count; // Average direction
                
                float averageWeight = totalWeight / count;

                if (steer.sqrMagnitude > 0)
                {
                    // Reynolds Steering
                    steer.Normalize();
                    steer *= entity.Species.maxSpeed;
                    steer -= entity.Velocity;
                    steer = Vector3.ClampMagnitude(steer, maxForce);
                    
                    // APPLY WEIGHT
                    return steer * averageWeight;
                }
            }

            return Vector3.zero;
        }
    }
}