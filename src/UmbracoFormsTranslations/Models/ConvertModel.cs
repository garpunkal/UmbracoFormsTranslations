﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UmbracoFormsTranslations.Models
{
    public class ConvertModel
    {
        public string Id { get; set; }

        public Guid Key => Guid.Parse(Id);
    }
}   