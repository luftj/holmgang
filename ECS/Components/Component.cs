using System;
namespace holmgang.Desktop
{
    public class Component
    {
        private static int idcounter = 0;
        public int ID; // use this for references when saving state

        public Component()
        {
            ID = idcounter++;
        }

        public virtual string saveComponent()
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
                } else
                {
                    ret += field.Name + ":" + field.GetValue(this) + "\n";
                }
            }
            ret+="</" + this.GetType().ToString() + ">\n";
            return ret; 
        }
    }
}
