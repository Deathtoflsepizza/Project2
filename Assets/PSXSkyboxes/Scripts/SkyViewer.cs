using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace HeyHeyThere.PSXSkyboxes
{
    /// <summary>
    /// The demo scene's sky viewer: left / right the sky, up / down the category, C the 6-sided or
    /// the panoramic material, drag to look; left alone, the camera turns slowly. Either input
    /// handling works. Remove it and set a material as the Skybox Material to keep one sky.
    /// </summary>
    public class SkyViewer : MonoBehaviour
    {
        public string[] names;
        public string[] categories;
        public Material[] sixSided;
        public Material[] panoramic;
        public bool useSixSided = true;
        public float turn = 3f;  // degrees a second, once left alone

        int index;
        float yaw, pitch, idle;

        void Start()
        {
            var e = transform.eulerAngles;
            yaw = e.y;
            pitch = e.x > 180f ? e.x - 360f : e.x;
            Show(0);
        }

        void Show(int i)
        {
            index = (i % names.Length + names.Length) % names.Length;
            RenderSettings.skybox = useSixSided ? sixSided[index] : panoramic[index];
            DynamicGI.UpdateEnvironment();
        }

        void Category(int by)
        {
            var i = index;
            do
                i = (i + by + names.Length) % names.Length;
            while (categories[i] == categories[index] && i != index);
            while (i > 0 && categories[i - 1] == categories[i])  // the first sky of that category
                i--;
            Show(i);
        }

        void Update()
        {
            idle += Time.deltaTime;
            if (Pressed(0)) Show(index - 1);
            if (Pressed(1)) Show(index + 1);
            if (Pressed(2)) Category(-1);
            if (Pressed(3)) Category(1);
            if (Pressed(4))
            {
                useSixSided = !useSixSided;
                Show(index);
            }
            var drag = Drag();
            if (drag != Vector2.zero)
            {
                yaw -= drag.x * 0.2f;
                pitch = Mathf.Clamp(pitch - drag.y * 0.2f, -85f, 85f);
                idle = 0f;
            }
            else if (idle > 4f)
                yaw += turn * Time.deltaTime;
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        void OnGUI()
        {
            GUI.Label(new Rect(16, 12, 900, 24), $"{names[index].Replace('_', ' ')}   |   {categories[index]}   |   " +
                      $"{index + 1} of {names.Length}   |   {(useSixSided ? "6 Sided" : "Panoramic")}");
            GUI.Label(new Rect(16, 34, 900, 24), "left / right: sky, up / down: category, C: 6 sided / panoramic, drag to look");
        }

#if ENABLE_INPUT_SYSTEM
        bool Pressed(int key)
        {
            if (Keyboard.current == null)
                return false;
            var k = Keyboard.current;
            var keys = new[] { k.leftArrowKey, k.rightArrowKey, k.upArrowKey, k.downArrowKey, k.cKey };
            if (keys[key].wasPressedThisFrame)
                idle = 0f;
            return keys[key].wasPressedThisFrame;
        }

        static Vector2 Drag() => Mouse.current != null && Mouse.current.leftButton.isPressed
            ? Mouse.current.delta.ReadValue() : Vector2.zero;
#else
        bool Pressed(int key)
        {
            var down = Input.GetKeyDown(new[] { KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.C }[key]);
            if (down)
                idle = 0f;
            return down;
        }

        static Vector2 Drag() => Input.GetMouseButton(0)
            ? new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * 10f : Vector2.zero;
#endif
    }
}
