using Moonwalk.Classes.Entities.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes
{
    /// <summary>
    /// A dictionary which has a type as its key, and a list of that type as the value. Things added to the lists
    /// are automatically sorted into the correct list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Assortment<T> : IEnumerable<T> where T : class
    {
        private Dictionary<Type, IList> lists;
        private List<Type> listTypes;

        public IList this[Type type]
        {
            get 
            { 
                return lists[type]; 
            }
        }

        public Assortment()
        {
            lists = new();
        }

        /// <summary>
        /// Use this constructor if you want to limit the types that can be added to the dictionary
        /// </summary>
        /// <param name="listTypes"></param>
        public Assortment(List<Type> listTypes)
        {
            //make sure the types do not inherit from each other
            foreach (Type type in listTypes)
            {
                foreach (Type type2 in listTypes)
                {
                    if (type2 == typeof(T))
                    {
                        break;
                    }

                    if (type.IsSubclassOf(type2) )
                    {
                        throw new Exception("No types in the list can inherit from one other unless it is the base class");
                    }
                }
            }


            lists = new();
            this.listTypes = listTypes;

            bool containsBaseType = false;

            //Add each key to the list
            foreach (Type type in listTypes)
            {
                if (type == typeof(T))
                {
                    containsBaseType = true;
                    continue;
                }

                Type listType = typeof(List<>).MakeGenericType(type);

                IList list = 
                    (IList)Activator.CreateInstance(listType);

                lists.Add(type, list);
            }

            if (containsBaseType)
            {
                Type listType = typeof(List<>).MakeGenericType(typeof(T));

                IList list =
                    (IList)Activator.CreateInstance(listType);

                lists.Add(typeof(T), list);
            }
        }

        public void Add(T item)
        {
            Type itemType = item.GetType();
            
            if (listTypes == null)
            {
                //Add to the key if it already exists
                if (lists.ContainsKey(itemType))
                {

                    lists[itemType].Add(item);
                }
                else        //Add new key and list if not
                {
                    Type listType = typeof(List<>).MakeGenericType(itemType);

                    lists.Add(itemType,
                        (IList)Activator.CreateInstance(listType));


                    lists[itemType].Add(item);
                }
            }
            else
            {
                //Search for a type that the item being added inherits from
                foreach (KeyValuePair<Type, IList> keyValuePair in lists)
                {
                    //When you find it, add to that list
                    if (itemType.IsSubclassOf(keyValuePair.Key)
                        || itemType.IsAssignableTo(keyValuePair.Key))
                    {

                        lists[keyValuePair.Key].Add(item);
                        break;
                    }
                }
            }
        }

        //Edit this method to look like Add (fix inheritance checking)
        public void Remove(T item)
        {
            Type itemType = item.GetType();

            if (listTypes == null)
            {
                //Add to the key if it already exists
                if (lists.ContainsKey(itemType))
                {

                    lists[itemType].Remove(item);
                }
                else        //Add new key and list if not
                {
                    Type listType = typeof(List<>).MakeGenericType(itemType);

                    lists.Add(itemType,
                        (IList)Activator.CreateInstance(listType));


                    lists[itemType].Remove(item);
                }
            }
            else
            {
                //Search for a type that the item being added inherits from
                foreach (KeyValuePair<Type, IList> keyValuePair in lists)
                {
                    //When you find it, add to that list
                    if (itemType.IsSubclassOf(keyValuePair.Key)
                        || itemType.IsAssignableTo(keyValuePair.Key))
                    {
                        lists[keyValuePair.Key].Remove(item);
                        break;
                    }
                }
            }
        }

        //Add inheritance checking
        public bool Contains(T item)
        {
            Type itemType = item.GetType();

            if (!lists.ContainsKey(itemType))
            {
                return false;
            }

            if (lists[itemType].Contains(item))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Allows each element to be iterated through
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            List<T> list = new List<T>();

            foreach (KeyValuePair<Type, IList> kv in lists)
            {
                foreach (T item in kv.Value)
                {
                    list.Add(item);
                }
            }

            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            List<T> list = new List<T>();

            foreach (KeyValuePair<Type, IList> kv in lists)
            {
                foreach (T item in kv.Value)
                {
                    list.Add(item);
                }
            }

            return list.GetEnumerator();
        }

        public List<T2> GetAllOfType<T2>()
        {
            List<T> list = new List<T>();

            foreach (T item in this)
            {
                //Check if the entity is the wanted type
                if (item is T2 || item.GetType().IsSubclassOf(typeof(T2)))
                {
                    list.Add(item);
                }
            }

            return list.Cast<T2>().ToList();
        }

        public List<T> ToList() {
            List<T> list = new();

            foreach (T data in this)
                list.Add(data);

            return list;
        }
    }
}
