﻿using System;

namespace EasyDapper.Core
{
  public class DuplicateAliasException : Exception
  {
    public DuplicateAliasException()
      : base("The alias is already in use in the statement.")
    {
    }
  }
}
