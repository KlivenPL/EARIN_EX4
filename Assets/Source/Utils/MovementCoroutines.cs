using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.KLib.Source.Utils.Rotation {
    public static class MovementCoroutines {
        public static IEnumerator KMove(this Transform transform, Vector3 start, Vector3 end, float time) {
            var rotations = KMove(start, end, time);

            foreach (var pos in rotations) {
                transform.position = pos;
                yield return new WaitForEndOfFrame();
            }
        }

        public static IEnumerable<Vector3> KMove(Vector3 start, Vector3 end, float time) {
            float passedTime = 0;
            for (float t = 0f; t < 1; t = Mathf.SmoothStep(0, 1f, passedTime / time)) {
                yield return Vector3.Lerp(start, end, t);
                passedTime += Time.deltaTime;
            }

            yield return end;
        }
    }
}
