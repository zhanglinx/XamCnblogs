﻿using XamCnblogs.Portable.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XamCnblogs.Portable.Interfaces
{
    public interface IQuestionsService
    {
        Task<ResponseMessage> GetQuestionsAsync(int position, int pageIndex = 1);
    }
}
