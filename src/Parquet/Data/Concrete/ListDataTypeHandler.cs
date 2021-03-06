﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Parquet.Data.Concrete
{
   class ListDataTypeHandler : IDataTypeHandler
   {
      public ListDataTypeHandler()
      {
      }

      public DataType DataType => DataType.Unspecified;

      public SchemaType SchemaType => SchemaType.Struct;

      public Type ClrType => null;

      public IList CreateEmptyList(bool isNullable, bool isArray, int capacity)
      {
         throw new NotSupportedException();
      }

      public Field CreateSchemaElement(IList<Thrift.SchemaElement> schema, ref int index, out int ownedChildCount)
      {
         Thrift.SchemaElement tseList = schema[index];

         ListField listField = ListField.CreateWithNoItem(tseList.Name);
         //as we are skipping elements set path hint
         listField.Path = $"{tseList.Name}{Schema.PathSeparator}{schema[index + 1].Name}";
         index += 2;          //skip this element and child container
         ownedChildCount = 1; //we should get this element assigned back
         return listField;
      }

      public void CreateThrift(Field field, Thrift.SchemaElement parent, IList<Thrift.SchemaElement> container)
      {
         ListField listField = (ListField)field;

         parent.Num_children += 1;

         //add list container
         var root = new Thrift.SchemaElement(field.Name)
         {
            Converted_type = Thrift.ConvertedType.LIST,
            Repetition_type = Thrift.FieldRepetitionType.OPTIONAL,
            Num_children = 1  //field container below
         };
         container.Add(root);

         //add field container
         var list = new Thrift.SchemaElement("list")
         {
            Repetition_type = Thrift.FieldRepetitionType.REPEATED
         };
         container.Add(list);

         //add the list item as well
         IDataTypeHandler fieldHandler = DataTypeFactory.Match(listField.Item);
         fieldHandler.CreateThrift(listField.Item, list, container);
      }

      public bool IsMatch(Thrift.SchemaElement tse, ParquetOptions formatOptions)
      {
         return tse.__isset.converted_type && tse.Converted_type == Thrift.ConvertedType.LIST;
      }

      public IList Read(Thrift.SchemaElement tse, BinaryReader reader, ParquetOptions formatOptions)
      {
         throw new NotSupportedException();
      }

      public void Write(Thrift.SchemaElement tse, BinaryWriter writer, IList values)
      {
         throw new NotSupportedException();
      }
   }
}
