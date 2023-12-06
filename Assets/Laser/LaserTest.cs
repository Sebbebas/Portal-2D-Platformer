using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LaserTest : MonoBehaviour
{
    public enum Mode
    {
        one,
        two,
        three,
        Total
    }

        public Mode mode = Mode.Total; 
        [SerializeField] float flat1;
        [SerializeField] float flat2;
        [SerializeField] float flat3;


    private void OnDrawGizmosSelected() => Gizmos.DrawCube(transform.position, new(flat1, flat1 + flat1));
}
