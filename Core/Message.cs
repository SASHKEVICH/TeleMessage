﻿using System;

namespace Core
{
    public class Message
    {
        public string Text { get; set; }
        public User AddresseeUser { get; set; }
        public DateTime Time { get; set; }
    }
}
