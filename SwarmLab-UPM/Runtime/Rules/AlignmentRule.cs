using System.Collections.Generic;
using SwarmLab;
using UnityEngine;

namespace Runtime.Rules
{
    [System.Serializable]
    public class AlignmentRule : SteeringRule
    {
        [Tooltip("Distance to see neighbors")]
        public float neighborRadius = 50; // 

        [Tooltip("Maximum steering force")]
        public float maxForce = 2f;

        public override Vector3 CalculateForce(Entity entity, List<Entity> neighbors)
        {
            Vector3 sum = Vector3.zero;
            float totalWeight = 0f;
            int count = 0;

            foreach (var neighbor in neighbors)
            {
                float distance = Vector3.Distance(entity.Position, neighbor.Position);

                if (distance > 0 && distance < neighborRadius)
                {
                    float weight = GetWeightFor(neighbor.Species);
                    if (weight <= 0.001f) continue;

                    sum += neighbor.Velocity * weight;
                    totalWeight += weight;
                    count++;
                }
            }

            if (count > 0)
            {
                // Average velocity
                sum /= totalWeight; // Weighted average direction
                
                float averageWeight = totalWeight / count;

                // Reynolds Steering
                sum.Normalize();
                sum *= entity.Species.maxSpeed;
                
                Vector3 steer = sum - entity.Velocity;
                steer = Vector3.ClampMagnitude(steer, maxForce);
                
                // APPLY WEIGHT
                return steer * averageWeight;
            }

            return Vector3.zero;
        }
    }
}