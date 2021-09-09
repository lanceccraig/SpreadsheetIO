using System;
using LanceC.SpreadsheetIO.Mapping;
using LanceC.SpreadsheetIO.Mapping.Internal.Extensions;
using LanceC.SpreadsheetIO.Shared;

namespace LanceC.SpreadsheetIO.Reading.Internal.Parsing
{
    internal class DateTimeResourcePropertyParserStrategy : IDefaultResourcePropertyParserStrategy
    {
        public Type PropertyType { get; } = typeof(DateTime);

        public ResourcePropertyParseResultKind TryParse<TResource>(string cellValue, PropertyMap<TResource> map, out object? value)
            where TResource : class
        {
            var noValue = string.IsNullOrEmpty(cellValue);
            if (noValue)
            {
                value = null;
                return Nullable.GetUnderlyingType(map.Property.PropertyType) is not null
                    ? ResourcePropertyParseResultKind.Empty
                    : ResourcePropertyParseResultKind.Missing;
            }

            var extension = map.Options.FindExtension<DateKindMapOptionsExtension>();
            if (extension is null || extension.DateKind == CellDateKind.Number)
            {
                return TryParseFromNumber(cellValue, out value);
            }
            else
            {
                return TryParseFromText(cellValue, out value);
            }
        }

        private static ResourcePropertyParseResultKind TryParseFromNumber(string cellValue, out object? value)
        {
            var hasDoubleValue = double.TryParse(cellValue, out var doubleValue);
            if (!hasDoubleValue)
            {
                value = null;
                return ResourcePropertyParseResultKind.Invalid;
            }

            try
            {
                var dateTimeValue = DateTime.FromOADate(doubleValue);
                value = dateTimeValue;
                return ResourcePropertyParseResultKind.Success;
            }
            catch (ArgumentException)
            {
                value = null;
                return ResourcePropertyParseResultKind.Invalid;
            }
        }

        private static ResourcePropertyParseResultKind TryParseFromText(string cellValue, out object? value)
        {
            var hasDateTimeValue = DateTime.TryParse(cellValue, out var dateTimeValue);
            if (!hasDateTimeValue)
            {
                value = null;
                return ResourcePropertyParseResultKind.Invalid;
            }

            value = dateTimeValue;
            return ResourcePropertyParseResultKind.Success;
        }
    }
}
