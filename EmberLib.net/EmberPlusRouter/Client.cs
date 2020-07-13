﻿/*
   EmberLib.net -- .NET implementation of the Ember+ Protocol

   Copyright (C) 2012-2019 Lawo GmbH (http://www.lawo.com).
   Distributed under the Boost Software License, Version 1.0.
   (See accompanying file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using EmberLib.Glow.Framing;
using EmberLib;
using EmberLib.Glow;
using System.Xml;
using EmberLib.Xml;
using EmberLib.Framing;

namespace EmberPlusRouter
{
   class Client : IDisposable
   {
      public Client(GlowListener host, Socket socket, int maxPackageLength, Dispatcher dispatcher)
      {
         Host = host;
         Socket = socket;
         MaxPackageLength = maxPackageLength;
         Dispatcher = dispatcher;

         _reader = new GlowReader(GlowReader_RootReady, GlowReader_KeepAliveRequestReceived);
         _reader.Error += GlowReader_Error;
         _reader.FramingError += GlowReader_FramingError;
      }

      public GlowListener Host { get; private set; }
      public Socket Socket { get; private set; }
      public int MaxPackageLength { get; private set; }
      public Dispatcher Dispatcher { get; private set; }

      public void Read(byte[] buffer, int count)
      {
         GlowReader reader;

         lock(_sync)
         {
            reader = _reader;
            Console.WriteLine("Received {0} bytes from {1}", count, Socket.RemoteEndPoint);
         }

         if(reader != null)
            reader.ReadBytes(buffer, 0, count);
      }

      public void Write(GlowContainer glow)
      {
         var output = CreateOutput();

         glow.Encode(output);

         output.Finish();
      }

      public bool HasSubscribedToMatrix(Model.Matrix matrix)
      {
         lock(_sync)
            return _subscribedMatrices.Contains(matrix);
      }

      public void SubscribeToMatrix(Model.Matrix matrix, bool subscribe)
      {
         lock(_sync)
         {
            if(subscribe)
            {
               if(_subscribedMatrices.Contains(matrix) == false)
                  _subscribedMatrices.AddLast(matrix);
            }
            else
            {
               _subscribedMatrices.Remove(matrix);
            }
         }
      }

      #region Implementation
      GlowReader _reader;
      LinkedList<Model.Matrix> _subscribedMatrices = new LinkedList<Model.Matrix>();
      object _sync = new object();

      void GlowReader_RootReady(object sender, AsyncDomReader.RootReadyArgs e)
      {
         var root = e.Root as GlowContainer;

         if(root != null)
         {
            var buffer = new StringBuilder();
            var settings = new XmlWriterSettings
            {
               OmitXmlDeclaration = true,
               Indent = true,
               IndentChars = "  ",
            };

            using(var writer = XmlWriter.Create(Console.Out, settings))
               XmlExport.Export(root, writer);

            Dispatcher.DispatchGlow(root, this);
         }
         else
         {
            Console.WriteLine("Unexpected Ember Root: {0} ({1})", e.Root, e.Root.GetType());
         }
      }

      void GlowReader_Error(object sender, GlowReader.ErrorArgs e)
      {
         Console.WriteLine("GlowReader error {0}: {1}", e.ErrorCode, e.Message);
      }

      void GlowReader_FramingError(object sender, EmberLib.Framing.FramingReader.FramingErrorArgs e)
      {
         Console.WriteLine("GlowReader framing error: {0}", e.Message);
      }

      void GlowReader_KeepAliveRequestReceived(object sender, FramingReader.KeepAliveRequestReceivedArgs e)
      {
         Socket socket;

         lock(_sync)
            socket = Socket;

         if(socket != null)
            socket.Send(e.Response, e.ResponseLength, SocketFlags.None);
      }

      GlowOutput CreateOutput()
      {
         return new GlowOutput(MaxPackageLength, 0,
            (_, e) =>
            {
               Socket socket;
               GlowListener host;

               lock(_sync)
               {
                  socket = Socket;
                  host = Host;
               }

               if(socket != null)
               {
                  try
                  {
                     socket.Send(e.FramedPackage, e.FramedPackageLength, SocketFlags.None);
                  }
                  catch(SocketException)
                  {
                     if(host != null)
                        host.CloseClient(this);
                  }
               }
            });
      }
      #endregion

      #region IDisposable Members
      public void Dispose()
      {
         Socket socket;
         GlowReader reader;

         lock(_sync)
         {
            socket = Socket;
            reader = _reader;

            Socket = null;
            _reader = null;
            Host = null;
         }

         if(socket != null)
         {
            try
            {
               socket.Close();
            }
            catch
            {
            }
         }

         if(reader != null)
            reader.Dispose();
      }
      #endregion
   }
}
