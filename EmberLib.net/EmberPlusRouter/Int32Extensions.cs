/*
   EmberLib.net -- .NET implementation of the Ember+ Protocol

   Copyright (C) 2012-2019 Lawo GmbH (http://www.lawo.com).
   Distributed under the Boost Software License, Version 1.0.
   (See accompanying file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
*/

namespace EmberPlusRouter
{
   static class Int32Extensions
   {
      public static bool HasBits(this int flags, int bits)
      {
         return (flags & bits) == bits;
      }
   }
}
