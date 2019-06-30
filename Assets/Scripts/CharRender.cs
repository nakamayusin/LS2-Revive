using UnityEngine;

public class CharRender : MonoBehaviour
{

    public static readonly string[] IdleDir = { "Idle N", "Idle NW", "Idle W", "Idle SW", "Idle S", "Idle SE", "Idle E", "Idle NE" };
    public static readonly string[] WalkDir = {"Walk N", "Walk NW", "Walk W", "Walk SW", "Walk S", "Walk SE", "Walk E", "Walk NE" };
    public static readonly string[] AtkDir = { "Atk N", "Atk NW", "Atk W", "Atk SW", "Atk S", "Atk SE", "Atk E", "Atk NE" };

    Animator animator;
    int lastDirection;

    private void Awake()
    {
        //cache the animator component
        animator = GetComponent<Animator>();
    }

    public void SetController(int type, int style, int color)
    {
        animator.runtimeAnimatorController = RenderList.GetAnimate(type, style, color);
    }


    public void SetDirection(Vector2 direction, int dir)
    {
        string[] directionArray = null;

        if (direction.magnitude < .01f)
        {
            directionArray = IdleDir;
        }
        else
        {
            directionArray = WalkDir;
            lastDirection = dir;
        }

        //tell the animator to play the requested state
        animator.Play(directionArray[lastDirection]);
    }

    public void SetDirection(CharStatus status, int dir)
    {

        string[] directionArray = null;

        if (status == CharStatus.Idle)
        {
            directionArray = IdleDir;
        }
        else if (status == CharStatus.Moving)
        {
            directionArray = WalkDir;
            lastDirection = dir;
        }
        else if (status == CharStatus.Atk)
        {
            directionArray = AtkDir;
            lastDirection = dir;
        }

        animator.Play(directionArray[lastDirection]);
    }

    //helper functions

    //this function converts a Vector2 direction to an index to a slice around a circle
    //this goes in a counter-clockwise direction.
    public static int DirectionToIndex(Vector2 dir, int sliceCount){
        //get the normalized direction
        Vector2 normDir = dir.normalized;
        //calculate how many degrees one slice is
        float step = 360f / sliceCount;
        //calculate how many degress half a slice is.
        //we need this to offset the pie, so that the North (UP) slice is aligned in the center
        float halfstep = step / 2;
        //get the angle from -180 to 180 of the direction vector relative to the Up vector.
        //this will return the angle between dir and North.
        float angle = Vector2.SignedAngle(Vector2.up, normDir);
        //add the halfslice offset
        angle += halfstep;
        //if angle is negative, then let's make it positive by adding 360 to wrap it around.
        if (angle < 0){
            angle += 360;
        }
        //calculate the amount of steps required to reach this angle
        float stepCount = angle / step;
        //round it, and we have the answer!
        return Mathf.FloorToInt(stepCount);
    }







    //this function converts a string array to a int (animator hash) array.
    public static int[] AnimatorStringArrayToHashArray(string[] animationArray)
    {
        //allocate the same array length for our hash array
        int[] hashArray = new int[animationArray.Length];
        //loop through the string array
        for (int i = 0; i < animationArray.Length; i++)
        {
            //do the hash and save it to our hash array
            hashArray[i] = Animator.StringToHash(animationArray[i]);
        }
        //we're done!
        return hashArray;
    }

}
