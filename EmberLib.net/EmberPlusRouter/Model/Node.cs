/*
   EmberLib.net -- .NET implementation of the Ember+ Protocol

   Copyright (C) 2012-2019 Lawo GmbH (http://www.lawo.com).
   Distributed under the Boost Software License, Version 1.0.
   (See accompanying file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
*/

using System.Collections.Generic;

namespace EmberPlusRouter.Model
{
   class Node : Element
   {
      public Node(int number, Element parent, string identifier)
      : base(number, parent, identifier)
      {
      }

      public override IEnumerable<Element> Children
      {
         get { return _children; }
      }

      public override int ChildrenCount
      {
         get { return _children.Count; }
      }

      public override void AddChild(Element child)
      {
         _children.Add(child);
      }

      public static Node CreateRoot()
      {
         return new Node(0, null, null);
      }

      public override TResult Accept<TState, TResult>(IElementVisitor<TState, TResult> visitor, TState state)
      {
         return visitor.Visit(this, state);
      }

      #region Implementation
      List<Element> _children = new List<Element>();
      #endregion
   }
}
