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
    internal class ListDictionary<T> where T : class
    {
        private Dictionary<Type, IList> lists;
        private List<Type> listTypes;

        public IList this[Type type]
        {
            get { return lists[type]; }
        }

        public ListDictionary()
        {
            lists = new();
        }

        /// <summary>
        /// Use this constructor if you want to limit the types that can be added to the dictionary
        /// </summary>
        /// <param name="listTypes"></param>
        public ListDictionary(List<Type> listTypes)
        {
            lists = new();
            this.listTypes = listTypes;
        }

        public void Add(T item)
        {
            Type itemType = item.GetType();

            //If the list of possible types doesn't contain the type, don't add
            if (listTypes != null && !listTypes.Contains(itemType))
            {
                return;
            }

            if (!lists.ContainsKey(itemType))
            {
                Type listType = typeof(List<>).MakeGenericType(itemType);

                lists.Add(itemType,
                    (IList)Activator.CreateInstance(listType));
            }

            lists[itemType].Add(item);
        }

        public void Remove(T item)
        {
            Type itemType = item.GetType();

            if (!lists.ContainsKey(itemType))
            {
                return;
            }

            if (lists[itemType].Contains(item))
            {
                lists[itemType].Remove(item);
            }
        }

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
    }
}
