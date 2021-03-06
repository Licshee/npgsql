﻿#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using NpgsqlTypes;
using System;
using System.Text;
using Npgsql.BackendMessages;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Handles "conversions" for columns sent by the database with unknown OIDs.
    /// This differs from TextHandler in that its a text-only handler (we don't want to receive binary
    /// representations of the types registered here).
    /// Note that this handler is also used in the very initial query that loads the OID mappings
    /// (chicken and egg problem).
    /// Also used for sending parameters with unknown types (OID=0)
    /// </summary>
    class UnrecognizedTypeHandler : TextHandler
    {
        static readonly IBackendType UnrecognizedBackendType = new UnrecognizedBackendType();

        internal UnrecognizedTypeHandler(TypeHandlerRegistry registry) : base(UnrecognizedBackendType, registry) {}

        public override void PrepareRead(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            if (fieldDescription == null)
                throw new Exception($"Received an unknown field but {nameof(fieldDescription)} is null (i.e. COPY mode)");

            if (fieldDescription.IsBinaryFormat) {
                buf.Skip(len);
                throw new SafeReadException(new NotSupportedException($"The field '{fieldDescription.Name}' has a type currently unknown to Npgsql (OID {fieldDescription.TypeOID}). You can retrieve it as a string by marking it as unknown, please see the FAQ."));
            }
            base.PrepareRead(buf, len, fieldDescription);
        }
    }

    class UnrecognizedBackendType : IBackendType
    {
        public string Namespace => "";
        public string Name => "<unknown>";
        public uint OID => 0;
        public NpgsqlDbType? NpgsqlDbType => null;
        public string FullName => "<unknown>";
        public string DisplayName => "<unknown>";
    }
}
