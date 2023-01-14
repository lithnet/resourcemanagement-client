// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Lithnet.ResourceManagement.Client
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class XmlFormatExtensionPointAttribute : Attribute
    {
        private string _name;

        public XmlFormatExtensionPointAttribute(string memberName)
        {
            this._name = memberName;
        }

        public string MemberName
        {
            get
            {
                return this._name == null ? string.Empty : this._name;
            }
            set
            {
                this._name = value;
            }
        }

        public bool AllowElements { get; set; } = true;
    }
}