/*
   EmberLib.net -- .NET implementation of the Ember+ Protocol

   Copyright (C) 2012-2019 Lawo GmbH (http://www.lawo.com).
   Distributed under the Boost Software License, Version 1.0.
   (See accompanying file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
*/

namespace EmberPlusRouter.Model
{
   abstract class ParameterBase : Element
   {
      public ParameterBase(int number, Element parent, string identifier, Dispatcher dispatcher, bool isWriteable)
      : base(number, parent, identifier)
      {
         Dispatcher = dispatcher;
         IsWriteable = isWriteable;
      }

      public Dispatcher Dispatcher { get; private set; }
      public bool IsWriteable { get; private set; }
   }
}
