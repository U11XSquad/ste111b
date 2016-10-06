using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public abstract class InputCtrl : NetworkBehaviour
{

    public bool LeftHold
    {
        get;
        protected set;
    }
    public bool RightHold
    {
        get;
        protected set;
    }
    public bool UpHold
    {
        get;
        protected set;
    }
    public bool DownHold
    {
        get;
        protected set;
    }
    public abstract Vector3 Move
    {
        get;
        protected set;
    }
    public bool IsMoving
    {
        get;
        protected set;
    }

    protected bool lhold;
    public bool LHold
    {
        get { return lhold; }
        protected set { lhold = value; }
    }
    protected bool ltrigger;
    public bool LTrigger
    {
        get { return ltrigger; }
        protected set { ltrigger = value; }
    }

    protected bool nhold;
    public bool NHold
    {
        get { return nhold; }
        protected set { nhold = value; }
    }
    protected bool ntrigger;
    public bool NTrigger
    {
        get { return ntrigger; }
        protected set { ntrigger = value; }
    }

    protected bool hhold;
    public bool HHold
    {
        get { return hhold; }
        protected set { hhold = value; }
    }
    protected bool htrigger;
    public bool HTrigger
    {
        get { return htrigger; }
        protected set { htrigger = value; }
    }

    protected bool shold;
    public bool SHold
    {
        get { return shold; }
        protected set { shold = value; }
    }
    protected bool strigger;
    public bool STrigger
    {
        get { return strigger; }
        protected set { strigger = value; }
    }

    protected bool bhold;
    public bool BlockHold
    {
        get { return bhold; }
        protected set { bhold = value; }
    }
    protected bool btrigger;
    public bool BlockTrigger
    {
        get { return btrigger; }
        protected set { btrigger = value; }
    }
}
