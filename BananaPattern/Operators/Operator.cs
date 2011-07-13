using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BananaPattern.Extensions;

namespace BananaPattern.Operators
{
    public abstract class Operator
    {
        private static readonly Dictionary<string, Type> _operatorMap = new Dictionary<string, Type>();
        private Lazy<IntPtr> _target;
        public IntPtr Target
        {
            get
            {
                return _target.Value;
            }
            set
            {
                _target = new Lazy<IntPtr>(() => value);
            }
        }

        /// <summary>
        /// Used to programmatically register a type with a string identifier.
        /// </summary>
        /// <param name="identifier">String identifier for the type mapping.</param>
        /// <param name="operatorType">Type to instantiate when Create(string, string) is called.</param>
        /// <returns>The type which was formerly registered or, if there wasn't any, null.</returns>
        public static Type RegisterOperator(string identifier, Type operatorType)
        {
            if (!operatorType.IsSubclassOf(typeof(Operator)))
                throw new InvalidOperationException("The operatorType has to derive from Operator.");

            Type ret;
            bool wasRegistered = _operatorMap.TryGetValue(identifier, out ret);

            _operatorMap[identifier] = operatorType;

            return wasRegistered ? ret : null;
        }

        /// <summary>
        /// Unregister a registered type.
        /// </summary>
        /// <param name="identifier">Identifier of registered type</param>
        /// <returns>True, if there was a type registered for that identifier, false otherweise.</returns>
        public static bool UnregisterOperator(string identifier)
        {
            return _operatorMap.Remove(identifier);
        }

        /// <summary>
        /// Get the identifier for a type in the internal type map.
        /// </summary>
        /// <param name="typeOfOperator">The registered type.</param>
        /// <returns>The identifier for a type or null, when the type was not registered</returns>
        public static string GetRegisteredIdentifier(Type typeOfOperator)
        {
            var pair = _operatorMap.FirstOrDefault(p => p.Value == typeOfOperator);

            if (pair.Key == null)
            {
                EnforceScan();
                pair = _operatorMap.FirstOrDefault(p => p.Value == typeOfOperator);
            }

            return pair.Key;
        }

        /// <summary>
        /// Factory method for creating a new Operator identified by <paramref name="identifier"/>.
        /// Calls the Constructor of the registered type taking a string as argument.
        /// </summary>
        /// <param name="identifier">String identifier for the type mapping.</param>
        /// <param name="value">Constructor argument to initalize the operator instance from.</param>
        /// <returns></returns>
        public static Operator Create(string identifier, Func<string> valueFactory, Func<string> targetFactory)
        {
            Type type;
            bool isRegistered = _operatorMap.TryGetValue(identifier, out type);

            if (!isRegistered)
            {
                EnforceScan();
                isRegistered = _operatorMap.TryGetValue(identifier, out type);

                if (!isRegistered)
                {
                    throw new PatternException("No operator factory method associated with " + identifier 
                        + ". Check your OperatorAttributes.");
                }
            }

            return (Operator)Activator.CreateInstance(type, valueFactory, targetFactory);
        }

        private static void EnforceScan()
        {
            var operatorTypes = ScanLoadedAssembliesForOperatorTypes();

            foreach (var operatorType in operatorTypes)
            {
                OperatorAttribute attribute = operatorType.GetCustomAttributes(typeof(OperatorAttribute), false)
                    .Cast<OperatorAttribute>()
                    .First();

                if (!_operatorMap.ContainsKey(attribute.Identifier))
                {
                    _operatorMap[attribute.Identifier] = operatorType;
                }
            }
        }

        private static IEnumerable<Type> ScanLoadedAssembliesForOperatorTypes()
        {
            var nonDynamicAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic);

            var operatorTypes = nonDynamicAssemblies
                .SelectMany(a => a.GetExportedTypes()
                    .Where(t => t.IsSubclassOf(typeof(Operator)) && t.HasAttribute<OperatorAttribute>()));

            return operatorTypes;
        }

        /// <summary>
        /// Initializes a new Instance.
        /// </summary>
        /// <param name="target">Initial Target value.</param>
        public Operator(IntPtr target)
        {
            _target = new Lazy<IntPtr>(() => target);
        }

        /// <summary>
        /// Attempts to lazy initialize a new instance from a factory.
        /// </summary>
        /// <param name="targetFactory">Factory providing a parseable value for Target.</param>
        public Operator(Func<string> targetFactory)
        {
            _target = new Lazy<IntPtr>(() => ParseTarget(targetFactory()));
        }

        private static IntPtr ParseTarget(string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return IntPtr.Zero;
            }
            else
            {
                long value = Convert.ToInt64(target, 0x10);
                return new IntPtr(value);
            }
        }

        /// <summary>
        /// Executes the operation to the lazy evaluated target with the aid of <paramref name="memory"/>'s methods.
        /// </summary>
        /// <param name="memory">Memory instance to use while operating on the address.</param>
        /// <returns>The resulting address value.</returns>
        public abstract IntPtr Execute(IMemory memory);

        /// <summary>
        /// Should be true, if the Operator's results can be cached through builds.
        /// </summary>
        public abstract bool IsCacheable { get; }
    }
}
