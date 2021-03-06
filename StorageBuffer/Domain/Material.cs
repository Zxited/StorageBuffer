﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageBuffer.Domain
{
    public class Material : IItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public int Quantity { get; set; }
        public string Type { get; } = "Material";

        public string Data
        {
            get { return $"{Quantity}"; }
        }

        public Material(int id, string name, string comment, int quantity)
        {
            Id = id;
            Name = name;
            Comment = comment;
            Quantity = quantity;
        }

        public List<string> ToList()
        {
            return new List<string>() { Type, Id.ToString(), Name, Data };
        }
        public List<string> ToLongList()
        {
            return new List<string>() { Id.ToString(), Name, Comment, Quantity.ToString() };
        }
    }
}
