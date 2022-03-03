using System;
using HappyRE.Core.Entities.Model;

namespace HappyRE.Core.BLL.Repositories
{
    public interface ITokenRepository: IBaseRepository<Token>
    {
        bool IsSharing(Guid id);
        bool Verify(Token obj);
        void AddToken(Token obj);
        bool VerifyByCode(Token obj);
        void DeleteByUser(string userId);
    }
}