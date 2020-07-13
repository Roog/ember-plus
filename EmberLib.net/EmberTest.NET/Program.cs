/*
   EmberLib.net -- .NET implementation of the Ember+ Protocol

   Copyright (C) 2012-2019 Lawo GmbH (http://www.lawo.com).
   Distributed under the Boost Software License, Version 1.0.
   (See accompanying file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
*/

using System;
using System.Collections.Generic;
using System.Text;
using EmberLib;
using System.IO;
using System.Xml;
using System.Diagnostics;
using BerLib;
using EmberLib.Legacy;
using EmberLib.Xml;
using EmberLib.Glow;
using EmberLib.Framing;
using EmberLib.Glow.Framing;
using EmberLib.Glow.Formula;
using EmberLib.Legacy.Extensions;
using System.Globalization;
using EmberLib.Glow.PowerPack.Xml;

namespace EmberTest.NET
{
   class Program : EmberApplicationInterface
   {
      const BerClass DefaultClass = BerClass.ContextSpecific;

      #region Entry Point
      static void Main(string[] args)
      {
         var program = new Program();

         program.Run();
      }

      void Run()
      {
         Test_Functions();
         //Test_Real();
         //Test_Real2();
         //Test_GlowFormula();
         //Test_DOM();
         //Test_InteropDom();
         //Test_ReaderWriter();
         //Test_XmlImport();

         Console.Write("Press Enter to quit...");
         Console.ReadLine();
      }
      #endregion

      #region Functions
      void Test_Functions()
      {
         Action<EmberSequence, string> fillTupleDescription =
            (sequence, namePrefix) =>
            {
               sequence.Insert(new GlowTupleItemDescription(GlowParameterType.Integer) { Name = namePrefix + "1:integer" });
               sequence.Insert(new GlowTupleItemDescription(GlowParameterType.Boolean) { Name = namePrefix + "2:boolean" });
               sequence.Insert(new GlowTupleItemDescription(GlowParameterType.Octets) { Name = namePrefix + "3:octets" });
               sequence.Insert(new GlowTupleItemDescription(GlowParameterType.Real) { Name = namePrefix + "4:real" });
               sequence.Insert(new GlowTupleItemDescription(GlowParameterType.String) { Name = namePrefix + "5:string" });
            };

         // --------------- Invocation Command
         var glowCommand = new GlowCommand(GlowCommandType.Invoke)
         {
            Invocation = new GlowInvocation(GlowTags.Command.Invocation)
            {
               InvocationId = 123,
            },
         };

         var argsTuple = glowCommand.Invocation.EnsureArguments();
         argsTuple.Insert(new IntegerEmberLeaf(GlowTags.CollectionItem, 456));
         argsTuple.Insert(new BooleanEmberLeaf(GlowTags.CollectionItem, true));
         argsTuple.Insert(new OctetStringEmberLeaf(GlowTags.CollectionItem, new byte[] { 250, 251, 253 }));
         argsTuple.Insert(new RealEmberLeaf(GlowTags.CollectionItem, 123.321));
         argsTuple.Insert(new StringEmberLeaf(GlowTags.CollectionItem, "hallo"));

         AssertCodecSanity(glowCommand);
         Console.WriteLine(GetGlowXml(glowCommand));

         // --------------- Function
         var glowFunction = new GlowFunction(100)
         {
            Identifier = "testFunction",
            Description = "Test Function",
         };

         fillTupleDescription(glowFunction.EnsureArguments(), "arg");
         fillTupleDescription(glowFunction.EnsureResult(), "res");
         glowFunction.EnsureChildren().Insert(glowCommand);

         AssertCodecSanity(glowFunction);
         Console.WriteLine(GetXml(glowFunction));

         // --------------- QualifiedFunction
         var glowQualifiedFunction = new GlowQualifiedFunction(new[] { 1, 2, 3 })
         {
            Identifier = "testFunction",
            Description = "Test Function",
         };

         fillTupleDescription(glowQualifiedFunction.EnsureArguments(), "arg");
         fillTupleDescription(glowQualifiedFunction.EnsureResult(), "res");
         glowQualifiedFunction.EnsureChildren().Insert(glowCommand);

         AssertCodecSanity(glowQualifiedFunction);
         Console.WriteLine(GetXml(glowQualifiedFunction));

         var glowRoot = GlowRootElementCollection.CreateRoot();
         glowRoot.Insert(glowQualifiedFunction);
         AssertGlowXmlSanity(glowRoot);

         // --------------- InvocationResult
         var glowInvocationResult = GlowInvocationResult.CreateRoot(glowCommand.Invocation.InvocationId.Value);
         var resTuple = glowInvocationResult.EnsureResult();
         resTuple.Insert(new IntegerEmberLeaf(GlowTags.CollectionItem, 456));
         resTuple.Insert(new BooleanEmberLeaf(GlowTags.CollectionItem, true));
         resTuple.Insert(new OctetStringEmberLeaf(GlowTags.CollectionItem, new byte[] { 250, 251, 253 }));
         resTuple.Insert(new RealEmberLeaf(GlowTags.CollectionItem, 123.321));
         resTuple.Insert(new StringEmberLeaf(GlowTags.CollectionItem, "hallo"));

         AssertCodecSanity(glowInvocationResult);
         Console.WriteLine(GetXml(glowInvocationResult));

         AssertGlowXmlSanity(glowInvocationResult);
      }

      void AssertCodecSanity(EmberNode ember)
      {
         var originalXml = GetXml(ember);
         var output = new BerMemoryOutput();

         ember.Encode(output);

         var input = new BerMemoryInput(output.Memory);
         var reader = new EmberReader(input);

         var decoded = EmberNode.Decode(reader, new GlowApplicationInterface());
         var decodedXml = GetXml(decoded);

         if(originalXml != decodedXml)
            throw new Exception("Codec error!");
      }

      void AssertGlowXmlSanity(GlowContainer glow)
      {
         var originalXml = GetXml(glow);
         var glowXml = GetGlowXml(glow);

         GlowContainer decoded;

         using(var reader = new StringReader(glowXml))
         using(var xmlReader = XmlReader.Create(reader))
            decoded = GlowXmlImport.Import(xmlReader);

         var decodedXml = GetXml(decoded);

         if(originalXml != decodedXml)
            throw new Exception("Codec error!");
      }
      #endregion

      #region Real Debug
      void Test_Real()
      {
         var values = new[] { 32.1, 32.125, 32.123, 100, 200, 300, -1000, 5.5005005, 777777777.123456789 };

         foreach(var value in values)
         {
            var output = new BerMemoryOutput();
            BerEncoding.EncodeReal(output, value);

            var input = new BerMemoryInput(output.Memory);
            var decodedValue = BerEncoding.DecodeReal(input, output.Length);

            Console.WriteLine("value={0} decoded={1}", value, decodedValue);
         }
      }

      void Test_Real2()
      {
         var encoded = new byte[] { 0xC0, 0x04, 0xDF };
         var input = new BerMemoryInput(encoded);
         var decoded = BerEncoding.DecodeReal(input, encoded.Length);
         Console.WriteLine("decoded={0}", decoded);

         var output = new BerMemoryOutput();
         var reencoded = BerEncoding.EncodeReal(output, decoded);

         var bytes = output.ToArray();
         Console.WriteLine("reencoded={0}", BytesToString(bytes));
      }
      #endregion

      #region Formula
      void Test_FormulaPerf(string source, int iterations)
      {
         var result = Compiler.Compile(source, false);

         if(result.Success)
         {
            Stopwatch stopwatch;
            var integerValue = null as GlowValue;
            var doubleValue = null as GlowValue;

            Console.WriteLine("--- Interpreter compilation");
            stopwatch = Stopwatch.StartNew();
            for(int i = 0; i < 10000; i++)
               Compiler.Compile(source, false);
            stopwatch.Stop();
            Console.WriteLine("perf {0}ms", stopwatch.ElapsedMilliseconds);

            Console.WriteLine("--- Emitter compilation");
            stopwatch = Stopwatch.StartNew();
            for(int i = 0; i < 10000; i++)
               Compiler.Compile(source, true);
            stopwatch.Stop();
            Console.WriteLine("perf {0}ms", stopwatch.ElapsedMilliseconds);

            Console.WriteLine("--- Interpreter");
            stopwatch = Stopwatch.StartNew();
            for(int i = 0; i < iterations; i++)
            {
               doubleValue = result.Formula.Eval(100.0);
               integerValue = result.Formula.Eval(100);
            }
            stopwatch.Stop();

            Console.WriteLine("INT> {0}", integerValue.ToString(CultureInfo.InvariantCulture));
            Console.WriteLine("FLT> {0}", doubleValue.ToString(CultureInfo.InvariantCulture));
            Console.WriteLine("perf {0}ms", stopwatch.ElapsedMilliseconds);


            Console.WriteLine("--- Emitter");
            result = Compiler.Compile(source, true);
            stopwatch = Stopwatch.StartNew();
            for(int i = 0; i < iterations; i++)
            {
               doubleValue = result.Formula.Eval(100.0);
               integerValue = result.Formula.Eval(100);
            }
            stopwatch.Stop();

            Console.WriteLine("INT> {0}", integerValue.ToString(CultureInfo.InvariantCulture));
            Console.WriteLine("FLT> {0}", doubleValue.ToString(CultureInfo.InvariantCulture));
            Console.WriteLine("perf {0}ms", stopwatch.ElapsedMilliseconds);
         }
         else
         {
            Console.WriteLine("! {0}", result.Error.Message);
         }
      }

      void Test_GlowFormula()
      {
         const int iterations = 500000;
         string source;

         Console.Write(": ");
         while(String.IsNullOrEmpty(source = Console.ReadLine()) == false)
         {
            Test_FormulaPerf(source, iterations);

            Console.Write(": ");
         }
      }
      #endregion

      #region Xml Import
      void Test_XmlImport()
      {
         var frame = new EmberSequence(new BerTag(BerClass.Application, 1));
         var appDefined1 = EmberApplicationInterface.CreateApplicationDefinedSet(new BerTag(BerClass.ContextSpecific, 444), 1, frame);
         var appDefined2 = EmberApplicationInterface.CreateApplicationDefinedSequence(new BerTag(BerClass.ContextSpecific, 445), 2, appDefined1);
         appDefined2.Insert(new BerTag(BerClass.ContextSpecific, 1), -1);
         appDefined2.Insert(new BerTag(BerClass.ContextSpecific, 2), true);
         appDefined2.Insert(new BerTag(BerClass.ContextSpecific, 3), false);
         appDefined2.Insert(new BerTag(BerClass.ContextSpecific, 4), 12345.6789);
         appDefined2.Insert(new BerTag(BerClass.ContextSpecific, 5), "wasgeht�b?");

         Console.WriteLine("\r\n------------------------ XML Import");
         var xml1 = GetXml(frame);

         using(var stream = new StringReader(xml1))
         using(var reader = new XmlTextReader(stream))
         {
            var root = XmlImport.Import(reader, this);
            var xml2 = GetXml(root);

            Console.WriteLine(xml2);

            Debug.Assert(xml1 == xml2);
         }
      }
      #endregion

      #region DOM
      void Test_InteropDom()
      {
         Console.WriteLine("\r\n------------------------ Interop DOM");

         var testFilePath = @"N:\Temp\test.ber";
         using(var stream = File.OpenRead(testFilePath))
         {
            var input = new BerStreamInput(stream);
            var reader = new EmberReader(input);

            var root = EmberNode.Decode(reader, this);
            Console.WriteLine(GetXml(root));
         }
      }

      void Test_DOM()
      {
         Console.WriteLine("\r\n------------------------ DOM");

         EmberContainer container1;
         EmberContainer frame = new EmberFrame();

         container1 = new EmberSet(new BerTag(DefaultClass, 0));
         container1.Insert(new BerTag(DefaultClass, 0), -1);
         container1.Insert(new BerTag(DefaultClass, 1), 128);
         container1.Insert(new BerTag(DefaultClass, 2), -128);
         container1.Insert(new BerTag(DefaultClass, 3), 255);
         container1.Insert(new BerTag(DefaultClass, 4), -255);
         container1.Insert(new BerTag(DefaultClass, 5), 12345);
         container1.Insert(new BerTag(DefaultClass, 6), -12345);
         container1.Insert(new BerTag(DefaultClass, 7), 16384);
         container1.Insert(new BerTag(DefaultClass, 8), -16384);
         container1.Insert(new BerTag(DefaultClass, 9), 65535);
         container1.Insert(new BerTag(DefaultClass, 10), -65535);
         container1.Insert(new BerTag(DefaultClass, 11), 0);
         container1.Insert(new BerTag(DefaultClass, 12), 127);
         container1.Insert(new BerTag(DefaultClass, 13), -127);
         container1.Insert(new BerTag(DefaultClass, 1111222), 0xFFFFFFFF);
         container1.InsertOid(new BerTag(DefaultClass, 14), new int[] { 1, 3, 6, 0 });
         container1.InsertOid(new BerTag(DefaultClass, 15), new int[] { 1 });
         container1.InsertRelativeOid(new BerTag(DefaultClass, 16), new int[] { 1, 2, 3, 4, 5, 6 });
         frame.Insert(container1);

         container1 = new EmberSequence(new BerTag(DefaultClass, 1));
         container1.Insert(new BerTag(DefaultClass, 3), -0.54321);
         container1.Insert(new BerTag(DefaultClass, 5), "Wuppdich");

         var appDefined = EmberApplicationInterface.CreateApplicationDefinedSequence(new BerTag(BerClass.Application, 889), 2, container1);
         appDefined.Insert(new BerTag(DefaultClass, 100), true);

         frame.Insert(container1);

         var xml1 = GetXml(frame);

         var output = new BerMemoryOutput();
         frame.Encode(output);

         var memory = output.ToArray();
         using(var stream = new FileStream(@"N:\Temp\test.ber", FileMode.Create, FileAccess.Write))
            stream.Write(memory, 0, memory.Length);

         var input = new BerMemoryInput(memory);

         var stopwatch = new Stopwatch();
         stopwatch.Start();

         var asyncReader = new AsyncFrameReader(this);
         asyncReader.ReadBytes(output.Memory);
         var loadedFrame = asyncReader.DetachRoot();

         stopwatch.Stop();
         Console.WriteLine("load tree: {0}ms", stopwatch.ElapsedMilliseconds);

         var xml2 = GetXml(loadedFrame);

         Console.WriteLine(xml1);
         Console.WriteLine(xml2);

         Debug.Assert(xml1 == xml2);
      }
      #endregion

      #region Reader Writer
      void Test_ReaderWriter()
      {
         var output = new BerMemoryOutput();
         var writer = new EmberWriter(output);

         writer.WriteSequenceBegin(new BerTag(DefaultClass, 1));

         for(uint index = 0; index <= 20; index++)
            writer.Write(new BerTag(DefaultClass, index + 111122), index);

         var oid = new int[100];
         for(int index = 0; index < oid.Length; index++)
            oid[index] = 1000 + index;

         writer.WriteRelativeOid(new BerTag(DefaultClass, 500000), oid);

         writer.WriteContainerEnd();

         Console.WriteLine("\r\n------------------------ Reader, Writer");

         var asyncReader = new AsyncDomReader(null);
         asyncReader.ReadBytes(output.Memory);

         var root = asyncReader.DetachRoot();
         Console.WriteLine(GetXml(root));
      }
      #endregion

      #region EmberApplicationInterface Members
      public override EmberNode CreateNodeFromReader(uint type, BerReaderBase reader)
      {
         switch(type & ~BerType.ApplicationFlag)
         {
            case 1:
               return CreateSet(type, reader);

            case 2:
               return CreateSequence(type, reader);
         }

         return null;
      }

      public override EmberNode CreateNodeFromXml(uint type, BerTag tag, XmlReader reader)
      {
         switch(type & ~BerType.ApplicationFlag)
         {
            case 1:
               return CreateSet(type, tag, reader);

            case 2:
               return CreateSequence(type, tag, reader);
         }

         return null;
      }
      #endregion

      #region Implementation
      string BytesToString(byte[] bytes)
      {
         var buffer = new StringBuilder();

         foreach(var b in bytes)
            buffer.AppendFormat("{0:X2} ", b);

         return buffer.ToString().TrimEnd();
      }

      string GetXmlBody(Action<XmlWriter> export)
      {
         var buffer = new StringBuilder();
         var xws = new XmlWriterSettings
         {
            OmitXmlDeclaration = true,
            Indent = true,
            IndentChars = "  ",
         };

         using(var writer = XmlWriter.Create(buffer, xws))
            export(writer);

         return buffer.ToString();
      }

      string GetXml(EmberNode ember)
      {
         return GetXmlBody(writer => XmlExport.Export(ember, writer));
      }

      string GetGlowXml(GlowContainer glow)
      {
         return GetXmlBody(writer => GlowXmlExport.Export(glow, writer));
      }
      #endregion
   }
}
