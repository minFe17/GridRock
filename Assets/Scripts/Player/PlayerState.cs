public class PlayerState
{
    private bool isLeft = false;
    private bool isRight = false;
    private bool isJump = false;
    private bool isStun = false;

    public bool IsLeft
    {
        get { return isLeft; } 
        set 
        { 
            isLeft = value;
            if (isLeft)
                isRight = false;
        }
    }
    public bool IsRight
    {
        get { return isRight; }
        set 
        { 
            isRight = value; 
            if(isRight)
                isLeft = false;
        }
    }
    public bool IsJump
    { 
        get { return isJump; }
        set {isJump = value; }
    }
    public bool IsStun
    {
        get { return isStun; }
        set { isStun = value; }
    }

}