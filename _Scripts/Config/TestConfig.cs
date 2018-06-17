using FGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TestConfig
{
    public TestConfig()
    {
        id = 0;
        name = "abcd";
    }

    [Member(0)]
    public int id;

    [Member(1, "data")]
    public int len;

    [Member(2, 10)]
    public string name;

    [Member(3)]
    public int[] data;

    public override string ToString()
    {
        return string.Format("id = {0}\nname = {1}", id, name);
    }
}

[Serializable]
public class TestA
{
    [Member(0)]
    public int id;
    [Member(1)]
    public byte iden;
    [Member(2)]
    public int num;

    public override string ToString()
    {
        return id.ToString() + iden.ToString() + num.ToString();
    }
}

public class MSG_C2S_BASE
{
    public MSG_C2S_BASE()
    {
        type = 0;
    }

    public MSG_C2S_BASE(byte t)
    {
        type = t;
    }

    [Member(0)]
    public byte type;
}

public class MSG_C2S_TEST : MSG_C2S_BASE
{
    public MSG_C2S_TEST() : base(CONST.MSG_C2S_TEST)
    {
    }

    [Member(0)]
    public int id;
    [Member(1)]
    public string name;
    [Member(2)]
    public int gNum;
    [Member(3)]
    public int qNum;
    [Member(4)]
    public byte[] array;
    [Member(5)]
    public List<int> listInt = new List<int>();
}
