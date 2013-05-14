using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Codeable.Foundation.Core.System
{
    public interface IFindClassTypes
    {
        IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses);
        IEnumerable<Type> FindClassesOfType<T>(IList<string> assemblyNamesToInclude, bool onlyConcreteClasses);

        IList<Assembly> GetAssemblies(IList<string> assemblyNamesToInclude);
    }
}
