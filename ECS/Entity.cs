using System;
using System.Collections.Generic;

namespace holmgang.Desktop
{
    public class Entity
    {
        private static int idcounter = 0;
        int ID;
        List<Component> components;
        List<Component> removeList;
        List<Component> addList;

        public Entity()
        {
            ID = idcounter++;
            components = new List<Component>();
            removeList = new List<Component>();
            addList = new List<Component>();
        }

        //public static bool operator ==(Entity a, Entity b)
        //{
        //    if(a == null || b == null)
        //        return false;
        //    return a.ID == b.ID;
        //}
        //public static bool operator !=(Entity a, Entity b) => !(a == b);

        //public override bool Equals(object obj)
        //{
        //    var entity = obj as Entity;
        //    return entity != null && ID == entity.ID;
        //}

        public void attach(Component c)
        {
            components.Add(c);
        }

        public void detach(Component c)
        {
            components.Remove(c);
        }

        public bool has<T>() where T : class 
        {
            if(components.Exists(x => (x is T)))
                return true;
            else return false;
        }

        public T get<T>() where T : class 
        {
            return components.Find(x => x is T) as T;
            //if(ret.Count == 1)
            //    return ret[0];
            //else
                //throw new FieldAccessException("too many components of type " + (string)T.GetType());
        }

        public List<T> getAll<T>() where T : Component 
        {
            List<Component> bla = components.FindAll(x => x is T);
            List < T > ret = new List<T>();
            foreach(var item in bla)
            {
                ret.Add((T)item); // this is really strange... should be able to cast lists with given constraint?
            }
            //var ret = bla as List<T>;
            //var ret = (List<T>)bla;
            return ret;
        }

    }
}
