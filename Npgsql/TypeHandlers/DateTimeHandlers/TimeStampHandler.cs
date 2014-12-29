﻿using System;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    internal class TimeStampHandler : TypeHandlerWithPsv<DateTime, NpgsqlTimeStamp>, ITypeHandler<NpgsqlTimeStamp>
    {
        static readonly string[] _pgNames = { "timestamp" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        public override DateTime Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeStamp?
            return (DateTime)((ITypeHandler<NpgsqlTimeStamp>)this).Read(buf, fieldDescription, len);
        }

        NpgsqlTimeStamp ITypeHandler<NpgsqlTimeStamp>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return NpgsqlTimeStamp.Parse(buf.ReadString(len));
                case FormatCode.Binary:
                    return NpgsqlTimeStamp.FromInt64(buf.ReadInt64());
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }
    }
}
