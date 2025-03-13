using System;
using UnityEngine;

namespace Shump
{
    public class YParallaxController : MonoBehaviour
    {
        [SerializeField] Transform[] backgrounds;
        [SerializeField] private float smoothing = 10f;
        [SerializeField] float multiplier = 15f;

        private Transform cam;
        Vector3 previousCamPos;

        private void Awake()
        {
            cam = Camera.main.transform;
        }

        private void Start()
        {
            previousCamPos = cam.position;
        }

        void Update()
        {
            for (var i = 0; i < backgrounds.Length; i++)
            {
                var parallax = (previousCamPos.z - cam.position.z) * (i * multiplier);
                var targetZ = backgrounds[i].position.z + parallax;

                var targetPosition = new Vector3(backgrounds[i].position.x, backgrounds[i].position.y, targetZ);
                
                backgrounds[i].position =
                    Vector3.Lerp(backgrounds[i].position, targetPosition, smoothing * Time.deltaTime);
            }
            previousCamPos = cam.position;
        }
    }
}