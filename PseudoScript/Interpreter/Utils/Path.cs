using System;
using System.Collections.Generic;
using System.Linq;

namespace PseudoScript.Interpreter.Utils
{
    public class Path<T>
    {
        readonly List<T> path;

        public Path() : this(new List<T>()) { }

        public Path(T path) : this(new List<T>() { path }) { }

        public Path(Path<T> path) : this(new List<T>(path.path)) { }

        public Path(List<T> path)
        {
            this.path = new List<T>(path);
        }


        public T Next()
        {
            if (path.Count == 0)
            {
                return default;
            }

            T segment = path.First();
            path.RemoveAt(0);

            return segment;
        }

        public T Last()
        {
            if (path.Count == 0)
            {
                return default;
            }

            T segment = path.Last();
            path.RemoveAt(path.Count - 1);

            return segment;
        }

        public void Add(T value)
        {
            path.Add(value);
        }

        public int Count
        {
            get { return path.Count; }
        }

        public override string ToString()
        {
            return String.Join(".", path);
        }
    }
}
