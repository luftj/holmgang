using System;
using System.Collections.Generic;

namespace holmgang.Desktop
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OnlyOneAttribute : Attribute
    {
    }

    public class OnlyOneException : Exception
    {
        public OnlyOneException(Component c) : base(
            "Oooooonnnly oooooone!\nThere can be only on component of this type attached to an entity. Component type: " 
            + c.GetType()){}
    }

    public abstract class Component
    {
        public static int idcounter = 0;
        public int ID; // use this for references when saving state

        public Component()
        {
            ID = idcounter++;
        }

        //public void setAllFields(params FieldInfo[] fields)
        //{
        //    // make sure idcounter fits
        //}

        public virtual string serialise()
        {
            string ret = "<" + this.GetType().ToString() + ">\n";
            foreach(var field in this.GetType().GetFields())
            {
                if(field.FieldType.BaseType == typeof(Component))
                {
                    var val = field.GetValue(this);
                    if(val == null)
                        ret += field.Name + ":" + "null" + "\n";
                    else
                        ret += field.Name + ":" + (val as Component).ID + "\n";
                } else if(field.FieldType == typeof(Entity))
                {
                    var val = field.GetValue(this);
                    if(val == null)
                        ret += field.Name + ":" + "null" + "\n";
                    else
                        ret += field.Name + ":" + (val as Entity).ID + "\n";
                } else if(field.FieldType == typeof(List<string>))
                {
                    var val = field.GetValue(this);
                    if(val == null)
                        ret += field.Name + ":" + "null" + "\n";
                    else
                    {
                        ret += field.Name + ":";
                        var list = (val as List<string>);
                        for(int i = 0; i < list.Count;++i)
                        {
                            ret += list[i];
                            if(i == list.Count - 1)
                                ret += "\n";
                            else
                                ret += "|";
                        }
                    }
                }
                else
                {
                    ret += field.Name + ":" + field.GetValue(this) + "\n";
                }
            }
            ret+="</" + this.GetType().ToString() + ">\n";
            return ret; 
        }
    }
}
