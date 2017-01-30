// Copyright (c) 2016 AnyGet Authors
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace System.Management.Automation {
    using System;

    public class CmdletAttribute : Attribute
    {
        public CmdletAttribute(string verb, string noun)
        {

        }
    }

    public class OutputTypeAttribute : Attribute
    {
        public OutputTypeAttribute(Type[] types)
        {
        }
    }

    public class ParameterAttribute : Attribute {
        public ParameterAttribute()
        {
        }

        public int Position {get; set;}
        public bool ValueFromPipelineByPropertyName {get; set;}
    }

    public class ValidateNotNullAttribute : Attribute {
        public ValidateNotNullAttribute() {
            
        }
    }

    public class SwitchParameter {
        
    }

    public class PSCredential {
        
    }

    public static class VerbsCommon {
        public const string Find = nameof(Find);
        public const string Install = nameof(Install);
        public const string Uninstall = nameof(Uninstall);
        public const string Remove = nameof(Remove);
        public const string Add = nameof(Add);
        public const string Get = nameof(Get);
    }

    public class Cmdlet {
        
    }
}