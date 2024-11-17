﻿using StrategoServices.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract]
    public interface ICreateGameService
    {
        [OperationContract]
        GameSessionCreatedResponse CreateGameSession();
    }
}