using System.Collections.Generic;
using SwarmLab;
using UnityEngine;

namespace Runtime.Rules
{
    [System.Serializable]
    public class SeparationRule : SteeringRule
    {
        public override Vector3 CalculateForce(Entity entity, List<Entity> neighbors)
        {
            if (neighbors == null || neighbors.Count == 0)
                return Vector3.zero;

            Vector3 separationForce = Vector3.zero;

            foreach (var neighbor in neighbors)
            {
                if (neighbor == entity) continue;

                float weight = GetWeightFor(neighbor.Species);
                // If weight is 0 or negative, we assume no repulsion (or attraction handled elsewhere)
                if (weight <= 0f) continue;

                Vector3 toEntity = entity.Position - neighbor.Position;
                float sqrDist = toEntity.sqrMagnitude;

                if (sqrDist > 0.00001f)
                {
                    separationForce += toEntity * (weight / sqrDist);
                }
            }

            if (separationForce.sqrMagnitude > 0.00001f)
            {
                return separationForce.normalized;
            }

            return Vector3.zero;
        }
    }
}