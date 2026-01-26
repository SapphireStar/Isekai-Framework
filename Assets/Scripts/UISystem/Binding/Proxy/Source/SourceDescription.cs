using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceDescription
{
    private string path;

    private bool isStatic = false;
    public virtual bool IsStatic
    {
        get { return this.isStatic; }
        set { this.isStatic = value; }
    }

    public SourceDescription(string path)
    {
        this.path = path;
    }

    public virtual string Path
    {
        get { return this.path; }
        set
        {
            this.path = value;
        }
    }

    public override string ToString()
    {
        return string.IsNullOrEmpty(this.path)? "Path:null" : "Path" + this.path;
    }
}
