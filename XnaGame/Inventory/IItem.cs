using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XnaGame.Inventory
{
    public interface IItem
    {
        string Name { get; }
        string Description { get; }
        void Use();
        byte With();
    }
}
