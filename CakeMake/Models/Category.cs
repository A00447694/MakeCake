﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CakeMake.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public List<Cake> Cakes { get; set; }
    }
}
