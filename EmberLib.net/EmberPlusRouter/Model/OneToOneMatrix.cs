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
   class OneToOneMatrix : Matrix
   {
      public OneToOneMatrix(int number,
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
         sources = sources.Take(1);

         var firstSource = sources.FirstOrDefault();

         if(firstSource != null)
         {
            foreach(var signal in Targets)
            {
               if(signal.ConnectedSources.Contains(firstSource))
               {
                  signal.Connect(Enumerable.Empty<Signal>(), true);

                  Dispatcher.NotifyMatrixConnection(this, signal, null);
               }
            }

            target.Connect(sources, isAbsolute: true);
            return true;
         }

         return false;
      }

      public override TResult Accept<TState, TResult>(IElementVisitor<TState, TResult> visitor, TState state)
      {
         return visitor.Visit(this, state);
      }
   }
}
