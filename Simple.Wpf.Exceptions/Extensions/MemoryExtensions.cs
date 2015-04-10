using System;
using System.Collections.Generic;
using System.ComponentModel;
using Simple.Wpf.Exceptions.Models;

namespace Simple.Wpf.Exceptions.Extensions
{
    public static class MemoryExtensions
    {
        private static readonly IDictionary<MemoryUnits, string> UnitsAsString = new Dictionary<MemoryUnits, string>();
        private static readonly IDictionary<MemoryUnits, decimal> UnitsMulitpler = new Dictionary<MemoryUnits, decimal>();

        private static readonly Type MemoryUnitsType = typeof(MemoryUnits);

        public static string WorkingSetPrivateAsString(this Memory memory)
        {
            return ValueAsString(() => memory.WorkingSetPrivate, MemoryUnits.Mega, 2);
        }

        public static string ManagedAsString(this Memory memory)
        {
            return ValueAsString(() => memory.Managed, MemoryUnits.Mega, 2);
        }

        private static string ValueAsString(Func<decimal> valueFunc, MemoryUnits units, int decimalPlaces)
        {
            return string.Format("{0:0.00} {1}", decimal.Round(valueFunc() * GetMultipler(units), decimalPlaces), GetUnitString(units));
        }

        private static decimal GetMultipler(MemoryUnits units)
        {
            decimal unitsMulitpler;
            if (UnitsMulitpler.TryGetValue(units, out unitsMulitpler))
            {
                return unitsMulitpler;
            }

            unitsMulitpler = 1 / Convert.ToDecimal(units);

            UnitsMulitpler.Add(units, unitsMulitpler);
            return unitsMulitpler;
        }

        private static string GetUnitString(MemoryUnits units)
        {
            string unitsString;
            if (UnitsAsString.TryGetValue(units, out unitsString))
            {
                return unitsString;
            }

            var memInfo = MemoryUnitsType.GetMember(units.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            unitsString = ((DescriptionAttribute)attributes[0]).Description;

            UnitsAsString.Add(units, unitsString);
            return unitsString;
        }
    }
}
