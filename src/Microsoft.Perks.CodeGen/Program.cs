// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Perks.CodeGen
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(@"Microsoft Copyright 2015
Usage: rzc.exe <semicolon-separated file list> [<target directory> [<namespace>]]");
            }
            else
            {
                var compiler = new Compiler();
                var files = args[0].Split(';');
                if (args.Length >= 2)
                {
                    compiler.TargetDirectory = args[1];
                }
                if (args.Length >= 3)
                {
                    compiler.Namespace = args[2];
                }
                foreach (var file in files)
                {
                    compiler.Compile(file);
                }
            }
        }
    }
}
