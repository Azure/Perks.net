// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.CodeGenerators;

namespace Microsoft.Perks.CodeGen
{
    public class Compiler
    {
        public Compiler()
        {
            Namespace = "Microsoft.Rest.Generators";
            CopyrightHeader = @"// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.";
            TargetDirectory = "";
        }

        /// <summary>
        /// Gets or sets copyright header for the razor-generated file.
        /// </summary>
        public string CopyrightHeader { get; set; }

        public string TargetDirectory { get; set; }

        /// <summary>
        /// Specifies the base namespace for the razor-generated compilable outputs. 
        /// Defaults to Microsoft.Rest.Generators.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Generates code for a razor template in the specified namespace.
        /// </summary>
        /// <param name="cshtmlFilePath">Full path to razor template.</param>
        public void Compile(string cshtmlFilePath)
        {
            var basePath = Path.GetDirectoryName(cshtmlFilePath);

            if (Path.IsPathRooted(basePath))
            {
                throw new ArgumentException(".cshtml path must be relative");
            }
            if (basePath.Contains("."))
            {
                throw new ArgumentException(".cshtml path must not contain '.'");
            }

            var fileName = Path.GetFileName(cshtmlFilePath);
            var fileNameNoExtension = Path.GetFileNameWithoutExtension(fileName);
            var targetFileName = Path.Combine(TargetDirectory, $"{cshtmlFilePath}.cs");
            System.Console.WriteLine(targetFileName);
            var fullNamespace = $"{Namespace}.{basePath.Replace('/', '.').Replace('\\', '.')}";

            if (File.Exists(targetFileName) && File.GetLastWriteTimeUtc(targetFileName) >= File.GetLastWriteTimeUtc(cshtmlFilePath))
            {
                return;
            }

            var codeLang = new CSharpRazorCodeLanguage();
            var host = new RazorEngineHost(codeLang);
            host.GeneratedClassContext = new GeneratedClassContext(
                executeMethodName: GeneratedClassContext.DefaultExecuteMethodName,
                writeMethodName: GeneratedClassContext.DefaultWriteMethodName,
                writeLiteralMethodName: GeneratedClassContext.DefaultWriteLiteralMethodName,
                writeToMethodName: "WriteTo",
                writeLiteralToMethodName: "WriteLiteralTo",
                templateTypeName: "HelperResult",
                defineSectionMethodName: "DefineSection",
                generatedTagHelperContext: new GeneratedTagHelperContext());
            var engine = new RazorTemplateEngine(host);

            var file = File.ReadAllText(cshtmlFilePath);
            file = file.Replace("<exception", "«exception");
            using (var fileStream = new StringReader(file))
            {
                var code = engine.GenerateCode(
                    input: fileStream,
                    className: fileNameNoExtension,
                    rootNamespace: fullNamespace,
                    sourceFileName: fileName);

                var source = code.GeneratedCode;
                source = source.Replace("«exception", "<exception");
                source = CopyrightHeader + "\r\n\r\n" + source;
                var startIndex = 0;
                while (startIndex < source.Length)
                {
                    var startMatch = @"<%$ include: ";
                    var endMatch = @" %>";
                    startIndex = source.IndexOf(startMatch, startIndex);
                    if (startIndex == -1)
                    {
                        break;
                    }
                    var endIndex = source.IndexOf(endMatch, startIndex);
                    if (endIndex == -1)
                    {
                        break;
                    }
                    var includeFileName = source.Substring(startIndex + startMatch.Length,
                        endIndex - (startIndex + startMatch.Length));
                    Console.WriteLine("    Inlining file {0}", includeFileName);
                    var replacement =
                        File.ReadAllText(Path.Combine(basePath, includeFileName))
                            .Replace("\"", "\\\"")
                            .Replace("\r\n", "\\r\\n");
                    source = source.Substring(0, startIndex) + replacement +
                             source.Substring(endIndex + endMatch.Length);
                    startIndex = startIndex + replacement.Length;
                }
                if( File.Exists(targetFileName) ) { 
                    var oldFile = File.ReadAllText(targetFileName);    
                    if( oldFile == source ) {
                        return;
                    }
                }
                Directory.CreateDirectory(Path.GetDirectoryName(targetFileName));
                File.WriteAllText(targetFileName, source);
            }
        }
    }
}