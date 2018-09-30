using System;
using System.Collections.Generic;
using System.Linq;

namespace holmgang.Desktop
{
    public class Entity
    {
        private static int idcounter = 0;
        public int ID;
        List<Component> components { get; }
        List<Component> removeList { get; }
        List<Component> addList { get; }

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
            if(c.GetType().IsDefined(typeof(OnlyOneAttribute),true))
            {
                if(components.Exists(x => (x.GetType() == c.GetType())))
                    throw new OnlyOneException(c);
            }
            c.setOwner(this);
            components.Add(c);
        }

        public void detach(Component c)
        {
            c.setOwner(null);   // throw nullreference exceptions, when this component is still referenced somewhere
            components.Remove(c);
        }

        public void detachAll<T>() where T : Component
        {
            List<T> list = getAll<T>();
            foreach(var item in list)
            {
                detach(item);
            }
        }

        public bool has<T>() where T : Component 
        {
            if(components.Exists(x => (x is T)))
                return true;
            else return false;
        }

        /// <summary>
        /// Gets first component of type T in this entity.
        /// </summary>
        /// <returns>First occurence or null.</returns>
        /// <typeparam name="T">The component type.</typeparam>
        public T get<T>() where T : Component 
        {
            return components.Find(x => x is T) as T;
            //if(ret.Count == 1)
            //    return ret[0];
            //else
                //throw new FieldAccessException("too many components of type " + (string)T.GetType());
        }

        public List<T> getAll<T>() where T : Component 
        {
            //List<Component> all = 
            return components.FindAll(x => x is T).Cast<T>().ToList();
            //List<T> ret = all.Cast<T>().ToList(); //new List<T>();
            //foreach(var item in all)
            //{
            //    ret.Add((T)item); // this is really strange... should be able to cast lists with given constraint?
            //}
            //var ret = all as List<T>;
            //var ret = (List<T>)bla;
            //return ret;
        }

        public string saveEntity()
        {
            string ret = "<Entity>\n";
            ret += "id:" + ID +"\n";
            foreach(var c in components)
                ret += c.serialise();
            ret += "</Entity>\n";
            return ret;
        }

        public Component getByID(int id)
        {
            foreach(var c in components)
            {
                if(c.ID == id)
                    return c;
            }
            return null;
        }
    }
}
