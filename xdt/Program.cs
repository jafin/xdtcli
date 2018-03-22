using System.Diagnostics;

namespace XDT
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using Microsoft.Web.XmlTransform;

    /// <summary>
    /// Code ripped from http://www.tomdupont.net/2015/09/xdt-console-application.html
    /// </summary>
    public class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Required Arguments: [ConfigPath] [TransformPath] [TargetPath]");
                return 400;
            }

            WriteVersion();


            var configPath = args[0];
            if (!File.Exists(configPath))

            {
                Console.WriteLine($"Config not found {configPath}");
                return 404;
            }

            var transformPath = args[1];
            if (!File.Exists(transformPath))
            {
                Console.WriteLine($"Transform not found {transformPath}");
                return 404;
            }

            try
            {
                var targetPath = args[2];
                var configXml = File.ReadAllText(configPath);
                var transformXml = File.ReadAllText(transformPath);

                Console.WriteLine($"Source File: {configPath}");
                Console.WriteLine($"Transform: {transformPath}");
                Console.WriteLine($"Target: {targetPath}");

                using (var document = new XmlTransformableDocument())

                {
                    document.PreserveWhitespace = true;
                    document.LoadXml(configXml);
                    using (var transform = new XmlTransformation(transformXml, false, null))
                    {
                        if (transform.Apply(document))
                        {
                            var stringBuilder = new StringBuilder();
                            var xmlWriterSettings = new XmlWriterSettings
                            {
                                Indent = true,
                                IndentChars = "  "
                            };

                            using (var xmlTextWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings))
                            {
                                document.WriteTo(xmlTextWriter);
                            }

                            var resultXml = stringBuilder.ToString();
                            File.WriteAllText(targetPath, resultXml);
                            return 0;
                        }

                        Console.WriteLine("Transformation failed for unknown reason");
                    }
                }
            }
            catch (XmlTransformationException xmlTransformationException)
            {
                Console.WriteLine(xmlTransformationException.Message);
            }
            catch (XmlException xmlException)

            {
                Console.WriteLine(xmlException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return 500;
        }

        private static void WriteVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            Console.WriteLine($"Version {version}");
        }
    }
}