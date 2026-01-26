using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class BindingDescription
{
    public string TargetName { get; set; }
    public Type TargetType { get; set; }
    public string UpdateTrigger {  get; set; } 
    public BindingMode Mode { get; set; }
    public SourceDescription Source { get; set; }
    public object CommandParameter {  get; set; }

    public BindingDescription()
    {

    }

    public BindingDescription(string targetName, string path, BindingMode bindingMode = BindingMode.Default)
    {
        TargetName = targetName;
        Mode = bindingMode;
        this.Source = new SourceDescription(path);
    }

    public override string ToString()
    {
        StringBuilder buf = new StringBuilder();
        buf.Append("{binding ").Append(this.TargetName);

        if (!string.IsNullOrEmpty(this.UpdateTrigger))
            buf.Append(" UpdateTrigger:").Append(this.UpdateTrigger);

        if (this.Source != null)
            buf.Append(" ").Append(this.Source.ToString());

        if (this.CommandParameter != null)
            buf.Append(" CommandParameter:").Append(this.CommandParameter);

        buf.Append(" Mode:").Append(this.Mode.ToString());
        buf.Append(" }");
        return buf.ToString();
    }
}
