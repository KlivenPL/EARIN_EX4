using System.Collections.Generic;
using UnityEngine;

namespace Assets.KLib.Source.Utils.Rotation {
    public static class MovementCoroutines {
        public static IEnumerable<Vector3> SmoothlyMove(Vector3 start, Vector3 end, float time) {
            float passedTime = 0;
            for (float t = 0f; t < 1; t = Mathf.SmoothStep(0, 1f, passedTime / time)) {
                yield return Vector3.Lerp(start, end, t);
                passedTime += Time.deltaTime;
            }

            yield return end;
        }
    }
}
