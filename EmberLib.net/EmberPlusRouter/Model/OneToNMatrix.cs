/*
   EmberLib.net -- .NET implementation of the Ember+ Protocol

   Copyright (C) 2012-2019 Lawo GmbH (http://www.lawo.com).
   Distributed under the Boost Software License, Version 1.0.
   (See accompanying file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
*/

using System.Collections.Generic;
using System.Linq;

namespace EmberPlusRouter.Model
{
   class OneToNMatrix : Matrix
   {
      public OneToNMatrix(int number,
                          Element parent,
                          string identifier,
                          Dispatcher dispatcher,
                          IEnumerable<Signal> targets,
                          IEnumerable<Signal> sources,
                          Node labelsNode,
                          int? targetCount = null,
                          int? sourceCount = null)
      : base(number, parent, identifier, dispatcher, targets, sources, labelsNode, targetCount, sourceCount)
      {
      }

      protected override bool ConnectOverride(Signal target, IEnumerable<Signal> sources, ConnectOperation operation)
      {
         target.Connect(sources.Take(1), isAbsolute: true);

         return true;
      }

      public override TResult Accept<TState, TResult>(IElementVisitor<TState, TResult> visitor, TState state)
      {
         return visitor.Visit(this, state);
      }
   }
}
