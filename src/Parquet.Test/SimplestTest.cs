﻿using System;
using System.IO;
using Parquet.Data;
using Xunit;

namespace Parquet.Test
{
   public class SimplestTest
   {
      [Fact]
      public void Run_perfect_expressive_boolean_column()
      {
         var schema = new Schema(new DataField("id", DataType.Boolean, false, false));
         var ds = new DataSet(schema);

         ds.Add(true);
         ds.Add(false);
         ds.Add(true);

         DataSet ds1 = DataSetGenerator.WriteRead(ds);

      }

   }
}