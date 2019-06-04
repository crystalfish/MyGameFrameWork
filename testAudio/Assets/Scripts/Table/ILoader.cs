using System;
using System.Collections.Generic;
using System.Text;

namespace ReaL
{
    public interface ILoader
    {
        void ReadFile(System.IO.Stream stream);
        string GetFileName();
    }
}