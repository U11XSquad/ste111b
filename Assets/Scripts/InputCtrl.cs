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

    public bool LHold
    {
        get;
        protected set;
    }
    public bool LTrigger
    {
        get;
        protected set;
    }
    public bool NHold
    {
        get;
        protected set;
    }
    public bool NTrigger
    {
        get;
        protected set;
    }
    public bool HHold
    {
        get;
        protected set;
    }
    public bool HTrigger
    {
        get;
        protected set;
    }
    public bool SHold
    {
        get;
        protected set;
    }
    public bool STrigger
    {
        get;
        protected set;
    }
    public bool BlockHold
    {
        get;
        protected set;
    }
    public bool BlockTrigger
    {
        get;
        protected set;
    }
}
