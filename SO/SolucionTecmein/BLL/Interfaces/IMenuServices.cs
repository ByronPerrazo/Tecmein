﻿using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IMenuServices
    {
     Task<List<Menu>> ObtieneMenu(int secuencialUsuario);
    }
}
