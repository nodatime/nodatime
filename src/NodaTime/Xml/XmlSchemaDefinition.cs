// Copyright 2020 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NodaTime.Extensions;

namespace NodaTime.Xml
{
    /// <summary>
    /// Provides XML schema types.
    /// </summary>
    public static class XmlSchemaDefinition
    {
        /// <summary>
        /// Gets the XML namespace for all NodaTime types.
        /// </summary>
        /// <remarks>See [Namespaces in XML 1.1 (Second Edition)](https://www.w3.org/TR/xml-names11/).</remarks>
        public static XmlQualifiedName NodaTimeXmlNamespace { get; } = new XmlQualifiedName("nodatime", "https://nodatime.org/api/");

        /// <summary>
        /// Gets the compiled XML schema describing the structure for all NodaTime types that implement the <see cref="IXmlSerializable"/> interface.
        /// </summary>
        /// <remarks>
        /// All the pattern restrictions as regular expressions are not meant to fully validate the XML content,
        /// they only serve to describe the general shape of the XML content.
        /// </remarks>
        public static XmlSchema NodaTimeXmlSchema { get; }

        internal static XmlQualifiedName AddAnnualDateSchemaType(XmlSchemaSet schemaSet) => AddSchemaType(schemaSet, AnnualDateSchemaType);
        internal static XmlQualifiedName AddDurationSchemaType(XmlSchemaSet schemaSet) => AddSchemaType(schemaSet, DurationSchemaType);
        internal static XmlQualifiedName AddInstantSchemaType(XmlSchemaSet schemaSet) => AddSchemaType(schemaSet, InstantSchemaType);
        internal static XmlQualifiedName AddIntervalSchemaType(XmlSchemaSet schemaSet) => AddSchemaType(schemaSet, IntervalSchemaType);
        internal static XmlQualifiedName AddLocalDateSchemaType(XmlSchemaSet schemaSet) => AddSchemaType(schemaSet, LocalDateSchemaType);
        internal static XmlQualifiedName AddLocalDateTimeSchemaType(XmlSchemaSet schemaSet) => AddSchemaType(schemaSet, LocalDateTimeSchemaType);
        internal static XmlQualifiedName AddLocalTimeSchemaType(XmlSchemaSet schemaSet) => AddSchemaType(schemaSet, LocalTimeSchemaType);
        internal static XmlQualifiedName AddOffsetSchemaType(XmlSchemaSet schemaSet) => AddSchemaType(schemaSet, OffsetSchemaType);
        internal static XmlQualifiedName AddOffsetDateSchemaType(XmlSchemaSet schemaSet) => AddSchemaType(schemaSet, OffsetDateSchemaType);
        internal static XmlQualifiedName AddOffsetDateTimeSchemaType(XmlSchemaSet schemaSet) => AddSchemaType(schemaSet, OffsetDateTimeSchemaType);
        internal static XmlQualifiedName AddOffsetTimeSchemaType(XmlSchemaSet schemaSet) => AddSchemaType(schemaSet, OffsetTimeSchemaType);
        internal static XmlQualifiedName AddPeriodBuilderSchemaType(XmlSchemaSet schemaSet) => AddSchemaType(schemaSet, PeriodBuilderSchemaType);
        internal static XmlQualifiedName AddYearMonthSchemaType(XmlSchemaSet schemaSet) => AddSchemaType(schemaSet, YearMonthSchemaType);
        internal static XmlQualifiedName AddZonedDateTimeSchemaType(XmlSchemaSet schemaSet) => AddSchemaType(schemaSet, ZonedDateTimeSchemaType);

        private const string YearPattern = @"-?[0-9]{4}";
        private const string MonthPattern = @"[0-9]{2}";
        private const string DayPattern = @"[0-9]{2}";
        private const string TimePattern = @"[0-9]{2}:[0-9]{2}:[0-9]{2}(\.[0-9]{1,9})?";
        private const string OffsetPattern = @"(Z|[+-][0-9]{2}(:[0-9]{2}(:[0-9]{2})?)?)";
        private const string PeriodBuilderPattern = @"P(-?[0-9]+Y)?(-?[0-9]+M)?(-?[0-9]+W)?(-?[0-9]+D)?(T(-?[0-9]+H)?(-?[0-9]+M)?(-?[0-9]+S)?(-?[0-9]+s)?(-?[0-9]+t)?(-?[0-9]+n)?)?";
        private const string DurationPattern = @"-?[0-9]{1,8}:[0-9]{2}:[0-9]{2}:[0-9]{2}(\.[0-9]{1,9})?";

        static XmlSchemaDefinition()
        {
            var xsStringType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String);

            var calendarRestriction = CreateEnumerationRestriction("calendar", xsStringType, CalendarSystem.Ids);
            var localDateRestriction = CreatePatternRestriction("localDate", xsStringType, $"{YearPattern}-{MonthPattern}-{DayPattern}");
            var localDateTimeRestriction = CreatePatternRestriction("localDateTime", xsStringType,  $"{YearPattern}-{MonthPattern}-{DayPattern}T{TimePattern}");
            var zoneIds = CreateEnumerationRestriction("zoneIds", xsStringType, XmlSerializationSettings.DateTimeZoneProvider.GetAllZones().Select(e => e.Id));
            // The "zoneIds" purpose is to document the known zone identifiers. The "zone" restriction is a union between known zone ids and
            // xs:string so that validation won't fail when a new zone identifier is added to the Time Zone Database.
            var zoneRestriction = QualifySchemaType(new XmlSchemaSimpleType
            {
                Name = "zone",
                Content = new XmlSchemaSimpleTypeUnion { MemberTypes = new[] { zoneIds.QualifiedName, xsStringType.QualifiedName } }
            });

            var calendarAttribute = new XmlSchemaAttribute { Name = "calendar", SchemaTypeName = calendarRestriction.QualifiedName };
            var zoneAttribute = new XmlSchemaAttribute { Name = "zone", SchemaTypeName = zoneRestriction.QualifiedName, Use = XmlSchemaUse.Required };

            AnnualDateSchemaType = CreatePatternRestriction<AnnualDate>(xsStringType, $"{MonthPattern}-{DayPattern}");
            DurationSchemaType = CreatePatternRestriction<Duration>(xsStringType, DurationPattern);
            InstantSchemaType = CreatePatternRestriction<Instant>(xsStringType, $"{YearPattern}-{MonthPattern}-{DayPattern}T{TimePattern}Z");
            IntervalSchemaType = new XmlSchemaComplexType
            {
                Name = nameof(Interval),
                Attributes = {
                    new XmlSchemaAttribute { Name = "start", SchemaTypeName = InstantSchemaType.QualifiedName },
                    new XmlSchemaAttribute { Name = "end", SchemaTypeName = InstantSchemaType.QualifiedName }
                }
            };
            LocalDateSchemaType = CreateSchemaTypeWithAttributes<LocalDate>(localDateRestriction, calendarAttribute);
            LocalDateTimeSchemaType = CreateSchemaTypeWithAttributes<LocalDateTime>(localDateTimeRestriction, calendarAttribute);
            LocalTimeSchemaType = CreatePatternRestriction<LocalTime>(xsStringType, TimePattern);
            OffsetSchemaType = CreatePatternRestriction<Offset>(xsStringType, OffsetPattern);
            OffsetDateSchemaType = CreatePatternRestriction<OffsetDate>(xsStringType, $"{YearPattern}-{MonthPattern}-{DayPattern}{OffsetPattern}");
            OffsetDateTimeSchemaType = CreatePatternRestriction<OffsetDateTime>(xsStringType, $"{YearPattern}-{MonthPattern}-{DayPattern}T{TimePattern}{OffsetPattern}");
            OffsetTimeSchemaType = CreatePatternRestriction<OffsetTime>(xsStringType, $"{TimePattern}{OffsetPattern}");
            PeriodBuilderSchemaType = CreatePatternRestriction<PeriodBuilder>(xsStringType, PeriodBuilderPattern);
            YearMonthSchemaType = CreatePatternRestriction<YearMonth>(xsStringType, $"{YearPattern}-{MonthPattern}");
            ZonedDateTimeSchemaType = CreateSchemaTypeWithAttributes<ZonedDateTime>(OffsetDateTimeSchemaType, zoneAttribute, calendarAttribute);

            DependentSchemaTypes = new Dictionary<XmlSchemaType, IEnumerable<XmlSchemaType>>
            {
                [IntervalSchemaType] = new[] { InstantSchemaType },
                [LocalDateSchemaType] = new[] { localDateRestriction, calendarRestriction },
                [LocalDateTimeSchemaType] = new[] { localDateTimeRestriction, calendarRestriction },
                [ZonedDateTimeSchemaType] = new[] { OffsetDateTimeSchemaType, calendarRestriction, zoneRestriction, zoneIds },
            };

            NodaTimeXmlSchema = CreateNodaTimeXmlSchema();
        }

        private static readonly XmlSchemaType AnnualDateSchemaType;
        private static readonly XmlSchemaType DurationSchemaType;
        private static readonly XmlSchemaType InstantSchemaType;
        private static readonly XmlSchemaType IntervalSchemaType;
        private static readonly XmlSchemaType LocalDateSchemaType;
        private static readonly XmlSchemaType LocalDateTimeSchemaType;
        private static readonly XmlSchemaType LocalTimeSchemaType;
        private static readonly XmlSchemaType OffsetSchemaType;
        private static readonly XmlSchemaType OffsetDateSchemaType;
        private static readonly XmlSchemaType OffsetDateTimeSchemaType;
        private static readonly XmlSchemaType OffsetTimeSchemaType;
        private static readonly XmlSchemaType PeriodBuilderSchemaType;
        private static readonly XmlSchemaType YearMonthSchemaType;
        private static readonly XmlSchemaType ZonedDateTimeSchemaType;
        private static readonly Dictionary<XmlSchemaType, IEnumerable<XmlSchemaType>> DependentSchemaTypes;

        private static XmlQualifiedName AddSchemaType(this XmlSchemaSet schemaSet, XmlSchemaType schemaType)
        {
            var schema = new XmlSchema
            {
                TargetNamespace = NodaTimeXmlNamespace.Namespace,
                Items = { schemaType }
            };
            if (DependentSchemaTypes.TryGetValue(schemaType, out var dependentSchemaTypes))
            {
                foreach (var dependentSchemaType in dependentSchemaTypes)
                {
                    schema.Items.Add(dependentSchemaType);
                }
            }
            schemaSet.Add(schema);
            return schemaType.QualifiedName;
        }

        private static XmlSchema CreateNodaTimeXmlSchema()
        {
            var schemaSetForCollecting = new XmlSchemaSet();
            var addSchemaMethods = new Func<XmlSchemaSet, XmlQualifiedName>[]
            {
                AnnualDate.AddSchema, Duration.AddSchema, Instant.AddSchema, Interval.AddSchema, LocalDate.AddSchema, LocalDateTime.AddSchema, LocalTime.AddSchema,
                Offset.AddSchema, OffsetDate.AddSchema, OffsetDateTime.AddSchema, OffsetTime.AddSchema, PeriodBuilder.AddSchema, YearMonth.AddSchema, ZonedDateTime.AddSchema
            };
            foreach (var addSchemaMethod in addSchemaMethods)
            {
                addSchemaMethod(schemaSetForCollecting);
            }
            var xmlSchema = new XmlSchema
            {
                TargetNamespace = NodaTimeXmlNamespace.Namespace,
                Namespaces = new XmlSerializerNamespaces(new[] { NodaTimeXmlNamespace })
            };
            var schemaTypes = schemaSetForCollecting.Schemas().Cast<XmlSchema>().SelectMany(e => e.Items.OfType<XmlSchemaType>());
            foreach (var schemaType in schemaTypes.OrderBy(e => e.Name, StringComparer.Ordinal))
            {
                var schemaContainsType = xmlSchema.Items.OfType<XmlSchemaType>().Select(e => e.QualifiedName).Contains(schemaType.QualifiedName);
                if (!schemaContainsType)
                {
                    xmlSchema.Items.Add(schemaType);
                }
            }
            var schemaSetForCompiling = new XmlSchemaSet();
            schemaSetForCompiling.Add(xmlSchema);
            schemaSetForCompiling.Compile();
            return xmlSchema;
        }

        // See https://stackoverflow.com/questions/626319/add-attributes-to-a-simpletype-or-restriction-to-a-complextype-in-xml-schema/626385#626385
        private static XmlSchemaComplexType CreateSchemaTypeWithAttributes<T>(XmlSchemaType baseType, params XmlSchemaAttribute[] attributes) where T : IXmlSerializable
        {
            var content = new XmlSchemaSimpleContentExtension { BaseTypeName = baseType.QualifiedName };
            foreach (var attribute in attributes)
            {
                content.Attributes.Add(attribute);
            }
            return new XmlSchemaComplexType
            {
                Name = typeof(T).Name,
                ContentModel = new XmlSchemaSimpleContent { Content = content }
            };
        }

        private static XmlSchemaSimpleType CreatePatternRestriction<T>(XmlSchemaType baseType, string pattern) where T : IXmlSerializable =>
            CreatePatternRestriction(typeof(T).Name, baseType, pattern);

        private static XmlSchemaSimpleType CreatePatternRestriction(string name, XmlSchemaType baseType, string pattern) =>
            QualifySchemaType(new XmlSchemaSimpleType
            {
                Name = name,
                Content = new XmlSchemaSimpleTypeRestriction
                {
                    BaseTypeName = baseType.QualifiedName,
                    Facets = { new XmlSchemaPatternFacet { Value = pattern } }
                }
            });

        private static XmlSchemaType CreateEnumerationRestriction(string name, XmlSchemaType baseType, IEnumerable<string> values)
        {
            var restriction = new XmlSchemaSimpleTypeRestriction { BaseTypeName = baseType.QualifiedName };
            foreach (var value in values.OrderBy(e => e, StringComparer.OrdinalIgnoreCase))
            {
                restriction.Facets.Add(new XmlSchemaEnumerationFacet { Value = value });
            }
            return QualifySchemaType(new XmlSchemaSimpleType { Name = name, Content = restriction });
        }

        /// <summary>
        /// Sets the <see cref="XmlSchemaType.QualifiedName"/> of the given <paramref name="schemaType"/>
        /// to belong to <see cref="NodaTimeXmlNamespace"/>.
        /// </summary>
        /// <remarks>
        /// Going through an <see cref="XmlSchema"/> and an <see cref="XmlSchemaSet"/> is a bit convoluted but
        /// <c>XmlSchemaType.SetQualifiedName()</c> is internal.
        /// </remarks>
        /// <param name="schemaType">The schema type to qualify.</param>
        /// <typeparam name="T">The type of the <see cref="XmlSchemaType"/></typeparam>
        /// <returns>The qualified <paramref name="schemaType"/>.</returns>
        private static T QualifySchemaType<T>(T schemaType) where T : XmlSchemaType
        {
            var schema = new XmlSchema
            {
                TargetNamespace = NodaTimeXmlNamespace.Namespace,
                Items = { schemaType }
            };
            var xmlSchemaSet = new XmlSchemaSet();
            xmlSchemaSet.Add(schema);
            return schemaType;
        }
    }
}
