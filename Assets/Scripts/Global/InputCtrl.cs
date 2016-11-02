using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class InputCtrl : NetworkBehaviour
{
    public enum KeyName
    {
        K1 = 0,
        K2 = 1,
        K3 = 2,
        K4 = 3,
        K5 = 4,
        K6 = 5,
        K7 = 6,
        K8 = 7,
        K9 = 8,
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
    protected MyLinkedList<KeyRec> keyRec;
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
        keyRec = new MyLinkedList<KeyRec>();
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

    //逆时针下一顺位
    KeyName GetNextDir(KeyName x)
    {
        switch (x)
        {
            case KeyName.K1:
                return KeyName.K2;
            case KeyName.K2:
                return KeyName.K3;
            case KeyName.K3:
                return KeyName.K6;
            case KeyName.K6:
                return KeyName.K9;
            case KeyName.K9:
                return KeyName.K8;
            case KeyName.K8:
                return KeyName.K7;
            case KeyName.K7:
                return KeyName.K4;
            case KeyName.K4:
                return KeyName.K1;
            default:
                return KeyName.K5;
        }
    }

    //顺时针下一顺位
    KeyName GetPrevDir(KeyName x)
    {
        switch (x)
        {
            case KeyName.K1:
                return KeyName.K4;
            case KeyName.K4:
                return KeyName.K7;
            case KeyName.K7:
                return KeyName.K8;
            case KeyName.K8:
                return KeyName.K9;
            case KeyName.K9:
                return KeyName.K6;
            case KeyName.K6:
                return KeyName.K3;
            case KeyName.K3:
                return KeyName.K2;
            case KeyName.K2:
                return KeyName.K1;
            default:
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

        return keyRec.Any(rec =>
            rec.name == dir && rec.e == KeyEvent.Down && rec.frame >= currentFrame - cnt);
    }

    //序列索引比较器

    public bool Test360(float time)
    {
        //键值表及其计算
        int[] keyValTab = new int[9];
        System.Action<KeyName, bool> CalcValTab = (origin, ccw) =>
        {
            var x = origin;
            var i = 0;
            do
            {
                keyValTab[(int)x] = i;
                i++;
                if (ccw)
                    x = GetNextDir(x);
                else
                    x = GetPrevDir(x);
            } while (x != origin);
        };

        //计数
        int cnt = Mathf.CeilToInt(time / Time.fixedDeltaTime);

        //寻找最后的方向
        var it = keyRec.Last;
        while (it != null && it.Value.frame >= currentFrame - cnt && it.Value.e != KeyEvent.Down)
        {
            it = it.Previous;
        }
        if (it == null || it.Value.frame < currentFrame - cnt)
            return false;
        var last = it;

        //键值序列及其构造
        System.Func<bool, List<int>> GenSequence = ccw =>
        {
            List<int> ret = new List<int>();
            CalcValTab(last.Value.name, ccw);
            it = last;
            ret.Add(-1); //首先增加一个起始元素-1
            while (it != null && it.Value.frame >= currentFrame - cnt)
            {
                if (it.Value.e == KeyEvent.Down)
                {
                    ret.Add(keyValTab[(int)it.Value.name]);
                }
                it = it.Previous;
            }
            return ret;
        };

        //求LIS序列
        System.Func<List<int>, List<int>> GetLIS = seq =>
        {
            MyComparison<int> cmp = new MyComparison<int>((x, y) => seq[x] - seq[y]);
            //定长最小值索引序列
            List<int> indOfMinVal = new List<int>();
            int[] pre = new int[seq.Count];
            //起始元素-1
            seq[0] = pre[0] = -1;
            indOfMinVal.Add(0);
            //二分获得LIS长度
            for (int i = 1; i < seq.Count; i++)
            {
                int idx = indOfMinVal.BinarySearch(i, cmp);
                if (idx < 0)
                {
                    idx = -idx - 1;
                }
                pre[i] = indOfMinVal[idx - 1];
                if (idx == indOfMinVal.Count)
                {
                    indOfMinVal.Add(i);
                }
                else
                {
                    indOfMinVal[idx] = i;
                }
            }
            //倒退得到序列（不包括开头的-1）
            List<int> ret = new List<int>();
            for (int i = indOfMinVal[indOfMinVal.Count - 1]; i > 0; i = pre[i])
            {
                ret.Add(seq[i]);
            }
            ret.Reverse();
            return ret;
        };

        //检查LIS序列是否复合条件
        System.Func<List<int>, bool> CheckLIS = seq =>
        {
            if (seq.Count < 4 || seq[seq.Count - 1] - seq[0] < 6)
                return false;
            for (int i = 1; i < seq.Count; i++)
            {
                if (seq[i] - seq[i - 1] > 3)
                    return false;
            }
            return true;
        };

        return CheckLIS(GetLIS(GenSequence(true))) 
            || CheckLIS(GetLIS(GenSequence(false)));
    }

    public bool TestDash()
    {
        if (!dirDown)
            return false;
        int cnt = Mathf.CeilToInt(0.1f / Time.fixedDeltaTime);
        return keyRec.Any(rec =>
            rec.name == preDir && rec.e == KeyEvent.Down && rec.frame >= currentFrame - cnt);
    }
}
