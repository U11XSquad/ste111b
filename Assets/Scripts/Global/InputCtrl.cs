using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public abstract class InputCtrl : NetworkBehaviour
{
    public enum KeyName
    {
        K1,
        K2,
        K3,
        K4,
        K5,
        K6,
        K7,
        K8,
        K9,
        L,
        N,
        H,
        S
    }
    public enum KeyEvent
    {
        Down,
        Up
    }
    public struct KeyRec
    {
        public KeyName name;
        public KeyEvent e;
        public int frame;
    }
    protected LinkedList<KeyRec> keyRec;
    Dictionary<KeyName, bool> keyIsDown;
    int currentFrame;
    KeyName preDir;
    bool dirDown;

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

    protected virtual void Awake()
    {
        keyRec = new LinkedList<KeyRec>();
        keyIsDown = new Dictionary<KeyName, bool>();

        keyIsDown[KeyName.L] = false;
        keyIsDown[KeyName.N] = false;
        keyIsDown[KeyName.H] = false;
        keyIsDown[KeyName.S] = false;

        currentFrame = 0;
        preDir = KeyName.K5;
    }

    void PushKey(KeyName name, KeyEvent e)
    {
        var rec = new KeyRec();
        rec.name = name;
        rec.e = e;
        rec.frame = currentFrame;
        keyRec.AddLast(rec);
    }

    KeyName GetDir()
    {
        KeyName dir;
        if (LeftHold && !RightHold)
        {
            if (UpHold && !DownHold)
            {
                dir = KeyName.K7;
            }
            else if (DownHold && !UpHold)
            {
                dir = KeyName.K1;
            }
            else
            {
                dir = KeyName.K4;
            }
        }
        else if (RightHold && !LeftHold)
        {
            if (UpHold && !DownHold)
            {
                dir = KeyName.K9;
            }
            else if (DownHold && !UpHold)
            {
                dir = KeyName.K3;
            }
            else
            {
                dir = KeyName.K6;
            }
        }
        else
        {
            if (UpHold && !DownHold)
            {
                dir = KeyName.K8;
            }
            else if (DownHold && !UpHold)
            {
                dir = KeyName.K2;
            }
            else
            {
                dir = KeyName.K5;
            }
        }
        return dir;
    }

    KeyName ReverseDir(KeyName dir)
    {
        if (dir == KeyName.K1)
        {
            return KeyName.K9;
        }
        else if (dir == KeyName.K2)
        {
            return KeyName.K8;
        }
        else if (dir == KeyName.K3)
        {
            return KeyName.K7;
        }
        else if (dir == KeyName.K4)
        {
            return KeyName.K6;
        }
        else if (dir == KeyName.K6)
        {
            return KeyName.K4;
        }
        else if (dir == KeyName.K7)
        {
            return KeyName.K3;
        }
        else if (dir == KeyName.K8)
        {
            return KeyName.K2;
        }
        else if (dir == KeyName.K9)
        {
            return KeyName.K1;
        }
        else
        {
            return KeyName.K5;
        }
    }

    protected void RecordKey()
    {
        //增加帧
        currentFrame++;

        //删除过时的按键
        int cnt = Mathf.CeilToInt(1.0f / Time.fixedDeltaTime);
        while (keyRec.Count > 0 && keyRec.First.Value.frame < currentFrame - cnt)
        {
            keyRec.RemoveFirst();
        }

        //检测方向
        KeyName dir = GetDir();
        if (dir != preDir)
        {
            if (preDir != KeyName.K5)
            {
                PushKey(preDir, KeyEvent.Up);
            }
            if (dir != KeyName.K5)
            {
                PushKey(dir, KeyEvent.Down);
                dirDown = true;
            }
            else
            {
                dirDown = false;
            }
        }
        preDir = dir;
    }

    public bool Test46(float time)
    {
        if (preDir == KeyName.K5)
        {
            return false;
        }
        var dir = ReverseDir(preDir);
        int cnt = Mathf.CeilToInt(time / Time.fixedDeltaTime);
        foreach (var rec in keyRec)
        {
            if (rec.name == dir && rec.e == KeyEvent.Down && rec.frame >= currentFrame - cnt)
            {
                return true;
            }
        }

        return false;
    }

    public bool Test360(float time)
    {
        bool f2 = false, f4 = false, f6 = false, f8 = false;
        int cnt = Mathf.CeilToInt(time / Time.fixedDeltaTime);
        foreach (var rec in keyRec)
        {
            if (rec.e == KeyEvent.Down && rec.frame >= currentFrame - cnt)
            {
                if (rec.name == KeyName.K2)
                {
                    f2 = true;
                }
                else if (rec.name == KeyName.K4)
                {
                    f4 = true;
                }
                else if (rec.name == KeyName.K6)
                {
                    f6 = true;
                }
                else if (rec.name == KeyName.K8)
                {
                    f8 = true;
                }
            }
        }
        return f2 && f4 && f6 && f8;
    }

    public bool TestDash()
    {
        if (!dirDown)
            return false;
        int cnt = Mathf.CeilToInt(0.1f / Time.fixedDeltaTime);
        foreach (var rec in keyRec)
        {
            if (rec.name == preDir && rec.e == KeyEvent.Down && rec.frame >= currentFrame - cnt)
            {
                return true;
            }
        }
        return false;
    }
}
