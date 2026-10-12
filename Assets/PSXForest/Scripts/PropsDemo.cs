using System.Collections;
using UnityEngine;

namespace HeyHeyThere.PSXForest
{
    /// <summary>
    /// The demo scene's showcase: the camera circles the props and every animated prop plays its
    /// animations in turn (a creature's through its Animator), with nothing to press. Remove it to use the scene as a plain layout.
    /// </summary>
    public class PropsDemo : MonoBehaviour
    {
        public Transform cameraRig;
        public float degreesPerSecond = 8f;
        public float pause = 1.5f;

        void Start()
        {
            foreach (var anim in FindObjectsByType<Animation>(FindObjectsSortMode.None))
                StartCoroutine(Play(anim));
            foreach (var animator in FindObjectsByType<Animator>(FindObjectsSortMode.None))
                if (animator.runtimeAnimatorController != null)
                    StartCoroutine(Play(animator));
        }

        void Update()
        {
            if (cameraRig != null)
                cameraRig.Rotate(0f, degreesPerSecond * Time.deltaTime, 0f, Space.World);
        }

        IEnumerator Play(Animation anim)
        {
            yield return new WaitForSeconds(Random.Range(0f, pause));
            while (true)
                foreach (AnimationState state in anim)
                {
                    anim.Play(state.name);
                    yield return new WaitForSeconds(state.length + pause);
                }
        }

        /// <summary>A creature's controller, driven as a game would: it walks (and runs), attacks, is hit, dies and gets up.</summary>
        IEnumerator Play(Animator animator)
        {
            yield return new WaitForSeconds(Random.Range(0f, pause));
            while (true)
            {
                yield return new WaitForSeconds(pause);
                animator.SetFloat("Walk", 1f);
                yield return new WaitForSeconds(3f);
                animator.SetFloat("Walk", 0f);
                if (System.Array.Exists(animator.parameters, x => x.name == "Run"))
                {
                    animator.SetFloat("Run", 1f);
                    yield return new WaitForSeconds(2f);
                    animator.SetFloat("Run", 0f);
                }
                foreach (var trigger in new[] { "Attack", "Hit", "Die" })
                {
                    yield return new WaitForSeconds(pause);
                    animator.SetTrigger(trigger);
                    yield return new WaitForSeconds(0.25f);  // into its state
                    yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
                }
                yield return new WaitForSeconds(pause);
                animator.Rebind();
            }
        }
    }
}
