using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace holmgang.Desktop
{
    public struct UnresolvedCompRef
    {
        public Component c;
        public FieldInfo finfo;
        public int id;
    }
    public struct UnresolvedEntityRef
    {
        public Component c;
        public FieldInfo finfo;
        public int id;
    }

    public class SavefileHandler
    {
        Entity currentEntity;
        string currentComponentTypeString;
        Component currentComponent;
        Type currentComponentType;
        bool inEntity = false;
        bool inComponent = false;
        List<UnresolvedCompRef> unresolvedCompRefs;
        List<UnresolvedEntityRef> unresolvedEntityRefs;

        string filepath;
        List<Entity> results;

        public SavefileHandler(string filepath)
        {
            this.filepath = filepath;
            results = new List<Entity>();
            unresolvedCompRefs = new List<UnresolvedCompRef>();
            unresolvedEntityRefs = new List<UnresolvedEntityRef>();
        }

        public void readFile()
        {
            foreach(var line in File.ReadLines(filepath))
            {
                readLine(line);
            }
        }

        public void readLine(string line)
        {
            if(line == "<Entity>")
            {
                currentEntity = new Entity();
                inEntity = true;
            }
            else if(line == "</Entity>")
            {
                resolveCompRefs();
                results.Add(currentEntity);
                currentEntity = null;
                inEntity = false;
            }
            else if(inEntity && line.Contains("id"))
            {
                // set current entity id
                currentEntity.ID = Int32.Parse(line.Split(':')[1]);
            }
            else if(line.Contains("Component"))
            {
                if(line.StartsWith("</"))
                {
                    // end
                    inComponent = false;
                    currentEntity.attach(currentComponent);
                    currentComponent = null;
                } else
                {
                    inComponent = true;
                    currentComponentTypeString = line.Trim('<', '>');
                    currentComponentType = Type.GetType(currentComponentTypeString + ", " + this.GetType().Assembly); // todo save namespace in file
                    var ctor = currentComponentType.GetConstructor(new Type[0]); // get default constructor
                    currentComponent = (Component) ctor.Invoke(new object[0]);
                }
            } else if(inComponent)
            {
                //todo can/should this go to component class?
                var kvp = line.Split(":".ToCharArray(),count:2);
                var fi = currentComponent.GetType().GetField(kvp[0]);
                if(kvp[0] == "id")
                {   //make sure ids dont get reused
                    if(Component.idcounter <= Int32.Parse(kvp[1]))
                        Component.idcounter = Int32.Parse(kvp[1]);
                }
                if(fi.FieldType == typeof(Int32))
                    fi.SetValue(currentComponent, Int32.Parse(kvp[1]));
                else if(fi.FieldType == typeof(Single))
                    fi.SetValue(currentComponent, Single.Parse(kvp[1]));
                else if(fi.FieldType == typeof(String))
                    fi.SetValue(currentComponent, kvp[1]);
                else if(fi.FieldType == typeof(Vector2))
                    fi.SetValue(currentComponent, parseVector2(kvp[1]));
                else if(fi.FieldType == typeof(Camera2D))
                    fi.SetValue(currentComponent, new Camera2D(GameSingleton.Instance.graphics));
                else if(fi.FieldType.BaseType == typeof(Component))
                {
                    if(kvp[1] != "null")
                        unresolvedCompRefs.Add(new UnresolvedCompRef() {
                            c = currentComponent,
                            finfo = fi,
                            id = Int32.Parse(kvp[1])
                        });
                }
                else if(fi.FieldType == typeof(Entity))
                {
                    unresolvedEntityRefs.Add(new UnresolvedEntityRef() {
                        c = currentComponent,
                        finfo = fi,
                        id = Int32.Parse(kvp[1])
                    });
                }
                else if(fi.FieldType == typeof(List<string>))
                {
                    var list = new List<string>(kvp[1].Split('|'));
                    fi.SetValue(currentComponent, list);
                }
            }
        }

        public List<Entity> getResults()
        {
            resolveEntityRefs();
            return results;
        }

        Vector2 parseVector2(string s)
        {
            var xy = s.Split(' ');
            var x = xy[0];
            var y = xy[1];
            x = x.TrimStart('{', 'X', ':');
            y = y.Trim('}', 'Y', ':');
            return new Vector2(Single.Parse(x), Single.Parse(y));
        }

        void resolveCompRefs()
        {
            foreach(var ur in unresolvedCompRefs)
            {
                Component foundc = currentEntity.getByID(ur.id);
                ur.finfo.SetValue(ur.c, foundc);
            }
            unresolvedCompRefs.Clear();
        }

        void resolveEntityRefs()
        {
            foreach( var ur in unresolvedEntityRefs)
            {
                Entity founde = results.Find(x => x.ID == ur.id);
                ur.finfo.SetValue(ur.c, founde);
            }
            unresolvedEntityRefs.Clear();
        }
    }
}
