using System;
using UnityEngine;

namespace Shump
{
    public class YCameraController : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private float speed = 2f;

        private void Start()
        {
            transform.position = new Vector3(player.position.x, player.position.y, player.position.z);
        }

        void LateUpdate()
        {
            transform.position += Vector3.forward * speed * Time.deltaTime; ;
        }
    }
}