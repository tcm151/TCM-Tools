using UnityEngine;


namespace TCM.Movement
{
    public static class Gravity
    {
        //> GET GRAVITY
        public static Vector3 Down(Vector3 position) => position.normalized;
        
        //> GET RELATIVE UP
        public static Vector3 Up(Vector3 position) => position.normalized;
        
        //> GET GRAVITY AND RELATIVE UP
        public static (Vector3, Vector3) Get(Vector3 position)
        {
            Vector3 up = position.normalized;
            Vector3 gravity = -position.normalized * 9.81f;
            
            return (gravity, up);
        }
    }
}