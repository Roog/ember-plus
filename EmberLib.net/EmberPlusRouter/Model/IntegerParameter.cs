﻿/*
   EmberLib.net -- .NET implementation of the Ember+ Protocol

   Copyright (C) 2012-2019 Lawo GmbH (http://www.lawo.com).
   Distributed under the Boost Software License, Version 1.0.
   (See accompanying file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
*/

namespace EmberPlusRouter.Model
{
   class IntegerParameter : Parameter<long>
   {
      public IntegerParameter(int number, Element parent, string identifier, Dispatcher dispatcher, int min, int max, bool isWriteable)
      : base(number, parent, identifier, dispatcher, isWriteable)
      {
         Minimum = min;
         Maximum = max;
      }

      public long Minimum { get; private set; }
      public long Maximum { get; private set; }

      public override TResult Accept<TState, TResult>(IElementVisitor<TState, TResult> visitor, TState state)
      {
         return visitor.Visit(this, state);
      }
   }
}
