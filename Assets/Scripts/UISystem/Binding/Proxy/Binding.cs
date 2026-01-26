using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;

public class Binding : AbstractBinding
{
    private BindingMode bindingMode = BindingMode.Default;
    BindingDescription bindingDescription;
    private ISourceProxy sourceProxy;
    private ITargetProxy targetProxy;

    private EventHandler sourceValueChangedHandler;
    private EventHandler targetValueChangedHandler;

    private bool isUpdatingSource;
    private bool isUpdatingTarget;
    private string targetTypeName;
    private SendOrPostCallback updateTargetAction;

    public Binding(object source, object target, BindingDescription bindingDescription):base(source, target)
    {
        this.targetTypeName = target.GetType().Name;
        this.bindingDescription = bindingDescription;

        this.CreateTargetProxy(target, this.bindingDescription);
        this.CreateSourceProxy(this.DataContext, this.bindingDescription.Source);

        UpdateDataOnBind();
    }

    protected string GetViewName()
    {
        return "unknown";
    }
    protected override void OnDataContextChanged()
    {
        if (this.bindingDescription.Source.IsStatic)
            return;

        this.CreateSourceProxy(this.DataContext, this.bindingDescription.Source);
        
    }

    protected BindingMode BindingMode
    {
        get
        {
            if(this.bindingMode != BindingMode.Default)
                return this.bindingMode;

            this.bindingMode = this.bindingDescription.Mode;
            if (bindingMode == BindingMode.Default)
                this.bindingMode = this.targetProxy.DefaultMode;

            if (bindingMode == BindingMode.Default)
                Debug.Log("Not set the BindingMode!");

            return this.bindingMode;
        }
    }

    protected void UpdateDataOnBind()
    {
        try
        {
            if(this.UpdateTargetOnFirstBind(this.BindingMode) && this.targetProxy != null)
            {
                this.UpdateTargetFromSource();
            }
            if (this.UpdateSourceOnFirstBind(this.BindingMode) && this.targetProxy != null)
            {
                this.UpdateSourceFromTarget();
            }
        }
        catch (Exception e)
        {
            Debug.LogFormat("An exception occurs in UpdateTGargetOnBind.exception: {0}", e);
        }
    }

    protected void CreateSourceProxy(object source, SourceDescription description)
    {
        this.DisposeSourceProxy();

        string path = description.Path;
        var nodes = path.Split('.');
        string sourceNodeName = nodes[nodes.Length - 1];

        var type = source.GetType();
        var memberInfo = type.GetMember(sourceNodeName);
        if(memberInfo == null)
        {
            // 可能获取的是private的member，因此如果之前GetMember失败，再尝试一次private的获取
            memberInfo = type.GetMember(sourceNodeName, BindingFlags.NonPublic | BindingFlags.Instance);
        }
        if (memberInfo == null)
        {
            throw new MissingMemberException(type.Name, description.ToString());
        }

        // property
        var propertyInfo = memberInfo[0] as PropertyInfo;
        if(propertyInfo != null)
        {
            this.sourceProxy = new PropertyNodeProxy(source, propertyInfo);
        }

        // Listen to value changed
        if(this.IsSubscribeSourceValueChanged(this.BindingMode) && this.sourceProxy is INotifiable)
        {
            this.sourceValueChangedHandler = (sender, args) => this.UpdateTargetFromSource();
            (this.sourceProxy as INotifiable).ValueChanged += this.sourceValueChangedHandler;
        }
    }

    protected void DisposeSourceProxy()
    {
        try
        {
            if (this.sourceProxy != null)
            {
                if (this.sourceValueChangedHandler != null)
                {
                    (this.sourceProxy as INotifiable).ValueChanged -= this.sourceValueChangedHandler;
                    this.sourceValueChangedHandler = null;
                }

                //this.sourceProxy.Dispose();
                this.sourceProxy = null;
            }
        }
        catch (Exception) { }
    }

    protected void CreateTargetProxy(object target, BindingDescription description)
    {
        this.DisposeTargetProxy();

        var targetName = description.TargetName;
        var type = description.TargetType;

        var memberInfo = type.GetMember(targetName);
        if (memberInfo == null)
        {
            // 可能获取的是private的member，因此如果之前GetMember失败，再尝试一次private的获取
            memberInfo = type.GetMember(targetName, BindingFlags.NonPublic | BindingFlags.Instance);
        }
        if (memberInfo == null)
        {
            throw new MissingMemberException(type.Name, description.ToString());
        }

        var propertyInfo = memberInfo[0] as PropertyInfo;
        if(propertyInfo != null )
        {
            this.targetProxy = new PropertyTargetProxy(target, propertyInfo);
        }
        // continue try other proxy

        // Listen to value changed
        if (this.IsSubscribeTargetValueChanged(this.BindingMode) && this.targetProxy is INotifiable)
        {
            this.targetValueChangedHandler = (sender, args) => this.UpdateTargetFromSource();
            (this.targetProxy as INotifiable).ValueChanged += this.targetValueChangedHandler;
        }
    }

    protected void DisposeTargetProxy()
    {
        try
        {
            if (this.targetProxy != null)
            {
                if (this.targetValueChangedHandler != null)
                {
                    (this.targetProxy as INotifiable).ValueChanged -= this.targetValueChangedHandler;
                    this.targetValueChangedHandler = null;
                }
                //this.targetProxy.Dispose();
                this.targetProxy = null;
            }
        }
        catch (Exception) { }
    }

    protected virtual void UpdateTargetFromSource()
    {
        try
        {
            if (this.isUpdatingSource)
                return;

            this.isUpdatingTarget = true;

            IObtainable obtainable = this.sourceProxy as IObtainable;
            if (obtainable == null)
                return;

            IModifiable modifier = this.targetProxy as IModifiable;
            if (modifier == null)
                return;

            TypeCode typeCode = this.sourceProxy.TypeCode;
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    {
                        var value = obtainable.GetValue<bool>();
                        this.SetTargetValue(modifier, value);
                        break;
                    }
                case TypeCode.Byte:
                    {
                        var value = obtainable.GetValue<byte>();
                        this.SetTargetValue(modifier, value);
                        break;
                    }
                case TypeCode.Char:
                    {
                        var value = obtainable.GetValue<char>();
                        this.SetTargetValue(modifier, value);
                        break;
                    }
                case TypeCode.DateTime:
                    {
                        var value = obtainable.GetValue<DateTime>();
                        this.SetTargetValue(modifier, value);
                        break;
                    }
                case TypeCode.Decimal:
                    {
                        var value = obtainable.GetValue<float>();
                        this.SetTargetValue(modifier, value);
                        break;
                    }
                case TypeCode.Double:
                    {
                        var value = obtainable.GetValue<double>();
                        this.SetTargetValue(modifier, value);
                        break;
                    }
                case TypeCode.Int16:
                    {
                        var value = obtainable.GetValue<short>();
                        this.SetTargetValue(modifier, value);
                        break;
                    }
                case TypeCode.Int32:
                    {
                        var value = obtainable.GetValue<int>();
                        this.SetTargetValue(modifier, value);
                        break;
                    }
                case TypeCode.Int64:
                    {
                        var value = obtainable.GetValue<long>();
                        this.SetTargetValue(modifier, value);
                        break;
                    }
                case TypeCode.SByte:
                    {
                        var value = obtainable.GetValue<sbyte>();
                        this.SetTargetValue(modifier, value);
                        break;
                    }
                case TypeCode.Single:
                    {
                        var value = obtainable.GetValue<float>();
                        this.SetTargetValue(modifier, value);
                        break;
                    }
                case TypeCode.String:
                    {
                        var value = obtainable.GetValue<string>();
                        this.SetTargetValue(modifier, value);
                        break;
                    }
                case TypeCode.UInt16:
                    {
                        var value = obtainable.GetValue<ushort>();
                        this.SetTargetValue(modifier, value);
                        break;
                    }
                case TypeCode.UInt32:
                    {
                        var value = obtainable.GetValue<uint>();
                        this.SetTargetValue(modifier, value);
                        break;
                    }
                case TypeCode.UInt64:
                    {
                        var value = obtainable.GetValue<ulong>();
                        this.SetTargetValue(modifier, value);
                        break;
                    }
                case TypeCode.Object:
                    {
                        Type valueType = this.sourceProxy.Type;
                        if (valueType.Equals(typeof(Vector2)))
                        {
                            var value = obtainable.GetValue<Vector2>();
                            this.SetTargetValue(modifier, value);
                        }
                        else if (valueType.Equals(typeof(Vector3)))
                        {
                            var value = obtainable.GetValue<Vector3>();
                            this.SetTargetValue(modifier, value);
                        }
                        else if (valueType.Equals(typeof(Vector4)))
                        {
                            var value = obtainable.GetValue<Vector4>();
                            this.SetTargetValue(modifier, value);
                        }
                        else if (valueType.Equals(typeof(Color)))
                        {
                            var value = obtainable.GetValue<Color>();
                            this.SetTargetValue(modifier, value);
                        }
                        else if (valueType.Equals(typeof(Rect)))
                        {
                            var value = obtainable.GetValue<Rect>();
                            this.SetTargetValue(modifier, value);
                        }
                        else if (valueType.Equals(typeof(Quaternion)))
                        {
                            var value = obtainable.GetValue<Quaternion>();
                            this.SetTargetValue(modifier, value);
                        }
                        else if (valueType.Equals(typeof(TimeSpan)))
                        {
                            var value = obtainable.GetValue<TimeSpan>();
                            this.SetTargetValue(modifier, value);
                        }
                        else
                        {
                            var value = obtainable.GetValue();
                            this.SetTargetValue(modifier, value);
                        }
                        break;
                    }
                default:
                    {
                        var value = obtainable.GetValue();
                        this.SetTargetValue(modifier, value);
                        break;
                    }
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("An exception occurs when the target property is updated.Please check the binding \"{0}{1}\" in the view \"{2}\".exception: {3}", this.targetTypeName, this.bindingDescription.ToString(), GetViewName(), e);
            throw;
        }
        finally
        {
            this.isUpdatingTarget = false;
        }
    }

    protected virtual void UpdateSourceFromTarget()
    {
        try
        {
            if (this.isUpdatingTarget)
                return;

            this.isUpdatingSource = true;


            IObtainable obtainable = this.targetProxy as IObtainable;
            if (obtainable == null)
                return;

            IModifiable modifier = this.sourceProxy as IModifiable;
            if (modifier == null)
                return;

            TypeCode typeCode = this.targetProxy.TypeCode;
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    {
                        var value = obtainable.GetValue<bool>();
                        this.SetSourceValue(modifier, value);
                        break;
                    }
                case TypeCode.Byte:
                    {
                        var value = obtainable.GetValue<byte>();
                        this.SetSourceValue(modifier, value);
                        break;
                    }
                case TypeCode.Char:
                    {
                        var value = obtainable.GetValue<char>();
                        this.SetSourceValue(modifier, value);
                        break;
                    }
                case TypeCode.DateTime:
                    {
                        var value = obtainable.GetValue<DateTime>();
                        this.SetSourceValue(modifier, value);
                        break;
                    }
                case TypeCode.Decimal:
                    {
                        var value = obtainable.GetValue<decimal>();
                        this.SetSourceValue(modifier, value);
                        break;
                    }
                case TypeCode.Double:
                    {
                        var value = obtainable.GetValue<double>();
                        this.SetSourceValue(modifier, value);
                        break;
                    }
                case TypeCode.Int16:
                    {
                        var value = obtainable.GetValue<short>();
                        this.SetSourceValue(modifier, value);
                        break;
                    }
                case TypeCode.Int32:
                    {
                        var value = obtainable.GetValue<int>();
                        this.SetSourceValue(modifier, value);
                        break;
                    }
                case TypeCode.Int64:
                    {
                        var value = obtainable.GetValue<long>();
                        this.SetSourceValue(modifier, value);
                        break;
                    }
                case TypeCode.SByte:
                    {
                        var value = obtainable.GetValue<sbyte>();
                        this.SetSourceValue(modifier, value);
                        break;
                    }
                case TypeCode.Single:
                    {
                        var value = obtainable.GetValue<float>();
                        this.SetSourceValue(modifier, value);
                        break;
                    }
                case TypeCode.String:
                    {
                        var value = obtainable.GetValue<string>();
                        this.SetSourceValue(modifier, value);
                        break;
                    }
                case TypeCode.UInt16:
                    {
                        var value = obtainable.GetValue<ushort>();
                        this.SetSourceValue(modifier, value);
                        break;
                    }
                case TypeCode.UInt32:
                    {
                        var value = obtainable.GetValue<uint>();
                        this.SetSourceValue(modifier, value);
                        break;
                    }
                case TypeCode.UInt64:
                    {
                        var value = obtainable.GetValue<ulong>();
                        this.SetSourceValue(modifier, value);
                        break;
                    }
                case TypeCode.Object:
                    {
                        Type valueType = this.targetProxy.Type;
                        if (valueType.Equals(typeof(Vector2)))
                        {
                            var value = obtainable.GetValue<Vector2>();
                            this.SetSourceValue(modifier, value);
                        }
                        else if (valueType.Equals(typeof(Vector3)))
                        {
                            var value = obtainable.GetValue<Vector3>();
                            this.SetSourceValue(modifier, value);
                        }
                        else if (valueType.Equals(typeof(Vector4)))
                        {
                            var value = obtainable.GetValue<Vector4>();
                            this.SetSourceValue(modifier, value);
                        }
                        else if (valueType.Equals(typeof(Color)))
                        {
                            var value = obtainable.GetValue<Color>();
                            this.SetSourceValue(modifier, value);
                        }
                        else if (valueType.Equals(typeof(Rect)))
                        {
                            var value = obtainable.GetValue<Rect>();
                            this.SetSourceValue(modifier, value);
                        }
                        else if (valueType.Equals(typeof(Quaternion)))
                        {
                            var value = obtainable.GetValue<Quaternion>();
                            this.SetSourceValue(modifier, value);
                        }
                        else if (valueType.Equals(typeof(TimeSpan)))
                        {
                            var value = obtainable.GetValue<TimeSpan>();
                            this.SetSourceValue(modifier, value);
                        }
                        else
                        {
                            var value = obtainable.GetValue();
                            this.SetSourceValue(modifier, value);
                        }
                        break;
                    }
                default:
                    {
                        var value = obtainable.GetValue();
                        this.SetSourceValue(modifier, value);
                        break;
                    }
            }
        }
        catch (Exception e)
        {
             Debug.LogErrorFormat("An exception occurs when the source property is updated.Please check the binding \"{0}{1}\" in the view \"{2}\".exception: {3}", this.targetTypeName, this.bindingDescription.ToString(), GetViewName(), e);
        }
        finally
        {
            this.isUpdatingSource = false;
        }
    }

    protected void SetTargetValue<T>(IModifiable modifier, T value)
    {
        modifier.SetValue(value);
    }

    protected void SetSourceValue<T>(IModifiable modifier, T value)
    {
        modifier.SetValue(value);
    }
    protected void DoUpdateTargetFromSource()
    {

    }

    protected bool IsSubscribeSourceValueChanged(BindingMode bindingMode)
    {
        switch (bindingMode)
        {
            case BindingMode.Default:
                return true;

            case BindingMode.TwoWay:
            case BindingMode.OneWay:
                return true;

            case BindingMode.OneTime:
            case BindingMode.OneWayToSource:
                return false;

            default:
                throw new Exception("Unexpected BindingMode");
        }
    }

    protected bool IsSubscribeTargetValueChanged(BindingMode bindingMode)
    {
        switch (bindingMode)
        {
            case BindingMode.Default:
                return true;

            case BindingMode.OneWay:
            case BindingMode.OneTime:
                return false;

            case BindingMode.TwoWay:
            case BindingMode.OneWayToSource:
                return true;

            default:
                throw new Exception("Unexpected BindingMode");
        }
    }

    protected bool UpdateTargetOnFirstBind(BindingMode bindingMode)
    {
        switch (bindingMode)
        {
            case BindingMode.Default:
                return true;
            case BindingMode.TwoWay:
            case BindingMode.OneWay:
            case BindingMode.OneTime:
                return true;
            case BindingMode.OneWayToSource:
                return false;
            default:
                throw new Exception("Unexpected BindingMode");
               
        }
    }

    protected bool UpdateSourceOnFirstBind(BindingMode bindingMode)
    {
        switch (bindingMode)
        {
            case BindingMode.OneWayToSource:
                return true;

            case BindingMode.Default:
                return false;

            case BindingMode.TwoWay:
            case BindingMode.OneWay:
            case BindingMode.OneTime:
                return false;
            
            default:
                throw new Exception("Unexpected BindingMode");
        }
    }
}
